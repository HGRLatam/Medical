; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "Anomalous Medical Internal"
#define MyAppVersion GetFileVersion("..\..\..\Release\AnomalousMedical.exe")
#define MyAppPublisher "Anomalous Medical"
#define MyAppURL "http://www.anomalousmedical.com"
#define MyAppExeName "AnomalousMedical.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{26D84768-1722-4000-A6E4-4431D9FCD462}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\Anomalous Medical\InternalPlatform
DefaultGroupName=Anomalous Medical
LicenseFile=..\..\..\AnomalousMedical\Installer\License\en.rtf
OutputDir=..\..\..\Release\Setups
OutputBaseFilename=AnomalousMedicalSetup
Compression=lzma
SolidCompression=yes
SignTool=AnomalousMedicalSign $qAnomalous Medical Internal Installer$q $f
UninstallDisplayIcon={app}\AnomalousMedical.exe,0

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: ..\..\..\Release\AnomalousMedical.exe; DestDir: {app}; Flags: ignoreversion 
Source: ..\..\..\Release\AnomalousMedical64.exe; DestDir: {app}; Flags: ignoreversion 
Source: ..\..\..\Release\BEPUikPlugin.dll; DestDir: {app}; Flags: ignoreversion
Source: ..\..\..\Release\BulletPlugin.dll; DestDir: {app}; Flags: ignoreversion
Source: ..\..\..\Release\Engine.dll; DestDir: {app}; Flags: ignoreversion
Source: ..\..\..\Release\MyGUIPlugin.dll; DestDir: {app}; Flags: ignoreversion
Source: ..\..\..\Release\libRocketPlugin.dll; DestDir: {app}; Flags: ignoreversion
Source: ..\..\..\Release\OgrePlugin.dll; DestDir: {app}; Flags: ignoreversion
Source: ..\..\..\Release\ShapeLoader.dll; DestDir: {app}; Flags: ignoreversion
Source: ..\..\..\Release\Simulation.dll; DestDir: {app}; Flags: ignoreversion
Source: ..\..\..\Release\SoundPlugin.dll; DestDir: {app}; Flags: ignoreversion
Source: ..\..\..\Release\Standalone.dll; DestDir: {app}; Flags: ignoreversion
Source: ..\..\..\Release\DotNetZip.dll; DestDir: {app}; Flags: ignoreversion
Source: ..\..\..\Release\Mono.Anomalous.Security.dll; DestDir: {app}; Flags: ignoreversion
Source: ..\..\..\Release\FreeImageNET.dll; DestDir: {app}; Flags: ignoreversion
Source: ..\..\..\Release\Lucene.Net.dll; DestDir: {app}; Flags: ignoreversion
Source: ..\..\..\Release\GuiFramework.dll; DestDir: {app}; Flags: ignoreversion
Source: ..\..\..\Release\GuiFramework.Cameras.dll; DestDir: {app}; Flags: ignoreversion
Source: ..\..\..\Release\GuiFramework.Editor.dll; DestDir: {app}; Flags: ignoreversion
Source: ..\..\..\Release\libRocketWidget.dll; DestDir: {app}; Flags: ignoreversion
Source: ..\..\..\Release\OSPlatform.dll; DestDir: {app}; Flags: ignoreversion
Source: ..\..\..\Release\AnomalousMedical.dat; DestDir: {app}; Flags: ignoreversion

;x86 Files
Source: ..\..\..\Release\x86\BulletWrapper.dll; DestDir: {app}\x86; Flags: ignoreversion
Source: ..\..\..\Release\x86\d3dcompiler_47.dll; DestDir: {app}\x86; Flags: ignoreversion
Source: ..\..\..\Release\x86\FreeImage.dll; DestDir: {app}\x86; Flags: ignoreversion
Source: ..\..\..\Release\x86\libRocketWrapper.dll; DestDir: {app}\x86; Flags: ignoreversion
Source: ..\..\..\Release\x86\MyGUIWrapper.dll; DestDir: {app}\x86; Flags: ignoreversion
Source: ..\..\..\Release\x86\OgreCWrapper.dll; DestDir: {app}\x86; Flags: ignoreversion
Source: ..\..\..\Release\x86\OgreMain.dll; DestDir: {app}\x86; Flags: ignoreversion
Source: ..\..\..\Release\x86\OpenAL32.dll; DestDir: {app}\x86; Flags: ignoreversion
Source: ..\..\..\Release\x86\OSHelper.dll; DestDir: {app}\x86; Flags: ignoreversion
Source: ..\..\..\Release\x86\RenderSystem_Direct3D11.dll; DestDir: {app}\x86; Flags: ignoreversion
Source: ..\..\..\Release\x86\RenderSystem_GL.dll; DestDir: {app}\x86; Flags: ignoreversion
Source: ..\..\..\Release\x86\SoundWrapper.dll; DestDir: {app}\x86; Flags: ignoreversion
Source: ..\..\..\Release\x86\WinMTDriver.dll; DestDir: {app}\x86; Flags: ignoreversion
Source: ..\..\..\Release\x86\Zip.dll; DestDir: {app}\x86; Flags: ignoreversion

;x64 Files
Source: ..\..\..\Release\x64\BulletWrapper.dll; DestDir: {app}\x64; Flags: ignoreversion
Source: ..\..\..\Release\x64\d3dcompiler_47.dll; DestDir: {app}\x64; Flags: ignoreversion
Source: ..\..\..\Release\x64\FreeImage.dll; DestDir: {app}\x64; Flags: ignoreversion
Source: ..\..\..\Release\x64\libRocketWrapper.dll; DestDir: {app}\x64; Flags: ignoreversion
Source: ..\..\..\Release\x64\MyGUIWrapper.dll; DestDir: {app}\x64; Flags: ignoreversion
Source: ..\..\..\Release\x64\OgreCWrapper.dll; DestDir: {app}\x64; Flags: ignoreversion
Source: ..\..\..\Release\x64\OgreMain.dll; DestDir: {app}\x64; Flags: ignoreversion
Source: ..\..\..\Release\x64\OpenAL32.dll; DestDir: {app}\x64; Flags: ignoreversion
Source: ..\..\..\Release\x64\OSHelper.dll; DestDir: {app}\x64; Flags: ignoreversion
Source: ..\..\..\Release\x64\RenderSystem_Direct3D11.dll; DestDir: {app}\x64; Flags: ignoreversion
Source: ..\..\..\Release\x64\RenderSystem_GL.dll; DestDir: {app}\x64; Flags: ignoreversion
Source: ..\..\..\Release\x64\SoundWrapper.dll; DestDir: {app}\x64; Flags: ignoreversion
Source: ..\..\..\Release\x64\WinMTDriver.dll; DestDir: {app}\x64; Flags: ignoreversion
Source: ..\..\..\Release\x64\Zip.dll; DestDir: {app}\x64; Flags: ignoreversion

;Microcode Caches
Source: ..\..\..\Release\Direct3D11 Rendering Subsystem.mcc; DestDir: {app}; Flags: ignoreversion
Source: ..\..\..\Release\OpenGL Rendering Subsystem.mcc; DestDir: {app}; Flags: ignoreversion

;VS 2013 Redistributable
Source: "S:\dependencies\InstallerDependencies\Windows\vcredist_x86.exe"; DestDir: "{tmp}"; Flags: ignoreversion deleteafterinstall
Source: "S:\dependencies\InstallerDependencies\Windows\vcredist_x64.exe"; DestDir: "{tmp}"; Flags: ignoreversion deleteafterinstall

;.Net 4.5.2
Source: S:\dependencies\InstallerDependencies\Windows\NDP452-KB2901954-Web.exe; DestDir: {tmp}; 

;Old files to delete, this will probably have to stay pretty much forever unless we rename these dlls
[InstallDelete]
Type: files; Name: {app}\BulletWrapper.dll;
Type: files; Name: {app}\d3dcompiler_47.dll;
Type: files; Name: {app}\FreeImage.dll;
Type: files; Name: {app}\libRocketWrapper.dll;
Type: files; Name: {app}\MyGUIWrapper.dll;
Type: files; Name: {app}\OgreCWrapper.dll;
Type: files; Name: {app}\OgreMain.dll;
Type: files; Name: {app}\OpenAL32.dll;
Type: files; Name: {app}\OSHelper.dll;
Type: files; Name: {app}\RenderSystem_Direct3D11.dll;
Type: files; Name: {app}\RenderSystem_GL.dll;
Type: files; Name: {app}\SoundWrapper.dll;
Type: files; Name: {app}\WinMTDriver.dll;
Type: files; Name: {app}\Zip.dll;
Type: files; Name: {app}\IntroductionTutorial.dat;

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{tmp}\vcredist_x86.exe"; Parameters: "/q /norestart"; StatusMsg: "Installing Visual Studio 2013 Redistributable (x86)";
Filename: "{tmp}\vcredist_x64.exe"; Parameters: "/q /norestart"; StatusMsg: "Installing Visual Studio 2013 Redistributable (x64)";
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, "&", "&&")}}"; Flags: nowait postinstall skipifsilent

[Code]
procedure checkdotnetfx4();
var
  resultCode: Integer;
	release: cardinal;
begin
	RegQueryDWordValue(HKLM, 'Software\Microsoft\NET Framework Setup\NDP\v4\Full', 'Release', release);
	if (release < 379893) then
	begin
    if MsgBox('You need to install the Microsoft .Net Framework 4.5.2.'#13#10'If you are connected to the internet you can do this now.'#13#10'Would you like to continue?', mbConfirmation, MB_YESNO) = IDYES then
      begin
        Exec(ExpandConstant('{tmp}\NDP452-KB2901954-Web.exe'), '/norestart', '', SW_SHOW, ewWaitUntilTerminated, resultCode);
      end
    else
      begin
        MsgBox('You must install the Microsoft .Net Framework 4.5.2 for this program to work.'#13#10'Please visit www.anomalousmedical.com for more info.', mbInformation, MB_OK)
      end;
   end;
//	   if resultCode=3010 then
	   //Restart required
end;

procedure CurStepChanged(CurStep: TSetupStep);
begin
  if(CurStep = ssPostInstall) then
    checkdotnetfx4();
end;
