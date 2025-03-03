ARG ENVIRONMENT=development

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
RUN dotnet dev-certs https --trust
WORKDIR /src

COPY ["eCommerce.UserService/eCommerce.UserService.csproj", "eCommerce.UserService/"]
COPY ["eCommerce.Shared/eCommerce.Shared.csproj", "eCommerce.Shared/"]

RUN dotnet restore "eCommerce.UserService/eCommerce.UserService.csproj"

COPY . .

WORKDIR /src/eCommerce.UserService

RUN dotnet publish -c Release -o /app/publish;

FROM base AS final
WORKDIR /app
COPY --from=build /root/.dotnet/corefx/cryptography/x509stores/my/* /root/.dotnet/corefx/cryptography/x509stores/my/
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "eCommerce.UserService.dll"]