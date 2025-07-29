FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG TARGETARCH
WORKDIR /source

# copy csproj and restore as distinct layers
COPY "Ortzschestrate.Api/*.csproj" "Ortzschestrate.Api/"
COPY "Ortzschestrate.Data/*.csproj" "Ortzschestrate.Data/"
COPY "Ortzschestrate.Infrastructure/*.csproj" "Ortzschestrate.Infrastructure/"
COPY "Ortzschestrate.Utilities/*.csproj" "Ortzschestrate.Utilities/"
COPY "Ortzschestrate.Web3/*.csproj" "Ortzschestrate.Web3/"
RUN dotnet restore "Ortzschestrate.Api/Ortzschestrate.Api.csproj"

# copy and publish app and libraries
COPY "Ortzschestrate.Api/" "Ortzschestrate.Api/"
COPY "Ortzschestrate.Data/" "Ortzschestrate.Data/"
COPY "Ortzschestrate.Infrastructure/" "Ortzschestrate.Infrastructure/"
COPY "Ortzschestrate.Utilities/" "Ortzschestrate.Utilities/"
COPY "Ortzschestrate.Web3/" "Ortzschestrate.Web3/"
WORKDIR "/source/Ortzschestrate.Api"
RUN dotnet publish -a $TARGETARCH -o /app


# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
EXPOSE 8080
WORKDIR /app
COPY --from=build /app .
USER $APP_UID
ENTRYPOINT ["./Ortzschestrate.Api"]
