## Migrations and Reports
--Instal .NET EF Globally
-dotnet tool install --global dotnet-ef
-dotnet tool uninstall --global dotnet-ef --> Remove global
-dotnet tool install --global dotnet-ef --version 6.0.26 -->Installing especific version

--Open solution folder by cmd to execute
-dotnet ef migrations add Init --project API

--run Database
-dotnet ef database update --project API

--Install code Analyser
-dotnet tool install -g dotnet-format

--To generate Code Test Coverage 
-dotnet tool install -g dotnet-reportgenerator-globaltool -->Install the tool
-dotnet test --collect:"XPlat Code Coverage" --> Runs the test coverage
--This line below generates the HTML file with visible understandanble report
[reportgenerator -reports:
"Path\To\TestProject\TestResults\e1fbba88-4fd9-43ef-9d01-bbaf3d5fcd2b\coverage.cobertura.xml" 
-targetdir:"coveragereport" -reporttypes:Html]

--example
--reportgenerator -reports:"API.Test\TestResults\e1fbba88-4fd9-43ef-9d01-bbaf3d5fcd2b\coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html