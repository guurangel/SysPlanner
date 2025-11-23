# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app

# Etapa final
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Instalar dependências mínimas do Oracle Client
RUN apt-get update && apt-get install -y libaio1 wget unzip

# Baixar e instalar Oracle Instant Client
RUN mkdir -p /opt/oracle
WORKDIR /opt/oracle

# BASIC + SDK = EF Core + ODP.NET funcionam
RUN wget https://download.oracle.com/otn_software/linux/instantclient/1919000/instantclient-basiclite-linux.x64.zip -O basic.zip && \
    unzip basic.zip && rm basic.zip

ENV LD_LIBRARY_PATH=/opt/oracle/instantclient_19_19:$LD_LIBRARY_PATH

WORKDIR /app
COPY --from=build /app .

EXPOSE 8080
ENTRYPOINT ["dotnet", "SysPlanner.dll"]
