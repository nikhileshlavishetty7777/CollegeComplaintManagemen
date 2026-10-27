@echo off
REM Double-click helper: runs setup.ps1 without changing your PowerShell policy permanently
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0setup.ps1"
pause
