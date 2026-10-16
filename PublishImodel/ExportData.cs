using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Bentley.ECObjects.Instance;
using Bentley.ECObjects.Schema;
using Bentley.IMPR.XML2DGN;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace PublishImodel
{
    public class ExportData
    {
        private static int m_revitId;

        private Autodesk.Revit.ApplicationServices.Application m_revitApplication;

        private Document m_revitDocument;

        private Autodesk.Revit.DB.View m_revitView;

        private ExportOptionsData m_dwgOptionsData;

        private string m_tempBaseName;

        private string m_tempFolder;

        public string DestinationFileName
        {
            get;
            set;
        }

        public string DestinationFolder
        {
            get;
            set;
        }

        public DWGExportOptions DwgCustomOptions
        {
            get;
            set;
        }

        public static string FILE_EXTENSION
        {
            get
            {
                if (AssemblySettings.Instance.ExportAsMobileModel && Util.IsItgDgnDbConverterInstalled)
                {
                    return ".imodel";
                }
                return ".i.dgn";
            }
        }

        private string FullPath
        {
            get
            {
                return Path.Combine(this.DestinationFolder, this.DestinationFileName);
            }
        }

        public bool IsExportDGN
        {
            get;
            set;
        }

        public bool IsExportGraphics
        {
            get;
            set;
        }

        public bool IsExportIModelXML
        {
            get;
            set;
        }

        public bool IsMergeFiles
        {
            get;
            set;
        }

        public bool IsSharedCoords
        {
            get;
            set;
        }

        static ExportData()
        {
            ExportData.m_revitId = 1;
        }

        public ExportData(Autodesk.Revit.ApplicationServices.Application application, Document document, Autodesk.Revit.DB.View view)
        {
            this.m_revitApplication = application;
            this.m_revitDocument = document;
            this.m_revitView = (view == null ? document.ActiveView : view);
            this.m_dwgOptionsData = new ExportOptionsData(document);
            this.DestinationFileName = string.Concat(Util.Document_getDWGName(this.m_revitDocument, this.m_revitView, false), ExportData.FILE_EXTENSION);
            this.DestinationFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            this.m_tempBaseName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            this.m_tempFolder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        }

        //WS: this dll can't be decompiled. It exports dwg graphics to an xml.
        [DllImport("Bentley.IMPR.DWG2XML.dll", CharSet = CharSet.Unicode, ExactSpelling = false)]
        private static extern bool convertDWG2XML(string xmlFileName, string dwgFileName);

        private void DeleteTempFolder()
        {
            try
            {
                if (!AssemblySettings.Instance.RemainTmp)
                {
                    //ExpImpLogger.trace("Export - deleting our temp folder");

                    Directory.Delete(this.m_tempFolder, true);
                }
                else
                {
                    //ExpImpLogger.trace("Export - remaining our temp folder");
                }
            }
            catch
            {
            }
        }

        //WS: the main export function
        public bool Export(string hostInfo)
        //public bool Export(string hostInfo, ProgressManager man)
        {
            //double num1 = 15;

            Directory.CreateDirectory(this.m_tempFolder);

            //WS: exported dwg file full name.
            string dwgFileName = string.Concat(Path.Combine(this.m_tempFolder, this.m_tempBaseName), ".dwg");

            //WS: exported EC data xml file full name, so the name is like "*.dwg.xml".
            string ecxmlFileName = string.Concat(dwgFileName, ".xml");

            //WS: the xml file full name, exported from DWG for graphics.
            string xmlFileName = Path.ChangeExtension(dwgFileName, ".xml");

            //WS: the final dgn file full name
            string dgnFileName = Path.ChangeExtension(dwgFileName, ".dgn");

            bool flag = false;

            //bool isStarted = !man.IsStarted;

            //if (isStarted)
            //{
            //    man.Start();
            //}

            try
            {
                //man.SetSubTaskText(Resources.GetString("Exporting_file"));

                //WS: if ExportGraphics is not true, return true; otherwise, export graphics, if success, return true
                //Step 1: export Revit view graphics to dwg
                flag = !IsExportGraphics ? true : ExportGraphics();
                //flag = (!this.IsExportGraphics ? true : this.ExportGraphics(man));

                //WS: if export graphics fails or not export to iModel XmL, do nothing.
                if (!flag || !this.IsExportIModelXML)
                //if (man.CancellationPending || !flag || !this.IsExportIModelXML)
                {
                    //man.ReportProgress(num1);
                }
                else //WS: scan Revit model to get all elements
                {
                    IList<Element> elements = Util.View_getElements(this.m_revitDocument, this.m_revitView, Util.ScanMode.PropertyExport);

                    int count = elements.Count;

                    //WS: what is this for??
                    using (Transaction transaction = new Transaction(this.m_revitDocument))
                    {
                        try
                        {
                            transaction.Start("GetTopologies");
                            foreach (PlanTopology planTopology in this.m_revitDocument.PlanTopologies)
                            {
                                count += planTopology.GetRoomIds().Count;
                            }
                            transaction.Commit();
                        }
                        catch
                        {
                            //ExpImpLogger.error(string.Concat("Failed to count Rooms with exception: ", exception.ToString()));
                            if (transaction.GetStatus() != TransactionStatus.Uninitialized)
                            {
                                transaction.RollBack();
                            }
                        }
                    }

                    //if (count > 1500)
                    //{
                    //    num1 = 1000 / ((double)count + 1000) * 100;
                    //}

                    //man.ReportProgress(num1);

                    //WS: export EC data to an xml file
                    flag = this.ExportECData(ecxmlFileName, elements);

                }

                //WS: if export EC data successful and "Export to dgn" is true
                if (flag && this.IsExportDGN)
                //if (!man.CancellationPending && flag && this.IsExportDGN)
                {
                    flag = false;

                    //man.SetSubTaskText(Resources.GetString("Publishing_sub_task"));
                    //man.SneakForward(3);

                    //WS: monitor the temp folder for .pgs file, which is renamed from the dwg file.
                    using (FileSystemWatcher fileSystemWatcher = new FileSystemWatcher(this.m_tempFolder, "*.pgs"))
                    {
                        fileSystemWatcher.NotifyFilter = NotifyFilters.FileName;
                        fileSystemWatcher.Created += new FileSystemEventHandler((object sender, FileSystemEventArgs e) =>
                        {

                            //double num;

                            string str = null;

                            while (File.Exists(e.FullPath))
                            {
                                if (string.IsNullOrEmpty(str))
                                {
                                    try
                                    {
                                        str = File.ReadAllText(e.FullPath);
                                    }
                                    catch
                                    {
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }

                            while (File.Exists(e.FullPath))
                            {
                                try
                                {
                                    File.Delete(e.FullPath);
                                }
                                catch
                                {
                                }
                            }

                            //if (double.TryParse(str, out num))
                            //{
                            //    man.ReportProgress(38 + num * 60 * 0.95);
                            //}
                        });

                        fileSystemWatcher.EnableRaisingEvents = true;

                        using (ManualResetEvent manualResetEvent = new ManualResetEvent(false))
                        {
                            Thread thread = null;

                            if (AssemblySettings.Instance.SingleThread)
                            {
                                manualResetEvent.Set();
                            }
                            else
                            {
                                thread = new Thread(() =>
                                {

                                    //WS: export dwg graphics to an xml file.
                                    ExportData.ExportDWG2XML(xmlFileName, dwgFileName);

                                    manualResetEvent.Set();
                                })
                                {
                                    Name = "DWG2XML"
                                };

                                thread.Start();


                                //WS: keep checking to see if this xml file is generated.
                                do
                                {
                                    if (File.Exists(xmlFileName))
                                    //if (man.CancellationPending || File.Exists(str3))
                                    {
                                        //goto Label0;
                                        break;
                                    }
                                }
                                while (!manualResetEvent.WaitOne(100));
                            }

                            //WS: the full path of "Bentley.IMPR.DWG2DGN.exe"
                            string dwg2dgnExeFullPath = Path.Combine(Path.GetDirectoryName(this.GetType().Assembly.Location), "Bentley.IMPR.DWG2DGN.exe");

                            //WS: if "Bentley.IMPR.DWG2DGN.exe" doesn't exist.
                            if (!File.Exists(dwg2dgnExeFullPath))
                            {
                                flag = (AssemblySettings.Instance.SingleThread ? ExportData.ExportDWG2XML(xmlFileName, dwgFileName) : File.Exists(xmlFileName));

                                if (flag)
                                {
                                    flag = false;
                                    //if (!man.CancellationPending)
                                    //{
                                    using (ManualResetEvent manualResetEvent1 = new ManualResetEvent(false))
                                    {
                                        using (ManualResetEvent manualResetEvent2 = new ManualResetEvent(false))
                                        {
                                            RevitXml2DgnWriter revitXml2DgnWriter = new RevitXml2DgnWriter(dgnFileName, "RevitModel");

                                            revitXml2DgnWriter.SetFileProvenance(this.m_revitDocument.PathName, this.FullPath);

                                            Thread thread1 = new Thread(() =>
                                            {
                                                //WS: change ext "dwg" to ".pgs", why? for progress??
                                                string str = Path.ChangeExtension(dwgFileName, ".pgs");

                                                double progress = double.NaN;

                                                do
                                                {
                                                    if (File.Exists(str) || progress == revitXml2DgnWriter.Progress)
                                                    {
                                                        continue;
                                                    }
                                                    progress = revitXml2DgnWriter.Progress;
                                                    File.WriteAllText(str, (0.2 * progress).ToString());
                                                }
                                                while (!manualResetEvent1.WaitOne(50));

                                                do
                                                {
                                                    if (File.Exists(str) || progress == revitXml2DgnWriter.Progress)
                                                    {
                                                        continue;
                                                    }
                                                    progress = revitXml2DgnWriter.Progress;
                                                    File.WriteAllText(str, (0.2 + 0.8 * progress).ToString());
                                                }
                                                while (!manualResetEvent2.WaitOne(50));

                                                if (File.Exists(str))
                                                {
                                                    try
                                                    {
                                                        File.Delete(str);
                                                    }
                                                    catch
                                                    {
                                                    }
                                                }
                                            })
                                            {
                                                Name = "PushProgress"
                                            };

                                            thread1.Start();

                                            if (File.Exists(ecxmlFileName))
                                            {
                                                //WS: write EC xml file into dgn!
                                                revitXml2DgnWriter.SetECXML(ecxmlFileName);
                                            }

                                            manualResetEvent1.Set();

                                            //if (!man.CancellationPending)
                                            //{
                                            revitXml2DgnWriter.SetGeometryXML(xmlFileName, 0);
                                            //}

                                            manualResetEvent2.Set();
                                            Thread.Sleep(50);
                                            thread1.Abort();
                                        }
                                    }
                                    //}
                                }
                            }
                            else //WS: if "Bentley.IMPR.DWG2DGN.exe" exists.
                            {
                                using (ManualResetEvent manualResetEvent3 = new ManualResetEvent(false))
                                {
                                    int processId = 0;

                                    Thread thread2 = new Thread(() =>
                                    {

                                        //WS: this is the argument of starting process "Bentley.IMPR.DWG2DGN.exe", which
                                        //defines: if shared block, dwg file as input, and Revit file name
                                        //WS: note: add quotation marks to 
                                        string arguments = string.Format("{0}\"{1}\" \"{2}\" \"{3}\"", new object[] { (AssemblySettings.Instance.IsSharedBlocks ? "/S " : ""), dwgFileName, this.m_revitDocument.PathName, this.FullPath });

                                        //if (!man.CancellationPending)
                                        //{
                                        //WS: call this helper function in xml2dgn.dll, actually this starts a process of "Bentley.IMPR.DWG2DGN.exe", with argument
                                        Helpers.CallHiddenProcess(dwg2dgnExeFullPath, arguments, out processId, "Teigha");

                                        //}
                                        manualResetEvent3.Set();
                                    });

                                    thread2.Start();

                                    while (!manualResetEvent3.WaitOne(100))
                                    //while (!man.CancellationPending && !manualResetEvent3.WaitOne(100))
                                    {
                                    }

                                    //WS: if user cancel, not applicable here.
                                    //if (man.CancellationPending)
                                    //{
                                    //try
                                    //{
                                    //    using (Process processById = Process.GetProcessById(num2))
                                    //    {
                                    //        if (!processById.HasExited)
                                    //        {
                                    //            processById.Kill();
                                    //            processById.WaitForExit();
                                    //        }
                                    //    }
                                    //}
                                    //catch
                                    //{
                                    //}
                                    //thread2.Abort();
                                    //}
                                }
                            }

                            if (!manualResetEvent.WaitOne(0))
                            {
                                thread.Abort();
                            }
                        }

                        //if (!man.CancellationPending)
                        //{
                        //ExpImpLogger.trace("Export - publishing comlpeted");

                        flag = true;
                        //man.ReportProgress(99);
                        //}
                    } //end of using statement
                }

                //if (isStarted)
                //{
                //    man.Stop();
                //}

                //WS: if user cancels..
                //if (man.CancellationPending)
                //{
                //ExpImpLogger.trace("Export - export cancelled by user");
                //flag = false;
                //string fullPath = this.FullPath;
                //if (File.Exists(fullPath))
                //{
                //    //ExpImpLogger.debug(string.Concat((Util.FileDelete(fullPath) ? "Deleted" : "Failed to delete"), " uncompleted target file: ", fullPath));
                //}
                //if (string.Equals(Path.GetExtension(this.DestinationFileName), ".imodel", StringComparison.OrdinalIgnoreCase))
                //{
                //    string[] files = Directory.GetFiles(this.DestinationFolder, string.Concat(this.DestinationFileName, "*"), SearchOption.TopDirectoryOnly);
                //    for (int i = 0; i < (int)files.Length; i++)
                //    {
                //        string str6 = files[i];
                //        //ExpImpLogger.debug(string.Concat((Util.FileDelete(str6) ? "Deleted" : "Failed to delete"), " file related with uncompleted target file: ", str6));
                //    }
                //}
                //}

                //WS: delete the temp folder
                this.DeleteTempFolder();
            }
            catch (Exception exception2)
            {
                Exception exception1 = exception2;


                //ExpImpLogger.error(string.Concat("Export failed with exception: ", exception1.ToString()));
                //if (isStarted)
                //{
                //    man.Stop();
                //}
                this.DeleteTempFolder();
                string str7 = string.Format("Export failed, inner exception text: {0}", exception1.Message);

                //throw new ExpImpException(Resources.GetString("Export_failed"), str7, exception1);
                throw new Exception("Publish iModel Failed. " + str7, exception1);
            }
            return flag;
        }

        //WS: export dwg graphics to an xml file
        private static bool ExportDWG2XML(string xmlFileName, string dwgFileName)
        {
            bool flag;

            //WS: create a temp .ini file.
            string iniFileNme = Path.ChangeExtension(dwgFileName, ".ini");

            try
            {
                if (!File.Exists(iniFileNme))
                {
                    using (StreamWriter streamWriter = new StreamWriter(iniFileNme))
                    {
                        streamWriter.WriteLine("[Optional]");
                        streamWriter.WriteLine("ReadableXML=No");
                        streamWriter.WriteLine("ShareBlocks={0}", (AssemblySettings.Instance.IsSharedBlocks ? "Yes" : "No"));
                    }
                }
                ExportData.setDWG2XMLIniFile(iniFileNme);

                //WS: export dwg to xml file
                flag = ExportData.convertDWG2XML(xmlFileName, dwgFileName);
            }
            catch
            {
                //ExpImpLogger.error(exception.Message);
                flag = false;
            }
            return flag;
        }

        //WS: export EC data to an xml file in temp folder.
        private bool ExportECData(string destXmlFileName, IList<Element> elements)
        //private bool ExportECData(string destXmlFileName, IList<Element> elements, int iMaxElCount, ProgressManager man, double startPercent, double endPercent)
        {
            //bool flag;
            object name;
            object obj;
            object name1;


            int num1 = 0;

            //double num2 = 0.8 * (endPercent - startPercent);
            //double num3 = num2 / (double)iMaxElCount;

            //WS: what is this for??
            Thread thread = new Thread(() =>
            {
                int num = -1;

                //string str = Resources.GetString("Exporting_element_data_form");

                while (Thread.CurrentThread.IsAlive)
                {
                    if (num == num1)
                    {
                        continue;
                    }
                    num = num1;

                    //man.ReportProgress(startPercent + (double)num * num3);
                    //man.SetSubTaskText(string.Format(str, num, iMaxElCount));

                    Thread.Sleep(100);
                }
            });

            thread.Start();

            SchemaCreator schemaCreator = new SchemaCreator(this.m_revitDocument, this.m_revitView);

            List<IECInstance> eCInstances = new List<IECInstance>(elements.Count);

            //WS: disable site instance, so later on the cell for global origin won't be created!! it works!
            //if (this.m_revitDocument.SiteLocation != null)
            //{
            //    XYZ xYZ = new XYZ(0, 0, 0);
            //    ProjectPosition projectPosition = this.m_revitDocument.ActiveProjectLocation.GetProjectPosition(xYZ);
            //    double num4 = Math.Atan2(this.m_revitView.RightDirection.Y, this.m_revitView.RightDirection.X);
            //    if (Math.Abs(this.m_revitView.UpDirection.Z) > 1E-07 || this.m_revitView.ViewType == ViewType.ThreeD)
            //    {
            //        num4 = 0;
            //    }
            //    double num5 = (projectPosition != null ? (projectPosition.Angle - num4) * 180 / 3.14159265358979 : 0);
            //    num5 *= -1;
            //    if (num5 < -180)
            //    {
            //        num5 += 360;
            //    }
            //    if (num5 > 180)
            //    {
            //        num5 -= 360;
            //    }
            //    num5 = Math.Round(num5, 10);
            //    double latitude = this.m_revitDocument.SiteLocation.Latitude * 180 / 3.14159265358979;
            //    latitude = Math.Round(latitude, 10);
            //    double longitude = this.m_revitDocument.SiteLocation.Longitude * 180 / 3.14159265358979;
            //    longitude = Math.Round(longitude, 10);

            //    //WS: this is the key to add Site in
            //    eCInstances.Add(InstanceTools.CreateSite(schemaCreator.Schema.GetClass("Site"), latitude, longitude, num5));
            //}

            if (elements != null)
            {
                //ExpImpLogger.trace("Export - looping elements");

                //WS: loop through all elements to create each instance.
                foreach (Element element in elements)
                {
                    //if (!man.CancellationPending)
                    //{
                    num1++;
                    object obj1 = num1;

                    if (element.Category == null)
                    {
                        obj = null;
                    }
                    else
                    {
                        ////WS: 2025-08-25 added, if category name is "", obj = null
                        //if (element.Category.Name == "")
                        //{
                        //    obj = null;
                        //}
                        //else
                        //{
                        //    obj = element.Category.Name;
                        //}

                        obj = element.Category.Name;
                    }

                    if (Util.Element_getObjectType(element) == null)
                    {
                        name1 = null;
                    }
                    else
                    {
                        name1 = Util.Element_getObjectType(element).Name;
                    }

                    //ExpImpLogger.trace(string.Format("Export - Element {0}, Category: {1}, Object Type: {2}", obj1, obj, name1));

                    if (!Util.Element_isExportable(element, schemaCreator))
                    {
                        //ExpImpLogger.trace("Export - Element skipped");
                    }
                    else
                    {
                        //WS: 2025-08-25 added:
                        if (element.Category.Name == "")
                        {

                        }

                        //WS: get error here!!! Note ?? operator, if left is null, use right
                        IECClass @class = schemaCreator.Schema.GetClass(Util.EncodeForECNames(element.Category.Name)) ?? schemaCreator.AddClassFromElement(element);

                        IECInstance eCInstances1 = InstanceTools.CreateInstance(this.m_revitDocument, null, @class, element, schemaCreator);

                        if (eCInstances1 == null)
                        {
                            continue;
                        }

                        int mRevitId = ExportData.m_revitId;

                        ExportData.m_revitId = mRevitId + 1;

                        eCInstances1.InstanceId = string.Format("REVIT::{0:D}", mRevitId);

                        eCInstances.Add(eCInstances1);
                    }
                    //}
                    //else
                    //{
                    //    flag = false;
                    //    return flag;
                    //}
                }//end of foreach loop
            }

            //ExpImpLogger.trace("Export - looping Rooms");

            //WS: the following seems for processing rooms??
            HashSet<int> nums = new HashSet<int>();

            using (Transaction transaction = new Transaction(this.m_revitDocument))
            {
                try
                {
                    transaction.Start("CollectTopologies");

                    foreach (PlanTopology planTopology in this.m_revitDocument.PlanTopologies)
                    {
                        foreach (ElementId roomId in planTopology.GetRoomIds())
                        {
                            Room room = (Room)this.m_revitDocument.GetElement(roomId);
                            //if (!man.CancellationPending)
                            //{
                            num1++;
                            int num6 = Util.ElementId_getValue(room.Id);
                            if (nums.Contains(num6))
                            {
                                //ExpImpLogger.trace("Export - Element skipped");
                            }
                            else
                            {
                                nums.Add(num6);
                                object obj2 = num1;
                                if (room.Category == null)
                                {
                                    name = null;
                                }
                                else
                                {
                                    name = room.Category.Name;
                                }

                                //ExpImpLogger.trace(string.Format("Export - Element {0}, Category: {1}", obj2, name));

                                if (room.Category == null || !schemaCreator.IsExportableClass(room.Category.Name))
                                {
                                    continue;
                                }
                                IECClass eCClasses = schemaCreator.Schema.GetClass(Util.EncodeForECNames(room.Category.Name));
                                if (eCClasses == null)
                                {
                                    eCClasses = schemaCreator.AddClassFromElement(room);
                                    schemaCreator.MarkRoomClass(eCClasses);
                                }
                                eCInstances.Add(InstanceTools.CreateInstance(this.m_revitDocument, null, eCClasses, room, schemaCreator));
                            }
                            //}
                            //else
                            //{
                            //    flag = false;
                            //    return flag;
                            //}
                        }
                    }
                    transaction.Commit();
                }
                catch
                {
                    //ExpImpLogger.error(string.Concat("Failed to collect Rooms with exception: ", exception.ToString()));

                    if (transaction.GetStatus() != TransactionStatus.Uninitialized)
                    {
                        transaction.RollBack();
                    }
                }

                //comment the following
                //goto Label1;
            }

            //WS: comment the following 2 lines.
            //return flag;
            //Label1:

            thread.Abort();

            //ExpImpLogger.trace("Export - exporting EC instances for linked documents");

            //WS: exporting EC instances for linked documents
            IList<Element> elements1 = Util.Application_getLinkedFileSymbols(this.m_revitApplication, this.m_revitDocument);

            if (elements1 != null)
            {
                foreach (Element element1 in elements1)
                {
                    Document document = Util.Application_getDocumentBySymbol(this.m_revitApplication, element1);

                    if (document == null)
                    {
                        continue;
                    }

                    //man.SetSubTaskText(string.Format("{0}: {1} ({2})", Resources.GetString("PROCESSING_LINK"), Util.Element_getId(element1), Util.Document_getFileName(document)));

                    Util.Document_exportElements(schemaCreator, document, element1, eCInstances, ref ExportData.m_revitId);
                }
            }

            //WS: re-process all classes, add "InstanceLabelSpecification" property.
            schemaCreator.PostProcess();

            //man.ReportProgress(num2);
            //man.SneakForward((int)(endPercent - num2));

            //WS: this is the key step, i.e. write schema and instance into an xml file.
            //WS: note WriteData must be a static method, which will involve a constructor
            ECDataXmlWriter.WriteData(destXmlFileName, schemaCreator.Schema, eCInstances);

            //man.ReportProgress(endPercent);

            return true;
        }

        //WS: export graphics to dwg
        private bool ExportGraphics()
        //private bool ExportGraphics(ProgressManager man)
        {
            bool bExportDWG = false;

            DWGExportOptions dwgCustomOptions = this.DwgCustomOptions ?? new DWGExportOptions()
            {
                ExportingAreas = true,
                LineScaling = this.m_dwgOptionsData.ExportLineScaling,
                MergedViews = this.IsMergeFiles,
                SharedCoords = this.IsSharedCoords,
                TargetUnit = this.m_dwgOptionsData.ExportUnit,
                LayerMapping = this.m_dwgOptionsData.ExportLayerMapping,
                PropOverrides = this.m_dwgOptionsData.ExportLayersAndProperties
            };
            dwgCustomOptions.ExportOfSolids = SolidGeometry.Polymesh;
            dwgCustomOptions.FileVersion = ACADVersion.R2007;

            //if (!man.CancellationPending)
            //{
            bExportDWG = Util.Document_exportViewToDWG(this.m_revitDocument, this.m_revitView, this.m_tempFolder, this.m_tempBaseName, dwgCustomOptions);

            if (!bExportDWG)
            {
                //throw new ExpImpException(Resources.GetString("File_export_failed_Revit_API"), "File export failed - Revit API");
            }
            //}
            return bExportDWG;
        }

        public string GetDefaultFileName(bool addDocPath)
        {
            object[] title = new object[] { this.m_revitDocument.Title, this.m_revitView.ViewType.ToString(), this.m_revitView.Name, ExportData.FILE_EXTENSION };
            string str = string.Format("{0}-{1}-{2}{3}", title);
            if (!addDocPath || string.IsNullOrEmpty(this.m_revitDocument.PathName))
            {
                return str;
            }
            return Path.Combine(Path.GetDirectoryName(this.m_revitDocument.PathName), str);
        }

        //WS: whether keep going..
        public static bool ProceedExport(string destFullPath)
        {
            bool flag;

            if (File.Exists(destFullPath))
            {
                //DialogResult dialogResult = MessageBox.Show(string.Format(Resources.GetString("Already_Exist_Warning"), Path.GetFileName(destFullPath)), Resources.GetString("File_Overwrite_Alert"), MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                DialogResult dialogResult = MessageBox.Show(string.Format("The imodel file {0} already exist, do you want to back it up?", Path.GetFileName(destFullPath)), "Bentley imodel overwrite alert!", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                DialogResult dialogResult1 = dialogResult;
                if (dialogResult1 == DialogResult.Cancel)
                {
                    return false;
                }
                switch (dialogResult1)
                {
                    case DialogResult.Yes:
                        {
                            string str = string.Concat(destFullPath, ".bak");
                            while (File.Exists(str))
                            {
                                str = string.Concat(str, ".bak");
                            }
                            try
                            {
                                File.Move(destFullPath, str);
                                return true;
                            }
                            catch
                            {
                                //MessageBox.Show(Resources.GetString("Cannot_Backup_File"), Resources.GetString("Export_failed"), MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                MessageBox.Show("Cannot backup specified imodel file!", "Publish imodel has failed", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                flag = false;
                                break;
                            }
                            //break;
                        }
                    case DialogResult.No:
                        {
                            try
                            {
                                File.Delete(destFullPath);
                                return true;
                            }
                            catch
                            {
                                //MessageBox.Show(Resources.GetString("File_Cannot_Be_Overwriten"), Resources.GetString("Export_failed"), MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                MessageBox.Show("The specified imodel cannot be overwritten!", "Publish imodel has failed", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                flag = false;
                                break;
                            }
                            //break;
                        }
                    default:
                        {
                            return true;
                        }
                }
                return flag;
            }
            return true;
        }

        //WS: this dll can't be decompiled.
        [DllImport("Bentley.IMPR.DWG2XML.dll", CharSet = CharSet.Unicode, ExactSpelling = false)]
        private static extern bool setDWG2XMLIniFile(string iniFileName);
    }
}