using Autodesk.Revit.DB;
using Bentley.ECObjects;
using Bentley.ECObjects.Instance;
using Bentley.ECObjects.Schema;
using Bentley.ECObjects.Standards;
using Bentley.ECObjects.Units;
using Bentley.ECObjects.XML;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PublishImodel
{
    internal class SchemaCreator
    {
        public const string REVIT_SCHEMA_NAME = "Bentley_Revit_Connector_Schema";

        public const string RevitConnectorId = "RevitConnectorID";

        public const string RevitConnectorDocument = "RevitConnectorDocument";

        public const string ECOM_CUSTATTR_CLASS_CLASSTYPE = "ClassType";

        public const string ECOM_CUSTATTR_ATTR_CLASSTYPE = "Type";

        public const string ECOM_CLASSTYPE_AREA = "GraphicSpace";

        public const string CUSTOM_ATTRIBUTE_SPACE_TYPE = "SpaceType";

        public const string PROPERTY_VALUE_SPACE_TYPE_TYPE = "Type";

        public const string SPACE_TYPE_ROOM = "Room";

        private readonly static char[] SPLIT_SEPARATORS;

        private HashSet<string> m_classesToExclude;

        private HashSet<string> m_classesToInclude;

        private bool m_confLoaded;

        private IECSchema m_schema;

        private IECClass m_baseClass;

        public IECSchema Schema
        {
            get
            {
                return this.m_schema;
            }
        }

        static SchemaCreator()
        {
            SchemaCreator.SPLIT_SEPARATORS = new char[] { ',' };
        }

        //Constructor
        public SchemaCreator(Document document, View view)
        {
            this.m_schema = (AssemblySettings.Instance.UniqueSchemaName ? SchemaCreator.CreateBasicSchema(document, view) : SchemaCreator.CreateBasicSchema(null, null));
            this.m_baseClass = this.m_schema.GetClass("RevitBaseClass");
        }

        public IECClass AddClassFromElement(Element el)
        {
            Category category = el.Category;

            ECClass eCClasses = new ECClass(Util.EncodeForECNames(category.Name), this.m_baseClass)
            {
                DisplayLabel = category.Name
            };

            ECClass eCClasses1 = eCClasses;

            //ExpImpLogger.trace(string.Format("Export - creating class {0} - {1}", eCClasses1.Name, eCClasses1.DisplayLabel));
            //ExpImpLogger.trace("Export - adding type properties");

            Element element = Util.Element_getObjectType(el);

            if (element == null)
            {
                //ExpImpLogger.trace("Export - element has no type");
            }
            else
            {
                foreach (Parameter parameter in element.Parameters)
                {
                    this.AddPropertyFromParam(eCClasses1, InstanceTools.CheckParameter(parameter), el, true);
                }
            }

            //ExpImpLogger.trace("Export - adding instance properties");

            foreach (Parameter parameter1 in el.Parameters)
            {
                this.AddPropertyFromParam(eCClasses1, InstanceTools.CheckParameter(parameter1), el, false);
            }

            this.m_schema.AddClass(eCClasses1);

            return eCClasses1;
        }

        private static string AddInstanceLabelSpecificationCalculatedProperty(IECClass clss, string familyType, string familyName)
        {
            string str = SchemaCreator.MakeCalculatedLabelSpecificationExpression(familyType, familyName);
            if (str == "RevitConnectorID")
            {
                return str;
            }
            IECInstance eCInstances = SchemaCreator.CreateStandardCustomAttribute("CalculatedECPropertySpecification", "ECExpression", str);
            if (eCInstances == null)
            {
                return "RevitConnectorID";
            }
            ECProperty eCProperty = new ECProperty("ECInstanceLabelSpec", ECObjects.StringType)
            {
                DisplayLabel = "Label"
            };
            IECProperty eCProperty1 = eCProperty;
            eCProperty1.SetCustomAttribute(eCInstances);
            clss.Add(eCProperty1);
            return eCProperty1.Name;
        }


        public void AddPropertyFromParam(IECClass ecClass, Parameter param, Element element, bool isTypeParam)
        {
            string definitionName = Util.Parameter_getDefinitionName(param);

            if (!SchemaCreator.CanExportparameter(param))
            {
                //ExpImpLogger.trace(string.Format("Export - parameter {0} has invalid type - skipped", str));
                return;
            }

            IECType eCPropTypeFromRevit = SchemaCreator.GetECPropTypeFromRevit(param, element);

            if (eCPropTypeFromRevit == null)
            {
                //ExpImpLogger.trace(string.Format("Export - parameter {0} has no EC type - skipped", str));
                return;
            }
            string groupNameFromParam = SchemaCreator.MakeGroupNameFromParam(param, isTypeParam, ecClass);

            string propertyName = SchemaCreator.MakePropertyNameFromParam(param, isTypeParam);

            object[] name = new object[] { definitionName, propertyName, eCPropTypeFromRevit.Name, groupNameFromParam };

            //ExpImpLogger.trace(string.Format("Export - parameter {0}  - property {1}, type {2}, group {3}", name));
            if (ecClass.HasLocalProperty(propertyName))
            {
                //ExpImpLogger.info(string.Format("Export - property {0} is already present in the class - skipped", str2));
                return;
            }

            //WS: create a new property in schema.
            ECProperty eCProperty = new ECProperty(propertyName, eCPropTypeFromRevit)
            {
                DisplayLabel = definitionName,
                IsReadOnly = true
            };

            IECProperty eCProperty1 = eCProperty;

            GroupDef groupDef = new GroupDef(groupNameFromParam, 0);

            eCProperty1.SetCustomAttribute(groupDef.CreateCategory(this.m_schema));

            SchemaCreator.AddUnitDefinitionFromParam(param, eCProperty1);

            ecClass.Add(eCProperty1);
        }

        private static void AddUnitDefinitionFromParam(Parameter param, IECProperty prop)
        {
            //WS: got error here, throw error without even entering it!
            //WS: this is to get a unit for dgn from Revit element's parameter
            Unit displayUnit = UnitsConverter.GetRevitParameterDisplayUnit(param);

            //WS: Test
            //Unit displayUnit = null;

            if (displayUnit == null || displayUnit.UnitSystem == CustomUnits.ImprSystem)
            {
                return;
            }

            KindOfQuantity kindOfQuantity = UnitsConverter.GetRevitParameterKindOfQuantity(param);

            //KindOfQuantity kindOfQuantity = null;

            if (kindOfQuantity == null)
            {
                return;
            }

            if (string.Equals(displayUnit.Dimension.Name, kindOfQuantity.Dimension.Name, StringComparison.Ordinal))
            {
                IECInstance eCInstances = SchemaCreator.MakeECInstanceFromUnit(displayUnit, kindOfQuantity);

                if (eCInstances != null)
                {
                    prop.SetCustomAttribute(eCInstances);
                }

                return;
            }

            //WS: used for log file
            //object[] objArray = new object[] { Util.Parameter_getDefinitionName(param), displayUnit.Name, prop.Name, kindOfQuantity.Name };
            //ExpImpLogger.warning(string.Format("Export - parameter {0} - mapped units {1} for property {2} are incompatible with assigned KoG {3} - ignoring units", objArray));
        }

        private static string AppendToExpr(string expr, string part, string delim)
        {
            if (string.IsNullOrEmpty(expr))
            {
                return part;
            }
            return string.Concat(expr, delim, part);
        }

        public static bool CanExportparameter(Parameter param)
        {
            bool flag;
            try
            {
                if (param.Definition != null)
                {
                    //WS: modified:
                    //string temp = param.Definition.GetDataType().TypeId;

                    //WS: 2025-08-25
                    //flag = (param.Definition.ParameterType != ParameterType.Invalid || param.StorageType != StorageType.None ? true : false);
                    flag = (param.Definition.GetDataType() != null || param.StorageType != StorageType.None ? true : false);
                }
                else
                {
                    flag = false;
                }
            }
            catch
            {
                return false;
            }
            return flag;
        }

        //WS: create a basic schema
        private static IECSchema CreateBasicSchema(Document document, View view)
        {
            string str = "Bentley_Revit_Connector_Schema";
            DateTime now = DateTime.Now;
            string str1 = string.Concat("Bentley Revit Connector schema, exported on ", now.ToString());
            IECSchema eCSchemas = null;
            string templateSchemaPath = SchemaCreator.GetTemplateSchemaPath();

            //WS: the template schema doesn't exist.
            if (!File.Exists(templateSchemaPath))
            {
                if (document != null)
                {
                    str = Util.Document_getDWGName(document, view, true);
                }
                eCSchemas = new ECSchema(str, 1, 0, "bentleyrvt");
            }
            else
            {
                using (FileStream fileStream = new FileStream(templateSchemaPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (ECSchemaXmlStreamReader eCSchemaXmlStreamReader = new ECSchemaXmlStreamReader(fileStream))
                    {
                        eCSchemas = eCSchemaXmlStreamReader.Deserialize();
                    }
                }
            }

            eCSchemas.Description = str1;

            IECClass @class = eCSchemas.GetClass("MergedSchemaInfo");
            if (@class == null)
            {
                @class = new ECClass("MergedSchemaInfo")
                {
                    IsCustomAttribute = true
                };

                @class.Add(new ECProperty("MergedSchemaName", ECObjects.StringType));
                @class.Add(new ECProperty("MergedSchemaMajorVersion", ECObjects.IntegerType));
                @class.Add(new ECProperty("MergedSchemaMinorVersion", ECObjects.IntegerType));
                @class.Add(new ECProperty("MergedSchemaNamespacePrefix", ECObjects.StringType));

                eCSchemas.AddClass(@class);

                IECInstance eCInstances = @class.CreateInstance();

                if (eCInstances != null)
                {
                    eCInstances["MergedSchemaName"].StringValue = "Bentley_Revit_Schema";
                    eCInstances["MergedSchemaMajorVersion"].IntValue = 1;
                    eCInstances["MergedSchemaMinorVersion"].IntValue = 0;
                    eCInstances["MergedSchemaNamespacePrefix"].StringValue = "bentleyrvt";
                    eCSchemas.SetCustomAttribute(eCInstances);
                }
            }

            IECClass eCClasses = eCSchemas.GetClass("RevitBaseClass");
            if (eCClasses == null)
            {
                eCClasses = new ECClass("RevitBaseClass")
                {
                    IsDomainClass = false
                };
                ECProperty eCProperty = new ECProperty("RevitConnectorID", ECObjects.StringType)
                {
                    DisplayLabel = "Revit ID",
                    IsReadOnly = true
                };
                eCClasses.Add(eCProperty);
                ECProperty eCProperty1 = new ECProperty("RevitConnectorDocument", ECObjects.StringType)
                {
                    DisplayLabel = "Revit Document",
                    IsReadOnly = true
                };
                eCClasses.Add(eCProperty1);
                ECProperty eCProperty2 = new ECProperty("ElementUID", ECObjects.StringType)
                {
                    DisplayLabel = "Revit Element UID",
                    IsReadOnly = true
                };
                eCClasses.Add(eCProperty2);
                IECInstance eCInstances1 = SchemaCreator.CreateStandardCustomAttribute("SyncIDSpecification", "Property", "RevitConnectorID");
                if (eCInstances1 != null)
                {
                    eCClasses.SetCustomAttribute(eCInstances1);
                }
                eCSchemas.AddClass(eCClasses);
            }

            //WS: Disable the part that creates "Site" class
            //IECClass class1 = eCSchemas.GetClass("Site");

            //if (class1 == null)
            //{
            //    class1 = new ECClass("Site", eCClasses);
            //    ECProperty eCProperty3 = new ECProperty("Latitude", ECObjects.DoubleType)
            //    {
            //        IsReadOnly = true
            //    };
            //    class1.Add(eCProperty3);
            //    ECProperty eCProperty4 = new ECProperty("Longitude", ECObjects.DoubleType)
            //    {
            //        IsReadOnly = true
            //    };
            //    class1.Add(eCProperty4);
            //    ECProperty eCProperty5 = new ECProperty("Angle", ECObjects.DoubleType)
            //    {
            //        IsReadOnly = true
            //    };
            //    class1.Add(eCProperty5);
            //    ECProperty eCProperty6 = new ECProperty("Source", ECObjects.StringType)
            //    {
            //        IsReadOnly = true
            //    };
            //    class1.Add(eCProperty6);
            //    eCSchemas.AddClass(class1);
            //}


            IECClass eCClasses1 = eCSchemas.GetClass("ClashDetectionSyncSpecification");
            if (eCClasses1 == null)
            {
                ECClass eCClasses2 = new ECClass("ClashDetectionSyncSpecification")
                {
                    Description = "Custom attribute that identifies if the SyncID should be used for comparing clash results",
                    DisplayLabel = "Clash Detection Synchronization Specification",
                    IsCustomAttribute = true
                };
                eCClasses1 = eCClasses2;
                eCClasses1.Add(new ECProperty("Priority", ECObjects.IntegerType));
                eCSchemas.AddClass(eCClasses1);
                IECInstance eCInstances2 = eCClasses1.CreateInstance();
                if (eCInstances2 != null)
                {
                    eCInstances2["Priority"].IntValue = 1073741824;
                    eCSchemas.SetCustomAttribute(eCInstances2);
                }
            }
            return eCSchemas;
        }

        private static IECInstance CreateStandardCustomAttribute(string className, string propertyName, string propertyValue)
        {
            IECSchema bentleyStandardCustomAttributes = StandardSchemas.Bentley_Standard_CustomAttributes;
            if (bentleyStandardCustomAttributes == null)
            {
                return null;
            }
            IECClass @class = bentleyStandardCustomAttributes.GetClass(className);
            if (@class == null)
            {
                return null;
            }
            IECInstance eCInstances = @class.CreateInstance();
            if (eCInstances == null)
            {
                return null;
            }
            IECPropertyValue eCPropertyValue = eCInstances.FindPropertyValue(propertyName, true, false, false);
            if (eCPropertyValue == null)
            {
                return null;
            }
            eCPropertyValue.StringValue = propertyValue;
            return eCInstances;
        }

        public static IECType GetECPropTypeFromRevit(Parameter param, Element element)
        {
            StorageType storageType = param.StorageType;

            IECType ecType = null;

            //string temp1 = param.Definition.GetDataType().TypeId;
            //string temp2 = SpecTypeId.Boolean.YesNo.TypeId;
            //string temp3 = param.Definition.ParameterType.ToString();

            switch (storageType)
            {
                case StorageType.Integer:
                    {
                        //WS: modified

                        //WS: 2025-08-25
                        if (param.Definition.GetDataType() == SpecTypeId.Boolean.YesNo)
                        //if (param.Definition.ParameterType == ParameterType.YesNo)
                        {
                            ecType = ECObjects.BooleanType;
                            break;
                        }
                        else if (EnumResolver.IsEnum(param, element))
                        {
                            ecType = ECObjects.StringType;
                            break;
                        }
                        else if (param.Id.Value != -1002053)
                        {
                            ecType = ECObjects.IntegerType;
                            break;
                        }
                        else
                        {
                            ecType = ECObjects.StringType;
                            break;
                        }
                    }
                case StorageType.Double:
                    {
                        //WS: test

                        //WS: 2025-08-25
                        if (!UnitsConverter.ExportNumberAsValueString(param.Definition.GetDataType()))
                        //if (!UnitsConverter.ExportNumberAsValueString(param.Definition.ParameterType))
                        {
                            ecType = ECObjects.DoubleType;
                            break;
                        }
                        else
                        {
                            ecType = ECObjects.StringType;
                            break;
                        }

                        //WS: test, works
                        //ecType = ECObjects.StringType;
                        //break;
                    }
                case StorageType.String:
                    {
                        ecType = ECObjects.StringType;
                        break;
                    }
                case StorageType.ElementId:
                    {
                        ecType = ECObjects.StringType;
                        break;
                    }
            }
            return ecType;
        }

        public static string GetInstanceLabelSpecification(IECClass clss)
        {
            string name;
            string str = null;
            string name1 = null;
            using (IEnumerator<IECProperty> enumerator = clss.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    IECProperty current = enumerator.Current;
                    if (current.Name.StartsWith("name", StringComparison.OrdinalIgnoreCase))
                    {
                        name = current.Name;
                        return name;
                    }
                    else if (!current.Name.StartsWith("FamilyType", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!current.Name.StartsWith("FamilyName", StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }
                        name1 = current.Name;
                    }
                    else
                    {
                        str = current.Name;
                    }
                }
                return SchemaCreator.AddInstanceLabelSpecificationCalculatedProperty(clss, str, name1);
            }
            //return name;
        }

        //WS: get the schema template xml file.
        public static string GetTemplateSchemaPath()
        {
            string mPRProgramPath = Util.GetIMPRProgramPath();
            if (mPRProgramPath == null)
            {
                return null;
            }
            mPRProgramPath = string.Concat(mPRProgramPath, "schemas\\Bentley_Revit_Connector_Schema.01.01.ecschema.xml");
            return mPRProgramPath;
        }

        public bool IsExportableClass(string categoryName)
        {
            this.LoadConfiguration();
            bool flag = false;
            if (this.m_classesToInclude == null || this.m_classesToInclude.Count == 0)
            {
                flag = true;
            }
            else if (this.m_classesToInclude.Contains(categoryName))
            {
                flag = true;
            }
            if (flag && this.m_classesToExclude != null && this.m_classesToExclude.Contains(categoryName))
            {
                flag = false;
            }
            return flag;
        }

        private void LoadConfiguration()
        {
            if (this.m_confLoaded)
            {
                return;
            }
            string includeCategories = AssemblySettings.Instance.IncludeCategories;
            if (!string.IsNullOrEmpty(includeCategories))
            {
                string[] strArrays = includeCategories.Split(SchemaCreator.SPLIT_SEPARATORS, StringSplitOptions.RemoveEmptyEntries);
                if ((int)strArrays.Length > 0)
                {
                    this.m_classesToInclude = new HashSet<string>(StringComparer.Ordinal);
                    string[] strArrays1 = strArrays;
                    for (int i = 0; i < (int)strArrays1.Length; i++)
                    {
                        string str = strArrays1[i];
                        if (!string.IsNullOrWhiteSpace(str))
                        {
                            this.m_classesToInclude.Add(str.Trim());
                        }
                    }
                }
            }
            includeCategories = AssemblySettings.Instance.ExcludeCategories;
            if (!string.IsNullOrEmpty(includeCategories))
            {
                string[] strArrays2 = includeCategories.Split(SchemaCreator.SPLIT_SEPARATORS, StringSplitOptions.RemoveEmptyEntries);
                if ((int)strArrays2.Length > 0)
                {
                    this.m_classesToExclude = new HashSet<string>(StringComparer.Ordinal);
                    string[] strArrays3 = strArrays2;
                    for (int j = 0; j < (int)strArrays3.Length; j++)
                    {
                        string str1 = strArrays3[j];
                        if (!string.IsNullOrWhiteSpace(str1))
                        {
                            this.m_classesToExclude.Add(str1.Trim());
                        }
                    }
                }
            }
            this.m_confLoaded = true;
        }

        private static string MakeCalculatedLabelSpecificationExpression(string familyType, string familyName)
        {
            string str = " & \" \" & ";
            string empty = string.Empty;
            if (!string.IsNullOrEmpty(familyName))
            {
                empty = SchemaCreator.AppendToExpr(empty, string.Concat("this.", familyName), str);
            }
            if (!string.IsNullOrEmpty(familyType))
            {
                empty = SchemaCreator.AppendToExpr(empty, string.Concat("this.", familyType), str);
            }
            string str1 = "RevitConnectorID";
            if (empty != string.Empty)
            {
                str1 = string.Concat("\"(\" & this.", str1, "& \")\"");
            }
            empty = SchemaCreator.AppendToExpr(empty, str1, str);
            return empty;
        }

        private static IECInstance MakeECInstanceFromUnit(Unit unit, KindOfQuantity kindOfQuantity)
        {
            IECSchema unitAttributesSchema = UnitsSchemaCreator.UnitAttributesSchema;

            if (unitAttributesSchema == null)
            {
                return null;
            }

            IECClass @class = unitAttributesSchema.GetClass(UnitsConstants.UnitSpecification);

            if (@class == null)
            {
                return null;
            }

            IECInstance eCInstances = @class.CreateInstance();

            if (eCInstances != null)
            {
                eCInstances.SetAsString(UnitsConstants.KindOfQuantityName, kindOfQuantity.Name);
                eCInstances.SetAsString(UnitsConstants.DimensionName, unit.Dimension.Name);
                eCInstances.SetAsString(UnitsConstants.UnitName, unit.Name);
            }
            return eCInstances;
        }

        public static string MakeGroupNameFromParam(Parameter param, bool isTypeParam, IECClass ecClass)
        {
            StringBuilder stringBuilder = new StringBuilder(64);

            //WS: 2025-08-25 updated for 2024 later
            //stringBuilder.Append(LabelUtils.GetLabelFor(param.Definition.ParameterGroup));
            stringBuilder.Append(LabelUtils.GetLabelForGroup(param.Definition.GetGroupTypeId()));

            stringBuilder.Append(" (").Append(ecClass.DisplayLabel).Append(' ');
            if (!isTypeParam)
            {
                //stringBuilder.Append(Resources.GetString("Instance"));
                stringBuilder.Append("Instance");
            }
            else
            {
                //stringBuilder.Append(Resources.GetString("Type"));
                stringBuilder.Append("Type");
            }
            stringBuilder.Append(')');
            return stringBuilder.ToString();
        }

        //public static string zzzMakePropertyNameFromParam(Parameter param, bool isTypeParam)
        //{
        //    StringBuilder stringBuilder = new StringBuilder(Util.EncodeForECNames(Util.Parameter_getDefinitionName(param)), 64);

        //    //WS: i.e. '_', under score.
        //    stringBuilder.Append('\u005F');

        //    //WS: 2025-08-25 updated for 2024 later
        //    stringBuilder.Append(Util.EncodeForECNames(Enum.GetName(param.Definition.ParameterGroup.GetType(), param.Definition.ParameterGroup)));

        //    stringBuilder.Append('\u005F');

        //    //WS: 2025-08-25
        //    //stringBuilder.Append(Util.EncodeForECNames(param.Definition.ParameterType.ToString()));
        //    stringBuilder.Append(Util.EncodeForECNames(param.Definition.GetDataType().TypeId));
        //    if (isTypeParam)
        //    {
        //        stringBuilder.Append("_TYPE");
        //    }
        //    return stringBuilder.ToString();
        //}

        public static string MakePropertyNameFromParam(Parameter param, bool isTypeParam)
        {
            // Start with the parameter definition name
            StringBuilder stringBuilder = new StringBuilder(
                Util.EncodeForECNames(Util.Parameter_getDefinitionName(param)), 64);

            stringBuilder.Append('_');

            // ✅ Revit 2024: Use ForgeTypeId + LabelUtils.GetLabelForGroup
            ForgeTypeId groupTypeId = param.Definition.GetGroupTypeId();
            string groupName = LabelUtils.GetLabelForGroup(groupTypeId);

            stringBuilder.Append(Util.EncodeForECNames(groupName));

            stringBuilder.Append('_');

            // Data type (still a ForgeTypeId, but TypeId string is stable)
            stringBuilder.Append(Util.EncodeForECNames(param.Definition.GetDataType().TypeId));

            if (isTypeParam)
            {
                stringBuilder.Append("_TYPE");
            }

            return stringBuilder.ToString();
        }

        public void MarkRoomClass(IECClass ecRoomClass)
        {
            //ExpImpLogger.trace(string.Format("Export - marking class {0} as Room", ecRoomClass.Name));
            IECClass @class = this.m_schema.GetClass("ClassType");
            if (@class == null)
            {
                ECClass eCClasses = new ECClass("ClassType")
                {
                    DisplayLabel = "Class Type",
                    IsCustomAttribute = true,
                    IsDomainClass = false
                };
                @class = eCClasses;
                ECProperty eCProperty = new ECProperty("Type", ECObjects.StringType)
                {
                    Description = "Type of the class"
                };
                @class.Add(eCProperty);
                this.m_schema.AddClass(@class);
            }
            IECInstance eCInstances = @class.CreateInstance();
            eCInstances.SetAsString("Type", "GraphicSpace");
            ecRoomClass.SetCustomAttribute(eCInstances);
            @class = this.m_schema.GetClass("SpaceType");
            if (@class == null)
            {
                ECClass eCClasses1 = new ECClass("SpaceType")
                {
                    DisplayLabel = "Attribute Type",
                    IsCustomAttribute = true,
                    IsDomainClass = false
                };
                @class = eCClasses1;
                ECProperty eCProperty1 = new ECProperty("Type", ECObjects.StringType)
                {
                    Description = "Type of the attribute"
                };
                @class.Add(eCProperty1);
                this.m_schema.AddClass(@class);
            }
            eCInstances = @class.CreateInstance();
            eCInstances.SetAsString("Type", "Room");
            ecRoomClass.SetCustomAttribute(eCInstances);
        }

        public void PostProcess()
        {
            IECClass[] classes = this.m_schema.GetClasses();
            for (int i = 0; i < (int)classes.Length; i++)
            {
                IECClass eCClasses = classes[i];
                if (!eCClasses.IsCustomAttribute && eCClasses.IsDomainClass && !(eCClasses is IECRelationshipClass))
                {
                    string instanceLabelSpecification = SchemaCreator.GetInstanceLabelSpecification(eCClasses);
                    IECInstance eCInstances = SchemaCreator.CreateStandardCustomAttribute("InstanceLabelSpecification", "PropertyName", instanceLabelSpecification);
                    if (eCInstances != null)
                    {
                        eCClasses.SetCustomAttribute(eCInstances);
                    }
                }
            }
        }
    }
}