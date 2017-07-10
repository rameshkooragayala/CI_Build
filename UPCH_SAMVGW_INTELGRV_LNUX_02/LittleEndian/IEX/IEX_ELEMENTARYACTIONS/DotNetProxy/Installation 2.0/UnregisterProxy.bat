%windir%\Microsoft.NET\Framework\v2.0.50727\regasm.exe /unregister DotNetProxy.dll
gacutil -u DotNetProxy
%windir%\Microsoft.NET\Framework\v2.0.50727\caspol -machine -remfulltrust DotNetProxy.dll
pause