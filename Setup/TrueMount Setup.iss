; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppID={{98BFC619-6202-49CA-BEA6-D26F786FAC7D}
AppName=TrueMount
AppVersion=2.7.19.88
AppVerName=TrueMount 2
AppPublisher=Nefarius
AppPublisherURL=http://nefarius.darkhosters.net/windows/truemount2
AppSupportURL=http://nefarius.darkhosters.net/windows/truemount2
AppUpdatesURL=http://nefarius.darkhosters.net/windows/truemount2
DefaultDirName={pf}\TrueMount
DefaultGroupName=TrueMount
AllowNoIcons=false
OutputDir=M:\Development\C#\TrueMount 2
OutputBaseFilename=setup
Compression=lzma/Max
SolidCompression=false
WizardImageFile="M:\Development\C#\TrueMount 2\Setup\SetupImage.bmp"
WizardSmallImageFile="M:\Development\C#\TrueMount 2\Setup\SetupImageSmall.bmp"
MinVersion=,5.0.2195
VersionInfoVersion=1.0
VersionInfoCompany=Nefarius
VersionInfoDescription=TrueMount software installer
VersionInfoCopyright=Benjamin H�glinger 2010
VersionInfoProductName=TrueMount
VersionInfoProductVersion=2
SetupIconFile=M:\Development\C#\TrueMount 2\TrueMount\1276686629_preferences-desktop-cryptography.ico

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "german"; MessagesFile: "compiler:Languages\German.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
; NOTE: Don't use "Flags: ignoreversion" on any shared system files
DestDir: {app}; Source: "M:\Development\C#\TrueMount 2\Release\*"; Excludes: ".svn"; Flags: recursesubdirs; 

[Icons]
Name: "{group}\TrueMount"; Filename: "{app}\TrueMount.exe"
Name: "{commondesktop}\TrueMount"; Filename: "{app}\TrueMount.exe"; Tasks: desktopicon

[Run]
Filename: "{app}\TrueMount.exe"; Description: "{cm:LaunchProgram,TrueMount}"; Flags: nowait postinstall skipifsilent

[Dirs]
