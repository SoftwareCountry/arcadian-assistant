FROM microsoft/aspnetcore:2.0
ARG source
WORKDIR /app
EXPOSE 80 63302
COPY ${source:-obj/Docker/publish} .
ENTRYPOINT ["dotnet", "Arcadia.Assistant.Web.dll"]
