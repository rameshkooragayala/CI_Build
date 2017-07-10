%windir%\Microsoft.NET\Framework\v2.0.50727\regasm DotNetProxy.dll /codebase
gacutil -i DotNetProxy.dll
%windir%\Microsoft.NET\Framework\v2.0.50727\caspol -machine -addfulltrust DotNetProxy.dll
pause