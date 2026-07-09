@echo off
echo ===================================================
echo   Running API Integration Tests...
echo ===================================================
cd /d "%~dp0"
dotnet test

echo.
echo ===================================================
echo   Generating Allure Report...
echo ===================================================
allure generate "AutomationExercise.ApiTests\bin\Debug\net9.0\allure-results" --clean -o allure-report
echo.
echo Launching Allure Report server...
allure open allure-report
