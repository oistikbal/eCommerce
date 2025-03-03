ARG ENVIRONMENT=development

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 3000

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
RUN dotnet dev-certs https --trust
WORKDIR /src

COPY ["eCommerce.Gateway/eCommerce.Gateway.csproj", "eCommerce.Gateway/"]
COPY ["eCommerce.Shared/eCommerce.Shared.csproj", "eCommerce.Shared/"]

RUN dotnet restore "eCommerce.Gateway/eCommerce.Gateway.csproj"

COPY . .

WORKDIR /src/eCommerce.Gateway

RUN dotnet publish -c Release -o /app/publish;

FROM base AS final
WORKDIR /app
COPY --from=build /root/.dotnet/corefx/cryptography/x509stores/my/* /root/.dotnet/corefx/cryptography/x509stores/my/
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "eCommerce.Gateway.dll"]