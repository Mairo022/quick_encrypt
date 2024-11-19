@echo off

set "regFile=%temp%\qe_del_reg.reg"

(
echo Windows Registry Editor Version 5.00

echo [-HKEY_CLASSES_ROOT\*\shell\Quick Encrypt]
echo [-HKEY_CLASSES_ROOT\*\shell\Quick Encrypt Decrypt]
echo [-HKEY_CLASSES_ROOT\Directory\shell\Quick Encrypt]
echo [-HKEY_CLASSES_ROOT\Directory\shell\Quick Encrypt Decrypt]
) > "%regFile%"

regedit /s "%regFile%"
del /f /q "%regFile%"

echo Quick Encrypt actions removed from the menu.
pause