;--------------------------------
;Include Modern UI

  !include "MUI2.nsh"

;--------------------------------

; The name of the installer
  Name "MTG Deck Converter v2.1.0"

; The file to write
  OutFile "MTGDeckConverterInstaller v2.1.0.exe"

; Request application privileges for Windows Vista/7/8
  RequestExecutionLevel user

  !define MUI_ICON "converter.ico"

;--------------------------------
;Interface Settings

;--------------------------------
;Pages

  !define MUI_TEXT_WELCOME_INFO_TEXT "This wizard will install the MTG Deck Converter plugin v2.1.0 for OCTGN.$\n$\nClick Install to start the installation."
  !define MUI_PAGE_CUSTOMFUNCTION_SHOW WelcomeShowCallback
  
  !insertmacro MUI_PAGE_WELCOME
  !insertmacro MUI_PAGE_INSTFILES

;--------------------------------
;Languages

  !insertmacro MUI_LANGUAGE "English"

;--------------------------------


Section "" ;No components page, name is not important

  ; Set output path to the installation directory.
  SetOutPath $DOCUMENTS\Octgn\Plugins\MTGDeckConverter
  
  ; Put file there
  File ..\MTGDeckConverter\bin\Release\MTGDeckConverter.dll
  
SectionEnd ; end the section


Function WelcomeShowCallback
  SendMessage $mui.WelcomePage.Text ${WM_SETTEXT} 0 "STR:$(MUI_TEXT_WELCOME_INFO_TEXT)"
FunctionEnd