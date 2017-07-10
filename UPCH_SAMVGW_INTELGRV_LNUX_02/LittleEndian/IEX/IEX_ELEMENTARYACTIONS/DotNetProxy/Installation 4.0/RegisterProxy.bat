%windir%\Microsoft.NET\Framework\v4.0.30319\regasm DotNetProxy.dll /codebase
gacutil -i DotNetProxy.dll
%windir%\Microsoft.NET\Framework\v4.0.30319\caspol -machine -addfulltrust DotNetProxy.dll
pause