# Etapa 1: Build da aplicação
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Definindo o diretório de trabalho
WORKDIR /src

# Copiar o arquivo csproj e restaurar as dependências
COPY *.csproj ./
RUN dotnet restore

# Copiar o restante dos arquivos da aplicação para o diretório de trabalho
COPY . ./

# Publicar a aplicação em Release
RUN dotnet publish -c Release -o /app

# Etapa 2: Imagem de execução
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

# Definindo o diretório de trabalho na imagem de execução
WORKDIR /app

# Copiar os arquivos publicados da etapa anterior
COPY --from=build /app ./

# Expor a porta 5000 para permitir o acesso à aplicação dentro do contêiner
EXPOSE 5000

# Comando para rodar a aplicação
ENTRYPOINT ["dotnet", "Imaps.dll"]
