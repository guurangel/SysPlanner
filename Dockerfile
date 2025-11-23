# ==== BUILD ====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app

# ==== RUNTIME ====
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Dependências Oracle
RUN apt-get update && apt-get install -y libaio1 wget unzip && \
    rm -rf /var/lib/apt/lists/*

# Baixar Oracle Instant Client (Basic Lite 19.18)
WORKDIR /opt/oracle
RUN wget https://download.oracle.com/otn_software/linux/instantclient/1918000/instantclient-basiclite-linux.x64-19.18.0.0.0dbru.zip -O basic.zip && \
    unzip basic.zip && rm basic.zip

ENV LD_LIBRARY_PATH=/opt/oracle/instantclient_19_18:$LD_LIBRARY_PATH

WORKDIR /app
COPY --from=build /app .

EXPOSE 8080
ENTRYPOINT ["dotnet", "SysPlanner.dll"]
