: Create/Update build folder
if not exist "Builded\" mkdir Builded
if not exist "Builded\Data\" mkdir Builded\Data
if not exist "Builded\Data\Bin" mkdir Builded\Data\Bin
xcopy /s Reference Builded\Data\Bin /y

if not exist "Builded\Data\Database" mkdir Builded\Data\Database
xcopy /s Database Builded\Data\Database /y