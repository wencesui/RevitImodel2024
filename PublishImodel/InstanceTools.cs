using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.ExternalService;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Structure;
using Bentley.ECObjects;
using Bentley.ECObjects.Instance;
using Bentley.ECObjects.Schema;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace PublishImodel
{
    internal static class InstanceTools
    {
        private static bool AugmentInstanceHardCoded(IECInstance ecInst, IECClass ecClass, Element el)
        {
            IECProperty item = ecClass["Name_PG_IDENTITY_DATA_Text"] ?? ecClass["Comments_PG_IDENTITY_DATA_Text"];
            IECInstance customAttributes = null;
            if (item != null)
            {
                customAttributes = item.GetCustomAttributes("Category");
            }
            if (ecClass.Name == "RVT__x0020__Links" && !string.IsNullOrEmpty(el.Name))
            {
                if (ecClass["RevitConnectorLogicalName"] == null)
                {
                    ECProperty eCProperty = new ECProperty("RevitConnectorLogicalName", ECObjects.StringType)
                    {
                        DisplayLabel = "Logical Name",
                        IsReadOnly = true
                    };
                    IECProperty eCProperty1 = eCProperty;
                    if (customAttributes != null)
                    {
                        eCProperty1.SetCustomAttribute(customAttributes);
                    }
                    ecClass.Add(eCProperty1);
                }
                ecInst.SetAsString("RevitConnectorLogicalName", el.Name);
            }
            string familyName = InstanceTools.GetFamilyName(el);
            string str = familyName;
            if (!string.IsNullOrEmpty(familyName))
            {
                if (ecClass["FamilyName"] == null)
                {
                    ECProperty eCProperty2 = new ECProperty("FamilyName", ECObjects.StringType)
                    {
                        DisplayLabel = "Family",
                        IsReadOnly = true
                    };
                    IECProperty eCProperty3 = eCProperty2;
                    if (customAttributes != null)
                    {
                        eCProperty3.SetCustomAttribute(customAttributes);
                    }
                    ecClass.Add(eCProperty3);
                }
                ecInst.SetAsString("FamilyName", str);
            }
            Element element = Util.Element_getObjectType(el);
            if (element != null)
            {
                if (ecClass["FamilyType"] == null)
                {
                    ECProperty eCProperty4 = new ECProperty("FamilyType", ECObjects.StringType)
                    {
                        DisplayLabel = "Type",
                        IsReadOnly = true
                    };
                    IECProperty eCProperty5 = eCProperty4;
                    if (customAttributes != null)
                    {
                        eCProperty5.SetCustomAttribute(customAttributes);
                    }
                    ecClass.Add(eCProperty5);
                }
                ecInst.SetAsString("FamilyType", (element.Name == str ? string.Empty : element.Name));
            }
            FamilySymbol familySymbol = element as FamilySymbol;
            if (familySymbol != null && familySymbol.Family != null)
            {
                string uniqueId = familySymbol.Family.UniqueId;
                if (!string.IsNullOrEmpty(uniqueId))
                {
                    if (ecClass["FamilyUID"] == null)
                    {
                        ECProperty eCProperty6 = new ECProperty("FamilyUID", ECObjects.StringType)
                        {
                            DisplayLabel = "Family UID",
                            IsReadOnly = true
                        };
                        ECProperty eCProperty7 = eCProperty6;
                        if (customAttributes != null)
                        {
                            eCProperty7.SetCustomAttribute(customAttributes);
                        }
                        ecClass.Add(eCProperty7);
                    }
                    ecInst.SetAsString("FamilyUID", uniqueId);
                }
            }
            return true;
        }

        //WS: it seems to check parameter for some special parameters
        public static Parameter CheckParameter(Parameter param)
        {
            //WS: 2025-08-25 Update for 2024 later version.
            //int integerValue = param.Id.IntegerValue;
            long integerValue = param.Id.Value;

            switch (integerValue)
            {
                case -1150139:
                    {
                        //return param.Element[BuiltInParameter.FBX_LIGHT_TOTAL_LIGHT_LOSS];
                        return param.Element.get_Parameter(BuiltInParameter.FBX_LIGHT_BALLAST_LOSS);
                    }
                case -1150138:
                    {
                        //return param.Element[BuiltInParameter.FBX_LIGHT_INITIAL_COLOR_TEMPERATURE];
                        return param.Element.get_Parameter(BuiltInParameter.FBX_LIGHT_INITIAL_COLOR_TEMPERATURE);
                    }
                default:
                    {
                        if (integerValue == -1150102)
                        {
                            break;
                        }
                        else
                        {
                            return param;
                        }
                    }
            }
            //return param.Element[BuiltInParameter.FBX_LIGHT_LIMUNOUS_FLUX];
            return param.Element.get_Parameter(BuiltInParameter.FBX_LIGHT_LIMUNOUS_FLUX);
        }

        public static IECInstance CreateInstance(Document document, Element link, IECClass ecClass, Element el, SchemaCreator creator)
        {
            //ExpImpLogger.trace("Export - checking properties presence");
            Element element = Util.Element_getObjectType(el);
            if (element != null)
            {
                foreach (Parameter parameter in element.Parameters)
                {
                    if (!SchemaCreator.CanExportparameter(parameter) || ecClass.HasLocalProperty(SchemaCreator.MakePropertyNameFromParam(parameter, true)))
                    {
                        continue;
                    }
                    creator.AddPropertyFromParam(ecClass, parameter, el, true);
                }
            }
            foreach (Parameter parameter1 in el.Parameters)
            {
                if (!SchemaCreator.CanExportparameter(parameter1) || ecClass.HasLocalProperty(SchemaCreator.MakePropertyNameFromParam(parameter1, false)))
                {
                    continue;
                }
                creator.AddPropertyFromParam(ecClass, parameter1, el, false);
            }
            IECInstance eCInstances = ecClass.CreateInstance();
            IECPropertyValue str = eCInstances.FindPropertyValue("RevitConnectorDocument", true, false, false);
            if (str != null)
            {
                str.StringValue = Util.Document_getFileName(document);
            }
            int num = Util.Element_getId(el);
            //ExpImpLogger.trace(string.Concat("Export - setting Element ID ", num));
            str = eCInstances.FindPropertyValue("RevitConnectorID", true, false, false);
            if (str != null)
            {
                if (link == null)
                {
                    str.StringValue = num.ToString();
                }
                else
                {
                    int num1 = Util.Element_getId(link);
                    str.StringValue = string.Concat(num1.ToString(), "_", num.ToString());
                }
            }
            string uniqueId = el.UniqueId;
            //ExpImpLogger.trace(string.Concat("Export - setting Element UID ", uniqueId));
            str = eCInstances.FindPropertyValue("ElementUID", true, false, false);
            if (str != null)
            {
                str.StringValue = uniqueId ?? string.Empty;
            }
            HashSet<string> strs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (Parameter parameter2 in el.Parameters)
            {
                InstanceTools.SetPropertyValue(document, eCInstances, InstanceTools.CheckParameter(parameter2), el, false, strs);
            }

            if (element != null)
            {
                foreach (Parameter parameter3 in element.Parameters)
                {
                    InstanceTools.SetPropertyValue(document, eCInstances, InstanceTools.CheckParameter(parameter3), el, true, strs);
                }
            }
            InstanceTools.AugmentInstanceHardCoded(eCInstances, ecClass, el);
            return eCInstances;
        }

        //WS: this function should be disabled.
        //public static IECInstance CreateSite(IECClass ecClass, double lat, double lon, double angle)
        //{
        //    IECInstance str = ecClass.CreateInstance();
        //    str["Latitude"].DoubleValue = lat;
        //    str["Longitude"].DoubleValue = lon;
        //    str["Angle"].DoubleValue = angle;

        //    //WS: get error here..
        //    //str["Source"].StringValue = Resources.GetString("GCS_FROM_REVIT");
        //    str["Source"].StringValue = "From Revit";
        //    return str;
        //}

        private static string GetFamilyName(Element elem)
        {
            if (elem is Floor)
            {
                //return Resources.GetString("FLOOR");
                return "Floor";
            }
            if (elem is Group)
            {
                //return Resources.GetString("GROUP");
                return "Group";
            }
            if (elem is Rebar)
            {
                //return Resources.GetString("REBAR");
                return "Rebar bar";
            }
            if (elem is SlabEdge)
            {
                //return Resources.GetString("SLABEDGE");
                return "Slab Edge";
            }
            if (elem is Ceiling)
            {
                //return Resources.GetString("CEILING");
                return "Compoud ceiling";
            }
            FamilyInstance familyInstance = elem as FamilyInstance;
            if (familyInstance != null && familyInstance.Symbol != null && familyInstance.Symbol.Family != null)
            {
                return familyInstance.Symbol.Family.Name;
            }
            Wall wall = elem as Wall;
            if (wall != null && wall.WallType != null)
            {
                return wall.WallType.Kind.ToString();
            }
            Duct duct = elem as Duct;
            if (duct != null && duct.DuctType != null)
            {
                long integerValue = duct.DuctType.Id.Value;
                switch (integerValue)
                {
                    case 139183:
                    case 139184:
                    case 139185:
                    case 139191:
                        {
                            //return Resources.GetString("RECTANGULAR_DUCT");
                            return "Rectangular Duct";
                        }
                    case 139186:
                    case 139187:
                    case 139188:
                        {
                            //return Resources.GetString("ROUND_DUCT");
                            return "Round Duct";
                        }
                    case 139189:
                    case 139190:
                        {
                            break;
                        }
                    default:
                        {
                            if (integerValue != 519247)
                            {
                                switch (integerValue)
                                {
                                    case 530206:
                                    case 530207:
                                    case 530208:
                                        {
                                            break;
                                        }
                                    default:
                                        {
                                            goto Label0;
                                        }
                                }
                            }
                            //return Resources.GetString("OVAL_DUCT");
                            return "Oval Duct";
                        }
                }
            }
            Label0:
            CableTray cableTray = elem as CableTray;
            if (cableTray != null && cableTray.GetType() != null)
            {
                switch (cableTray.GetTypeId().Value)
                {
                    case 548230:
                        {
                            //return Resources.GetString("CABLE_TRAY_WITH_FITTINGS");
                            return "Cable Tray with Fittings";
                        }
                    case 548232:
                        {
                            //return Resources.GetString("CABLE_TRAY_WITHOUT_FITTINGS");
                            return "Cable Tray without Fittings";
                        }
                }
            }
            Conduit conduit = elem as Conduit;
            if (conduit != null && conduit.GetType() != null)
            {
                switch (conduit.GetTypeId().Value)
                {
                    case 548235:
                        {
                            //return Resources.GetString("CONDUIT_WITH_FITTINGS");
                            return "Conduit with Fittings";
                        }
                    case 548236:
                        {
                            //return Resources.GetString("CONDUIT_WITHOUT_FITTINGS");
                            return "Conduit without Fittings";
                        }
                }
            }
            FlexDuct flexDuct = elem as FlexDuct;
            if (flexDuct != null && flexDuct.FlexDuctType != null)
            {
                switch (flexDuct.FlexDuctType.Id.Value)
                {
                    case 139192:
                        {
                            //return Resources.GetString("FLEX_DUCT_RECTANGULAR");
                            return "Flex Duct Rectangular";
                        }
                    case 139193:
                        {
                            //return Resources.GetString("FLEX_DUCT_ROUND");
                            return "Flex Duct Round";
                        }
                }
            }
            if (elem is Pipe)
            {
                //return Resources.GetString("PIPE_TYPES");
                return "Pipe Types";
            }
            FlexPipe flexPipe = elem as FlexPipe;
            if (flexPipe == null || flexPipe.FlexPipeType == null || flexPipe.FlexPipeType.Id.Value != 150707)
            {
                return null;
            }
            //return Resources.GetString("FLEX_PIPE_ROUND");
            return "Flex Pipe Round";
        }

        public static IECProperty GetLocalProperty(this IECClass ecClass, string propertyName)
        {
            IECProperty eCProperty;
            using (IEnumerator<IECProperty> enumerator = ecClass.Properties(false).GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    IECProperty current = enumerator.Current;
                    if (!current.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    eCProperty = current;
                    return eCProperty;
                }
                return null;
            }
            //return eCProperty;
        }

        public static string GetParameterValueAsElement(Document activeDocument, Parameter param)
        {
            string name = null;
            string str = Util.Parameter_getDefinitionName(param);
            ElementId elementId = param.AsElementId();
            Element element = Util.Document_getElement(activeDocument, elementId);
            if (element == null || !(element is FamilySymbol))
            {
                //WS: modified:

                //string temp = param.Definition.GetDataType().ToString();
                //string temp1 = param.Definition.GetDataType().TypeId;

                //WS: 2025-08-25
                //if (element == null && param.Definition.ParameterType == ParameterType.Material)
                if (element == null && param.Definition.GetDataType() == SpecTypeId.Reference.Material)
                {
                    return "<By Category>";
                }
                string str1 = str;
                string str2 = str1;
                if (str1 != null)
                {
                    switch (str2)
                    {
                        case "Horizontal Projection":
                        case "Top Vertical Projection":
                        case "Bottom Vertical Projection":
                            {
                                if (element != null)
                                {
                                    break;
                                }
                                switch (Util.ElementId_getValue(param.AsElementId()))
                                {
                                    case -7:
                                        {
                                            return "Default";
                                        }
                                    case -6:
                                    case -5:
                                        {
                                            return "Other";
                                        }
                                    case -4:
                                        {
                                            return "Bottom of Column";
                                        }
                                    case -3:
                                        {
                                            return "Center Of Column";
                                        }
                                    case -2:
                                        {
                                            return "Top Of Column";
                                        }
                                    case -1:
                                        {
                                            return "Auto-detect";
                                        }
                                    default:
                                        {
                                            return "Other";
                                        }
                                }
                                //break;
                            }
                        case "Vertical Projection":
                            {
                                if (element != null)
                                {
                                    break;
                                }
                                int num = Util.ElementId_getValue(param.AsElementId());
                                if (num == -10)
                                {
                                    return "Default";
                                }
                                switch (num)
                                {
                                    case -4:
                                        {
                                            return string.Concat("Bottom of ", (param.Element is Floor ? "Slab" : "Beam"));
                                        }
                                    case -3:
                                        {
                                            return string.Concat("Center ", (param.Element is Floor ? "Of Slab" : "of Beam"));
                                        }
                                    case -2:
                                        {
                                            return string.Concat("Top ", (param.Element is Floor ? "Of Slab" : "of beam"));
                                        }
                                    case -1:
                                        {
                                            return "Auto-detect";
                                        }
                                }
                                return "Other";
                            }
                        case "Position":
                            {
                                if (element != null)
                                {
                                    break;
                                }
                                switch (Util.ElementId_getValue(param.AsElementId()))
                                {
                                    case -3:
                                        {
                                            return "Perpendicular to Face";
                                        }
                                    case -2:
                                        {
                                            return "Parallel to Ground";
                                        }
                                }
                                return "Other";
                            }
                        case "Nosing Profile":
                            {
                                if (element == null)
                                {
                                    return "Default";
                                }
                                if (element.Name != "Stair Nosing - Pan")
                                {
                                    break;
                                }
                                name = "Stair Nosing - Pan";
                                break;
                            }
                        case "Top Constraint":
                            {
                                if (element == null)
                                {
                                    return "Unconnected";
                                }
                                name = "Up to level";
                                break;
                            }
                        case "System Type":
                            {
                                if (element != null)
                                {
                                    break;
                                }
                                return "Undefined";
                            }
                    }
                }
            }
            else
            {
                name = ((FamilySymbol)element).Family.Name;
            }
            string str3 = null;

            if (Util.ElementId_getValue(elementId) < 0)
            {
                //str3 = Resources.GetString("REFERENCE_NONE");
                str3 = "None";
            }
            else if (element != null)
            {
                str3 = Util.Element_getValueString(element);
                str3 = (!string.IsNullOrEmpty(str3) ? string.Format("{0} <{1}>", element.Name, str3) : element.Name);
            }
            else
            {
                //str3 = Resources.GetString("REFERENCE_NONE");
                str3 = "None";
            }
            if (name == null || !(name != str3))
            {
                return str3;
            }
            return string.Concat(name, " : ", str3);
        }

        public static bool HasLocalProperty(this IECClass ecClass, string propertyName)
        {
            return ecClass.GetLocalProperty(propertyName) != null;
        }

        public static void SetPropertyValue(Document activeDocument, IECInstance ecInst, Parameter param, Element element, bool isTypeParam, ICollection<string> assignedNames)
        {
            string str = Util.Parameter_getDefinitionName(param);
            //ExpImpLogger.trace(string.Format("Export - parameter {0} ", str));
            try
            {
                if (!param.HasValue)
                {
                    //ExpImpLogger.trace(string.Format("Export - parameter {0} has no value - skipped", str));
                    return;
                }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                //ExpImpLogger.warning(string.Concat(string.Format("Export - parameter {0} has invalid value - skipped due to: ", str), exception.ToString()));
                return;
            }

            if (SchemaCreator.CanExportparameter(param))
            {
                IECType eCPropTypeFromRevit = SchemaCreator.GetECPropTypeFromRevit(param, element);

                if (eCPropTypeFromRevit == null)
                {
                    //ExpImpLogger.trace(string.Format("Export - parameter {0} has no EC type - skipped", str));
                    return;
                }

                string str1 = SchemaCreator.MakePropertyNameFromParam(param, isTypeParam);

                if (assignedNames.Contains(str1))
                {
                    //ExpImpLogger.info(string.Format("Export - parameter {0} is already mapped to {1} property for the instance - skipping", str, str1));
                    return;
                }

                assignedNames.Add(str1);

                IECPropertyValue eCPropertyValue = ecInst.FindPropertyValue(str1, true, false, false);

                if (eCPropertyValue == null)
                {
                    IECProperty localProperty = ecInst.ClassDefinition.GetLocalProperty(str1);
                    if (localProperty == null)
                    {
                        //ExpImpLogger.warning(string.Format("Export - can't find property {0} in EC class {1} for parameter {1} - skipping", str1, ecInst.ClassDefinition.Name, str));
                        return;
                    }
                    //ExpImpLogger.info(string.Format("Export - created value container for parameter {0} ({1}) using {2} EC property name", str, str1, localProperty.Name));
                    eCPropertyValue = localProperty.CreateValue(ecInst);
                }

                string name = null;

                if (param.Id.Value == -1002053)
                {
                    name = activeDocument.GetWorksetTable().GetWorkset(new WorksetId(param.AsInteger())).Name;
                    if (name != null)
                    {
                        eCPropertyValue.StringValue = name;
                        return;
                    }
                }
                else if (param.Id.Value == -1005438)
                {
                    FamilyThermalProperties familyThermalProperty = FamilyThermalProperties.Find(activeDocument, param.AsString());
                    name = (familyThermalProperty != null ? familyThermalProperty.AnalyticConstructionName : "<None>");
                    if (name != null)
                    {
                        eCPropertyValue.StringValue = name;
                        return;
                    }
                }
                else if (param.Id.Value == -1114147)
                {
                    IExternalServer server = ExternalServiceRegistry.GetService(ExternalServices.BuiltInExternalServices.PipeFittingAndAccessoryPressureDropService).GetServer(new Guid(param.AsString()));
                    name = (server != null ? server.GetName() : "Use Definition on Type");
                    if (name != null)
                    {
                        eCPropertyValue.StringValue = name;
                        return;
                    }
                }
                else if (StorageType.ElementId != param.StorageType)
                {
                    if (eCPropTypeFromRevit == ECObjects.DoubleType)
                    {
                        //WS: modified
                        double revitParameterValueInBentleyUnit = UnitsConverter.GetRevitParameterValueInBentleyUnit(param);

                        //WS: test
                        //double revitParameterValueInBentleyUnit = 1000;

                        string str2 = revitParameterValueInBentleyUnit.ToString(CultureInfo.InvariantCulture);
                        string str3 = revitParameterValueInBentleyUnit.ToString("F5", CultureInfo.InvariantCulture);
                        str3 = str3.Remove(str3.Length - 1);
                        if (str2.StartsWith(str3) || str3.StartsWith(str2))
                        {
                            eCPropertyValue.DoubleValue = revitParameterValueInBentleyUnit;
                        }
                        else
                        {
                            eCPropertyValue.StringValue = str3;
                        }

                        //object[] objArray = new object[] { eCPropTypeFromRevit.Name, param.AsDouble(), param.AsValueString(), eCPropertyValue.NativeValue };
                        //ExpImpLogger.trace(string.Format("Export - parameter type/value/display value/EC value {0}  {1} {2} {3}", objArray));
                        return;
                    }

                    if (eCPropTypeFromRevit == ECObjects.IntegerType)
                    {
                        eCPropertyValue.IntValue = param.AsInteger();

                        //object[] name1 = new object[] { eCPropTypeFromRevit.Name, param.AsInteger(), param.AsValueString(), eCPropertyValue.NativeValue };
                        //ExpImpLogger.trace(string.Format("Export - parameter type/value/display value/EC value {0}  {1} {2} {3}", name1));
                        return;
                    }

                    if (eCPropTypeFromRevit == ECObjects.BooleanType)
                    {
                        eCPropertyValue.NativeValue = param.AsInteger() != 0;

                        //object[] objArray1 = new object[] { eCPropTypeFromRevit.Name, param.AsInteger(), param.AsValueString(), eCPropertyValue.NativeValue };
                        //ExpImpLogger.trace(string.Format("Export - parameter type/value/display value/EC value {0}  {1} {2} {3}", objArray1));
                        return;
                    }

                    name = EnumResolver.GetValueName(param, element) ?? (param.StorageType != StorageType.String ? param.AsValueString() : param.AsString());

                    if (name != null)
                    {
                        eCPropertyValue.StringValue = name;

                        //object[] stringValue = new object[] { eCPropTypeFromRevit.Name, param.AsString(), null, null };
                        //stringValue[2] = (param.AsValueString() == null ? "null" : param.AsValueString());
                        //stringValue[3] = eCPropertyValue.StringValue;
                        //ExpImpLogger.trace(string.Format("Export - parameter type/value/display value/EC value {0}  {1} {2} {3}", stringValue));
                        return;
                    }

                    //object[] name2 = new object[] { eCPropTypeFromRevit.Name, "null", "null", "null" };
                    //ExpImpLogger.trace(string.Format("Export - parameter type/value/display value/EC value {0}  {1} {2} {3}", name2));
                }
                else
                {
                    name = InstanceTools.GetParameterValueAsElement(activeDocument, param);
                    if (name != null)
                    {
                        eCPropertyValue.StringValue = name;
                        return;
                    }
                }
                return;
            }
            //ExpImpLogger.trace(string.Format("Export - parameter {0} has invalid type - skipped", str));
        }
    }
}
