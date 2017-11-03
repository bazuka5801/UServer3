: Create/Update build folder

RMDIR /S /Q Builded
IF EXIST Builded (
	echo msgbox "Can't remove 'Builded' folder!" > %tmp%\tmp.vbs
	cscript /nologo %tmp%\tmp.vbs
	del %tmp%\tmp.vbs
)

if not exist "Builded\" mkdir Builded
if not exist "Builded\Data\" mkdir Builded\Data
if not exist "Builded\Data\Bin" mkdir Builded\Data\Bin
xcopy /s Reference Builded\Data\Bin /y

if not exist "Builded\Data\Database" mkdir Builded\Data\Database
xcopy /s Database Builded\Data\Database /y