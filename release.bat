@echo off

rem :::::::::::::  ������ �����ļ���projName��Ŀ���ļ���objName ����Ĭ�� :::::::::::::::::

set projName=NB-ModuleDebuger

set objName=NBģ��������

set srcPath=E:\code\%projName%
 
set dstPath=G:\release\%objName%


rem :::::::::::::  ���������ļ�  ::::::::::::::

del /q /f %dstPath%\%objName%\* 

xcopy  %srcPath%\%projName%\bin\Release\%projName%.exe  %dstPath%\%objName%\ /y /d

rem :::::::::::::  ��������ļ�  ::::::::::::::

for /f "eol=-" %%i in (ReleaseLog.txt) do (
set execName=%%~nxi
goto pack
)
:pack
	move %dstPath%\%objName%\%projName%.exe %dstPath%\%objName%\%execName%.exe 
	rar a -ep1 %dstPath%\%execName%.rar %dstPath%\%objName%


rem :::::::::::::  ����������Ŀ¼  ::::::::::::::

set workPath=E:\���Թ���
xcopy  %dstPath%\%objName%  %workPath%\%objName%\ /s /y /d


rem :::::::::::::  ���������ݷ�ʽ  ::::::::::::::

set fileName=%workPath%\%objName%\%execName%.exe
set shortName=%execName%.exe

:makeLink
	set TargetPath="%fileName%"
	set ShortcutPath="%userprofile%\desktop\%shortName%.lnk"
	set IconLocation="%fileName%,0"
	set HotKey=""

	echo Set WshShell=WScript.CreateObject("WScript.Shell") >tmp.vbs
	echo Set Shortcut=WshShell.CreateShortCut(%ShortcutPath%) >>tmp.vbs
	echo Shortcut.Hotkey = %HotKey% >>tmp.vbs
	echo Shortcut.IconLocation=%IconLocation% >>tmp.vbs
	echo Shortcut.TargetPath=%TargetPath% >>tmp.vbs
	echo Shortcut.Save >>tmp.vbs
	
	call tmp.vbs
	del /f /s /q tmp.vbs

rem :::::::::::::  Release ��� ::::::::::::::

pause

start %dstPath%


