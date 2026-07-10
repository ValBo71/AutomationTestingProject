@echo off
echo Running RestSharp API Tests...
dotnet test

echo.
echo Generating Allure Report...
if exist "AutomationExercise.RestSharp.ApiTests\bin\Debug\net8.0\allure-results" (
    allure generate "AutomationExercise.RestSharp.ApiTests\bin\Debug\net8.0\allure-results" --clean -o allure-report
    echo opening report...
    allure open allure-report
) else if exist "AutomationExercise.RestSharp.ApiTests\allure-results" (
    allure generate "AutomationExercise.RestSharp.ApiTests\allure-results" --clean -o allure-report
    echo opening report...
    allure open allure-report
) else (
    echo [WARNING] allure-results directory not found! Make sure allure is generating results.
)
pause
