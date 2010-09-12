namespace TrueMount.ROOT.CIMV2 {
    using System;
    using System.ComponentModel;
    using System.Management;
    using System.Collections;
    using System.Globalization;
    using System.ComponentModel.Design.Serialization;
    using System.Reflection;
    
    
    // Functions ShouldSerialize<PropertyName> are functions used by VS property browser to check if a particular property has to be serialized. These functions are added for all ValueType properties ( properties of type Int32, BOOL etc.. which cannot be set to null). These functions use Is<PropertyName>Null function. These functions are also used in the TypeConverter implementation for the properties to check for NULL value of property so that an empty value can be shown in Property browser in case of Drag and Drop in Visual studio.
    // Functions Is<PropertyName>Null() are used to check if a property is NULL.
    // Functions Reset<PropertyName> are added for Nullable Read/Write properties. These functions are used by VS designer in property browser to set a property to NULL.
    // Every property added to the class for WMI property has attributes set to define its behavior in Visual Studio designer and also to define a TypeConverter to be used.
    // Datetime conversion functions ToDateTime and ToDmtfDateTime are added to the class to convert DMTF datetime to System.DateTime and vice-versa.
    // An Early Bound class generated for the WMI class.Win32_ComputerSystem
    public class ComputerSystem : System.ComponentModel.Component {
        
        // Private property to hold the WMI namespace in which the class resides.
        private static string CreatedWmiNamespace = "ROOT\\CIMV2";
        
        // Private property to hold the name of WMI class which created this class.
        private static string CreatedClassName = "Win32_ComputerSystem";
        
        // Private member variable to hold the ManagementScope which is used by the various methods.
        private static System.Management.ManagementScope statMgmtScope = null;
        
        private ManagementSystemProperties PrivateSystemProperties;
        
        // Underlying lateBound WMI object.
        private System.Management.ManagementObject PrivateLateBoundObject;
        
        // Member variable to store the 'automatic commit' behavior for the class.
        private bool AutoCommitProp;
        
        // Private variable to hold the embedded property representing the instance.
        private System.Management.ManagementBaseObject embeddedObj;
        
        // The current WMI object used
        private System.Management.ManagementBaseObject curObj;
        
        // Flag to indicate if the instance is an embedded object.
        private bool isEmbedded;
        
        // Below are different overloads of constructors to initialize an instance of the class with a WMI object.
        public ComputerSystem() {
            this.InitializeObject(null, null, null);
        }
        
        public ComputerSystem(string keyName) {
            this.InitializeObject(null, new System.Management.ManagementPath(ComputerSystem.ConstructPath(keyName)), null);
        }
        
        public ComputerSystem(System.Management.ManagementScope mgmtScope, string keyName) {
            this.InitializeObject(((System.Management.ManagementScope)(mgmtScope)), new System.Management.ManagementPath(ComputerSystem.ConstructPath(keyName)), null);
        }
        
        public ComputerSystem(System.Management.ManagementPath path, System.Management.ObjectGetOptions getOptions) {
            this.InitializeObject(null, path, getOptions);
        }
        
        public ComputerSystem(System.Management.ManagementScope mgmtScope, System.Management.ManagementPath path) {
            this.InitializeObject(mgmtScope, path, null);
        }
        
        public ComputerSystem(System.Management.ManagementPath path) {
            this.InitializeObject(null, path, null);
        }
        
        public ComputerSystem(System.Management.ManagementScope mgmtScope, System.Management.ManagementPath path, System.Management.ObjectGetOptions getOptions) {
            this.InitializeObject(mgmtScope, path, getOptions);
        }
        
        public ComputerSystem(System.Management.ManagementObject theObject) {
            Initialize();
            if ((CheckIfProperClass(theObject) == true)) {
                PrivateLateBoundObject = theObject;
                PrivateSystemProperties = new ManagementSystemProperties(PrivateLateBoundObject);
                curObj = PrivateLateBoundObject;
            }
            else {
                throw new System.ArgumentException("Class name does not match.");
            }
        }
        
        public ComputerSystem(System.Management.ManagementBaseObject theObject) {
            Initialize();
            if ((CheckIfProperClass(theObject) == true)) {
                embeddedObj = theObject;
                PrivateSystemProperties = new ManagementSystemProperties(theObject);
                curObj = embeddedObj;
                isEmbedded = true;
            }
            else {
                throw new System.ArgumentException("Class name does not match.");
            }
        }
        
        // Property returns the namespace of the WMI class.
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string OriginatingNamespace {
            get {
                return "ROOT\\CIMV2";
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ManagementClassName {
            get {
                string strRet = CreatedClassName;
                if ((curObj != null)) {
                    if ((curObj.ClassPath != null)) {
                        strRet = ((string)(curObj["__CLASS"]));
                        if (((strRet == null) 
                                    || (strRet == string.Empty))) {
                            strRet = CreatedClassName;
                        }
                    }
                }
                return strRet;
            }
        }
        
        // Property pointing to an embedded object to get System properties of the WMI object.
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ManagementSystemProperties SystemProperties {
            get {
                return PrivateSystemProperties;
            }
        }
        
        // Property returning the underlying lateBound object.
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public System.Management.ManagementBaseObject LateBoundObject {
            get {
                return curObj;
            }
        }
        
        // ManagementScope of the object.
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public System.Management.ManagementScope Scope {
            get {
                if ((isEmbedded == false)) {
                    return PrivateLateBoundObject.Scope;
                }
                else {
                    return null;
                }
            }
            set {
                if ((isEmbedded == false)) {
                    PrivateLateBoundObject.Scope = value;
                }
            }
        }
        
        // Property to show the commit behavior for the WMI object. If true, WMI object will be automatically saved after each property modification.(ie. Put() is called after modification of a property).
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool AutoCommit {
            get {
                return AutoCommitProp;
            }
            set {
                AutoCommitProp = value;
            }
        }
        
        // The ManagementPath of the underlying WMI object.
        [Browsable(true)]
        public System.Management.ManagementPath Path {
            get {
                if ((isEmbedded == false)) {
                    return PrivateLateBoundObject.Path;
                }
                else {
                    return null;
                }
            }
            set {
                if ((isEmbedded == false)) {
                    if ((CheckIfProperClass(null, value, null) != true)) {
                        throw new System.ArgumentException("Class name does not match.");
                    }
                    PrivateLateBoundObject.Path = value;
                }
            }
        }
        
        // Public static scope property which is used by the various methods.
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static System.Management.ManagementScope StaticScope {
            get {
                return statMgmtScope;
            }
            set {
                statMgmtScope = value;
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsAdminPasswordStatusNull {
            get {
                if ((curObj["AdminPasswordStatus"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Die Eigenschaft \"AdminPasswordStatus\" gibt die Hardwaresicherheitseinstellungen f" +
            "ür den Status des Administratorkennworts an.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public AdminPasswordStatusValues AdminPasswordStatus {
            get {
                if ((curObj["AdminPasswordStatus"] == null)) {
                    return ((AdminPasswordStatusValues)(System.Convert.ToInt32(4)));
                }
                return ((AdminPasswordStatusValues)(System.Convert.ToInt32(curObj["AdminPasswordStatus"])));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsAutomaticManagedPagefileNull {
            get {
                if ((curObj["AutomaticManagedPagefile"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"Die ""AutomaticManagedPagefile""-Eigenschaft bestimmt, ob die systemverwaltete Auslagerungsdatei aktiviert wird. Diese Funktion ist unter Windows Server 2003, XP und niedrigeren Versionen nicht verfügbar.
Werte: TRUE oder FALSE. Ist die Eigenschaft auf TRUE gesetzt, ist die automatische verwaltete Auslagerungsdatei aktiviert.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public bool AutomaticManagedPagefile {
            get {
                if ((curObj["AutomaticManagedPagefile"] == null)) {
                    return System.Convert.ToBoolean(0);
                }
                return ((bool)(curObj["AutomaticManagedPagefile"]));
            }
            set {
                curObj["AutomaticManagedPagefile"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsAutomaticResetBootOptionNull {
            get {
                if ((curObj["AutomaticResetBootOption"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Die Eigenschaft \"AutomaticResetBootOption\" legt fest, ob die Option zum automatis" +
            "chen Starten aktiviert ist, z. B. ob der Computer nach einem Systemfehler neu st" +
            "artet.\nWerte: TRUE oder FALSE. TRUE gibt an, dass die Option aktiviert ist.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public bool AutomaticResetBootOption {
            get {
                if ((curObj["AutomaticResetBootOption"] == null)) {
                    return System.Convert.ToBoolean(0);
                }
                return ((bool)(curObj["AutomaticResetBootOption"]));
            }
            set {
                curObj["AutomaticResetBootOption"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsAutomaticResetCapabilityNull {
            get {
                if ((curObj["AutomaticResetCapability"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"Die Eigenschaft ""AutomaticResetCapability"" legt fest, ob der automatische Neustart auf diesem Computer verfügbar ist. Diese Funktion ist nur in Windows NT verfügbar (nicht in Windows 95).
Werte: TRUE oder FALSE. TRUE gibt an, dass der automatische Neustart aktiviert ist.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public bool AutomaticResetCapability {
            get {
                if ((curObj["AutomaticResetCapability"] == null)) {
                    return System.Convert.ToBoolean(0);
                }
                return ((bool)(curObj["AutomaticResetCapability"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsBootOptionOnLimitNull {
            get {
                if ((curObj["BootOptionOnLimit"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Gibt den Systemvorgang an, der ausgeführt wird, wenn der Neustartlimit erreicht i" +
            "st.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public BootOptionOnLimitValues BootOptionOnLimit {
            get {
                if ((curObj["BootOptionOnLimit"] == null)) {
                    return ((BootOptionOnLimitValues)(System.Convert.ToInt32(4)));
                }
                return ((BootOptionOnLimitValues)(System.Convert.ToInt32(curObj["BootOptionOnLimit"])));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsBootOptionOnWatchDogNull {
            get {
                if ((curObj["BootOptionOnWatchDog"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Die Eigenschaft \"BootOptionOnWatchDog\" gibt den Neustartvorgang an, der ausgeführ" +
            "t wird, nachdem die Zeit für den Überwachungszeitgeber verstrichen ist.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public BootOptionOnWatchDogValues BootOptionOnWatchDog {
            get {
                if ((curObj["BootOptionOnWatchDog"] == null)) {
                    return ((BootOptionOnWatchDogValues)(System.Convert.ToInt32(4)));
                }
                return ((BootOptionOnWatchDogValues)(System.Convert.ToInt32(curObj["BootOptionOnWatchDog"])));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsBootROMSupportedNull {
            get {
                if ((curObj["BootROMSupported"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Die Eigenschaft \"BootROMSupported\" legt fest, ob ein Start-ROM unterstützt wird.\n" +
            "Werte: TRUE oder FALSE. Falls \"BootROMSupported\" gleich TRUE ist, bedeutet dies," +
            " dass ein Start-ROM unterstützt wird.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public bool BootROMSupported {
            get {
                if ((curObj["BootROMSupported"] == null)) {
                    return System.Convert.ToBoolean(0);
                }
                return ((bool)(curObj["BootROMSupported"]));
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Die Eigenschaft \"BootupState\" gibt an, wie das System gestartet wurde. Der abgesi" +
            "cherte Start umgeht die vom Benutzer festgelegten Startdateien.\nEinschränkungen:" +
            " Wert erforderlich.")]
        public string BootupState {
            get {
                return ((string)(curObj["BootupState"]));
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Die Eigenschaft \"Caption\" gibt eine kurze Textbeschreibung (eine Zeile) des Objek" +
            "ts an.")]
        public string Caption {
            get {
                return ((string)(curObj["Caption"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsChassisBootupStateNull {
            get {
                if ((curObj["ChassisBootupState"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Die Eigenschaft \"ChassisBootupState\" gibt den Startstatus des Gehäuses an.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public ChassisBootupStateValues ChassisBootupState {
            get {
                if ((curObj["ChassisBootupState"] == null)) {
                    return ((ChassisBootupStateValues)(System.Convert.ToInt32(0)));
                }
                return ((ChassisBootupStateValues)(System.Convert.ToInt32(curObj["ChassisBootupState"])));
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"Die Eigenschaft ""CreationClassName"" gibt den Namen der Klasse oder Teilklasse an, die beim Erstellen einer Instanz verwendet wird. Wenn diese Eigenschaft mit anderen Schlüsseleigenschaften dieser Klasse verwendet wird, können alle Instanzen der Klasse und der Teilklassen eindeutig erkannt werden.")]
        public string CreationClassName {
            get {
                return ((string)(curObj["CreationClassName"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsCurrentTimeZoneNull {
            get {
                if ((curObj["CurrentTimeZone"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Die Eigenschaft \"CurrentTimeZone\" gibt den Zeitraum für den Offset des unitären C" +
            "omputersystems von CUT (Coordinated Universal Time) an.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public short CurrentTimeZone {
            get {
                if ((curObj["CurrentTimeZone"] == null)) {
                    return System.Convert.ToInt16(0);
                }
                return ((short)(curObj["CurrentTimeZone"]));
            }
            set {
                curObj["CurrentTimeZone"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsDaylightInEffectNull {
            get {
                if ((curObj["DaylightInEffect"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Die Eigenschaft \"DaylightInEffect\" gibt an, ob die Sommerzeit angewendet wird. \nW" +
            "erte: TRUE oder FALSE. Wenn TRUE, wird die Sommerzeit zurzeit verwendet. Dies be" +
            "deutet fast immer, dass die aktuelle Zeit ein Stunde vor der Standardzeit ist.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public bool DaylightInEffect {
            get {
                if ((curObj["DaylightInEffect"] == null)) {
                    return System.Convert.ToBoolean(0);
                }
                return ((bool)(curObj["DaylightInEffect"]));
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Die Eigenschaft \"Description\" gibt eine Textbeschreibung des Objekts an. ")]
        public string Description {
            get {
                return ((string)(curObj["Description"]));
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Die DNSHostName-Eigenschaft gibt den DNS-Hostnamen des lokalen Computers an.")]
        public string DNSHostName {
            get {
                return ((string)(curObj["DNSHostName"]));
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Die Eigenschaft \"Domain\" gibt die Namen der Domäne an, zu der der Computer gehört" +
            ".")]
        public string Domain {
            get {
                return ((string)(curObj["Domain"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsDomainRoleNull {
            get {
                if ((curObj["DomainRole"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"Die Eigenschaft ""DomainRole"" gibt die Funktion des Computers in der Domäne/Arbeitsgruppe an. Die Domäne/Arbeitsgruppe besteht aus mehreren Computern im gleichen Netzwerk. Dieser Computer kann z. B. als ""Mitglied der Domäne/Arbeitsgruppe "" (Wert=1) angezeigt werden.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public DomainRoleValues DomainRole {
            get {
                if ((curObj["DomainRole"] == null)) {
                    return ((DomainRoleValues)(System.Convert.ToInt32(6)));
                }
                return ((DomainRoleValues)(System.Convert.ToInt32(curObj["DomainRole"])));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsEnableDaylightSavingsTimeNull {
            get {
                if ((curObj["EnableDaylightSavingsTime"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"Die Eigenschaft ""EnableDaylightSavings"" zeigt an, ob die Uhrzeit auf diesem Computer automatisch auf Sommerzeit bzw. Winterzeit umgestellt wird. FALSE - Die Uhrzeit wird nicht eine Stunde vor- oder nachgestellt. NULL - Der Status bezüglich automatischer Umstellung auf dem System ist nicht bekannt.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public bool EnableDaylightSavingsTime {
            get {
                if ((curObj["EnableDaylightSavingsTime"] == null)) {
                    return System.Convert.ToBoolean(0);
                }
                return ((bool)(curObj["EnableDaylightSavingsTime"]));
            }
            set {
                curObj["EnableDaylightSavingsTime"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsFrontPanelResetStatusNull {
            get {
                if ((curObj["FrontPanelResetStatus"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Die Eigenschaft \"FrontPanelResetStatus\" gibt die Hardwaresicherheitseinstellungen" +
            " für die Reset-Taste des Computers an.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public FrontPanelResetStatusValues FrontPanelResetStatus {
            get {
                if ((curObj["FrontPanelResetStatus"] == null)) {
                    return ((FrontPanelResetStatusValues)(System.Convert.ToInt32(4)));
                }
                return ((FrontPanelResetStatusValues)(System.Convert.ToInt32(curObj["FrontPanelResetStatus"])));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsInfraredSupportedNull {
            get {
                if ((curObj["InfraredSupported"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Die Eigenschaft \"InfraredSupported\" gibt an, ob im Computersystem ein Infrarotans" +
            "chluss vorhanden ist.\nWerte: TRUE oder FALSE. Falls \"InfraredSupported\" gleich T" +
            "RUE ist, bedeutet dies, dass ein Infrarotanschluss vorhanden ist.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public bool InfraredSupported {
            get {
                if ((curObj["InfraredSupported"] == null)) {
                    return System.Convert.ToBoolean(0);
                }
                return ((bool)(curObj["InfraredSupported"]));
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Dieses Objekt enthält die zum Suchen des ursprünglichen Ladegeräts oder des Start" +
            "dienstes erforderlichen Daten, um den Start des Betriebssystems anzufordern. Die" +
            " Ladeparameter (z. B. Pfadname und Parameter) können zusätzlich angegeben werden" +
            ".")]
        public string[] InitialLoadInfo {
            get {
                return ((string[])(curObj["InitialLoadInfo"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsInstallDateNull {
            get {
                if ((curObj["InstallDate"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Die Eigenschaft \"InstallDate\" gibt an, wann das Objekt installiert wurde. Wenn de" +
            "r Wert nicht angegeben ist, kann das Objekt trotzdem installiert sein.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public System.DateTime InstallDate {
            get {
                if ((curObj["InstallDate"] != null)) {
                    return ToDateTime(((string)(curObj["InstallDate"])));
                }
                else {
                    return System.DateTime.MinValue;
                }
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsKeyboardPasswordStatusNull {
            get {
                if ((curObj["KeyboardPasswordStatus"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Die Eigenschaft \"KeyboardPasswordStatus\" gibt die Hardwaresicherheitseinstellunge" +
            "n für den Status des Tastaturkennworts an.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public KeyboardPasswordStatusValues KeyboardPasswordStatus {
            get {
                if ((curObj["KeyboardPasswordStatus"] == null)) {
                    return ((KeyboardPasswordStatusValues)(System.Convert.ToInt32(4)));
                }
                return ((KeyboardPasswordStatusValues)(System.Convert.ToInt32(curObj["KeyboardPasswordStatus"])));
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"Dieses Objekt enthält die Daten, die entweder das ursprüngliche Ladegerät (dessen Schlüssel) oder den Startdienst, der den letzten Ladevorgangs des Betriebssystems angefordert hat, identifizieren. Die Ladeparameter (z. B. Pfadname und Parameter) können zusätzlich angegeben werden.")]
        public string LastLoadInfo {
            get {
                return ((string)(curObj["LastLoadInfo"]));
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Die Eigenschaft \"Manufacturer\" gibt den Hersteller des Computers an.\nBeispiel: Ac" +
            "me")]
        public string Manufacturer {
            get {
                return ((string)(curObj["Manufacturer"]));
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Die Eigenschaft \"Model\" gibt den Modellnamen des Computers an.")]
        public string Model {
            get {
                return ((string)(curObj["Model"]));
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Die Eigenschaft \"Name\" definiert die Objektbezeichnung. Wenn sich diese Eigenscha" +
            "ft in einer Teilklasse befindet, kann die Eigenschaft \"Name\" als Eigenschaft \"Ke" +
            "y\" überschrieben werden.")]
        public string Name {
            get {
                return ((string)(curObj["Name"]));
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"Das Objekt ""CIM_ComputerSystem"" und seine abgeleiteten Funktionen sind Objekte der obersten Ebene von CIM, die den Bereich für zahlreiche Komponenten definieren. Eindeutige ""CIM_System""-Schlüssel sind erforderlich. Zum Erstellen des ""CIM_ComputerSystem""-Namens wird eine Heuristik definiert, um immer den gleichen Namen zu generieren. Es wird empfohlen, die Heuristik zu verwenden. Die Eigenschaft ""NameFormat"" gibt an, wie der Systemname mit einer Heuristik generiert wurde. Die Heuristik wird in der Spezifikation ""CIM V2 Common Model"" beschrieben. In ""NameFormat"" wird die Reihenfolge zum Zuordnen des Computersystemnamens definiert. Der mit der Heuristik erstellte ""CIM_ComputerSystem""-Name ist der Schlüsselwert des Systems. Es können andere Namen zugeordnet und für ""CIM_ComputerSystem"" verwendet werden (durch Aliase).")]
        public string NameFormat {
            get {
                return ((string)(curObj["NameFormat"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsNetworkServerModeEnabledNull {
            get {
                if ((curObj["NetworkServerModeEnabled"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Die Eigenschaft \"NetworkServerModeEnabled\" legt fest, ob der Netzwerkservermodus " +
            "aktiviert ist.\nWerte: TRUE oder FALSE. TRUE gibt an, dass der Netzwerkservermodu" +
            "s aktiviert ist.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public bool NetworkServerModeEnabled {
            get {
                if ((curObj["NetworkServerModeEnabled"] == null)) {
                    return System.Convert.ToBoolean(0);
                }
                return ((bool)(curObj["NetworkServerModeEnabled"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsNumberOfLogicalProcessorsNull {
            get {
                if ((curObj["NumberOfLogicalProcessors"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Die \"NumberOfLogicalProcessors\"-Eigenschaft gibt die Anzahl von logischen Prozess" +
            "oren an, die derzeit im System verfügbar sind.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint NumberOfLogicalProcessors {
            get {
                if ((curObj["NumberOfLogicalProcessors"] == null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["NumberOfLogicalProcessors"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsNumberOfProcessorsNull {
            get {
                if ((curObj["NumberOfProcessors"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"Die Eigenschaft ""NumberOfProcessors"" gibt die Anzahl der verfügbaren physikalischen Prozessoren an, nicht die gesamten Prozessoren im System. Die verfügbaren Prozessoren können bestimmt werden, indem die Anzahl der dem Computersystemobjekt zugeordneten Prozessorinstanzen mit ""Win32_ComputerSystemProcessor"" aufgelistet werden.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint NumberOfProcessors {
            get {
                if ((curObj["NumberOfProcessors"] == null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["NumberOfProcessors"]));
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Das Array \"OEMLogoBitmap\" enthält die Daten für eine vom OEM erstellte Bitmap.")]
        public byte[] OEMLogoBitmap {
            get {
                return ((byte[])(curObj["OEMLogoBitmap"]));
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Diese Struktur enthält vom OEM definierte formfreie Zeichenfolgen. Beispiel: Teil" +
            "nummern für Referenzdokumente, Kontaktinformationen usw.")]
        public string[] OEMStringArray {
            get {
                return ((string[])(curObj["OEMStringArray"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsPartOfDomainNull {
            get {
                if ((curObj["PartOfDomain"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"Die Eigenschaft ""PartOfDomain"" gibt an, ob der Computer Mitglied in einer Domäne oder einer Arbeitsgruppe ist. Falls der Wert TRUE ist, ist der Computer Mitglied in einer Domäne. Falls der Wert FALSE ist, ist der Computer Mitglied in einer Arbeitsgruppe. Falls der Wert NULL ist, ist der Computer kein Mitglied einer Netzwerkgruppe oder die Mitgliedschaftsinformationen sind unbekannt.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public bool PartOfDomain {
            get {
                if ((curObj["PartOfDomain"] == null)) {
                    return System.Convert.ToBoolean(0);
                }
                return ((bool)(curObj["PartOfDomain"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsPauseAfterResetNull {
            get {
                if ((curObj["PauseAfterReset"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Die Eigenschaft \"PauseAfterReset\" gibt die Zeitverzögerung für den Neustart an. S" +
            "ie wird nach einem Energiezyklus, einem Neustart (lokal oder remote) und einem a" +
            "utomatischen Neustart verwendet. Der Wert -1 gibt an, dass der Verzögerungswert " +
            "unbekannt ist.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public long PauseAfterReset {
            get {
                if ((curObj["PauseAfterReset"] == null)) {
                    return System.Convert.ToInt64(0);
                }
                return ((long)(curObj["PauseAfterReset"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsPCSystemTypeNull {
            get {
                if ((curObj["PCSystemType"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Die \"PCSystemType\"-Eigenschaft gibt den Computertyp an, mit dem ein Benutzer arbe" +
            "itet, z. B. Laptop, Desktop, Tablet-PC usw. ")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public PCSystemTypeValues PCSystemType {
            get {
                if ((curObj["PCSystemType"] == null)) {
                    return ((PCSystemTypeValues)(System.Convert.ToInt32(9)));
                }
                return ((PCSystemTypeValues)(System.Convert.ToInt32(curObj["PCSystemType"])));
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"Gibt die Energie-spezifischen Funktionen eines Computersystems und des Betriebssystems an. Die Werte 0=""Unbekannt"", 1=""Nicht unterstützt"" und 2=""Deaktiviert"" können angegeben werden. Der Wert 3=""Aktiviert"" gibt an, dass die Energieverwaltungsfunktionen aktiviert sind, aber die exakte Funktion unbekannt ist oder die Informationen nicht verfügbar sind. ""Automatische Energiesparmodi"" (4) gibt an, dass ein System seinen Energiestatus basierend auf dem Energieverbrauch oder anderen Kriterien ändern kann. ""Energiestatus einstellbar"" (5) gibt an, dass die Methode ""SetPowerState"" unterstützt wird. ""Energiezyklus unterstützt"" (6) gibt an, dass die Methode ""SetPowerState"" mit dem Parameter ""PowerState"" 5 (""Energiezyklus"") ausgeführt werden kann. ""Geplante Reaktivierung unterstützt"" (7) gibt an, dass die Methode ""SetPowerState"" mit dem Parameter ""PowerState"" 5 (""Energiezyklus"") und dem Parameter ""Time"" ausgeführt werden kann.")]
        public PowerManagementCapabilitiesValues[] PowerManagementCapabilities {
            get {
                System.Array arrEnumVals = ((System.Array)(curObj["PowerManagementCapabilities"]));
                PowerManagementCapabilitiesValues[] enumToRet = new PowerManagementCapabilitiesValues[arrEnumVals.Length];
                int counter = 0;
                for (counter = 0; (counter < arrEnumVals.Length); counter = (counter + 1)) {
                    enumToRet[counter] = ((PowerManagementCapabilitiesValues)(System.Convert.ToInt32(arrEnumVals.GetValue(counter))));
                }
                return enumToRet;
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsPowerManagementSupportedNull {
            get {
                if ((curObj["PowerManagementSupported"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"Boolescher Wert, der angibt, dass das Computersystem und das Betriebssystem die Energieverwaltung unterstützen. Dieser Wert zeigt nicht an, ob Energieverwaltungsfunktionen aktiviert sind, oder welche Funktionen unterstützt werden. Weitere Informationen finden Sie im Array ""PowerManagementCapabilities"". Wenn der Wert ""False"" ist, sollte der Wert 1 für die Zeichenfolge ""Nicht unterstützt"", der einzige Eintrag im Array sein.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public bool PowerManagementSupported {
            get {
                if ((curObj["PowerManagementSupported"] == null)) {
                    return System.Convert.ToBoolean(0);
                }
                return ((bool)(curObj["PowerManagementSupported"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsPowerOnPasswordStatusNull {
            get {
                if ((curObj["PowerOnPasswordStatus"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Die Eigenschaft \"PowerOnPasswordStatus\" gibt die Hardwaresicherheitseinstellungen" +
            " für den Status des Reaktivierungskennworts an.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public PowerOnPasswordStatusValues PowerOnPasswordStatus {
            get {
                if ((curObj["PowerOnPasswordStatus"] == null)) {
                    return ((PowerOnPasswordStatusValues)(System.Convert.ToInt32(4)));
                }
                return ((PowerOnPasswordStatusValues)(System.Convert.ToInt32(curObj["PowerOnPasswordStatus"])));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsPowerStateNull {
            get {
                if ((curObj["PowerState"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"Gibt den aktuellen Energiestatus des Computersystems und des Betriebssystems an. Dieser wird wie folgt definiert: Der Wert 4 (Unbekannt) gibt an, dass sich das System im Energiesparmodus befindet, aber der genaue Status unbekannt ist. 2 (Niedriger Energiestatus) gibt an, dass sich das System im Energiesparmodus befindet, aber noch funktioniert und die Leistung verringert ist. 3 (Standby) gibt an, dass das System nicht funktioniert, aber schnell reaktiviert werden kann. 7 (Warnung) gibt an, dass sich das System sowohl in einem Warnungs- als auch in einem Energiesparmodus befindet.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public PowerStateValues PowerState {
            get {
                if ((curObj["PowerState"] == null)) {
                    return ((PowerStateValues)(System.Convert.ToInt32(10)));
                }
                return ((PowerStateValues)(System.Convert.ToInt32(curObj["PowerState"])));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsPowerSupplyStateNull {
            get {
                if ((curObj["PowerSupplyState"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Die Eigenschaft \"PowerSupplyState\" gibt den Status der Stromversorgung des Gehäus" +
            "es beim letzten Neustart an.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public PowerSupplyStateValues PowerSupplyState {
            get {
                if ((curObj["PowerSupplyState"] == null)) {
                    return ((PowerSupplyStateValues)(System.Convert.ToInt32(0)));
                }
                return ((PowerSupplyStateValues)(System.Convert.ToInt32(curObj["PowerSupplyState"])));
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Eine Zeichenfolge, die angibt, wie der primäre Systembesitzer erreicht werden kan" +
            "n; z. B. Rufnummer, E-Mail-Adresse usw.")]
        public string PrimaryOwnerContact {
            get {
                return ((string)(curObj["PrimaryOwnerContact"]));
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Der Name des primären Systembesitzers.")]
        public string PrimaryOwnerName {
            get {
                return ((string)(curObj["PrimaryOwnerName"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsResetCapabilityNull {
            get {
                if ((curObj["ResetCapability"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"Bei Aktivierung (Wert = 4) kann das unitäre Computersystem über die Hardware (z. B. den Netzschalter) zurückgesetzt werden. Bei Deaktivierung (Wert = 3) ist das Zurücksetzen über die Hardware nicht zugelassen. Es können auch andere Werte für die Eigenschaft definiert werden: ""Nicht implementiert"" (5), ""Andere"" (1) und ""Unbekannt"" (2).")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public ResetCapabilityValues ResetCapability {
            get {
                if ((curObj["ResetCapability"] == null)) {
                    return ((ResetCapabilityValues)(System.Convert.ToInt32(0)));
                }
                return ((ResetCapabilityValues)(System.Convert.ToInt32(curObj["ResetCapability"])));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsResetCountNull {
            get {
                if ((curObj["ResetCount"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Die Eigenschaft \"ResetCount\" gibt die Anzahl der automatischen Neustarts seit dem" +
            " letzten manuellen Neustart an. Der Wert 1 gibt an, dass die Anzahl unbekannt is" +
            "t.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public short ResetCount {
            get {
                if ((curObj["ResetCount"] == null)) {
                    return System.Convert.ToInt16(0);
                }
                return ((short)(curObj["ResetCount"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsResetLimitNull {
            get {
                if ((curObj["ResetLimit"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Die Eigenschaft \"ResetLimit\" gibt die Anzahl der aufeinander folgenden Neustarts " +
            "an. Der Wert 1 gibt an, dass das Limit unbekannt ist.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public short ResetLimit {
            get {
                if ((curObj["ResetLimit"] == null)) {
                    return System.Convert.ToInt16(0);
                }
                return ((short)(curObj["ResetLimit"]));
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"Ein Array bzw. eine Ansammlung von Zeichenfolgen, die die Funktion des Systems in der IT-Umgebung angeben. Teilklassen des Systems können diese Eigenschaft außer Kraft setzen, um explizite Rollenwerte festzulegen. Ansonsten kann eine Arbeitsgruppe die Heuristiken, Konventionen und Richtlinien für bestimmte Rollen umschreiben. Z. B. kann die Eigenschaft ""Roles"" für eine Netzwerksysteminstanz die Zeichenfolge 'Schalter' oder 'Brücke' enthalten.")]
        public string[] Roles {
            get {
                return ((string[])(curObj["Roles"]));
            }
            set {
                curObj["Roles"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"Die Eigenschaft ""Status"" gibt den aktuellen Status des Objekts an. Es können betriebsbereite oder nicht betriebsbereite Zustände definiert werden. Betriebsbereite Zustände sind ""OK"", ""Heruntergestuft"" und ""Künftiger Fehler"". ""Künftiger Fehler"" gibt an, dass ein Element ordnungsgemäß funktioniert, aber in naher Zukunft ein Fehler auftreten wird. Ein Beispiel ist eine SMART-aktivierte Festplatte. Nicht betriebsbereite Zustände sind ""Fehler"", ""Starten"", ""Beenden"" und ""Dienst"". ""Dienst"" kann während des erneuten Spiegelns eines Datenträgers, beim erneuten Laden einer Benutzerberechtigungsliste oder einem anderen administrativen Vorgang zutreffen. Nicht alle Vorgänge sind online.")]
        public string Status {
            get {
                return ((string)(curObj["Status"]));
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Die Eigenschaft \"SupportContactDescription\" ist ein Array, das die Supportinforma" +
            "tionen für das Win32-Computersystem enthält.")]
        public string[] SupportContactDescription {
            get {
                return ((string[])(curObj["SupportContactDescription"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsSystemStartupDelayNull {
            get {
                if ((curObj["SystemStartupDelay"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"Die Eigenschaft ""SystemStartupDelay"" gibt die Verzögerungszeit vor dem Starten des Betriebssystems an.

Hinweis: Die Berechtigung ""SE_SYSTEM_ENVIRONMENT"" ist auf IA64-Bit-Systemen erforderlich. Diese Berechtigung ist auf 32-Bit-Systemen nicht erforderlich.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public ushort SystemStartupDelay {
            get {
                if ((curObj["SystemStartupDelay"] == null)) {
                    return System.Convert.ToUInt16(0);
                }
                return ((ushort)(curObj["SystemStartupDelay"]));
            }
            set {
                curObj["SystemStartupDelay"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"Das Eigenschaftsarray ""SystemStartupOptions"" zeigt die Startoptionen des Computersystems an. Hinweis: Diese Eigenschaft kann auf IA-64-Bit-Systemen nicht geschrieben werden.
Einschränkungen: Es muss ein Wert angegeben sein.

Hinweis: Die Berechtigung ""SE_SYSTEM_ENVIRONMENT"" ist auf IA64-Bit-Computersystemen erforderlich. Diese Berechtigungen ist auf anderen Systemen nicht vorausgesetzt.")]
        public string[] SystemStartupOptions {
            get {
                return ((string[])(curObj["SystemStartupOptions"]));
            }
            set {
                curObj["SystemStartupOptions"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsSystemStartupSettingNull {
            get {
                if ((curObj["SystemStartupSetting"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"Die Eigenschaft ""SystemStartupSetting"" gibt den Index des Standardstartprofils an. Gewöhnlich wird Null (0) zurückgegeben, da die Profilzeichenfolge beim Schreiben an den Anfang der Liste verschoben wird. Dadurch wird der Standardwert für Windows NT festgelegt.

Hinweis: Die Berechtigung ""SE_SYSTEM_ENVIRONMENT"" ist auf IA64-Bit-Computersystemen erforderlich. Diese Berechtigungen ist auf 32-Bit-Systemen nicht vorausgesetzt.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public byte SystemStartupSetting {
            get {
                if ((curObj["SystemStartupSetting"] == null)) {
                    return System.Convert.ToByte(0);
                }
                return ((byte)(curObj["SystemStartupSetting"]));
            }
            set {
                curObj["SystemStartupSetting"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Die Eigenschaft \"SystemType\" gibt den Systemtyp an, der auf dem Win32-Computer au" +
            "sgeführt wird.\nEinschränkungen: Wert erforderlich")]
        public string SystemType {
            get {
                return ((string)(curObj["SystemType"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsThermalStateNull {
            get {
                if ((curObj["ThermalState"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Die Eigenschaft \"ThermalState\" gibt den thermischen Status des Gehäuses beim letz" +
            "ten Neustart an.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public ThermalStateValues ThermalState {
            get {
                if ((curObj["ThermalState"] == null)) {
                    return ((ThermalStateValues)(System.Convert.ToInt32(0)));
                }
                return ((ThermalStateValues)(System.Convert.ToInt32(curObj["ThermalState"])));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsTotalPhysicalMemoryNull {
            get {
                if ((curObj["TotalPhysicalMemory"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Die Eigenschaft \"TotalPhysicalMemory\" gibt die Gesamtgröße des physikalischen Spe" +
            "ichers an.\nBeispiel: 67108864")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public ulong TotalPhysicalMemory {
            get {
                if ((curObj["TotalPhysicalMemory"] == null)) {
                    return System.Convert.ToUInt64(0);
                }
                return ((ulong)(curObj["TotalPhysicalMemory"]));
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Die Eigenschaft \"UserName\" gibt den Namen des angemeldeten Benutzers an.\nEinschrä" +
            "nkungen: Ein Wert ist erforderlich.\nBeispiel: johnsmith")]
        public string UserName {
            get {
                return ((string)(curObj["UserName"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsWakeUpTypeNull {
            get {
                if ((curObj["WakeUpType"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Die Eigenschaft \"WakeUp\" gibt das Ereignis an, das das System reaktiviert hat.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public WakeUpTypeValues WakeUpType {
            get {
                if ((curObj["WakeUpType"] == null)) {
                    return ((WakeUpTypeValues)(System.Convert.ToInt32(9)));
                }
                return ((WakeUpTypeValues)(System.Convert.ToInt32(curObj["WakeUpType"])));
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Die Eigenschaft \"Workgroup\" enthält den Namen der Arbeitsgruppe. Dieser Wert gilt" +
            " nur, wenn die Eigenschaft \"PartOfDomain\" auf FALSE gesetzt ist.")]
        public string Workgroup {
            get {
                return ((string)(curObj["Workgroup"]));
            }
            set {
                curObj["Workgroup"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        private bool CheckIfProperClass(System.Management.ManagementScope mgmtScope, System.Management.ManagementPath path, System.Management.ObjectGetOptions OptionsParam) {
            if (((path != null) 
                        && (string.Compare(path.ClassName, this.ManagementClassName, true, System.Globalization.CultureInfo.InvariantCulture) == 0))) {
                return true;
            }
            else {
                return CheckIfProperClass(new System.Management.ManagementObject(mgmtScope, path, OptionsParam));
            }
        }
        
        private bool CheckIfProperClass(System.Management.ManagementBaseObject theObj) {
            if (((theObj != null) 
                        && (string.Compare(((string)(theObj["__CLASS"])), this.ManagementClassName, true, System.Globalization.CultureInfo.InvariantCulture) == 0))) {
                return true;
            }
            else {
                System.Array parentClasses = ((System.Array)(theObj["__DERIVATION"]));
                if ((parentClasses != null)) {
                    int count = 0;
                    for (count = 0; (count < parentClasses.Length); count = (count + 1)) {
                        if ((string.Compare(((string)(parentClasses.GetValue(count))), this.ManagementClassName, true, System.Globalization.CultureInfo.InvariantCulture) == 0)) {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        
        private bool ShouldSerializeAdminPasswordStatus() {
            if ((this.IsAdminPasswordStatusNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeAutomaticManagedPagefile() {
            if ((this.IsAutomaticManagedPagefileNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetAutomaticManagedPagefile() {
            curObj["AutomaticManagedPagefile"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private bool ShouldSerializeAutomaticResetBootOption() {
            if ((this.IsAutomaticResetBootOptionNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetAutomaticResetBootOption() {
            curObj["AutomaticResetBootOption"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private bool ShouldSerializeAutomaticResetCapability() {
            if ((this.IsAutomaticResetCapabilityNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeBootOptionOnLimit() {
            if ((this.IsBootOptionOnLimitNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeBootOptionOnWatchDog() {
            if ((this.IsBootOptionOnWatchDogNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeBootROMSupported() {
            if ((this.IsBootROMSupportedNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeChassisBootupState() {
            if ((this.IsChassisBootupStateNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeCurrentTimeZone() {
            if ((this.IsCurrentTimeZoneNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetCurrentTimeZone() {
            curObj["CurrentTimeZone"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private bool ShouldSerializeDaylightInEffect() {
            if ((this.IsDaylightInEffectNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeDomainRole() {
            if ((this.IsDomainRoleNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeEnableDaylightSavingsTime() {
            if ((this.IsEnableDaylightSavingsTimeNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetEnableDaylightSavingsTime() {
            curObj["EnableDaylightSavingsTime"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private bool ShouldSerializeFrontPanelResetStatus() {
            if ((this.IsFrontPanelResetStatusNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeInfraredSupported() {
            if ((this.IsInfraredSupportedNull == false)) {
                return true;
            }
            return false;
        }
        
        // Converts a given datetime in DMTF format to System.DateTime object.
        static System.DateTime ToDateTime(string dmtfDate) {
            System.DateTime initializer = System.DateTime.MinValue;
            int year = initializer.Year;
            int month = initializer.Month;
            int day = initializer.Day;
            int hour = initializer.Hour;
            int minute = initializer.Minute;
            int second = initializer.Second;
            long ticks = 0;
            string dmtf = dmtfDate;
            System.DateTime datetime = System.DateTime.MinValue;
            string tempString = string.Empty;
            if ((dmtf == null)) {
                throw new System.ArgumentOutOfRangeException();
            }
            if ((dmtf.Length == 0)) {
                throw new System.ArgumentOutOfRangeException();
            }
            if ((dmtf.Length != 25)) {
                throw new System.ArgumentOutOfRangeException();
            }
            try {
                tempString = dmtf.Substring(0, 4);
                if (("****" != tempString)) {
                    year = int.Parse(tempString);
                }
                tempString = dmtf.Substring(4, 2);
                if (("**" != tempString)) {
                    month = int.Parse(tempString);
                }
                tempString = dmtf.Substring(6, 2);
                if (("**" != tempString)) {
                    day = int.Parse(tempString);
                }
                tempString = dmtf.Substring(8, 2);
                if (("**" != tempString)) {
                    hour = int.Parse(tempString);
                }
                tempString = dmtf.Substring(10, 2);
                if (("**" != tempString)) {
                    minute = int.Parse(tempString);
                }
                tempString = dmtf.Substring(12, 2);
                if (("**" != tempString)) {
                    second = int.Parse(tempString);
                }
                tempString = dmtf.Substring(15, 6);
                if (("******" != tempString)) {
                    ticks = (long.Parse(tempString) * ((long)((System.TimeSpan.TicksPerMillisecond / 1000))));
                }
                if (((((((((year < 0) 
                            || (month < 0)) 
                            || (day < 0)) 
                            || (hour < 0)) 
                            || (minute < 0)) 
                            || (minute < 0)) 
                            || (second < 0)) 
                            || (ticks < 0))) {
                    throw new System.ArgumentOutOfRangeException();
                }
            }
            catch (System.Exception e) {
                throw new System.ArgumentOutOfRangeException(null, e.Message);
            }
            datetime = new System.DateTime(year, month, day, hour, minute, second, 0);
            datetime = datetime.AddTicks(ticks);
            System.TimeSpan tickOffset = System.TimeZone.CurrentTimeZone.GetUtcOffset(datetime);
            int UTCOffset = 0;
            int OffsetToBeAdjusted = 0;
            long OffsetMins = ((long)((tickOffset.Ticks / System.TimeSpan.TicksPerMinute)));
            tempString = dmtf.Substring(22, 3);
            if ((tempString != "******")) {
                tempString = dmtf.Substring(21, 4);
                try {
                    UTCOffset = int.Parse(tempString);
                }
                catch (System.Exception e) {
                    throw new System.ArgumentOutOfRangeException(null, e.Message);
                }
                OffsetToBeAdjusted = ((int)((OffsetMins - UTCOffset)));
                datetime = datetime.AddMinutes(((double)(OffsetToBeAdjusted)));
            }
            return datetime;
        }
        
        // Converts a given System.DateTime object to DMTF datetime format.
        static string ToDmtfDateTime(System.DateTime date) {
            string utcString = string.Empty;
            System.TimeSpan tickOffset = System.TimeZone.CurrentTimeZone.GetUtcOffset(date);
            long OffsetMins = ((long)((tickOffset.Ticks / System.TimeSpan.TicksPerMinute)));
            if ((System.Math.Abs(OffsetMins) > 999)) {
                date = date.ToUniversalTime();
                utcString = "+000";
            }
            else {
                if ((tickOffset.Ticks >= 0)) {
                    utcString = string.Concat("+", ((System.Int64 )((tickOffset.Ticks / System.TimeSpan.TicksPerMinute))).ToString().PadLeft(3, '0'));
                }
                else {
                    string strTemp = ((System.Int64 )(OffsetMins)).ToString();
                    utcString = string.Concat("-", strTemp.Substring(1, (strTemp.Length - 1)).PadLeft(3, '0'));
                }
            }
            string dmtfDateTime = ((System.Int32 )(date.Year)).ToString().PadLeft(4, '0');
            dmtfDateTime = string.Concat(dmtfDateTime, ((System.Int32 )(date.Month)).ToString().PadLeft(2, '0'));
            dmtfDateTime = string.Concat(dmtfDateTime, ((System.Int32 )(date.Day)).ToString().PadLeft(2, '0'));
            dmtfDateTime = string.Concat(dmtfDateTime, ((System.Int32 )(date.Hour)).ToString().PadLeft(2, '0'));
            dmtfDateTime = string.Concat(dmtfDateTime, ((System.Int32 )(date.Minute)).ToString().PadLeft(2, '0'));
            dmtfDateTime = string.Concat(dmtfDateTime, ((System.Int32 )(date.Second)).ToString().PadLeft(2, '0'));
            dmtfDateTime = string.Concat(dmtfDateTime, ".");
            System.DateTime dtTemp = new System.DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, 0);
            long microsec = ((long)((((date.Ticks - dtTemp.Ticks) 
                        * 1000) 
                        / System.TimeSpan.TicksPerMillisecond)));
            string strMicrosec = ((System.Int64 )(microsec)).ToString();
            if ((strMicrosec.Length > 6)) {
                strMicrosec = strMicrosec.Substring(0, 6);
            }
            dmtfDateTime = string.Concat(dmtfDateTime, strMicrosec.PadLeft(6, '0'));
            dmtfDateTime = string.Concat(dmtfDateTime, utcString);
            return dmtfDateTime;
        }
        
        private bool ShouldSerializeInstallDate() {
            if ((this.IsInstallDateNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeKeyboardPasswordStatus() {
            if ((this.IsKeyboardPasswordStatusNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeNetworkServerModeEnabled() {
            if ((this.IsNetworkServerModeEnabledNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeNumberOfLogicalProcessors() {
            if ((this.IsNumberOfLogicalProcessorsNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeNumberOfProcessors() {
            if ((this.IsNumberOfProcessorsNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializePartOfDomain() {
            if ((this.IsPartOfDomainNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializePauseAfterReset() {
            if ((this.IsPauseAfterResetNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializePCSystemType() {
            if ((this.IsPCSystemTypeNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializePowerManagementSupported() {
            if ((this.IsPowerManagementSupportedNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializePowerOnPasswordStatus() {
            if ((this.IsPowerOnPasswordStatusNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializePowerState() {
            if ((this.IsPowerStateNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializePowerSupplyState() {
            if ((this.IsPowerSupplyStateNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeResetCapability() {
            if ((this.IsResetCapabilityNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeResetCount() {
            if ((this.IsResetCountNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeResetLimit() {
            if ((this.IsResetLimitNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetRoles() {
            curObj["Roles"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private bool ShouldSerializeSystemStartupDelay() {
            if ((this.IsSystemStartupDelayNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetSystemStartupDelay() {
            curObj["SystemStartupDelay"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private void ResetSystemStartupOptions() {
            curObj["SystemStartupOptions"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private bool ShouldSerializeSystemStartupSetting() {
            if ((this.IsSystemStartupSettingNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetSystemStartupSetting() {
            curObj["SystemStartupSetting"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private bool ShouldSerializeThermalState() {
            if ((this.IsThermalStateNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeTotalPhysicalMemory() {
            if ((this.IsTotalPhysicalMemoryNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeWakeUpType() {
            if ((this.IsWakeUpTypeNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetWorkgroup() {
            curObj["Workgroup"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        [Browsable(true)]
        public void CommitObject() {
            if ((isEmbedded == false)) {
                PrivateLateBoundObject.Put();
            }
        }
        
        [Browsable(true)]
        public void CommitObject(System.Management.PutOptions putOptions) {
            if ((isEmbedded == false)) {
                PrivateLateBoundObject.Put(putOptions);
            }
        }
        
        private void Initialize() {
            AutoCommitProp = true;
            isEmbedded = false;
        }
        
        private static string ConstructPath(string keyName) {
            string strPath = "ROOT\\CIMV2:Win32_ComputerSystem";
            strPath = string.Concat(strPath, string.Concat(".Name=", string.Concat("\"", string.Concat(keyName, "\""))));
            return strPath;
        }
        
        private void InitializeObject(System.Management.ManagementScope mgmtScope, System.Management.ManagementPath path, System.Management.ObjectGetOptions getOptions) {
            Initialize();
            if ((path != null)) {
                if ((CheckIfProperClass(mgmtScope, path, getOptions) != true)) {
                    throw new System.ArgumentException("Class name does not match.");
                }
            }
            PrivateLateBoundObject = new System.Management.ManagementObject(mgmtScope, path, getOptions);
            PrivateSystemProperties = new ManagementSystemProperties(PrivateLateBoundObject);
            curObj = PrivateLateBoundObject;
        }
        
        // Different overloads of GetInstances() help in enumerating instances of the WMI class.
        public static ComputerSystemCollection GetInstances() {
            return GetInstances(null, null, null);
        }
        
        public static ComputerSystemCollection GetInstances(string condition) {
            return GetInstances(null, condition, null);
        }
        
        public static ComputerSystemCollection GetInstances(System.String [] selectedProperties) {
            return GetInstances(null, null, selectedProperties);
        }
        
        public static ComputerSystemCollection GetInstances(string condition, System.String [] selectedProperties) {
            return GetInstances(null, condition, selectedProperties);
        }
        
        public static ComputerSystemCollection GetInstances(System.Management.ManagementScope mgmtScope, System.Management.EnumerationOptions enumOptions) {
            if ((mgmtScope == null)) {
                if ((statMgmtScope == null)) {
                    mgmtScope = new System.Management.ManagementScope();
                    mgmtScope.Path.NamespacePath = "root\\CIMV2";
                }
                else {
                    mgmtScope = statMgmtScope;
                }
            }
            System.Management.ManagementPath pathObj = new System.Management.ManagementPath();
            pathObj.ClassName = "Win32_ComputerSystem";
            pathObj.NamespacePath = "root\\CIMV2";
            System.Management.ManagementClass clsObject = new System.Management.ManagementClass(mgmtScope, pathObj, null);
            if ((enumOptions == null)) {
                enumOptions = new System.Management.EnumerationOptions();
                enumOptions.EnsureLocatable = true;
            }
            return new ComputerSystemCollection(clsObject.GetInstances(enumOptions));
        }
        
        public static ComputerSystemCollection GetInstances(System.Management.ManagementScope mgmtScope, string condition) {
            return GetInstances(mgmtScope, condition, null);
        }
        
        public static ComputerSystemCollection GetInstances(System.Management.ManagementScope mgmtScope, System.String [] selectedProperties) {
            return GetInstances(mgmtScope, null, selectedProperties);
        }
        
        public static ComputerSystemCollection GetInstances(System.Management.ManagementScope mgmtScope, string condition, System.String [] selectedProperties) {
            if ((mgmtScope == null)) {
                if ((statMgmtScope == null)) {
                    mgmtScope = new System.Management.ManagementScope();
                    mgmtScope.Path.NamespacePath = "root\\CIMV2";
                }
                else {
                    mgmtScope = statMgmtScope;
                }
            }
            System.Management.ManagementObjectSearcher ObjectSearcher = new System.Management.ManagementObjectSearcher(mgmtScope, new SelectQuery("Win32_ComputerSystem", condition, selectedProperties));
            System.Management.EnumerationOptions enumOptions = new System.Management.EnumerationOptions();
            enumOptions.EnsureLocatable = true;
            ObjectSearcher.Options = enumOptions;
            return new ComputerSystemCollection(ObjectSearcher.Get());
        }
        
        [Browsable(true)]
        public static ComputerSystem CreateInstance() {
            System.Management.ManagementScope mgmtScope = null;
            if ((statMgmtScope == null)) {
                mgmtScope = new System.Management.ManagementScope();
                mgmtScope.Path.NamespacePath = CreatedWmiNamespace;
            }
            else {
                mgmtScope = statMgmtScope;
            }
            System.Management.ManagementPath mgmtPath = new System.Management.ManagementPath(CreatedClassName);
            System.Management.ManagementClass tmpMgmtClass = new System.Management.ManagementClass(mgmtScope, mgmtPath, null);
            return new ComputerSystem(tmpMgmtClass.CreateInstance());
        }
        
        [Browsable(true)]
        public void Delete() {
            PrivateLateBoundObject.Delete();
        }
        
        public uint JoinDomainOrWorkgroup(string AccountOU, uint FJoinOptions, string Name, string Password, string UserName) {
            if ((isEmbedded == false)) {
                System.Management.ManagementBaseObject inParams = null;
                inParams = PrivateLateBoundObject.GetMethodParameters("JoinDomainOrWorkgroup");
                inParams["AccountOU"] = ((System.String )(AccountOU));
                inParams["FJoinOptions"] = ((System.UInt32 )(FJoinOptions));
                inParams["Name"] = ((System.String )(Name));
                inParams["Password"] = ((System.String )(Password));
                inParams["UserName"] = ((System.String )(UserName));
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("JoinDomainOrWorkgroup", inParams, null);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else {
                return System.Convert.ToUInt32(0);
            }
        }
        
        public uint Rename(string Name, string Password, string UserName) {
            if ((isEmbedded == false)) {
                System.Management.ManagementBaseObject inParams = null;
                inParams = PrivateLateBoundObject.GetMethodParameters("Rename");
                inParams["Name"] = ((System.String )(Name));
                inParams["Password"] = ((System.String )(Password));
                inParams["UserName"] = ((System.String )(UserName));
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("Rename", inParams, null);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else {
                return System.Convert.ToUInt32(0);
            }
        }
        
        public uint SetPowerState(ushort PowerState, System.DateTime Time) {
            if ((isEmbedded == false)) {
                System.Management.ManagementBaseObject inParams = null;
                inParams = PrivateLateBoundObject.GetMethodParameters("SetPowerState");
                inParams["PowerState"] = ((System.UInt16 )(PowerState));
                inParams["Time"] = ToDmtfDateTime(((System.DateTime)(Time)));
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("SetPowerState", inParams, null);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else {
                return System.Convert.ToUInt32(0);
            }
        }
        
        public uint UnjoinDomainOrWorkgroup(uint FUnjoinOptions, string Password, string UserName) {
            if ((isEmbedded == false)) {
                System.Management.ManagementBaseObject inParams = null;
                inParams = PrivateLateBoundObject.GetMethodParameters("UnjoinDomainOrWorkgroup");
                inParams["FUnjoinOptions"] = ((System.UInt32 )(FUnjoinOptions));
                inParams["Password"] = ((System.String )(Password));
                inParams["UserName"] = ((System.String )(UserName));
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("UnjoinDomainOrWorkgroup", inParams, null);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else {
                return System.Convert.ToUInt32(0);
            }
        }
        
        public enum AdminPasswordStatusValues {
            
            Deaktiviert = 0,
            
            Aktiviert = 1,
            
            Nicht_implementiert = 2,
            
            Unbekannt = 3,
            
            NULL_ENUM_VALUE = 4,
        }
        
        public enum BootOptionOnLimitValues {
            
            Reserviert = 0,
            
            Betriebssystem = 1,
            
            Systemprogramme = 2,
            
            Nicht_neu_starten = 3,
            
            NULL_ENUM_VALUE = 4,
        }
        
        public enum BootOptionOnWatchDogValues {
            
            Reserviert = 0,
            
            Betriebssystem = 1,
            
            Systemprogramme = 2,
            
            Nicht_neu_starten = 3,
            
            NULL_ENUM_VALUE = 4,
        }
        
        public enum ChassisBootupStateValues {
            
            Andere = 1,
            
            Unbekannt = 2,
            
            Sicher = 3,
            
            Warnung = 4,
            
            Kritisch = 5,
            
            Nicht_wiederherstellbar = 6,
            
            NULL_ENUM_VALUE = 0,
        }
        
        public enum DomainRoleValues {
            
            Eigenständige_Arbeitsstation = 0,
            
            Mitglied_der_Domäne_Arbeitsgruppe = 1,
            
            Eigenständiger_Server = 2,
            
            Mitgliedsserver = 3,
            
            Reservedomänencontroller = 4,
            
            Primärer_Domänencontroller = 5,
            
            NULL_ENUM_VALUE = 6,
        }
        
        public enum FrontPanelResetStatusValues {
            
            Deaktiviert = 0,
            
            Aktiviert = 1,
            
            Nicht_implementiert = 2,
            
            Unbekannt = 3,
            
            NULL_ENUM_VALUE = 4,
        }
        
        public enum KeyboardPasswordStatusValues {
            
            Deaktiviert = 0,
            
            Aktiviert = 1,
            
            Nicht_implementiert = 2,
            
            Unbekannt = 3,
            
            NULL_ENUM_VALUE = 4,
        }
        
        public enum PCSystemTypeValues {
            
            Nicht_angegeben = 0,
            
            Desktop = 1,
            
            Mobil = 2,
            
            Arbeitsstation = 3,
            
            Enterprise_Server = 4,
            
            SOHO_Server = 5,
            
            Appliance_PC = 6,
            
            Performance_Server = 7,
            
            Maximum = 8,
            
            NULL_ENUM_VALUE = 9,
        }
        
        public enum PowerManagementCapabilitiesValues {
            
            Unbekannt = 0,
            
            Nicht_unterstützt = 1,
            
            Deaktiviert = 2,
            
            Aktiviert = 3,
            
            Automatische_Energiesparmodi = 4,
            
            Energiestatus_einstellbar = 5,
            
            Energiezyklus_unterstützt = 6,
            
            Geplante_Reaktivierung_unterstützt = 7,
            
            NULL_ENUM_VALUE = 8,
        }
        
        public enum PowerOnPasswordStatusValues {
            
            Deaktiviert = 0,
            
            Aktiviert = 1,
            
            Nicht_implementiert = 2,
            
            Unbekannt = 3,
            
            NULL_ENUM_VALUE = 4,
        }
        
        public enum PowerStateValues {
            
            Unbekannt = 0,
            
            Kein_Energiesparmodus = 1,
            
            Energiesparmodus_Niedriger_Energiestatus = 2,
            
            Energiesparmodus_Standby = 3,
            
            Energiesparmodus_Unbekannt = 4,
            
            Energiezyklus = 5,
            
            Ausschalten = 6,
            
            Energiesparmodus_Warnung = 7,
            
            Energiesparmodus_Ruhezustand = 8,
            
            Energiesparmodus_Standby0 = 9,
            
            NULL_ENUM_VALUE = 10,
        }
        
        public enum PowerSupplyStateValues {
            
            Andere = 1,
            
            Unbekannt = 2,
            
            Sicher = 3,
            
            Warnung = 4,
            
            Kritisch = 5,
            
            Nicht_wiederherstellbar = 6,
            
            NULL_ENUM_VALUE = 0,
        }
        
        public enum ResetCapabilityValues {
            
            Andere = 1,
            
            Unbekannt = 2,
            
            Deaktiviert = 3,
            
            Aktiviert = 4,
            
            Nicht_implementiert = 5,
            
            NULL_ENUM_VALUE = 0,
        }
        
        public enum ThermalStateValues {
            
            Andere = 1,
            
            Unbekannt = 2,
            
            Sicher = 3,
            
            Warnung = 4,
            
            Kritisch = 5,
            
            Nicht_wiederherstellbar = 6,
            
            NULL_ENUM_VALUE = 0,
        }
        
        public enum WakeUpTypeValues {
            
            Reserviert = 0,
            
            Andere = 1,
            
            Unbekannt = 2,
            
            APM_Zeitgeber = 3,
            
            Modemrufzeichen = 4,
            
            LAN_Remote = 5,
            
            Netzschalter = 6,
            
            PCI_PME_ = 7,
            
            Netzbetrieb_wiederhergestellt = 8,
            
            NULL_ENUM_VALUE = 9,
        }
        
        // Enumerator implementation for enumerating instances of the class.
        public class ComputerSystemCollection : object, ICollection {
            
            private ManagementObjectCollection privColObj;
            
            public ComputerSystemCollection(ManagementObjectCollection objCollection) {
                privColObj = objCollection;
            }
            
            public virtual int Count {
                get {
                    return privColObj.Count;
                }
            }
            
            public virtual bool IsSynchronized {
                get {
                    return privColObj.IsSynchronized;
                }
            }
            
            public virtual object SyncRoot {
                get {
                    return this;
                }
            }
            
            public virtual void CopyTo(System.Array array, int index) {
                privColObj.CopyTo(array, index);
                int nCtr;
                for (nCtr = 0; (nCtr < array.Length); nCtr = (nCtr + 1)) {
                    array.SetValue(new ComputerSystem(((System.Management.ManagementObject)(array.GetValue(nCtr)))), nCtr);
                }
            }
            
            public virtual System.Collections.IEnumerator GetEnumerator() {
                return new ComputerSystemEnumerator(privColObj.GetEnumerator());
            }
            
            public class ComputerSystemEnumerator : object, System.Collections.IEnumerator {
                
                private ManagementObjectCollection.ManagementObjectEnumerator privObjEnum;
                
                public ComputerSystemEnumerator(ManagementObjectCollection.ManagementObjectEnumerator objEnum) {
                    privObjEnum = objEnum;
                }
                
                public virtual object Current {
                    get {
                        return new ComputerSystem(((System.Management.ManagementObject)(privObjEnum.Current)));
                    }
                }
                
                public virtual bool MoveNext() {
                    return privObjEnum.MoveNext();
                }
                
                public virtual void Reset() {
                    privObjEnum.Reset();
                }
            }
        }
        
        // TypeConverter to handle null values for ValueType properties
        public class WMIValueTypeConverter : TypeConverter {
            
            private TypeConverter baseConverter;
            
            private System.Type baseType;
            
            public WMIValueTypeConverter(System.Type inBaseType) {
                baseConverter = TypeDescriptor.GetConverter(inBaseType);
                baseType = inBaseType;
            }
            
            public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type srcType) {
                return baseConverter.CanConvertFrom(context, srcType);
            }
            
            public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Type destinationType) {
                return baseConverter.CanConvertTo(context, destinationType);
            }
            
            public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
                return baseConverter.ConvertFrom(context, culture, value);
            }
            
            public override object CreateInstance(System.ComponentModel.ITypeDescriptorContext context, System.Collections.IDictionary dictionary) {
                return baseConverter.CreateInstance(context, dictionary);
            }
            
            public override bool GetCreateInstanceSupported(System.ComponentModel.ITypeDescriptorContext context) {
                return baseConverter.GetCreateInstanceSupported(context);
            }
            
            public override PropertyDescriptorCollection GetProperties(System.ComponentModel.ITypeDescriptorContext context, object value, System.Attribute[] attributeVar) {
                return baseConverter.GetProperties(context, value, attributeVar);
            }
            
            public override bool GetPropertiesSupported(System.ComponentModel.ITypeDescriptorContext context) {
                return baseConverter.GetPropertiesSupported(context);
            }
            
            public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(System.ComponentModel.ITypeDescriptorContext context) {
                return baseConverter.GetStandardValues(context);
            }
            
            public override bool GetStandardValuesExclusive(System.ComponentModel.ITypeDescriptorContext context) {
                return baseConverter.GetStandardValuesExclusive(context);
            }
            
            public override bool GetStandardValuesSupported(System.ComponentModel.ITypeDescriptorContext context) {
                return baseConverter.GetStandardValuesSupported(context);
            }
            
            public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, System.Type destinationType) {
                if ((baseType.BaseType == typeof(System.Enum))) {
                    if ((value.GetType() == destinationType)) {
                        return value;
                    }
                    if ((((value == null) 
                                && (context != null)) 
                                && (context.PropertyDescriptor.ShouldSerializeValue(context.Instance) == false))) {
                        return  "NULL_ENUM_VALUE" ;
                    }
                    return baseConverter.ConvertTo(context, culture, value, destinationType);
                }
                if (((baseType == typeof(bool)) 
                            && (baseType.BaseType == typeof(System.ValueType)))) {
                    if ((((value == null) 
                                && (context != null)) 
                                && (context.PropertyDescriptor.ShouldSerializeValue(context.Instance) == false))) {
                        return "";
                    }
                    return baseConverter.ConvertTo(context, culture, value, destinationType);
                }
                if (((context != null) 
                            && (context.PropertyDescriptor.ShouldSerializeValue(context.Instance) == false))) {
                    return "";
                }
                return baseConverter.ConvertTo(context, culture, value, destinationType);
            }
        }
        
        // Embedded class to represent WMI system Properties.
        [TypeConverter(typeof(System.ComponentModel.ExpandableObjectConverter))]
        public class ManagementSystemProperties {
            
            private System.Management.ManagementBaseObject PrivateLateBoundObject;
            
            public ManagementSystemProperties(System.Management.ManagementBaseObject ManagedObject) {
                PrivateLateBoundObject = ManagedObject;
            }
            
            [Browsable(true)]
            public int GENUS {
                get {
                    return ((int)(PrivateLateBoundObject["__GENUS"]));
                }
            }
            
            [Browsable(true)]
            public string CLASS {
                get {
                    return ((string)(PrivateLateBoundObject["__CLASS"]));
                }
            }
            
            [Browsable(true)]
            public string SUPERCLASS {
                get {
                    return ((string)(PrivateLateBoundObject["__SUPERCLASS"]));
                }
            }
            
            [Browsable(true)]
            public string DYNASTY {
                get {
                    return ((string)(PrivateLateBoundObject["__DYNASTY"]));
                }
            }
            
            [Browsable(true)]
            public string RELPATH {
                get {
                    return ((string)(PrivateLateBoundObject["__RELPATH"]));
                }
            }
            
            [Browsable(true)]
            public int PROPERTY_COUNT {
                get {
                    return ((int)(PrivateLateBoundObject["__PROPERTY_COUNT"]));
                }
            }
            
            [Browsable(true)]
            public string[] DERIVATION {
                get {
                    return ((string[])(PrivateLateBoundObject["__DERIVATION"]));
                }
            }
            
            [Browsable(true)]
            public string SERVER {
                get {
                    return ((string)(PrivateLateBoundObject["__SERVER"]));
                }
            }
            
            [Browsable(true)]
            public string NAMESPACE {
                get {
                    return ((string)(PrivateLateBoundObject["__NAMESPACE"]));
                }
            }
            
            [Browsable(true)]
            public string PATH {
                get {
                    return ((string)(PrivateLateBoundObject["__PATH"]));
                }
            }
        }
    }
}
