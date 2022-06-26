using System;

namespace ColoryrServer.SDK;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class NotesSDK : Attribute
{
    public string Text;
    public string[] Input;
    public string[] Output;

    public NotesSDK(string text, string[] input = null, string[] output = null)
    {
        Text = text;
        Input = input ?? new string[1];
        Output = output ?? new string[1];
    }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class DLLIN : Attribute
{
    public bool Debug;
    public DLLIN(bool debug = false)
    {
        Debug = debug;
    }
}