FROM microsoft/dotnet:2.0-runtime
ARG source
WORKDIR /app
EXPOSE 63301
COPY ${source:-obj/Docker/publish} .
ENTRYPOINT ["dotnet", "Arcadia.Assistant.Server.Console.dll"]
