//WS: 2019-03-20 Revit Addin
//Background: the Bentley Revit iModel Plug in, the issue is in the imodel, there is a cell named "KMLPlaceMark" which is the project base point, 
//it's too far away from other physical objects. So when using "Fit All" command, the objects are at a corner. After converting this imodel to Vue, in SPR, same issue.
//So the purpose is to don't create this cell in iModel!
//Re-write from decompiled "Revit.ExportDWG2018.dll" in Bentley Revit iModel Plugin folder C:\Program Files\Bentley\imEW for Revit
//Note: removed following:
//1. Option Form.
//2. resource, all strings now are hard-coded.
//3. progress form.
//4. writing log file.
//5. Check license.

//Note: To disable the part that creates the "KMLPlaceMark" cell, I disabled 3 parts 
//1. Disable export EC data of "Site" in function ExportECData in ExportData.cs
//2. Accordingly, disable creating class "Site" in "SchemaCreator.cs".
//3. And disabled Create Instance of Site in "InstanceTools.cs"


//The basic workflow is
//1. Export Revit model to dwg, using default settings
//2. Export EC data (schema and instances) into an xml file, like "*.dwg.xml".
//3. Export dwg graphics to an xml file by using "Bentley.IMPR.DWG2XML.dll".
//4. Using "Bentley.IMPR.XML2DGN.dll" to create dgn, and import graphic xml above and EC data xml, and then publish to i.dgn.
//5. The "Bentley.IMPR.DWG2DGN.exe" is an exe, so it can be run as process. Actually it's calling "Bentley.IMPR.XML2DGN.dll" to create dgn and i.dgn.

//WS: 2021-11-12
//Rebuild the project based on Revit 2020 API. Note it's .Net Framework 4.7

//WS: 2022-12-0
//Rewrite for Revit 2022. But because in API, DisplayUnitType is not available, it should be replaced by ForgeTypeId.
//Also ParameterType is obsolete, it should also be replaced by ForgeTypeId, use SpecTypeId for enum properties.
//So write UnitsConverter.cs
//1.Replace old "DisplayUnitType" enum by using "UnitTypeId"'s member
//2.Replace old "ParameterType" enum by using SpecTypeId, or SpecTypeId.String, or SpecTypeId.Boolean or or SpecTypeId.Int or SpecTypeId.Reference members

//Note: currently only replace "DisplayUnitType", still use "ParameterType" because for Jansen project, the label sort out and mapping still have "Invalid"
//which is an enum in "ParameterType"
//So later on, when we must replace "ParameterType" enum by using SpecTypeId, we need to be careful about this "Invalid" value.

//WS: 2025-08-25
//Updated for Revit 2024

//WS: 2025-11-25 Add try/catch in external command.

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Globalization;
using System.IO;
using System.Threading;

namespace PublishImodel
{
    /// <summary>
    /// To add an external command to Autodesk Revit 
    /// the developer should implement an object that 
    /// supports the IExternalCommand interface.
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        private string m_hostinfo = "IMPR_RVT_????_x??";

        private bool m_ExportGraphics = true;

        private bool m_ExportIModelXML = true;

        private bool m_ExportDGN = true;

        private bool m_OpenDgnViewer;

        public bool ExportDGN
        {
            get
            {
                return this.m_ExportDGN;
            }
            set
            {
                this.m_ExportDGN = value;
            }
        }

        public bool ExportGraphics
        {
            get
            {
                return this.m_ExportGraphics;
            }
            set
            {
                this.m_ExportGraphics = value;
            }
        }

        public bool ExportIModelXML
        {
            get
            {
                return this.m_ExportIModelXML;
            }
            set
            {
                this.m_ExportIModelXML = value;
            }
        }

        public bool OpenDgnViewer
        {
            get
            {
                return this.m_OpenDgnViewer;
            }
            set
            {
                this.m_OpenDgnViewer = value;
            }
        }

        public Command()
        {
            try
            {
                //WS: log in to ProjectWise
                Util.PWSessionStart();
            }
            catch
            {
            }
        }

        //WS: Revit Add in execute point..
        //WS: 2025-11-25 Add try/catch
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                Result result;
                Application application = commandData.Application.Application;
                Document document = commandData.Application.ActiveUIDocument.Document;

                this.m_hostinfo = Util.Application_getHostInfo(application);

                AssemblySettings instance = AssemblySettings.Instance;

                string language = instance.Language;

                if (!string.IsNullOrEmpty(language))
                {
                    try
                    {
                        //WS: what's culture specific resource?
                        Thread.CurrentThread.CurrentUICulture = new CultureInfo(language);
                    }
                    catch
                    {
                    }
                }

                DWGExportOptions predefinedOptions = null;

                if (!instance.ExportWithDefaults)
                {
                    //WS: in config file "instance.ExportSetupName" is ""
                    predefinedOptions = DWGExportOptions.GetPredefinedOptions(document, instance.ExportSetupName);

                    if (predefinedOptions == null)
                    //if (predefinedOptions == null && !OptionsForm.ContinuePublishWithDefaults())
                    {
                        return (Result)Util.GetResult(Util.Result.Cancelled);
                    }
                }

                //ProgressManager progressManager = new ProgressManager(Resources.GetString("Save_As_Bentley_DGN"));

                //using (Licensing licensing = new Licensing(this.m_hostinfo))
                //{
                //licensing.CheckLicense();

                //WS: No output folder defined in config file
                string outputFolder = instance.OutputFolder;

                if (!string.IsNullOrEmpty(outputFolder))
                {
                    //ExpImpLogger.trace("Silent export mode");
                    //ExpImpLogger.trace(string.Format("Output folder", outputFolder));

                    string str = string.Concat(Util.Document_getDWGName(document, document.ActiveView, false), ExportData.FILE_EXTENSION);
                    str = Path.Combine(outputFolder, str);

                    //ExpImpLogger.trace(string.Format("Exporting to file {0}", str));

                    this.ExecuteExport(application, document, document.ActiveView, str, instance.IsShared, predefinedOptions);

                    result = (Result)Util.GetResult(Util.Result.Succeeded);

                }
                else
                {
                    //WS: output folder is null, so starting from here..
                    result = this.ExecuteExport(application, document, document.ActiveView, null, instance.IsShared, predefinedOptions);

                }
                //}

                return result;
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", ex.ToString());

                return Result.Failed;              
            }
        }

        public Result ExecuteExport(Application application, Document activeDocument, View viewToExport, string exportFileLocation, bool sharedCoords, DWGExportOptions exportOptions)
        //public Result ExecuteExport(Application application, Document activeDocument, View viewToExport, string exportFileLocation, bool sharedCoords, DWGExportOptions exportOptions, ProgressManager progressMan)
        {
            bool flag = false;

            //WS: check if logged into PW.
            try
            {
                //WS: integration with ProjectWise.
                flag = Util.PWSessionStarted();
            }
            catch
            {
            }

            //WS: instantiate ExportData class..
            ExportData exportDatum = new ExportData(application, activeDocument, viewToExport);

            bool flag1 = null != exportFileLocation;

            //WS: if export folder is null
            if (!flag1)
            {
                //WS: get a path for export in temp folder.
                string str = Util.AskUserForSaveFilePath(exportDatum.DestinationFileName, ExportData.FILE_EXTENSION, flag);
                exportFileLocation = str;
                if (str == null)
                {
                    return (Result)Util.GetResult(Util.Result.Cancelled);
                }
            }

            exportDatum.DestinationFolder = Path.GetDirectoryName(exportFileLocation);
            exportDatum.DestinationFileName = Path.GetFileName(exportFileLocation);
            exportDatum.IsMergeFiles = false;
            exportDatum.IsSharedCoords = sharedCoords;
            exportDatum.DwgCustomOptions = exportOptions;
            exportDatum.IsExportGraphics = this.ExportGraphics;
            exportDatum.IsExportIModelXML = this.ExportIModelXML;
            exportDatum.IsExportDGN = this.ExportDGN;

            //ExpImpLogger.trace("Export started");
            //ExpImpLogger.trace(string.Concat("Culture:", Thread.CurrentThread.CurrentUICulture.Name));

            //WS: define a return value first.
            Util.Result result = Util.Result.Succeeded;

            try
            {
                try
                { 
                    //WS: check if should be proceed.. if integreated with PW, keep going..
                    if (flag1 || flag || ExportData.ProceedExport(exportFileLocation))
                    {
                        //WS: Export start..
                        if (!exportDatum.Export(this.m_hostinfo) || !File.Exists(exportFileLocation))
                        {
                            //MessageBox.Show(Resources.GetString("Export_Failed_Verbose"), Resources.GetString("We_Are_Sorry"), MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        }
                        else 
                        {
                            if (flag)
                            {
                                string defaultFileName = exportDatum.GetDefaultFileName(false);
                                string str1 = null;
                                //ExpImpLogger.trace(string.Concat("Ask saving ", exportFileLocation, " into ProjectWise"));

                                //string str2 = Util.SaveAsDocumentDialog(Resources.GetString("PW_Dialog_Title"), exportFileLocation, defaultFileName, Resources.GetString("PW_Dialog_Description"), defaultFileName, ExportData.FILE_EXTENSION);
                                string str2 = Util.SaveAsDocumentDialog("Publish iModel to PW Datasource", exportFileLocation, defaultFileName, "iModel from Revit file", defaultFileName, ExportData.FILE_EXTENSION);

                                str1 = str2;
                                if (str2 != null)
                                {
                                    Util.FileDelete(exportFileLocation);
                                    exportFileLocation = str1;
                                }
                                else
                                {
                                    exportDatum.DestinationFileName = exportDatum.GetDefaultFileName(true);

                                    //ExpImpLogger.trace(string.Concat("Saving to ProjectWise cancelled, ask to save as ", exportDatum.DestinationFileName, " locally"));

                                    str1 = Util.AskUserForSaveFilePathLocal(exportDatum.DestinationFileName, ExportData.FILE_EXTENSION, true);
                                    
                                    //WS: start up Export..
                                    if (str1 == null || !ExportData.ProceedExport(str1))
                                    {
                                        //ExpImpLogger.trace(string.Concat("Saving locally also cancelled, deleting ", exportFileLocation));

                                        Util.FileDelete(exportFileLocation);
                                        result = Util.Result.Cancelled;
                                    }
                                    else //WS: if successful
                                    {
                                        File.Move(exportFileLocation, str1); //WS: move the file from temp folder to destination folder.
                                        exportFileLocation = str1;
                                    }
                                }
                            }
                            //ExpImpLogger.trace(string.Concat("Export completed to ", exportFileLocation));

                            if (this.OpenDgnViewer && !flag1 && File.Exists(exportFileLocation))
                            {
                                Util.IModel_openInBentleyViewer(exportFileLocation);
                            }
                        }
                    }
                }
                catch
                //catch (ExpImpException expImpException1)
                {
                    //ExpImpException expImpException = expImpException1;
                    result = Util.Result.Failed;

                    //ExpImpLogger.error(expImpException.LogTxt);
                    //StringBuilder stringBuilder = new StringBuilder(expImpException.Message);
                    //if (expImpException.InnerException != null)
                    //{
                    //    if (stringBuilder.Length > 0)
                    //    {
                    //        stringBuilder.AppendLine();
                    //    }
                    //    stringBuilder.Append(expImpException.InnerException.Message);
                    //}
                    //stringBuilder.AppendLine().AppendLine();
                    //stringBuilder.AppendLine(Resources.GetString("For_more_details_see1"));
                    //stringBuilder.Append(Resources.GetString("For_more_details_see2"));

                    //MessageBox.Show(stringBuilder.ToString(), Resources.GetString("Save_As_Bentley_DGN"), MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
            finally
            {
                //ExpImpLogger.trace("Export finished");
                //ExpImpLogger.DestroyLogger();
            }

            return (Result)Util.GetResult(result);
        }
    }
}
