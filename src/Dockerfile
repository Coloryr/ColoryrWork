FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY build_out/ASP/Debug/net7.0 ./
ENTRYPOINT ["dotnet", "ColoryrServer.ASP.dll"]