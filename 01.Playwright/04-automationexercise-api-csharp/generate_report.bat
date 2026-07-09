@echo off
echo ===================================================
echo   Generating Allure Report for API Tests...
echo ===================================================
cd /d "%~dp0"
allure generate "AutomationExercise.ApiTests\bin\Debug\net9.0\allure-results" --clean -o allure-report
echo.
echo Launching Allure Report server...
allure open allure-report
