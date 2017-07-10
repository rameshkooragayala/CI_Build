@echo on

cd bin
cleartool co -reserved -usehijack -c "Update Binary" LightSanity.exe  FullSanity.exe HealthCheck.exe
attrib -r *.* /s
copy ..\LightSanity\bin\Debug\LightSanity.exe            .\
copy ..\FullSanity\bin\Debug\FullSanity.exe              .\
copy ..\HealthCheck\bin\Debug\HealthCheck.exe            .\
cleartool ci -c . LightSanity.exe  FullSanity.exe HealthCheck.exe 

@echo off
pause