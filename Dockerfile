FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build
WORKDIR /src
COPY ["/src/Project.Backend.csproj", "."]
RUN dotnet restore "Project.Backend.csproj"
COPY /src/ .
RUN dotnet build "Project.Backend.csproj" -c Release -o /app
ARG buildnumber="notset"
RUN dotnet publish "Project.Backend.csproj" -c Release --version-suffix $buildnumber -o /app

FROM base AS final
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT Production
ENV ASPNETCORE_URLS "http://+"
EXPOSE 80
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Project.Backend.dll"]