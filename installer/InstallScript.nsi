;--------------------------------
;Include Modern UI
!include "MUI2.nsh"

;Include LogicLib to be able to check if the Data directory exists
!include LogicLib.nsh

;FileExists is already part of LogicLib, but returns true for directories as well as files
!macro _FileExists2 _a _b _t _f
	!insertmacro _LOGICLIB_TEMP
	StrCpy $_LOGICLIB_TEMP "0"
	StrCmp `${_b}` `` +4 0 ;if path is not blank, continue to next check
	IfFileExists `${_b}` `0` +3 ;if path exists, continue to next check (IfFileExists returns true if this is a directory)
	IfFileExists `${_b}\*.*` +2 0 ;if path is not a directory, continue to confirm exists
	StrCpy $_LOGICLIB_TEMP "1" ;file exists
	;now we have a definitive value - the file exists or it does not
	StrCmp $_LOGICLIB_TEMP "1" `${_t}` `${_f}`
!macroend
!undef FileExists
!define FileExists `"" FileExists2`
!macro _DirExists _a _b _t _f
	!insertmacro _LOGICLIB_TEMP
	StrCpy $_LOGICLIB_TEMP "0"	
	StrCmp `${_b}` `` +3 0 ;if path is not blank, continue to next check
	IfFileExists `${_b}\*.*` 0 +2 ;if directory exists, continue to confirm exists
	StrCpy $_LOGICLIB_TEMP "1"
	StrCmp $_LOGICLIB_TEMP "1" `${_t}` `${_f}`
!macroend
!define DirExists `"" DirExists`

;--------------------------------

; The name of the installer
Name "OCTGN Deck Converter v3.9.0"

; The file to write
OutFile "OCTGNDeckConverterInstaller.exe"

; Request application privileges for Windows Vista/7/8
RequestExecutionLevel user

!define MUI_ICON "converter.ico"

!define MUI_TEXT_WELCOME_INFO_TEXT "This wizard will install the OCTGN Deck Converter plugin v3.9.0 for OCTGN.$\n$\nClick Install to start the installation."
!define MUI_PAGE_CUSTOMFUNCTION_SHOW WelcomeShowCallback
  
!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_INSTFILES

!insertmacro MUI_LANGUAGE "English"

Section "" ; No components page, name is not important

	ClearErrors
	ReadRegStr $2 HKCU "SOFTWARE\OCTGN" 'InstallPath'
	FileOpen $0 "$2\data.path" r
		IfErrors error
	FileRead $0 $1
	FileClose $0
  
	; Set output path to the installation directory.
	${If} ${DirExists} "$1"
		SetOutPath "$1\Plugins\OCTGNDeckConverter"
	${Else}
		SetOutPath "$APPDATA\..\Local\Programs\OCTGN\$1\Plugins\OCTGNDeckConverter"
	${EndIf}
  
	; Put file there
	File ..\OCTGNDeckConverter\bin\Release\OCTGNDeckConverter.dll
	Goto exit

	error:
		MessageBox MB_OK "The OCTGN data.path file could not be found, check if OCTGN is installed or report bug."
	exit:
		FileClose $0
SectionEnd


Function WelcomeShowCallback
	SendMessage $mui.WelcomePage.Text ${WM_SETTEXT} 0 "STR:$(MUI_TEXT_WELCOME_INFO_TEXT)"
FunctionEnd
