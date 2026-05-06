FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar los archivos del proyecto
COPY *.sln ./
COPY *.csproj ./
RUN dotnet restore

# Copiar el resto del código y compilar
COPY . ./
RUN dotnet publish -c Release -o out

# Imagen final para ejecutar
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copiar los archivos compilados
COPY --from=build /app/out .

# Configurar el puerto que Render espera
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Iniciar la aplicación
ENTRYPOINT ["dotnet", "GestionDocumental.dll"]