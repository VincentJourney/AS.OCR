#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["AS.OCR.Api/AS.OCR.Api.csproj", "AS.OCR.Api/"]
COPY ["AS.OCR.Extension.SDK/AS.OCR.Extension.SDK.csproj", "AS.OCR.Extension.SDK/"]
COPY ["AS.OCR.Model/AS.OCR.Model.csproj", "AS.OCR.Model/"]
COPY ["AS.OCR.Commom/AS.OCR.Commom.csproj", "AS.OCR.Commom/"]
COPY ["AS.OCR.Service/AS.OCR.Service.csproj", "AS.OCR.Service/"]
COPY ["AS.OCR.IService/AS.OCR.IService.csproj", "AS.OCR.IService/"]
COPY ["AS.OCR.Dapper/AS.OCR.Dapper.csproj", "AS.OCR.Dapper/"]
COPY ["AS.OCR.IDAO/AS.OCR.IDAO.csproj", "AS.OCR.IDAO/"]
RUN dotnet restore "AS.OCR.Api/AS.OCR.Api.csproj"
COPY . .
WORKDIR "/src/AS.OCR.Api"
RUN dotnet build "AS.OCR.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AS.OCR.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AS.OCR.Api.dll"]