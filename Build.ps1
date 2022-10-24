(& dotnet nuget locals http-cache -c) | Out-Null
& dotnet run --project "$PSScriptRoot\eng\src\BuildMetalamaFrameworkExtensions.csproj" -- $args
exit $LASTEXITCODE

