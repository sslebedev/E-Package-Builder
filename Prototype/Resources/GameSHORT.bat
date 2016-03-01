setlocal
set CURRENT_DIR=%~dp0
set CURRENT_DIR=%CURRENT_DIR:~0,-1%
set GAME_NAME=Game###FULLNAME###
set GAME_NAME_SHORT=Game-###SHORTNAME###

REM Sets Game Output dir relative to game's dir.
set DEVELOPMENT_DIR=%CURRENT_DIR%\..\..
for /f "delims=" %%a in ("%DEVELOPMENT_DIR%") do set DEVELOPMENT_DIR=%%~fa
set GAME_OUTPUT_DIR=%DEVELOPMENT_DIR%\Games\%GAME_NAME_SHORT%

REM To override output dir, define it here.
REM To prevent extra copy, undefine it here.
REM set GAME_OUTPUT_DIR=

set BIN_DIR=%DEVELOPMENT_DIR%\bin
set COMMAND_RUNNER=%BIN_DIR%\CommandRunner\cr.exe
if not defined UNITY set UNITY=%BIN_DIR%\Unity\WindowsEditor\Unity.exe
set UNITY_LOG_DIR=%localappdata%\Unity\Editor
set UNITY_LOG=%UNITY_LOG_DIR%\Editor.log

set UNITY_PROJECT=%CURRENT_DIR%
set UNITY_OUTPUT_DIR=%UNITY_PROJECT%\Builds\Release
set UNITY_BUILD_METHOD=CommandLineBuild.ReleaseGame

set UNITY_CLEAR_LOGS=if exist %UNITY_LOG_DIR%\*.log del /f %UNITY_LOG_DIR%\*.log
set UNITY_PRINT_LOG=if exist %UNITY_LOG% type %UNITY_LOG%

set WINDOWS_TURN_OFF_FIREWALL=netsh advfirewall set CurrentProfile state off
set WINDOWS_FIREWALL_ALLOW_UNITY=netsh advfirewall set allowedprogram program=%UNITY% name=Unity

if exist %UNITY% %WINDOWS_FIREWALL_ALLOW_UNITY%
if not exist %UNITY% echo No relative Unity found at %UNITY% - using default.&&set UNITY=Unity.exe

if not exist %COMMAND_RUNNER% echo No relative CommandRunner (cr.exe) found at %COMMAND_RUNNER% - using default.&&set COMMAND_RUNNER=cr.exe

echo Building %GAME_NAME_SHORT%... (%date% %time%)

if defined GAME_OUTPUT_DIR if exist %GAME_OUTPUT_DIR% rmdir /s /q %GAME_OUTPUT_DIR%
if defined GAME_OUTPUT_DIR mkdir %GAME_OUTPUT_DIR%

REM Workaround for build copy file issue - should be able to be removed with 2.0.1
if not exist %UNITY_OUTPUT_DIR% mkdir %UNITY_OUTPUT_DIR%

echo %UNITY% -batchMode -nographics -quit -projectPath %UNITY_PROJECT% -executeMethod %UNITY_BUILD_METHOD%
%COMMAND_RUNNER% /c:"%UNITY_CLEAR_LOGS%" /c:"%UNITY% -batchmode -nographics -quit -projectPath %UNITY_PROJECT% -executeMethod %UNITY_BUILD_METHOD%" /c:"%UNITY_PRINT_LOG%"

if not exist %UNITY_OUTPUT_DIR%\* echo No output files found: %UNITY_OUTPUT_DIR%\*
if defined GAME_OUTPUT_DIR if exist %UNITY_OUTPUT_DIR%\* xcopy /e /i /y %UNITY_OUTPUT_DIR%\* %GAME_OUTPUT_DIR%

REM Temporary - pathing is not working for signing key.
if exist d:\bin\DSASigner.exe copy d:\bin\DSASigner.exe %DEVELOPMENT_DIR%
if exist d:\bin\AVP_SHOW_2.xml copy d:\bin\AVP_SHOW_2.xml %DEVELOPMENT_DIR%
if exist %DEVELOPMENT_DIR%\..\DSASigner\DSASigner.exe copy %DEVELOPMENT_DIR%\..\DSASigner\DSASigner.exe %DEVELOPMENT_DIR%
if exist %DEVELOPMENT_DIR%\..\DSASigner\AVP_SHOW_2.xml copy %DEVELOPMENT_DIR%\..\DSASigner\AVP_SHOW_2.xml %DEVELOPMENT_DIR%

echo Finished building %GAME_NAME_SHORT%. (%date% %time%)
endlocal
