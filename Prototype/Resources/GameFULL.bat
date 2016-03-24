set PYTHON_CMD=c:\python32\python.exe
if not exist %PYTHON_CMD% set PYTHON_CMD=%PYTHON32%\python.exe
if not exist %PYTHON_CMD% set PYTHON_CMD=python.exe

%PYTHON_CMD% -B bin\buildWrapper.py %1 %2