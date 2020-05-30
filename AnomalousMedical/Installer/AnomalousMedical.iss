; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "Anomalous Medical Open Source"
#define MyAppVersion GetFileVersion("..\bin\Release\netcoreapp3.1\win-x64\Standalone.dll")
#define MyAppPublisher "Threax Software, LLC"
#define MyAppURL "https://www.anomalousmedical.com"
#define MyAppExeName "AnomalousMedical.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{823A12E8-2D9C-4507-9E23-AF9D648ABA1E}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={commonpf}\Anomalous Medical\OpenSource
DefaultGroupName=Anomalous Medical Open Source
LicenseFile=en.rtf
OutputDir=..\bin\Release\Setups
OutputBaseFilename=AnomalousMedicalSetup
Compression=lzma
SolidCompression=yes
UninstallDisplayIcon={app}\AnomalousMedical.exe,0

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: ..\bin\Release\netcoreapp3.1\win-x64\publish\AnomalousMedical.exe; DestDir: {app}; Flags: ignoreversion 

;VS Redistributable
Source: "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\VC\Redist\MSVC\14.24.28127\vc_redist.x86.exe"; DestDir: "{tmp}"; Flags: ignoreversion deleteafterinstall
Source: "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\VC\Redist\MSVC\14.24.28127\vc_redist.x64.exe"; DestDir: "{tmp}"; Flags: ignoreversion deleteafterinstall

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{tmp}\vc_redist.x86.exe"; Parameters: "/q /norestart"; StatusMsg: "Installing Visual Studio Redistributable (x86)";
Filename: "{tmp}\vc_redist.x64.exe"; Parameters: "/q /norestart"; StatusMsg: "Installing Visual Studio Redistributable (x64)";
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, "&", "&&")}}"; Flags: nowait postinstall skipifsilent