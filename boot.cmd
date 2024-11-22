REM set execution policy first so that a setup script can be run
powershell.exe -command "&{Set-ExecutionPolicy RemoteSigned -force}"

REM Now run the true configuration script
powershell.exe -file c:\Fractal_Source\sandbox-config.ps1