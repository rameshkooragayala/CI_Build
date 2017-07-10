%windir%\Microsoft.NET\Framework\v4.0.30319\regasm.exe /unregister DotNetProxy.dll
gacutil -u DotNetProxy
%windir%\Microsoft.NET\Framework\v4.0.30319\caspol -machine -remfulltrust DotNetProxy.dll
pause