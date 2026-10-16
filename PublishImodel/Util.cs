using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Bentley.ECObjects.Instance;
using Bentley.ECObjects.Schema;
using Microsoft.Win32;
using Revit.PWIntegration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
//using System.Windows.Forms;

namespace PublishImodel
{
    internal static class Util
    {
        public const string PRODUCT_SHORT_NAME = "imEW for Revit";

        private static bool projectWiseIntegrationDisabled_;

        private static string PWAppCfgRegSubkey;

        private static bool s_pwIntegrationStarted;

        private static int lastUsedPWProjectId;

        public static bool IsItgDgnDbConverterInstalled
        {
            get
            {
                object value = Registry.GetValue(string.Concat("HKEY_LOCAL_MACHINE\\", Util.SoftwareBentleyRegPath, "ItgDgnDbImporter 1.5 x64"), "Version", null);
                return null != value;
            }
        }

        public static bool ProjectWiseIntegrationEnabled
        {
            get
            {
                return Util.projectWiseIntegrationDisabled_;
            }
            set
            {
                Util.projectWiseIntegrationDisabled_ = value;
            }
        }

        public static bool PWIntegrationFeatureEnabled
        {
            get
            {
                if (Util.projectWiseIntegrationDisabled_)
                {
                    return false;
                }
                return object.Equals(Registry.GetValue(string.Concat("HKEY_LOCAL_MACHINE\\", Util.SoftwareBentleyRegPath, "RevitIntegrations"), "PWIntegration", "0"), "1");
            }
        }

        public static string SoftwareBentleyIMPRRegPath
        {
            get
            {
                return string.Concat(Util.SoftwareBentleyRegPath, "imEW for Revit");
            }
        }

        public static string SoftwareBentleyRegPath
        {
            get
            {
                return "Software\\Bentley\\";
            }
        }

        static Util()
        {
        }

        public static Document Application_getDocumentBySymbol(Application application, Element symbol)
        {
            Document document = null;
            if (application != null && symbol != null)
            {
                DocumentSet documents = application.Documents;
                if (documents != null)
                {
                    string name = symbol.Name;
                    int num = name.IndexOf(" :", StringComparison.Ordinal);
                    if (-1 != num)
                    {
                        name = name.Substring(0, num);
                    }
                    foreach (Document document1 in documents)
                    {
                        if (Util.Document_getFileName(document1) != name)
                        {
                            continue;
                        }
                        document = document1;
                        break;
                    }
                }
            }
            return document;
        }

        public static string Application_getHostInfo(Application application)
        {
            string empty = string.Empty;
            if (application != null)
            {
                empty = string.Concat("IMPR_", application.VersionName, "_x", (Environment.Is64BitProcess ? "64" : "86"));
                empty = empty.Replace(' ', '\u005F');
            }
            return empty;
        }

        public static List<Document> Application_getLinkedDocuments(Application application, Document document)
        {
            List<Document> documents = null;
            if (document != null && application != null)
            {
                IList<Element> elements = Util.Application_getLinkedFileSymbols(application, document);
                if (elements != null)
                {
                    foreach (Element element in elements)
                    {
                        Document document1 = Util.Application_getDocumentBySymbol(application, element);
                        if (document1 == null)
                        {
                            continue;
                        }
                        if (documents == null)
                        {
                            documents = new List<Document>();
                        }
                        if (documents.Contains(document1))
                        {
                            continue;
                        }
                        documents.Add(document1);
                    }
                }
            }
            return documents;
        }

        public static IList<Element> Application_getLinkedFileSymbols(Application application, Document document)
        {
            IList<Element> elements = null;
            using (FilteredElementCollector filteredElementCollector = new FilteredElementCollector(document))
            {
                filteredElementCollector.OfCategoryId(new ElementId(BuiltInCategory.OST_RvtLinks));
                filteredElementCollector.WhereElementIsNotElementType();
                elements = filteredElementCollector.ToElements();
            }
            return elements;
        }

        //WS: return an export output folder.
        public static string AskUserForSaveFilePath(string docName, string extension, bool pwIntegration)
        {
            //WS: if not integrated with ProjectWise.
            if (!pwIntegration)
            {
                return Util.AskUserForSaveFilePathLocal(docName, extension, false);
            }

            //WS: if integrated with PW already.
            string filetPath = Path.Combine(Path.GetTempPath(), (docName.EndsWith(extension, StringComparison.OrdinalIgnoreCase) ? Path.GetFileName(docName) : string.Concat(Path.GetFileNameWithoutExtension(docName), extension)));

            //WS: if the file is already there, delete it first.
            if (!Util.FileDelete(filetPath))
            {
                //WS: create a temp folder named with New Guid.
                string tempPath = Path.GetTempPath();
                Guid guid = Guid.NewGuid();
                filetPath = Path.Combine(tempPath, string.Concat(guid.ToString(), extension));
            }
            return filetPath;
        }

        public static string AskUserForSaveFilePathLocal(string docName, string extension, bool disableExtensionChange = false)
        {
            string str;
            using (System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog())
            {
                int num = 0;
                StringBuilder stringBuilder = new StringBuilder(128);
                if (!disableExtensionChange || extension.Equals(".i.dgn", StringComparison.OrdinalIgnoreCase))
                {
                    //stringBuilder.Append(Resources.GetString("File_Description")).Append("|*.i.dgn");
                    stringBuilder.Append("Bentley iModel File (*.i.dgn)").Append("|*.i.dgn");
                    num++;
                }
                if ((!disableExtensionChange || extension.Equals(".imodel", StringComparison.OrdinalIgnoreCase)) && Util.IsItgDgnDbConverterInstalled)
                {
                    if (stringBuilder.Length > 0)
                    {
                        stringBuilder.Append('|');
                    }
                    //stringBuilder.Append(Resources.GetString("File_Description2")).Append("|*.imodel");
                    stringBuilder.Append("Bentley iModel File (*.imodel)").Append("|*.imodel");
                    if (extension.Equals(".imodel", StringComparison.OrdinalIgnoreCase))
                    {
                        num++;
                    }
                }
                saveFileDialog.InitialDirectory = Path.GetDirectoryName(docName);
                saveFileDialog.FileName = (docName.EndsWith(extension, StringComparison.OrdinalIgnoreCase) ? Path.GetFileName(docName.Substring(0, docName.Length - extension.Length)) : Path.GetFileNameWithoutExtension(docName));
                saveFileDialog.Filter = stringBuilder.ToString();
                saveFileDialog.FilterIndex = num;
                saveFileDialog.AddExtension = false;
                saveFileDialog.OverwritePrompt = false;
                saveFileDialog.SupportMultiDottedExtensions = true;
                if (saveFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                {
                    return null;
                }
                else
                {
                    string fileName = saveFileDialog.FileName;
                    if (!disableExtensionChange)
                    {
                        if (saveFileDialog.FilterIndex != 1)
                        {
                            extension = ".imodel";
                        }
                        else
                        {
                            extension = ".i.dgn";
                        }
                    }
                    int num1 = fileName.IndexOf(extension, fileName.LastIndexOf(Path.DirectorySeparatorChar) + 1, StringComparison.OrdinalIgnoreCase);
                    if (num1 < 0)
                    {
                        fileName = string.Concat(fileName, extension);
                    }
                    else if (num1 < fileName.Length - extension.Length)
                    {
                        fileName = string.Concat(fileName.Substring(0, num1), extension);
                    }
                    str = fileName;
                }
            }
            return str;
        }

        public static bool Document_addECSchema(SchemaCreator creator, Document document)
        {
            if (document != null)
            {
                IList<Element> elements = Util.Document_getElements(document);
                if (elements != null)
                {
                    foreach (Element element in elements)
                    {
                        Util.ElementAddToECSchema(creator, element);
                    }
                    return true;
                }
            }
            return false;
        }

        public static bool Document_addECSchemaDeep(Application application, SchemaCreator creator, Document document, View view)
        {
            Util.Document_addECSchema(creator, document);
            List<Document> documents = Util.Application_getLinkedDocuments(application, document);
            if (documents != null)
            {
                foreach (Document document1 in documents)
                {
                    if (view == null)
                    {
                        Util.Document_addECSchema(creator, document1);
                    }
                    else
                    {
                        Util.View_addECSchema(creator, document, view);
                    }
                }
            }
            return true;
        }

        public static bool Document_exportElements(SchemaCreator creator, Document document, Element link, List<IECInstance> instList, ref int revitId)
        {
            if (document != null)
            {
                IList<Element> elements = Util.Document_getElements(document);
                if (elements != null)
                {
                    foreach (Element element in elements)
                    {
                        Util.Element_export(creator, document, link, element, instList, ref revitId);
                    }
                    return true;
                }
            }
            return false;
        }

        public static bool Document_exportToIFC(Document document, string ifcFilePath)
        {
            if (!Util.IsIFCExportEnabledInAPI())
            {
                return false;
            }
            string str = string.Concat(Path.GetFileNameWithoutExtension(ifcFilePath), ".ifc");
            string directoryName = Path.GetDirectoryName(ifcFilePath);
            if (!directoryName.EndsWith("\\"))
            {
                directoryName = string.Concat(directoryName, "\\");
            }
            IFCExportOptions fCExportOption = new IFCExportOptions()
            {
                FilterViewId = ElementId.InvalidElementId,
                ExportBaseQuantities = true,
                FamilyMappingFile = null,
                FileVersion = IFCVersion.IFC2x3,
                WallAndColumnSplitting = false
            };
            document.Export(directoryName, str, fCExportOption);
            if (File.Exists(ifcFilePath))
            {
                return true;
            }
            return false;
        }

        public static bool Document_exportViewToDWG(Document document, View viewToExport, string dwgFolder, 
                                                    string dwgName, DWGExportOptions dwgExportOptions)
        {
            if (!viewToExport.CanBePrinted)
            {
                //throw new ExpImpException(string.Format(Resources.GetString("Only_printable_views_can_be_exported"), viewToExport.Name), string.Format("Only printable views can be exported. View \"{0}\" is not printable", viewToExport.Name));
            }

            ViewSet viewSets = new ViewSet();

            viewSets.Insert(viewToExport);

            //WS: Revit API's export function to export to dwg
            return document.Export(dwgFolder, dwgName, Util.PrepareViewSetForExport(viewSets), dwgExportOptions);
        }

        public static ViewSet Document_getAllPrintableViews(Document document)
        {
            IList<Element> elements = null;
            ViewSet viewSets = new ViewSet();
            using (FilteredElementCollector filteredElementCollector = new FilteredElementCollector(document))
            {
                filteredElementCollector.OfCategoryId(new ElementId(BuiltInCategory.OST_Views));
                filteredElementCollector.WhereElementIsNotElementType();
                elements = filteredElementCollector.ToElements();
            }
            foreach (Element element in elements)
            {
                View view = element as View;
                if (view == null || !view.CanBePrinted)
                {
                    continue;
                }
                viewSets.Insert(view);
            }
            return viewSets;
        }

        public static View Document_getAPrintableView(Document document, View notThisOne)
        {
            View view;
            IList<Element> elements = null;

            using (FilteredElementCollector filteredElementCollector = new FilteredElementCollector(document))
            {
                filteredElementCollector.OfCategoryId(new ElementId(BuiltInCategory.OST_Views));
                filteredElementCollector.WhereElementIsNotElementType();
                elements = filteredElementCollector.ToElements();
            }

            using (IEnumerator<Element> enumerator = elements.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    View current = enumerator.Current as View;
                    if (current == null || !current.CanBePrinted || notThisOne == current)
                    {
                        continue;
                    }
                    view = current;
                    return view;
                }
                return null;
            }

            //return view;
        }

        public static string Document_getDWGName(Document document, View view, bool filterCharacters)
        {
            string str = document.Title.Replace(".", "_");
            string name = view.Name;
            string str1 = view.ViewType.ToString();
            string[] strArrays = new string[] { str, "_", str1, "_", name };
            string str2 = string.Concat(strArrays);
            if (filterCharacters)
            {
                str2 = Util.StripForECNames(str2);
            }
            return str2;
        }

        public static IECSchema Document_getECSchemaDeep(Application application, Document document, View view)
        {
            SchemaCreator schemaCreator = new SchemaCreator(document, view);
            Util.Document_addECSchemaDeep(application, schemaCreator, document, view);
            return schemaCreator.Schema;
        }

        public static Element Document_getElement(Document document, ElementId elementId)
        {
            return document.GetElement(elementId);
        }

        public static IList<Element> Document_getElements(Document document)
        {
            IList<Element> elements;
            using (FilteredElementCollector filteredElementCollector = new FilteredElementCollector(document))
            {
                filteredElementCollector.WhereElementIsNotElementType();
                elements = filteredElementCollector.ToElements();
            }
            return elements;
        }

        public static string Document_getFileName(Document document)
        {
            string str = null;
            if (document != null)
            {
                string pathName = document.PathName;
                int num = pathName.LastIndexOf('\\');
                if (-1 != num)
                {
                    str = pathName.Substring(num + 1);
                }
            }
            return str;
        }

        public static bool Document_hasLinks(Application application, Document document)
        {
            if (application != null && document != null && Util.Application_getLinkedDocuments(application, document) != null)
            {
                return true;
            }
            return false;
        }

        public static bool Element_export(SchemaCreator creator, Document document, Element link, Element el, List<IECInstance> instList, ref int revitId)
        {
            if (Util.Element_isExportable(el, creator))
            {
                IECClass @class = creator.Schema.GetClass(Util.EncodeForECNames(el.Category.Name)) ?? creator.AddClassFromElement(el);
                IECInstance eCInstances = InstanceTools.CreateInstance(document, link, @class, el, creator);
                if (eCInstances != null)
                {
                    int num = revitId;
                    int num1 = num;
                    revitId = num + 1;
                    eCInstances.InstanceId = string.Format("REVIT::{0:D}", num1);
                    instList.Add(eCInstances);
                }
            }
            return true;
        }

        public static int Element_getId(Element element)
        {
            return Util.ElementId_getValue(element.Id);
        }

        //WS: it seems use this way to filter out those deleted elements??
        public static Element Element_getObjectType(Element element)
        {
            Element element1 = null;

            if (element != null)
            {
                ElementId typeId = element.GetTypeId();

                if (null != typeId)
                {
                    return element.Document.GetElement(typeId);
                }
            }
            return element1;
        }

        public static string Element_getObjectTypeName(Element element)
        {
            if (element != null)
            {
                Element element1 = Util.Element_getObjectType(element);
                if (element1 != null)
                {
                    return element1.Name;
                }
            }
            return "";
        }

        public static string Element_getValueString(Element element)
        {
            string str;
            if (element != null)
            {
                if (element.Parameters.Size == 2)
                {
                    IEnumerator enumerator = element.Parameters.GetEnumerator();
                    try
                    {
                        while (enumerator.MoveNext())
                        {
                            Parameter current = (Parameter)enumerator.Current;
                            if (!(current.Definition.Name != "Name") || current.StorageType == StorageType.ElementId || current.StorageType == StorageType.None)
                            {
                                continue;
                            }
                            str = current.AsValueString();
                            return str;
                        }
                        return string.Empty;
                    }
                    finally
                    {
                        IDisposable disposable = enumerator as IDisposable;
                        if (disposable != null)
                        {
                            disposable.Dispose();
                        }
                    }
                }
                else if (element is RebarCoverType)
                {
                    IEnumerator enumerator1 = element.Parameters.GetEnumerator();
                    try
                    {
                        while (enumerator1.MoveNext())
                        {
                            Parameter parameter = (Parameter)enumerator1.Current;
                            if (parameter.Definition.Name != "Length")
                            {
                                continue;
                            }
                            str = parameter.AsValueString();
                            return str;
                        }
                        return string.Empty;
                    }
                    finally
                    {
                        IDisposable disposable1 = enumerator1 as IDisposable;
                        if (disposable1 != null)
                        {
                            disposable1.Dispose();
                        }
                    }
                }
                else
                {
                    return string.Empty;
                }
                //return str;
            }
            return string.Empty;
        }

        public static bool Element_isExportable(Element element, SchemaCreator creator)
        {
            if (element == null || element.Category == null || Util.Element_getObjectType(element) == null)
            {
                return false;
            }
            return creator.IsExportableClass(element.Category.Name);
        }

        public static bool ElementAddToECSchema(SchemaCreator creator, Element el)
        {
            if (Util.Element_isExportable(el, creator) && creator.Schema.GetClass(Util.EncodeForECNames(el.Category.Name)) == null)
            {
                creator.AddClassFromElement(el);
            }
            return true;
        }

        public static int ElementId_getValue(ElementId eid)
        {
            return (int)eid.Value;
        }

        //WS: process  category name, replace any illegal character with..
        public static string EncodeForECNames(string name)
        {
            for (int i = 0; i < name.Length; i++)
            {
                char chr = name[i];

                if (!Util.IsLegalCharacter(chr))
                {
                    string str = chr.ToString();

                    name = name.Replace(str, string.Format("__x{0:x4}__", (ushort)chr));

                    i += 8; //WS: note add 8 which is the char for replacement
                }
            }

            //WS: if starting with digit, add "_" in front.
            //if (char.IsDigit(name[0]))
            //{
            //    name = string.Concat("_", name);
            //}


            if (!string.IsNullOrEmpty(name) && char.IsDigit(name[0]))
            {
                name = string.Concat("_", name);
            }

            return name;
        }

        public static bool FileDelete(string fileName)
        {
            bool flag;

            if (!string.IsNullOrEmpty(fileName) && File.Exists(fileName))
            {
                try
                {
                    File.Delete(fileName);
                    return true;
                }
                catch
                {
                    flag = false;
                }
                return flag;
            }
            return true;
        }

        public static bool FileRename(string oldFileName, string newFileName)
        {
            bool flag;
            if (!File.Exists(oldFileName))
            {
                return false;
            }
            if (!Util.FileDelete(newFileName))
            {
                return false;
            }
            try
            {
                File.Move(oldFileName, newFileName);
                return true;
            }
            catch
            {
                flag = false;
            }
            return flag;
        }

        public static Result GetAppResult(Util.Result res)
        {
            switch (res)
            {
                case Util.Result.Failed:
                    {
                        return Result.Failed;
                    }
                case Util.Result.Succeeded:
                    {
                        return Result.Succeeded;
                    }
                case Util.Result.Cancelled:
                    {
                        return Result.Cancelled;
                    }
            }
            return Result.Failed;
        }

        public static string GetBenServicesPath()
        {
            string str;
            string str1 = null;
            try
            {
                using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(string.Concat(Util.SoftwareBentleyRegPath, "BEnServices\\{F377D209-E461-4331-82EB-6919FD8F51DA}"), false))
                {
                    str1 = registryKey.GetValue("DisplayIcon").ToString();
                }
                return str1;
            }
            catch
            {
                str = null;
            }
            return str;
        }

        public static string GetIMPRDisplayIcon()
        {
            string str;
            string str1 = null;
            try
            {
                using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(Util.SoftwareBentleyIMPRRegPath, false))
                {
                    object value = registryKey.GetValue("DisplayIcon");
                    if (value != null)
                    {
                        str1 = value.ToString();
                    }
                }
                return str1;
            }
            catch
            {
                str = null;
            }
            return str;
        }

        public static string GetIMPRProgramPath()
        {
            return string.Concat(Path.GetDirectoryName(typeof(Util).Assembly.Location), "\\");
        }

        private static Util.PWSS4IntegrationStatus GetPWSS4IntegrationStatus()
        {
            Util.PWSS4IntegrationStatus pWSS4IntegrationStatu;
            if (!PWIntegration.CheckPWSS4Version(false))
            {
                return Util.PWSS4IntegrationStatus.NotIntegrated;
            }
            try
            {
                if (Util.PWAppCfgRegSubkey == null)
                {
                    Util.PWAppCfgRegSubkey = PWIntegration.GetPWAppCfgRegSubkey();
                }
                using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(string.Concat(Util.PWAppCfgRegSubkey, "\\DmsOff")))
                {
                    string integratedAppClassName = PWIntegration.GetIntegratedAppClassName();
                    if (!string.IsNullOrEmpty(integratedAppClassName))
                    {
                        if (PWIntegration.CheckPWSS4Version(true))
                        {
                            integratedAppClassName = string.Concat("PW ", integratedAppClassName);
                        }
                        object value = registryKey.GetValue(integratedAppClassName);
                        if (value != null)
                        {
                            pWSS4IntegrationStatu = (value.ToString().Equals("Disabled", StringComparison.OrdinalIgnoreCase) ? Util.PWSS4IntegrationStatus.Enabled : Util.PWSS4IntegrationStatus.Disabled);
                        }
                        else
                        {
                            pWSS4IntegrationStatu = Util.PWSS4IntegrationStatus.Enabled;
                        }
                    }
                    else
                    {
                        pWSS4IntegrationStatu = Util.PWSS4IntegrationStatus.Enabled;
                    }
                }
            }
            catch
            {
                pWSS4IntegrationStatu = Util.PWSS4IntegrationStatus.Enabled;
            }
            return pWSS4IntegrationStatu;
        }

        public static Result GetResult(Util.Result res)
        {
            switch (res)
            {
                case Util.Result.Failed:
                    {
                        return Result.Failed;
                    }
                case Util.Result.Succeeded:
                    {
                        return Result.Succeeded;
                    }
                case Util.Result.Cancelled:
                    {
                        return Result.Cancelled;
                    }
            }
            return Result.Failed;
        }

        public static bool HCIntegrationFeatureEnabled()
        {
            return object.Equals(Registry.GetValue(string.Concat("HKEY_LOCAL_MACHINE\\", Util.SoftwareBentleyRegPath, "RevitIntegrations"), "HCIntegration", "0"), "1");
        }

        public static int IModel_openInBentleyViewer(string iModelSourceFileName)
        {
            int num = Util.IModel_openInLatestVersion(string.Concat(Util.SoftwareBentleyRegPath, "MicroStation"), iModelSourceFileName);
            if (num != 0)
            {
                num = Util.IModel_openInLatestVersion(string.Concat(Util.SoftwareBentleyRegPath, "Navigator"), iModelSourceFileName);
            }
            if (num != 0)
            {
                num = Util.IModel_openInLatestVersion(string.Concat(Util.SoftwareBentleyRegPath, "View"), iModelSourceFileName);
            }
            return num;
        }

        private static int IModel_openInLatestVersion(string regPath, string iModelSourceFileName)
        {
            int num;
            int num1 = 8110700;
            string str = null;
            try
            {
                using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(regPath, false))
                {
                    if (registryKey != null)
                    {
                        RegistryKey registryKey1 = null;
                        string[] subKeyNames = registryKey.GetSubKeyNames();
                        for (int i = 0; i < (int)subKeyNames.Length; i++)
                        {
                            using (RegistryKey registryKey2 = registryKey.OpenSubKey(subKeyNames[i], false))
                            {
                                try
                                {
                                    object value = registryKey2.GetValue("Version");
                                    if (value != null)
                                    {
                                        int num2 = Convert.ToInt32(value.ToString());
                                        string str1 = registryKey2.GetValue("DisplayIcon").ToString();
                                        if (num2 > num1 && File.Exists(str1))
                                        {
                                            registryKey1 = registryKey2;
                                            num1 = num2;
                                            str = str1;
                                            registryKey1.GetValue("ProgramPath").ToString();
                                        }
                                    }
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                    else
                    {
                        num = 32768;
                        return num;
                    }
                }
            }
            catch
            {
                num = 32768;
                return num;
            }
            if (!File.Exists(str))
            {
                return 32768;
            }
            using (Process process = Process.Start(str, string.Concat("\"", iModelSourceFileName, "\"")))
            {
                num = (process != null ? 0 : 32768);
            }
            return num;
        }

        public static bool IsIFCExportEnabledInAPI()
        {
            return true;
        }

        //WS: only the following is legal A-Z, a-Z, 0-9, "_".
        public static bool IsLegalCharacter(char symbol)
        {
            if (symbol >= 'a' && symbol <= 'z' || symbol >= 'A' && symbol <= 'Z' || symbol >= '0' && symbol <= '9' || symbol == '\u005F')
            {
                return true;
            }

            return false;
        }

        public static string Parameter_getDefinitionName(Parameter parameter)
        {
            string str;
            try
            {
                if (parameter.Id.Value == -1150107)
                {
                    //str = Resources.GetString("FBX_LIGHT_INITIAL_COLOR_TEMPERATURE");
                    str = "Initial Color";
                }
                else if (parameter.Id.Value == -1010503)
                {
                    //str = Resources.GetString("FBX_LIGHT_LIMUNOUS_FLUX");
                    str = "Initial Intensity";
                }
                else if (parameter.Id.Value == -1114217)
                {
                    //str = Resources.GetString("FBX_LIGHT_TOTAL_LIGHT_LOSS");
                    str = "Light Loss Factor";
                }
                else if (parameter.Id.Value != -1005438)
                {
                    str = (parameter.Definition != null ? parameter.Definition.Name : "NULLDefinition");
                }
                else
                {
                    //str = Resources.GetString("ANALYTIC_CONSTRUCTION_GBXML_TYPEID");
                    str = "Analytic Construction";
                }
            }
            catch
            {
                str = "!Exception!_Unknown!";
            }
            return str;
        }

        public static ICollection<ElementId> PrepareViewSetForExport(ViewSet views)
        {
            ICollection<ElementId> elementIds = new List<ElementId>();
            foreach (View view in views)
            {
                elementIds.Add(view.Id);
            }
            return elementIds;
        }

        public static void PWSessionEnd()
        {
            if (Util.s_pwIntegrationStarted)
            {
                PWIntegration.UnInitialize();
            }
            Util.s_pwIntegrationStarted = false;
        }

        public static bool PWSessionStart()
        {
            Util.PWSS4IntegrationStatus pWSS4IntegrationStatus = Util.GetPWSS4IntegrationStatus();
            if (pWSS4IntegrationStatus == Util.PWSS4IntegrationStatus.Enabled || pWSS4IntegrationStatus == Util.PWSS4IntegrationStatus.NotIntegrated && Util.PWIntegrationFeatureEnabled)
            {
                if (Util.s_pwIntegrationStarted)
                {
                    Util.s_pwIntegrationStarted = PWIntegration.CheckConnection();
                }
                if (!Util.s_pwIntegrationStarted)
                {
                    try
                    {
                        PWIntegration.Initialize();
                        if (PWIntegration.LoginDialog() == 0)
                        {
                            Util.s_pwIntegrationStarted = true;
                        }
                    }
                    catch
                    {
                    }
                }
            }
            return Util.s_pwIntegrationStarted;
        }

        public static bool PWSessionStarted()
        {
            if (Util.GetPWSS4IntegrationStatus() == Util.PWSS4IntegrationStatus.Disabled)
            {
                return false;
            }
            return Util.s_pwIntegrationStarted;
        }

        public static PushButton RibbonPanel_addButton(RibbonPanel ribbonPanelButtons, string a, string b, string c, string d)
        {
            PushButtonData pushButtonDatum = new PushButtonData(a, b, c, d);
            return (PushButton)ribbonPanelButtons.AddItem(pushButtonDatum);
        }

        public static Process RunIMPRToMakeECIFC(string iModelSourceFileName, string hostId)
        {
            Process process = null;
            string tempFileName = Path.GetTempFileName();
            if (File.Exists(tempFileName))
            {
                using (StreamWriter streamWriter = File.AppendText(tempFileName))
                {
                    streamWriter.WriteLine(string.Concat("save as v8 \"", iModelSourceFileName, ".dgn\""));
                }
                string mPRDisplayIcon = Util.GetIMPRDisplayIcon();
                string mPRProgramPath = Util.GetIMPRProgramPath();
                if (mPRDisplayIcon != null && mPRProgramPath != null && File.Exists(mPRDisplayIcon))
                {
                    bool flag = false;
                    process = new Process();
                    string[] strArrays = new string[] { " \"-S", tempFileName, "\" \"", iModelSourceFileName, "\" -H", hostId };
                    string str = string.Concat(strArrays);
                    process.StartInfo.WorkingDirectory = mPRProgramPath;
                    process.StartInfo.FileName = mPRDisplayIcon;
                    process.StartInfo.UseShellExecute = false;
                    if (!flag)
                    {
                        process.StartInfo.EnvironmentVariables.Add("MS_NOSPLASH", "1");
                    }
                    else
                    {
                        str = string.Concat(" -ishowgui", str);
                    }
                    process.StartInfo.Arguments = str;
                    process.Start();
                }
            }
            return process;
        }

        public static string SaveAsDocumentDialog(string title, string exportFile, string name, string description, string fileName, string format)
        {
            string str = PWIntegration.SaveAsDocumentDialog(title, ref Util.lastUsedPWProjectId, 0, exportFile, name, description, fileName, format);
            return str;
        }

        public static Process StartBenServicesFromECIFC(string ECIFCPath)
        {
            Process process = null;
            string str = null;
            string benServicesPath = Util.GetBenServicesPath();
            str = benServicesPath;
            if (benServicesPath == null)
            {
                return process;
            }
            process = new Process();
            process.StartInfo.FileName = str;
            process.StartInfo.Arguments = string.Concat("\"", ECIFCPath, "\"");
            process.Start();
            return process;
        }

        public static string StripForECNames(string name)
        {
            for (int i = 0; i < name.Length; i++)
            {
                char chr = name[i];
                if (!Util.IsLegalCharacter(chr))
                {
                    name = name.Replace(chr.ToString(), null);
                    i--;
                }
            }
            if (char.IsDigit(name, 0))
            {
                name = string.Concat("_", name);
            }
            return name;
        }

        public static bool View_addECSchema(SchemaCreator creator, Document document, View view)
        {
            if (view == null)
            {
                return false;
            }
            IList<Element> elements = Util.View_getElements(document, view, Util.ScanMode.PropertyExport);
            if (elements != null)
            {
                foreach (Element element in elements)
                {
                    Util.ElementAddToECSchema(creator, element);
                }
            }
            return true;
        }

        //WS: scan the revit model to get elements.
        public static IList<Element> View_getElements(Document document, View view, Util.ScanMode mode)
        {
            List<Element> elements = new List<Element>();

            FilteredElementCollector filteredElementCollector = new FilteredElementCollector(view.Document, view.Id);

            switch (mode)
            {
                case Util.ScanMode.All:
                    {
                        filteredElementCollector.WhereElementIsNotElementType();
                        break;
                    }
                case Util.ScanMode.PropertyExport:
                    {
                        filteredElementCollector.WhereElementIsNotElementType();
                        break;
                    }
            }

            //WS: add elements list to the end of a List.
            elements.AddRange(filteredElementCollector.ToElements());

            Dictionary<string, Element> strs = new Dictionary<string, Element>();

            foreach (Element element in elements)
            {
                Level level = document.GetElement(element.LevelId) as Level;

                if (level == null || strs.ContainsKey(level.Name))
                {
                    continue;
                }

                strs.Add(level.Name, level);
            }

            elements.AddRange(strs.Values);

            return elements;
        }

        public static string View_tryGetValidDWGName(View view, string dwgName, string dwgFolder)
        {
            string str = "";
            string str1 = dwgName.Replace(".dwg", null).Replace('.', '-');
            string str2 = (view.ViewType == ViewType.CeilingPlan ? "Reflected " : "");
            str2 = string.Concat("-", str2);
            string str3 = string.Concat(" - ", view.Name.Replace('.', '-').Replace('*', '-').Replace('/', '-').Replace('\\', '-').Replace('?', '-').Replace('|', '-').Replace('<', '-').Replace('>', '-').Replace('\"', '-'));
            string[] strArrays = new string[] { view.ViewType.ToString(), Util.Element_getObjectTypeName(view), null };
            string title = view.Title;
            char[] chrArray = new char[] { ':' };
            strArrays[2] = title.Split(chrArray)[0];
            string[] strArrays1 = strArrays;
            for (int i = 0; i < (int)strArrays1.Length; i++)
            {
                string str4 = string.Concat(str2, strArrays1[i], str3);
                str = string.Concat(str1, str4);
                if (File.Exists(Path.Combine(dwgFolder, string.Concat(str, ".dwg"))))
                {
                    break;
                }
            }
            return str;
        }

        private enum PWSS4IntegrationStatus
        {
            NotIntegrated = -1,
            Enabled = 0,
            Disabled = 1
        }

        public enum Result
        {
            Failed,
            Succeeded,
            Cancelled
        }

        public enum ScanMode
        {
            All,
            PropertyExport
        }

        public class Transaction : IDisposable
        {
            private Transaction transaction_;

            public Transaction(Document doc, string name)
            {
                this.transaction_ = new Transaction(doc, name);
            }

            public void Dispose()
            {
                this.transaction_.Dispose();
            }

            public void Start()
            {
                this.transaction_.Start();
            }
        }
    }
}
