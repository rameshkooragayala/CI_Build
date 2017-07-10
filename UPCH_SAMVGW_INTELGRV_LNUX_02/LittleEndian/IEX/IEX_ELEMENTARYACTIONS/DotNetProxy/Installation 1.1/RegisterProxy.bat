%windir%\Microsoft.NET\Framework\v1.1.4322\regasm DotNetProxy.dll /codebase
gacutil -i DotNetProxy.dll
%windir%\Microsoft.NET\Framework\v1.1.4322\caspol -machine -addfulltrust DotNetProxy.dll
pause