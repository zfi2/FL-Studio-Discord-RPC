; FL Studio Discord RPC Installer Script
; Created with Inno Setup

#define MyAppName "FL Studio Discord RPC"
#define MyAppVersion "1.0"
#define MyAppPublisher "zfi2"
#define MyAppURL "https://github.com/zfi2/FL-Studio-Discord-RPC"
#define MyAppExeName "FLStudioRPC.exe"

[Setup]
; App information
AppId={{FL-STUDIO-DISCORD-RPC}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}

; Installation directory
DefaultDirName={autopf}\FL Studio Discord RPC
DisableDirPage=no
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes

; Output
OutputDir=installer_output
OutputBaseFilename=FLStudioRPC_Setup
SetupIconFile=FLStudioRPC.ico
Compression=lzma
SolidCompression=yes

; UI
WizardStyle=modern
PrivilegesRequired=lowest

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "Create a &desktop icon"; GroupDescription: "Additional icons:"; Flags: unchecked
Name: "autostart"; Description: "Open FL Studio Discord RPC on startup"; GroupDescription: "Startup options:"; Flags: checkedonce

[Files]
Source: "bin\Release\FLStudioRPC.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Release\FLStudioRPC.ico"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Release\FLStudioRPC.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Release\Colorful.Console.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Release\DiscordRPC.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Release\Newtonsoft.Json.dll"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
; Run the app after installation
Filename: "{app}\{#MyAppExeName}"; Description: "Launch FL Studio Discord RPC"; Flags: nowait postinstall skipifsilent

[Registry]
; Add to startup if user selected the option
Root: HKCU; Subkey: "Software\Microsoft\Windows\CurrentVersion\Run"; ValueType: string; ValueName: "FLStudioRPC"; ValueData: """{app}\{#MyAppExeName}"""; Tasks: autostart

[Code]
// Custom code to handle existing running instance
function InitializeSetup(): Boolean;
var
  ResultCode: Integer;
begin
  // Try to close any running instance
  if CheckForMutexes('FLStudioRPC') then
  begin
    if MsgBox('FL Studio Discord RPC is currently running. Setup will close it to continue. Continue?', mbConfirmation, MB_YESNO) = IDYES then
    begin
      // Kill the process
      Exec('taskkill', '/F /IM FLStudioRPC.exe', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
      Result := True;
    end
    else
      Result := False;
  end
  else
    Result := True;
end;
