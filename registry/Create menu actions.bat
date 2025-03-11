@echo off

set "programPath=%~dp0EncryptionTool.exe"
set "programPath=%programPath:\=\\%"

set "regFile=%temp%\qe_to_reg.reg"

(
echo Windows Registry Editor Version 5.00

echo [HKEY_CLASSES_ROOT\*\shell\Quick Encrypt]
echo @="Quick Encrypt"
echo [HKEY_CLASSES_ROOT\*\shell\Quick Encrypt\command]
echo @="\"%programPath%\" path:\"%%1\" action:encrypt"

echo [HKEY_CLASSES_ROOT\*\shell\Quick Encrypt Decrypt]
echo @="Quick Decrypt"
echo [HKEY_CLASSES_ROOT\*\shell\Quick Encrypt Decrypt\command]
echo @="\"%programPath%\" path:\"%%1\" action:decrypt"

echo [HKEY_CLASSES_ROOT\*\shell\Quick Encrypt Delete]
echo @="Quick Encrypt Delete"
echo [HKEY_CLASSES_ROOT\*\shell\Quick Encrypt Delete\command]
echo @="\"%programPath%\" path:\"%%1\" action:delete"

echo [HKEY_CLASSES_ROOT\Directory\shell\Quick Encrypt]
echo @="Quick Encrypt"
echo [HKEY_CLASSES_ROOT\Directory\shell\Quick Encrypt\command]
echo @="\"%programPath%\" path:\"%%1\" action:encrypt"

echo [HKEY_CLASSES_ROOT\Directory\shell\Quick Encrypt Decrypt]
echo @="Quick Decrypt"
echo [HKEY_CLASSES_ROOT\Directory\shell\Quick Encrypt Decrypt\command]
echo @="\"%programPath%\" path:\"%%1\" action:decrypt"

echo [HKEY_CLASSES_ROOT\Directory\shell\Quick Encrypt Delete]
echo @="Quick Encrypt Delete"
echo [HKEY_CLASSES_ROOT\Directory\shell\Quick Encrypt Delete\command]
echo @="\"%programPath%\" path:\"%%1\" action:delete"
) > "%regFile%"

regedit /s "%regFile%"
del /f /q "%regFile%"

echo Quick Encrypt actions added to the menu.
pause