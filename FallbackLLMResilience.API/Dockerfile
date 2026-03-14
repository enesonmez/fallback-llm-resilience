# Build aşaması
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Sadece gerekli csproj dosyasını kopyalayıp restore yapıyoruz
COPY ["FallbackLLMResilience.API.csproj", "./"]
RUN dotnet restore "FallbackLLMResilience.API.csproj"

# Tüm dosyaları kopyalayıp yayınlıyoruz
COPY . .
RUN dotnet publish "FallbackLLMResilience.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Çalıştırma aşaması
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FallbackLLMResilience.API.dll"]