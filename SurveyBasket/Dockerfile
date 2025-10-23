## ---------------------------
## Build stage
## ---------------------------
#FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
#WORKDIR /src
#
#COPY SurveyBasket.csproj .
#RUN dotnet restore
#COPY . .
#RUN dotnet publish -c Release -o /app/out
#
#
#FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
#WORKDIR /app
#
#LABEL maintainer="Khaled Zaabat <khaled.zaabat@ensia.edu.dz>" \
      #description="SurveyBasket ASP.NET Core Application" \
      #version="1.0"
#
#COPY --from=build /app/out .
#ENV ASPNETCORE_URLS=http://+:8080
#EXPOSE 8080
#ENTRYPOINT ["dotnet", "SurveyBasket.dll"]
#
## ---------------------------
## Development stage (with EF tools)
## ---------------------------
#FROM mcr.microsoft.com/dotnet/sdk:9.0 AS development
#WORKDIR /app
#
#LABEL maintainer="Khaled Zaabat <khaled.zaabat@ensia.edu.dz>" \
      #description="SurveyBasket ASP.NET Core Application (Development)" \
      #version="1.0-dev"
#
##RUN dotnet tool install --global dotnet-ef --version 9.*
##
##COPY --from=build /app/out .
##COPY . /src
##COPY entrypoint.sh .
##RUN chmod +x ./entrypoint.sh
#
#ENV PATH="$PATH:/root/.dotnet/tools"
#ENV ASPNETCORE_URLS=http://+:8080
#
#EXPOSE 8080
#ENTRYPOINT ["bash", "./entrypoint.sh"]
#


# ---------------------------
# Build stage
# ---------------------------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY SurveyBasket.csproj .
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app/out

# ---------------------------
# Runtime stage
# ---------------------------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

LABEL maintainer="Khaled Zaabat <khaled.zaabat@ensia.edu.dz>" \
      description="SurveyBasket ASP.NET Core Application" \
      version="1.0"

COPY --from=build /app/out .
COPY ./Templates ./Templates

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "SurveyBasket.dll"]
