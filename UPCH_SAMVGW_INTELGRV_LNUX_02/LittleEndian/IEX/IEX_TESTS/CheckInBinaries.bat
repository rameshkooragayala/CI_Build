@echo on

cd bin
cleartool co -reserved -usehijack -c "Update Binary" CommonTests.exe IEX.Tests.Engine.dll IEX.Tests.Utils.dll IEX.HealthCheck.dll MultiRunner.exe 
attrib -r *.* /s
copy ..\src\CommonTests\bin\Debug\CommonTests.exe              .\
copy ..\src\IEX.Tests.Engine\bin\Debug\IEX.Tests.Engine.dll    .\
copy ..\src\IEX.Tests.Utils\bin\Debug\IEX.Tests.Utils.dll      .\
copy ..\src\IEX.HealthCheck\bin\Debug\IEX.HealthCheck.dll      .\
copy ..\src\MultiRunner\bin\Debug\MultiRunner.exe              .\
cleartool ci -c . CommonTests.exe IEX.Tests.Engine.dll IEX.Tests.Utils.dll IEX.HealthCheck.dll MultiRunner.exe 

@echo off
pause