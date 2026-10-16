//WS: This class is used for loading config file and resettings.

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.XPath;

namespace PublishImodel
{
    internal class AssemblySettings
    {
        private const string CONF_KEY_LANGUAGE = "LANGUAGE";

        private const string CONF_KEY_OUTPUTFOLDER = "OUTPUTFOLDER";

        private const string CONF_KEY_IFCDIR = "IFCDIR";

        private const string CONF_KEY_ATPAUTOSTART = "ATPAUTOSTART";

        private const string CONF_VALUE_ATPAUTOSTART_ON = "on";

        private const string CONF_KEY_INCLUDECATEGORIES = "IncludeCategories";

        private const string CONF_KEY_EXCLUDECATEGORIES = "ExcludeCategories";

        private const string CONF_KEY_IMODELSCHEMANAME = "IMODELSCHEMANAME";

        private const string CONF_VALUE_IMODELSCHEMANAME_UNIQUE = "unique";

        private const string CONF_KEY_SHAREDCOORDINATES = "SHAREDCOORDINATES";

        private const string CONF_KEY_SHAREDBLOCKS = "SHAREDBLOCKS";

        private const string CONF_KEY_EXPORTWITHDEFAULTS = "ExportWithDefaults";

        private const string CONF_KEY_EXPORTSETUPNAME = "ExportSetupName";

        private const string CONF_KEY_EXPORTSETUPNAMESTORY = "ExportSetupNameStory";

        private const string CONF_KEY_EXPORTASMOBILEMODEL = "ExportAsMobileModel";

        private const string CONF_VALUE_YES = "yes";

        private const string CONF_KEY_REMAINTMP = "RemainTmp";

        private const string CONF_KEY_SINGLETHREAD = "SingleThread";

        private readonly string USER_RELPATH = "Bentley\\imEW for Revit";

        private static AssemblySettings s_instance;

        //WS: config object
        private Configuration m_config;

        private bool m_isModified;

        private bool m_isShared;

        private string m_exportSetupName;

        private string m_exportSetupNameStory;

        private bool m_isExportWithDefaults;

        private bool m_isExportAsMobileModel;

        public bool ATPAutoStartMode
        {
            get
            {
                string value = this.GetValue("ATPAUTOSTART");
                if (string.IsNullOrEmpty(value))
                {
                    return false;
                }
                return value.Equals("on", StringComparison.OrdinalIgnoreCase);
            }
        }

        public string ExcludeCategories
        {
            get
            {
                return this.GetValue("ExcludeCategories");
            }
        }

        public bool ExportAsMobileModel
        {
            get
            {
                return this.m_isExportAsMobileModel;
            }
            set
            {
                if (this.m_isExportAsMobileModel != value)
                {
                    this.m_isModified = true;
                    this.m_isExportAsMobileModel = value;
                }
            }
        }

        public string ExportSetupName
        {
            get
            {
                return this.m_exportSetupName;
            }
            set
            {
                if (this.m_exportSetupName != value)
                {
                    List<string> strs = new List<string>(AssemblySettings.SplitList(this.m_exportSetupNameStory));
                    while (strs.Remove(value))
                    {
                    }
                    while (strs.Remove(this.m_exportSetupName))
                    {
                    }
                    this.m_exportSetupNameStory = this.m_exportSetupName;
                    foreach (string str in strs)
                    {
                        AssemblySettings assemblySetting = this;
                        assemblySetting.m_exportSetupNameStory = string.Concat(assemblySetting.m_exportSetupNameStory, Path.PathSeparator, str);
                    }
                    this.m_isModified = true;
                    this.m_exportSetupName = value;
                }
            }
        }

        public string[] ExportSetupNameStory
        {
            get
            {
                return AssemblySettings.SplitList(this.m_exportSetupNameStory);
            }
        }

        public bool ExportWithDefaults
        {
            get
            {
                return this.m_isExportWithDefaults;
            }
            set
            {
                if (this.m_isExportWithDefaults != value)
                {
                    this.m_isModified = true;
                    this.m_isExportWithDefaults = value;
                }
            }
        }

        public string IFCDir
        {
            get
            {
                string value = this.GetValue("IFCDIR");
                if (!string.IsNullOrEmpty(value))
                {
                    return value;
                }
                return "c:\\";
            }
        }

        public string IncludeCategories
        {
            get
            {
                return this.GetValue("IncludeCategories");
            }
        }

        public static AssemblySettings Instance
        {
            get
            {
                if (AssemblySettings.s_instance == null)
                {
                    AssemblySettings.s_instance = new AssemblySettings();
                }
                return AssemblySettings.s_instance;
            }
        }

        public bool IsShared
        {
            get
            {
                return this.m_isShared;
            }
            set
            {
                if (this.m_isShared != value)
                {
                    this.m_isModified = true;
                    this.m_isShared = value;
                }
            }
        }

        public bool IsSharedBlocks
        {
            get
            {
                bool flag = false;
                if (bool.TryParse(this.GetValue("SHAREDBLOCKS"), out flag))
                {
                    return flag;
                }
                return false;
            }
        }

        public string Language
        {
            get
            {
                return this.GetValue("LANGUAGE");
            }
        }

        public string OutputFolder
        {
            get
            {
                string value = this.GetValue("OUTPUTFOLDER");
                if (!string.IsNullOrEmpty(value) && value[value.Length - 1] != Path.DirectorySeparatorChar)
                {
                    value = string.Concat(value, Path.DirectorySeparatorChar);
                }
                return value;
            }
        }

        public bool RemainTmp
        {
            get
            {
                string value = this.GetValue("RemainTmp");
                if (string.IsNullOrEmpty(value))
                {
                    return false;
                }
                return value.Equals("yes", StringComparison.OrdinalIgnoreCase);
            }
        }

        public bool SingleThread
        {
            get
            {
                string value = this.GetValue("SingleThread");
                if (string.IsNullOrEmpty(value))
                {
                    return false;
                }
                return value.Equals("yes", StringComparison.OrdinalIgnoreCase);
            }
        }

        public bool UniqueSchemaName
        {
            get
            {
                string value = this.GetValue("IMODELSCHEMANAME");
                if (string.IsNullOrEmpty(value))
                {
                    return false;
                }
                return value.Equals("unique", StringComparison.OrdinalIgnoreCase);
            }
        }

        static AssemblySettings()
        {
        }

        private AssemblySettings()
        {
            this.Reload(false);
        }

        public string GetAOutputFolder()
        {
            string outputFolder = this.OutputFolder;
            if (outputFolder != null)
            {
                return outputFolder;
            }
            return "c:\\";
        }

        private string GetValue(string key)
        {
            if (this.m_config == null)
            {
                return null;
            }
            KeyValueConfigurationElement item = this.m_config.AppSettings.Settings[key];
            if (item == null)
            {
                return null;
            }
            return item.Value.Trim();
        }

        //WS: load config xml file
        internal void Reload(bool reset)
        {
            //WS: assembly, i.e. the PublishImodel.dll location.
            string location = Assembly.GetExecutingAssembly().Location;
            try
            {
                //WS: This is the folder "C:\Users\sui55523\AppData\Roaming\Bentley\imEW for Revit"
                string str = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), this.USER_RELPATH);

                if (!Directory.Exists(str))
                {
                    Directory.CreateDirectory(str);
                }

                str = Path.Combine(str, string.Concat(Path.GetFileName(location), ".config"));

                if (!File.Exists(str) || reset)
                {
                    File.Copy(string.Concat(location, ".config"), str); //WS: copy the PublishImodel.dll.config from the dll folder to user appdata folder.
                }

                ExeConfigurationFileMap exeConfigurationFileMap = new ExeConfigurationFileMap()
                {
                    ExeConfigFilename = str
                };

                this.m_config = ConfigurationManager.OpenMappedExeConfiguration(exeConfigurationFileMap, ConfigurationUserLevel.None);
            }
            catch
            {
                try
                {
                    this.m_config = ConfigurationManager.OpenExeConfiguration(location);
                }
                catch
                {
                    this.m_config = null;
                }
            }
            this.m_isModified = false;
            if (!bool.TryParse(this.GetValue("SHAREDCOORDINATES"), out this.m_isShared))
            {
                this.m_isModified = true;
                this.m_isShared = true;
            }
            if (!bool.TryParse(this.GetValue("ExportWithDefaults"), out this.m_isExportWithDefaults))
            {
                this.m_isModified = true;
                this.m_isExportWithDefaults = true;
            }
            this.m_exportSetupName = this.GetValue("ExportSetupName");
            this.m_exportSetupNameStory = this.GetValue("ExportSetupNameStory");
            if (!bool.TryParse(this.GetValue("ExportAsMobileModel"), out this.m_isExportAsMobileModel))
            {
                this.m_isModified = true;
                this.m_isExportAsMobileModel = false;
            }
        }

        public void Save()
        {
            if (this.m_isModified)
            {
                this.SaveKeyValue("SHAREDCOORDINATES", this.m_isShared.ToString().ToLowerInvariant());
                this.SaveKeyValue("ExportWithDefaults", this.m_isExportWithDefaults.ToString().ToLowerInvariant());
                this.SaveKeyValue("ExportSetupName", this.m_exportSetupName);
                this.SaveKeyValue("ExportSetupNameStory", this.m_exportSetupNameStory);
                this.SaveKeyValue("ExportAsMobileModel", this.m_isExportAsMobileModel.ToString().ToLowerInvariant());
                this.m_isModified = false;
            }
        }

        private bool SaveKeyValue(string key, string value)
        {
            bool flag;
            if (this.m_config == null)
            {
                return false;
            }
            if (value == null)
            {
                value = "";
            }
            string str = string.Format("<add key=\"{0}\" value=\"{1}\"/>", key, value);
            XmlDocument xmlDocument = new XmlDocument();
            if (File.Exists(this.m_config.FilePath))
            {
                xmlDocument.Load(this.m_config.FilePath);
                XPathNavigator xPathNavigator = xmlDocument.CreateNavigator();
                XPathNavigator xPathNavigator1 = xPathNavigator.SelectSingleNode("/configuration/appSettings");
                if (xPathNavigator1 != null)
                {
                    XPathNavigator xPathNavigator2 = xPathNavigator1.SelectSingleNode(string.Format("add[@key=\"{0}\"]", key));
                    if (xPathNavigator2 != null)
                    {
                        XPathNavigator xPathNavigator3 = xPathNavigator2.SelectSingleNode("@value");
                        if (xPathNavigator3 != null)
                        {
                            xPathNavigator3.SetValue(value);
                        }
                        else
                        {
                            xPathNavigator2.CreateAttribute("", "value", "", value);
                        }
                    }
                    else
                    {
                        xPathNavigator1.AppendChild(str);
                    }
                }
                else
                {
                    XPathNavigator xPathNavigator4 = xPathNavigator.SelectSingleNode("/configuration");
                    xPathNavigator4.AppendChild(string.Format("<appSettings>{0}</appSettings>", str));
                }
            }
            else
            {
                xmlDocument.LoadXml(string.Format("<configuration><appSettings>{0}</appSettings></configuration>", str));
            }
            try
            {
                xmlDocument.Save(this.m_config.FilePath);
                return true;
            }
            catch
            {
                flag = false;
            }
            return flag;
        }

        private static string[] SplitList(string list)
        {
            if (string.IsNullOrEmpty(list))
            {
                return new string[0];
            }
            char[] pathSeparator = new char[] { Path.PathSeparator };
            string[] strArrays = list.Split(pathSeparator, StringSplitOptions.RemoveEmptyEntries);
            for (int i = (int)strArrays.Length - 1; i >= 0; i--)
            {
                strArrays[i] = strArrays[i].Trim();
            }
            return strArrays;
        }
    }
}