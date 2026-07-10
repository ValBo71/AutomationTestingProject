@echo off
echo Running Java RestAssured API Tests...
call mvn clean test

echo.
echo Generating Allure Report...
if exist "target\allure-results" (
    call allure generate target\allure-results --clean -o allure-report
    echo opening report...
    call allure open allure-report
) else (
    echo [WARNING] target\allure-results directory not found! Make sure Maven test run completed successfully.
)
pause
