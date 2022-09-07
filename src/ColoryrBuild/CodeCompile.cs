using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Host.Mef;
using RoslynPad.Roslyn.Diagnostics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition.Hosting;
using System.Linq;
using System.Reflection;
using RoslynPad.Roslyn;
using System.Composition;
using System.Runtime.CompilerServices;
using System.IO;
using Microsoft.CodeAnalysis.Text;

namespace ColoryrBuild;

public class CodeCompileWorkspace : Workspace
{
    public DocumentId OpenDocumentId { get; private set; }
    public IRoslynHost RoslynHost { get; }

    public CodeCompileWorkspace(HostServices hostServices, IRoslynHost roslynHost)
        : base(hostServices, WorkspaceKind.Host)
    {
        RoslynHost = roslynHost;
    }

    public new void SetCurrentSolution(Solution solution)
    {
        var oldSolution = CurrentSolution;
        var newSolution = base.SetCurrentSolution(solution);
        RaiseWorkspaceChangedEventAsync(WorkspaceChangeKind.SolutionChanged, oldSolution, newSolution);
    }

    public override bool CanOpenDocuments => true;

    public override bool CanApplyChange(ApplyChangesKind feature)
    {
        switch (feature)
        {
            case ApplyChangesKind.ChangeDocument:
            case ApplyChangesKind.ChangeDocumentInfo:
            case ApplyChangesKind.AddMetadataReference:
            case ApplyChangesKind.RemoveMetadataReference:
            case ApplyChangesKind.AddAnalyzerReference:
            case ApplyChangesKind.RemoveAnalyzerReference:
                return true;
            default:
                return false;
        }
    }

    public void OpenDocument(DocumentId documentId, SourceTextContainer textContainer)
    {
        OpenDocumentId = documentId;
        OnDocumentOpened(documentId, textContainer);
        OnDocumentContextUpdated(documentId);
    }

    public event Action<DocumentId, SourceText>? ApplyingTextChange;

    protected override void Dispose(bool finalize)
    {
        base.Dispose(finalize);

        ApplyingTextChange = null;
    }

    protected override void ApplyDocumentTextChanged(DocumentId document, SourceText newText)
    {
        if (OpenDocumentId != document)
        {
            return;
        }

        ApplyingTextChange?.Invoke(document, newText);

        OnDocumentTextChanged(document, newText, PreservationMode.PreserveIdentity);
    }
}

internal interface IDocumentationProviderService : IWorkspaceService
{
    DocumentationProvider GetDocumentationProvider(string assemblyFullPath);
}

public class DummyScriptMetadataResolver : MetadataReferenceResolver
{
    public static DummyScriptMetadataResolver Instance { get; } = new DummyScriptMetadataResolver();

    private DummyScriptMetadataResolver() { }

    public override bool Equals(object other) => ReferenceEquals(this, other);

    public override int GetHashCode() => RuntimeHelpers.GetHashCode(this);

    public override bool ResolveMissingAssemblies => false;

    public override PortableExecutableReference ResolveMissingAssembly(MetadataReference definition, AssemblyIdentity referenceIdentity) => null;

    public override ImmutableArray<PortableExecutableReference> ResolveReference(string reference, string? baseFilePath, MetadataReferenceProperties properties) =>
        ImmutableArray<PortableExecutableReference>.Empty;
}

[ExportWorkspaceServiceFactory(typeof(IDocumentationProviderService), ServiceLayer.Host), Shared]
internal sealed class DocumentationProviderServiceFactory : IWorkspaceServiceFactory
{
    private readonly IDocumentationProviderService _service;

    [ImportingConstructor]
    public DocumentationProviderServiceFactory(IDocumentationProviderService service) => _service = service;

    public IWorkspaceService CreateService(HostWorkspaceServices workspaceServices) => _service;
}

[Export(typeof(IDocumentationProviderService)), Shared]
internal sealed class DocumentationProviderService : IDocumentationProviderService
{
    private readonly ConcurrentDictionary<string, DocumentationProvider> _assemblyPathToDocumentationProviderMap = new();

    public DocumentationProvider GetDocumentationProvider(string location)
    {
        string? finalPath = Path.ChangeExtension(location, "xml");

        return _assemblyPathToDocumentationProviderMap.GetOrAdd(location, _ =>
        {
            if (!File.Exists(finalPath))
            {
                return null;
            }

            return XmlDocumentationProvider.CreateFromFile(finalPath);
        });
    }
}

internal class CodeCompile : IRoslynHost
{
    internal static readonly ImmutableArray<string> PreprocessorSymbols =
            ImmutableArray.CreateRange(new[] { "TRACE", "DEBUG" });

    private readonly ConcurrentDictionary<DocumentId, Action<DiagnosticsUpdatedArgs>> _diagnosticsUpdatedNotifiers;
    private readonly IDocumentationProviderService _documentationProviderService;
    private readonly CompositionHost _compositionContext;

    public HostServices HostServices { get; }
    public ParseOptions ParseOptions { get; }
    public CodeCompileWorkspace Workspace { get; }
    public Project Project { get; }
    public Solution Solution { get; }
    public ImmutableArray<MetadataReference> DefaultReferences { get; }
    public ImmutableArray<string> DefaultImports { get; }
    public ImmutableArray<string> DisabledDiagnostics { get; }

    internal static readonly ImmutableArray<Assembly> DefaultCompositionAssemblies =
            ImmutableArray.Create(
                // Microsoft.CodeAnalysis.Workspaces
                typeof(WorkspacesResources).Assembly,
                // Microsoft.CodeAnalysis.CSharp.Workspaces
                typeof(CSharpWorkspaceResources).Assembly,
                // Microsoft.CodeAnalysis.Features
                typeof(FeaturesResources).Assembly,
                // Microsoft.CodeAnalysis.CSharp.Features
                typeof(CSharpFeaturesResources).Assembly,
                // RoslynPad.Roslyn
                typeof(RoslynHost).Assembly);

    public CodeCompile(IEnumerable<Assembly> additionalAssemblies,
        RoslynHostReferences references,
        ImmutableArray<string>? disabledDiagnostics = null)
    {
        if (references == null) references = RoslynHostReferences.Empty;

        var partTypes = additionalAssemblies
            .SelectMany(x => x.DefinedTypes)
            .Select(x => x.AsType());

        _compositionContext = new ContainerConfiguration()
            .WithParts(partTypes)
            .CreateContainer();

        HostServices = MefHostServices.Create(_compositionContext);

        Workspace = new CodeCompileWorkspace(HostServices, this);

        Solution = Workspace.CurrentSolution;
        Project = CreateProject(Solution, CreateCompilationOptions(AppContext.BaseDirectory));

        _diagnosticsUpdatedNotifiers = new ConcurrentDictionary<DocumentId, Action<DiagnosticsUpdatedArgs>>();

        ParseOptions = CreateDefaultParseOptions();

        _documentationProviderService = GetService<IDocumentationProviderService>();

        DefaultReferences = references.GetReferences(DocumentationProviderFactory);
        DefaultImports = references.Imports;

        DisabledDiagnostics = disabledDiagnostics ?? ImmutableArray<string>.Empty;
        GetService<IDiagnosticService>().DiagnosticsUpdated += OnDiagnosticsUpdated;
    }

    public Func<string, DocumentationProvider> DocumentationProviderFactory => _documentationProviderService.GetDocumentationProvider;

    protected virtual ParseOptions CreateDefaultParseOptions() => new CSharpParseOptions(
        preprocessorSymbols: PreprocessorSymbols,
        languageVersion: LanguageVersion.Preview);

    public MetadataReference CreateMetadataReference(string location) => MetadataReference.CreateFromFile(location,
        documentation: _documentationProviderService.GetDocumentationProvider(location));

    private void OnDiagnosticsUpdated(object? sender, DiagnosticsUpdatedArgs diagnosticsUpdatedArgs)
    {
        var documentId = diagnosticsUpdatedArgs.DocumentId;
        if (documentId == null) return;

        if (_diagnosticsUpdatedNotifiers.TryGetValue(documentId, out var notifier))
        {
            if (diagnosticsUpdatedArgs.Kind == DiagnosticsUpdatedKind.DiagnosticsCreated)
            {
                var remove = diagnosticsUpdatedArgs.Diagnostics.RemoveAll(d => DisabledDiagnostics.Contains(d.Id));
                if (remove.Length != diagnosticsUpdatedArgs.Diagnostics.Length)
                {
                    diagnosticsUpdatedArgs = diagnosticsUpdatedArgs.WithDiagnostics(remove);
                }
            }

            notifier(diagnosticsUpdatedArgs);
        }
    }

    public TService GetService<TService>() => _compositionContext.GetExport<TService>();

    public void CloseDocument(DocumentId documentId)
    {
        if (documentId == null) throw new ArgumentNullException(nameof(documentId));

        Workspace.CloseDocument(documentId);

        var document = Workspace.CurrentSolution.GetDocument(documentId);

        _diagnosticsUpdatedNotifiers.TryRemove(documentId, out _);
    }

    public Document? GetDocument(DocumentId documentId)
    {
        if (documentId == null) throw new ArgumentNullException(nameof(documentId));

        return Workspace.CurrentSolution.GetDocument(documentId);
    }

    public DocumentId AddDocument(DocumentCreationArgs args)
    {
        if (args == null) throw new ArgumentNullException(nameof(args));

        return AddDocument(Workspace, args);
    }

    public DocumentId AddRelatedDocument(DocumentCreationArgs args)
    {
        if (args == null) throw new ArgumentNullException(nameof(args));

        var documentId = AddDocument(args);

        return documentId;
    }

    private DocumentId AddDocument(CodeCompileWorkspace workspace, DocumentCreationArgs args)
    {
        var document = CreateDocument(Project, args);
        var documentId = document.Id;

        workspace.SetCurrentSolution(document.Project.Solution);
        workspace.OpenDocument(documentId, args.SourceTextContainer);

        if (args.OnDiagnosticsUpdated != null)
        {
            _diagnosticsUpdatedNotifiers.TryAdd(documentId, args.OnDiagnosticsUpdated);
        }

        var onTextUpdated = args.OnTextUpdated;
        if (onTextUpdated != null)
        {
            workspace.ApplyingTextChange += (d, s) =>
            {
                if (documentId == d) onTextUpdated(s);
            };
        }

        return documentId;
    }

    public void UpdateDocument(Document document)
    {
        if (document == null) throw new ArgumentNullException(nameof(document));

        Workspace.TryApplyChanges(document.Project.Solution);
    }

    protected virtual CompilationOptions CreateCompilationOptions(string local)
    {
        var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
            usings: DefaultImports,
            allowUnsafe: true,
            sourceReferenceResolver: new SourceFileResolver(ImmutableArray<string>.Empty, local),
            // all #r references are resolved by the editor/msbuild
            metadataReferenceResolver: DummyScriptMetadataResolver.Instance,
            nullableContextOptions: NullableContextOptions.Enable);
        return compilationOptions;
    }

    protected virtual Document CreateDocument(Project project, DocumentCreationArgs args)
    {
        var id = DocumentId.CreateNewId(project.Id);
        var solution = project.Solution.AddDocument(id, args.Name ?? project.Name, args.SourceTextContainer.CurrentText);
        return solution.GetDocument(id)!;
    }

    protected virtual Project CreateProject(Solution solution, CompilationOptions compilationOptions)
    {
        var name = "ColoryrWork";
        var id = ProjectId.CreateNewId(name);

        var parseOptions = ParseOptions.WithKind(SourceCodeKind.Regular);

        solution = solution.AddProject(ProjectInfo.Create(
            id,
            VersionStamp.Create(),
            name,
            name,
            LanguageNames.CSharp,
            isSubmission: false,
            parseOptions: parseOptions,
            compilationOptions: compilationOptions,
            metadataReferences: DefaultReferences,
            projectReferences: null));

        var project = solution.GetProject(id)!;

        if (GetUsings(project) is { Length: > 0 } usings)
        {
            project = project.AddDocument("RoslynPadGeneratedUsings", usings).Project;
        }

        return project;

        static string GetUsings(Project project)
        {
            if (project.CompilationOptions is CSharpCompilationOptions options)
            {
                return string.Join(" ", options.Usings.Select(i => $"global using {i};"));
            }

            return string.Empty;
        }
    }
}
