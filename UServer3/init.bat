: Create/Update build folder
if not exist "Builded\" mkdir Builded
if not exist "Builded\Data\" mkdir Builded\Data
xcopy /s Reference Builded\Data