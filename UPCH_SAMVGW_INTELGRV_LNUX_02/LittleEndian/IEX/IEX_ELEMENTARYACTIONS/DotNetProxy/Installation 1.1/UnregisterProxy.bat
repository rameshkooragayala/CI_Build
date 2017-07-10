%windir%\Microsoft.NET\Framework\v1.1.4322\regasm.exe /unregister DotNetProxy.dll
gacutil -u DotNetProxy
%windir%\Microsoft.NET\Framework\v1.1.4322\caspol -machine -remfulltrust DotNetProxy.dll
pause