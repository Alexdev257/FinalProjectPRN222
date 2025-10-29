# =========================
# 1?? BUILD STAGE
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution + csproj files ?? restore tr??c
COPY ["FinalProjectPRN222.sln", "./"]
COPY ["FPP.Application/FPP.Application.csproj", "FPP.Application/"]
COPY ["FPP.Domain/FPP.Domain.csproj", "FPP.Domain/"]
COPY ["FPP.Infrastructure/FPP.Infrastructure.csproj", "FPP.Infrastructure/"]
COPY ["FPP.Presentation/FPP.Presentation.csproj", "FPP.Presentation/"]

# Restore dependencies
RUN dotnet restore "FinalProjectPRN222.sln"

# Copy toàn b? source code
COPY . .

# Build và publish project WebApp
WORKDIR "/src/FPP.Presentation"
ENV NUGET_PACKAGES=/root/.nuget/packages
RUN dotnet restore "FPP.Presentation.csproj" --disable-parallel
RUN dotnet publish "FPP.Presentation.csproj" -c Release -o /app/publish --no-restore

# =========================
# 2?? RUNTIME STAGE
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy file t? build stage
COPY --from=build /app/publish .

# M? c?ng
EXPOSE 80
EXPOSE 443

# Ch?y app
ENTRYPOINT ["dotnet", "FPP.Presentation.dll"]