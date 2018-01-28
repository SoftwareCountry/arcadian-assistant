## Instructions on Docker Compose
```
dotnet publish ./Arcadia.Assistant.sln -c Debug -o ./obj/Docker/publish
docker-compose build
docker-compose up
```

## Prerequisites
1. IIS 10 to enable HTTP/2
2. [ANCM](https://aka.ms/dotnetcore-2-windowshosting)

## Connection strings / other secrets configuration
In order to make things run, connection strings and resource urls have to be provided. 
Both Web and Server listens on environment variables, command line arguments and user secrets. Do the following to run it in development:
1. Open Arcadia.Assistant.Server.Console properties, go to Debug tab and insert `/ConnectionStrings:ArcadiaCSP "<your_connection_string_here>"` to command line window.
Alternatively, that can be added to Environment variables window. @ArcadiaTeam, please refer to Teams wiki in order to get proper connection strings