﻿FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ShippingService/ShippingService.csproj", "ShippingService/"]
RUN dotnet restore "ShippingService/ShippingService.csproj"
COPY . .
WORKDIR "/src/ShippingService"
RUN dotnet build "ShippingService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ShippingService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ShippingService.dll"]
