# Use the official Microsoft SQL Server 2022 image
FROM mcr.microsoft.com/mssql/server:2022-latest

# Accept EULA and set a strong password via build arguments or environment variables
# Note: In a real production scenario, use secrets or environment variables at runtime
ENV ACCEPT_EULA=Y
ENV MSSQL_SA_PASSWORD=Temp/mh220

# Optional: Copy initialization scripts if needed
# COPY ./init.sql /docker-entrypoint-initdb.d/

EXPOSE 1433
