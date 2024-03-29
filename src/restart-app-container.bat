@echo off

REM Check if the container is running
FOR /f "tokens=*" %%i IN ('docker ps --format "{{.Names}}" ^| findstr /C:"src-theforum_app-1"') DO SET containerName=%%i

REM If it is, then restart the container
IF NOT "%containerName%"=="" (
    echo Restarting container %containerName%...
    docker restart %containerName%
) ELSE (
    echo Container not running.
)
