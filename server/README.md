## Instructions on Docker Compose
```
dotnet publish ./Arcadia.Assistant.sln -c Debug -o ./obj/Docker/publish
docker-compose build
docker-compose up
```

## Prerequisites
1. IIS 10 to enable HTTP/2
2. [ANCM](https://aka.ms/dotnetcore-2-windowshosting)