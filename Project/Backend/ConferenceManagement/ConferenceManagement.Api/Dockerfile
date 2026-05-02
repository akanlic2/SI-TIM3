FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["ConferenceManagement.Api/ConferenceManagement.Api.csproj", "ConferenceManagement.Api/"]
COPY ["ConferenceManagement.Application/ConferenceManagement.Application.csproj", "ConferenceManagement.Application/"]
COPY ["ConferenceManagement.Dal/ConferenceManagement.Dal.csproj", "ConferenceManagement.Dal/"]
COPY ["ConferenceManagement.Domain/ConferenceManagement.Domain.csproj", "ConferenceManagement.Domain/"]

RUN dotnet restore "ConferenceManagement.Api/ConferenceManagement.Api.csproj"

COPY . .
WORKDIR "/src/ConferenceManagement.Api"
RUN dotnet build "ConferenceManagement.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build
RUN dotnet publish "ConferenceManagement.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ConferenceManagement.Api.dll"]