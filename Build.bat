pushd "%~dp0"

nuget restore

set version=1.0.0.3
packages\NAnt.Portable.0.92\NAnt.exe -D:config.solution.buildmode=Release -D:Build_Number=%version% -buildfile:"%~dp0%Nant.build"
