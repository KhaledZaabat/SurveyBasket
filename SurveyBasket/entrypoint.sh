
set -e

echo "Waiting for SQL Server to start..."
sleep 15

cd /src


dotnet new tool-manifest --force
dotnet tool install dotnet-ef --version 9.* --local


if [ ! -d "Migrations" ] || [ -z "$(ls -A Migrations)" ]; then
    echo "No migrations found, creating InitialCreate..."
    dotnet tool run dotnet-ef migrations add InitialCreate --project SurveyBasket.csproj --startup-project SurveyBasket.csproj
else
    echo "Migrations already exist."
fi


echo "Applying migrations..."
dotnet tool run dotnet-ef database update --project SurveyBasket.csproj --startup-project SurveyBasket.csproj


echo "Starting SurveyBasket API..."
cd /app
exec dotnet SurveyBasket.dll
