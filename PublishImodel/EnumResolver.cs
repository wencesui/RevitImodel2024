using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace PublishImodel
{
    public class EnumResolver
    {
        public EnumResolver()
        {
        }

        public static void GetBuiltInParametersDisplayNameMap(Element elm, Dictionary<string, string> bltMap)
        {
            string[] names = Enum.GetNames(typeof(BuiltInParameter));

            for (int i = 0; i < (int)names.Length; i++)
            {
                string str = names[i];
                BuiltInParameter builtInParameter = (BuiltInParameter)Enum.Parse(typeof(BuiltInParameter), str);

                //Parameter parameter = elm[builtInParameter];
                Parameter parameter = elm.get_Parameter(builtInParameter);

                if (parameter != null)
                {
                    bltMap.Add(str, Util.Parameter_getDefinitionName(parameter));
                }
            }
        }

        public static Type GetType(string categoryName, string enumName)
        {
            string str = EnumResolver.MapTypeName(categoryName, enumName);
            if (str != null)
            {
                Type type = typeof(StructuralWallUsage).Module.GetType(str);
                if (null != type && type.IsValueType && type.IsEnum)
                {
                    return type;
                }
            }
            return null;
        }

        public static string GetValueName(string categoryName, string enumName, int enumVal)
        {
            string str = EnumResolver.HardcodedValue(enumName, enumVal, categoryName);
            if (str == null)
            {
                Type type = EnumResolver.GetType(categoryName, enumName);
                if (null != type)
                {
                    try
                    {
                        string name = Enum.GetName(type, enumVal);
                        str = name;
                        if (name == null)
                        {
                            str = enumVal.ToString();
                            //ExpImpLogger.warning(string.Format("Export - enumeration parameter type {0} resolved as value {1}", enumName, enumVal));
                        }
                    }
                    catch
                    {
                        str = enumVal.ToString();
                        //ExpImpLogger.warning(string.Format("Export - enumeration parameter type {0} resolved as value {1}", enumName, enumVal));
                    }
                }
            }
            return str;
        }

        public static string GetValueName(Parameter param, Element element)
        {
            string valueName = null;

            //WS: modified:
            //string temp = param.Definition.ParameterType.ToString();
            //string temp2 = param.Definition.GetDataType().ToString();

            //WS: 2025-08-25
            //if (param.Definition.ParameterType == ParameterType.Invalid && param.StorageType == StorageType.Integer)
            if (param.Definition.GetDataType() == null && param.StorageType == StorageType.Integer)
            {
                valueName = EnumResolver.GetValueName((element.Category == null ? "" : element.Category.Name), Util.Parameter_getDefinitionName(param), param.AsInteger());
            }
            return valueName;
        }

        public static string HardcodedValue(string enumName, int enumVal, string categoryName)
        {
            int num;
            string str = enumName;
            string str1 = str;
            if (str != null)
            {
                switch (str1)
                {
                    case "Location Line":
                        {
                            switch (enumVal)
                            {
                                case 0:
                                    {
                                        return "Wall Centerline";
                                    }
                                case 1:
                                    {
                                        return "Core Centerline";
                                    }
                                case 2:
                                    {
                                        return "Finish Face: Exterior";
                                    }
                                case 3:
                                    {
                                        return "Finish Face: Interior";
                                    }
                                case 4:
                                    {
                                        return "Core Face: Exterior";
                                    }
                                case 5:
                                    {
                                        return "Core Face: Interior";
                                    }
                            }
                            return "Other";
                        }
                    case "Wrapping at Inserts":
                        {
                            switch (enumVal)
                            {
                                case 0:
                                    {
                                        return "Do not wrap";
                                    }
                                case 1:
                                    {
                                        return "Exterior";
                                    }
                                case 2:
                                    {
                                        return "Interior";
                                    }
                                case 3:
                                    {
                                        return "Both";
                                    }
                            }
                            return "Other";
                        }
                    case "Wrapping at Ends":
                        {
                            switch (enumVal)
                            {
                                case 0:
                                    {
                                        return "None";
                                    }
                                case 1:
                                    {
                                        return "Exterior";
                                    }
                                case 2:
                                    {
                                        return "Interior";
                                    }
                            }
                            return "Other";
                        }
                    case "Color":
                    case "Color Filter":
                    case "Coarse Scale Fill Color":
                        {
                            int num1 = enumVal;
                            if (num1 <= 65535)
                            {
                                if (num1 > 255)
                                {
                                    if (num1 == 65280)
                                    {
                                        return "Green";
                                    }
                                    if (num1 == 65535)
                                    {
                                        return "Yellow";
                                    }
                                }
                                else
                                {
                                    if (num1 == 0)
                                    {
                                        return "Black";
                                    }
                                    if (num1 == 255)
                                    {
                                        return "Red";
                                    }
                                }
                            }
                            else if (num1 > 16711935)
                            {
                                if (num1 == 16776960)
                                {
                                    return "Cyan";
                                }
                                if (num1 == 16777215)
                                {
                                    return "White";
                                }
                            }
                            else
                            {
                                if (num1 == 16711680)
                                {
                                    return "Blue";
                                }
                                if (num1 == 16711935)
                                {
                                    return "Magenta";
                                }
                            }
                            object[] objArray = new object[] { enumVal & 255, enumVal >> 8 & 255, enumVal >> 16 & 255 };
                            return string.Format("RGB {0:D3}-{1:D3}-{2:D3}", objArray);
                        }
                    case "Wall Closure":
                        {
                            switch (enumVal)
                            {
                                case 0:
                                    {
                                        return "By host";
                                    }
                                case 1:
                                    {
                                        return "Neither";
                                    }
                                case 2:
                                    {
                                        return "Interior";
                                    }
                                case 3:
                                    {
                                        return "Exterior";
                                    }
                                case 4:
                                    {
                                        return "Both";
                                    }
                            }
                            return enumVal.ToString();
                        }
                    case "Riser Type":
                        {
                            switch (enumVal)
                            {
                                case 0:
                                    {
                                        return "None";
                                    }
                                case 1:
                                    {
                                        return "Straight";
                                    }
                                case 2:
                                    {
                                        return "Slanted";
                                    }
                            }
                            return "Other";
                        }
                    case "Riser to Tread Connection":
                        {
                            switch (enumVal)
                            {
                                case 0:
                                    {
                                        return "Extend Riser Behind Tread";
                                    }
                                case 1:
                                    {
                                        return "Extend Tread Under Riser";
                                    }
                            }
                            return "Other";
                        }
                    case "Trim Stringers at Top":
                        {
                            switch (enumVal)
                            {
                                case 0:
                                    {
                                        return "Do not trim";
                                    }
                                case 1:
                                    {
                                        return "Match Level";
                                    }
                                case 2:
                                    {
                                        return "Match Landing Stringer";
                                    }
                            }
                            return "Other";
                        }
                    case "Left Stringer":
                    case "Right Stringer":
                        {
                            switch (enumVal)
                            {
                                case 0:
                                    {
                                        return "None";
                                    }
                                case 1:
                                    {
                                        return "Closed";
                                    }
                                case 2:
                                    {
                                        return "Open";
                                    }
                            }
                            return "Other";
                        }
                    case "Apply Nosing Profile":
                        {
                            switch (enumVal)
                            {
                                case 0:
                                    {
                                        return "Front Only";
                                    }
                                case 1:
                                    {
                                        return "Front and Left";
                                    }
                                case 2:
                                    {
                                        return "Front and Right";
                                    }
                                case 3:
                                    {
                                        return "Front, Left and Right";
                                    }
                            }
                            return "Other";
                        }
                    case "Use Landing Height Adjustment":
                        {
                            switch (enumVal)
                            {
                                case 0:
                                    {
                                        return "No";
                                    }
                                case 1:
                                    {
                                        return "Yes";
                                    }
                            }
                            return "Maybe";
                        }
                    case "Angled Joins":
                        {
                            switch (enumVal)
                            {
                                case 0:
                                    {
                                        return "Add Vertical/Horizontal Segments";
                                    }
                                case 1:
                                    {
                                        return "No Connector";
                                    }
                            }
                            return "Other";
                        }
                    case "Tangent Joins":
                        {
                            switch (enumVal)
                            {
                                case 0:
                                    {
                                        return "Add Vertical/Horizontal Segments";
                                    }
                                case 1:
                                    {
                                        return "No Connector";
                                    }
                                case 2:
                                    {
                                        return "Extend Rails to Meet";
                                    }
                            }
                            return "Other";
                        }
                    case "Rail Connections":
                        {
                            switch (enumVal)
                            {
                                case 0:
                                    {
                                        return "Trim";
                                    }
                                case 1:
                                    {
                                        return "Weld";
                                    }
                            }
                            return "Other";
                        }
                    case "Loss Method":
                        {
                            switch (enumVal)
                            {
                                case 0:
                                    {
                                        return "Not Defined";
                                    }
                                case 1:
                                    {
                                        return "K Coefficient from table";
                                    }
                                case 2:
                                    {
                                        return "K Coefficient";
                                    }
                                case 3:
                                    {
                                        return "Specific Loss";
                                    }
                                case 4:
                                    {
                                        return "Other";
                                    }
                                case 5:
                                    {
                                        return "Use Definition on Type";
                                    }
                                default:
                                    {
                                        return "Other";
                                    }
                            }
                            //break;
                        }
                    case "Top Connection Symbol":
                        {
                            if (enumVal == 0)
                            {
                                return "None";
                            }
                            return "Other";
                        }
                    case "Top Release":
                        {
                            switch (enumVal)
                            {
                                case 0:
                                    {
                                        return "Fixed";
                                    }
                                case 1:
                                    {
                                        return "Pinned";
                                    }
                                case 2:
                                    {
                                        return "Bending Moment";
                                    }
                                case 3:
                                    {
                                        return "User Defined";
                                    }
                            }
                            return "Other";
                        }
                    case "Bottom Release":
                        {
                            switch (enumVal)
                            {
                                case 0:
                                    {
                                        return "Fixed";
                                    }
                                case 1:
                                    {
                                        return "Pinned";
                                    }
                                case 2:
                                    {
                                        return "Bending Moment";
                                    }
                                case 3:
                                    {
                                        return "User Defined";
                                    }
                            }
                            return "Other";
                        }
                    case "Rafter Cut":
                        {
                            int num2 = enumVal;
                            switch (num2)
                            {
                                case 0:
                                    {
                                        return "Plumb Cut";
                                    }
                                case 1:
                                    {
                                        return "Two Cut - Plumb";
                                    }
                                case 2:
                                    {
                                        return "Two Cut - Square";
                                    }
                                default:
                                    {
                                        switch (num2)
                                        {
                                            case 33615:
                                                {
                                                    return "Plumb Cut";
                                                }
                                            case 33616:
                                            case 33617:
                                                {
                                                    return "Other";
                                                }
                                            case 33618:
                                                {
                                                    return "Two Cut - Square";
                                                }
                                            case 33619:
                                                {
                                                    return "Two Cut - Plumb";
                                                }
                                            default:
                                                {
                                                    return "Other";
                                                }
                                        }
                                        //break;
                                    }
                            }
                            //break;
                        }
                    case "Analyze As":
                        {
                            switch (enumVal)
                            {
                                case 0:
                                    {
                                        return "Hanger";
                                    }
                                case 1:
                                    {
                                        return "Gravity";
                                    }
                                case 2:
                                    {
                                        return "Lateral";
                                    }
                                case 3:
                                    {
                                        return "Not for Analysis";
                                    }
                                case 4:
                                    {
                                        return "Foundation";
                                    }
                                case 5:
                                    {
                                        return "Other";
                                    }
                                case 6:
                                    {
                                        return "Not for analysis";
                                    }
                                default:
                                    {
                                        return "Other";
                                    }
                            }
                            //break;
                        }
                    case "Rafter or Truss":
                        {
                            switch (enumVal)
                            {
                                case 0:
                                    {
                                        return "Truss";
                                    }
                                case 1:
                                    {
                                        return "Rafter";
                                    }
                            }
                            return "Other";
                        }
                    case "Preferred Junction Type":
                        {
                            switch (enumVal)
                            {
                                case 0:
                                    {
                                        return "Tap";
                                    }
                                case 1:
                                    {
                                        return "Tee";
                                    }
                            }
                            return "Other";
                        }
                    case "Vertical Justification":
                        {
                            switch (enumVal)
                            {
                                case 0:
                                    {
                                        return "Middle";
                                    }
                                case 1:
                                    {
                                        return "Bottom";
                                    }
                                case 2:
                                    {
                                        return "Top";
                                    }
                            }
                            return "Other";
                        }
                    case "Horizontal Justification":
                        {
                            switch (enumVal)
                            {
                                case 0:
                                    {
                                        return "Center";
                                    }
                                case 1:
                                    {
                                        return "Left";
                                    }
                                case 2:
                                    {
                                        return "Right";
                                    }
                            }
                            return "Other";
                        }
                    case "yz Justification":
                        {
                            switch (enumVal)
                            {
                                case 0:
                                    {
                                        return "Uniform";
                                    }
                                case 1:
                                    {
                                        return "Independent";
                                    }
                            }
                            return "Other";
                        }
                    case "Start y Justification":
                    case "End y Justification":
                    case "y Justification":
                        {
                            switch (enumVal)
                            {
                                case 0:
                                    {
                                        return "Left";
                                    }
                                case 1:
                                    {
                                        return "Center";
                                    }
                                case 2:
                                    {
                                        return "Origin";
                                    }
                                case 3:
                                    {
                                        return "Right";
                                    }
                            }
                            return "Other";
                        }
                    case "Start z Justification":
                    case "End z Justification":
                    case "z Justification":
                        {
                            switch (enumVal)
                            {
                                case 0:
                                    {
                                        return "Top";
                                    }
                                case 1:
                                    {
                                        return "Center";
                                    }
                                case 2:
                                    {
                                        return "Origin";
                                    }
                                case 3:
                                    {
                                        return "Bottom";
                                    }
                            }
                            return "Other";
                        }
                    case "Text Background":
                        {
                            switch (enumVal)
                            {
                                case 0:
                                    {
                                        return "Opaque";
                                    }
                                case 1:
                                    {
                                        return "Transparent";
                                    }
                            }
                            return "Other";
                        }
                    case "Text Orientation":
                        {
                            switch (enumVal)
                            {
                                case 0:
                                    {
                                        return "Horizontal";
                                    }
                                case 1:
                                    {
                                        return "Vertical";
                                    }
                            }
                            return "Other";
                        }
                    case "Text Location":
                        {
                            switch (enumVal)
                            {
                                case 0:
                                    {
                                        return "Above Leader";
                                    }
                                case 1:
                                    {
                                        return "Below Leader";
                                    }
                                case 2:
                                    {
                                        return "In-line with Leader";
                                    }
                            }
                            return "Other";
                        }
                    case "Indicator as Prefix / Suffix":
                        {
                            switch (enumVal)
                            {
                                case 0:
                                    {
                                        return "Prefix";
                                    }
                                case 1:
                                    {
                                        return "Suffix";
                                    }
                            }
                            return "Other";
                        }
                    case "Coordinate Origin":
                        {
                            switch (enumVal)
                            {
                                case 0:
                                    {
                                        return "Project Base Point";
                                    }
                                case 1:
                                    {
                                        return "Survey Point";
                                    }
                                case 2:
                                    {
                                        return "Relative";
                                    }
                            }
                            return "Other";
                        }
                    case "Top Value":
                    case "Bottom Value":
                        {
                            switch (enumVal)
                            {
                                case 0:
                                    {
                                        return "None";
                                    }
                                case 1:
                                    {
                                        return "North / South";
                                    }
                                case 2:
                                    {
                                        return "East / West";
                                    }
                            }
                            return "Other";
                        }
                    case "Alternative Units":
                        {
                            switch (enumVal)
                            {
                                case 0:
                                    {
                                        return "None";
                                    }
                                case 1:
                                    {
                                        return "Right";
                                    }
                                case 2:
                                    {
                                        return "Below";
                                    }
                            }
                            return "Other";
                        }
                    case "Shape":
                        {
                            switch (enumVal)
                            {
                                case 0:
                                    {
                                        return "Thick";
                                    }
                                case 1:
                                    {
                                        return "Solid";
                                    }
                            }
                            return "Other";
                        }
                    case "Dimming Lamp Color Temperature Shift":
                        {
                            switch (enumVal)
                            {
                                case 0:
                                    {
                                        return "<None>";
                                    }
                                case 1:
                                    {
                                        return "Incandescent Lamp Curve";
                                    }
                            }
                            return "Other";
                        }
                    case "Curved Edge Condition":
                        {
                            switch (enumVal)
                            {
                                case 0:
                                    {
                                        return "<Not applicable>";
                                    }
                                case 1:
                                    {
                                        return "Conform to curve";
                                    }
                                case 2:
                                    {
                                        return "Project to side";
                                    }
                            }
                            return "Other";
                        }
                    case "Orient to":
                        {
                            switch (enumVal)
                            {
                                case 1:
                                    {
                                        return "Workplane";
                                    }
                                case 2:
                                    {
                                        return "Project";
                                    }
                            }
                            return "Other";
                        }
                    case "Stringer Position":
                        {
                            switch (enumVal)
                            {
                                case 0:
                                    {
                                        return "None";
                                    }
                                case 1:
                                    {
                                        return "Left and Right";
                                    }
                                case 2:
                                    {
                                        return "Left Only";
                                    }
                                case 3:
                                    {
                                        return "Middle Only";
                                    }
                                case 4:
                                    {
                                        return "Right Only";
                                    }
                                case 5:
                                    {
                                        return "Left and Middle";
                                    }
                                case 6:
                                    {
                                        return "Middle and Right";
                                    }
                                case 7:
                                    {
                                        return "Left, Middle, and Right";
                                    }
                            }
                            return "Other";
                        }
                    case "Type of Side Stringers":
                        {
                            switch (enumVal)
                            {
                                case 0:
                                    {
                                        return "Closed";
                                    }
                                case 1:
                                    {
                                        return "Open";
                                    }
                            }
                            return "Other";
                        }
                    case "Baluster Spacing Type (Stair Railing Only)":
                        {
                            switch (enumVal)
                            {
                                case 0:
                                    {
                                        return "Use Baluster Separation";
                                    }
                                case 1:
                                    {
                                        return "Balusters Per Tread (Stair Railing Only)";
                                    }
                            }
                            return "Other";
                        }
                    case "Flow State":
                        {
                            if (enumVal == 0)
                            {
                                return "Laminar";
                            }
                            return "Other";
                        }
                    case "Deformation":
                        {
                            switch (enumVal)
                            {
                                case 0:
                                    {
                                        return "Deformed";
                                    }
                                case 1:
                                    {
                                        return "Plain";
                                    }
                            }
                            return "Other";
                        }
                    case "Orientation":
                        {
                            switch (enumVal)
                            {
                                case 0:
                                    {
                                        return "Normal";
                                    }
                                case 1:
                                    {
                                        return "Horizontal";
                                    }
                            }
                            return "Other";
                        }
                    case "Stick Symbol Location":
                        {
                            switch (enumVal)
                            {
                                case 0:
                                    {
                                        return "Center of Geometry";
                                    }
                                case 1:
                                    {
                                        return "Top of Geometry";
                                    }
                                case 2:
                                    {
                                        return "Bottom of Geometry";
                                    }
                                case 3:
                                    {
                                        return "Location Line";
                                    }
                            }
                            return "Other";
                        }
                    case "Column Style":
                        {
                            switch (enumVal)
                            {
                                case 0:
                                    {
                                        return "Vertical";
                                    }
                                case 1:
                                    {
                                        return "Slanted - Angle Driven";
                                    }
                                case 2:
                                    {
                                        return "Slanted - End Point Driven";
                                    }
                            }
                            return "Other";
                        }
                    case "Structural Usage":
                        {
                            string str2 = categoryName;
                            string str3 = str2;
                            if (str2 == null)
                            {
                                break;
                            }
                            if (str3 == "Structural Framing")
                            {
                                switch (enumVal)
                                {
                                    case 0:
                                        {
                                            return "Undefined";
                                        }
                                    case 1:
                                        {
                                            return "Wall";
                                        }
                                    case 2:
                                        {
                                            return "Column";
                                        }
                                    case 3:
                                        {
                                            return "Girder";
                                        }
                                    case 4:
                                        {
                                            return "Joist";
                                        }
                                    case 5:
                                        {
                                            return "Purlin";
                                        }
                                    case 6:
                                        {
                                            return "Other";
                                        }
                                    case 7:
                                        {
                                            return "Brace";
                                        }
                                    case 8:
                                        {
                                            return "Horizontal Bracing";
                                        }
                                    case 9:
                                        {
                                            return "Kicker Bracing";
                                        }
                                    case 10:
                                        {
                                            return "Automatic";
                                        }
                                    case 11:
                                        {
                                            return "Chord";
                                        }
                                    case 12:
                                        {
                                            return "Web";
                                        }
                                    default:
                                        {
                                            return "Other";
                                        }
                                }
                            }
                            if (str3 == "Walls")
                            {
                                switch (enumVal)
                                {
                                    case 0:
                                        {
                                            return "Non-bearing";
                                        }
                                    case 1:
                                        {
                                            return "Bearing";
                                        }
                                    case 2:
                                        {
                                            return "Shear";
                                        }
                                    case 3:
                                        {
                                            return "Structural combined";
                                        }
                                }
                                return "Other";
                            }
                            if (str3 == "Curtain Panels")
                            {
                                if (enumVal == 0)
                                {
                                    return "Non-bearing";
                                }
                                return "Other";
                            }
                            break;
                        }
                    case "Flex Pattern":
                        {
                            if (enumVal == 0)
                            {
                                return "Single Line";
                            }
                            return "Other";
                        }
                    case "Base Cut Style":
                    case "Top Cut Style":
                        {
                            switch (enumVal)
                            {
                                case 0:
                                    {
                                        return "Perpendicular";
                                    }
                                case 1:
                                    {
                                        return "Horizontal";
                                    }
                                case 2:
                                    {
                                        return "Vertical";
                                    }
                            }
                            return "Other";
                        }
                    case "z-Direction Justification":
                        {
                            switch (enumVal)
                            {
                                case 0:
                                    {
                                        return "Top";
                                    }
                                case 1:
                                    {
                                        return "Center";
                                    }
                                case 2:
                                    {
                                        return "Bottom";
                                    }
                            }
                            return "Other";
                        }
                    case "Lateral Justification":
                        {
                            switch (enumVal)
                            {
                                case 0:
                                    {
                                        return "Center";
                                    }
                                case 1:
                                    {
                                        return "Side 1";
                                    }
                                case 2:
                                    {
                                        return "Side 2";
                                    }
                            }
                            return "Other";
                        }
                    case "Start Attachment Type":
                    case "End Attachment Type":
                        {
                            int num3 = enumVal;
                            if (num3 == 1)
                            {
                                return "Distance";
                            }
                            if (num3 == 4)
                            {
                                return "End Elevation";
                            }
                            return "Other";
                        }
                    case "End of Referenced Column":
                        {
                            switch (enumVal)
                            {
                                case 0:
                                    {
                                        return "Bottom";
                                    }
                                case 1:
                                    {
                                        return "Top";
                                    }
                            }
                            return "Other";
                        }
                    case "Circuit Naming":
                        {
                            switch (enumVal)
                            {
                                case 0:
                                    {
                                        return "Prefixed";
                                    }
                                case 1:
                                    {
                                        return "Standard";
                                    }
                                case 2:
                                    {
                                        return "Panel Name";
                                    }
                                case 3:
                                    {
                                        return "By Phase";
                                    }
                            }
                            return "Other";
                        }
                    case "Style":
                        {
                            switch (enumVal)
                            {
                                case 0:
                                    {
                                        return "Standard";
                                    }
                                case 1:
                                    {
                                        return "Stirrup / Tie";
                                    }
                            }
                            return "Other";
                        }
                    case "Layout Rule":
                        {
                            num = enumVal;
                            switch (num)
                            {
                                case 0:
                                    {
                                        return "Single";
                                    }
                                case 1:
                                    {
                                        return "Fixed Number";
                                    }
                                case 2:
                                    {
                                        return "Maximum Spacing";
                                    }
                                case 3:
                                    {
                                        return "Number with Spacing";
                                    }
                                case 4:
                                    {
                                        return "Minimum Clear Spacing";
                                    }
                            }
                            return "Other";
                        }
                    case "Categorize as":
                        {
                            num = enumVal;
                            switch (num)
                            {
                                case 0:
                                    {
                                        return "Wall";
                                    }
                                case 1:
                                    {
                                        return "Panel";
                                    }
                            }
                            return "Other";
                        }
                    case "Start Release":
                    case "End Release":
                        {
                            num = enumVal;
                            switch (num)
                            {
                                case 0:
                                    {
                                        return "Fixed";
                                    }
                                case 1:
                                    {
                                        return "Pinned";
                                    }
                                case 2:
                                    {
                                        return "Bending Moment";
                                    }
                                case 3:
                                    {
                                        return "User Defined";
                                    }
                            }
                            return "Other";
                        }
                    case "Rigid Links":
                        {
                            num = enumVal;
                            switch (num)
                            {
                                case 0:
                                    {
                                        return "From Column";
                                    }
                                case 1:
                                    {
                                        return "Yes";
                                    }
                                case 2:
                                    {
                                        return "No";
                                    }
                            }
                            return "Other";
                        }
                    case "Rail Shape":
                        {
                            num = enumVal;
                            switch (num)
                            {
                                case 0:
                                    {
                                        return "Rectangular";
                                    }
                                case 1:
                                    {
                                        return "Circle";
                                    }
                            }
                            return "Other";
                        }
                    case "Baluster Shape":
                        {
                            num = enumVal;
                            switch (num)
                            {
                                case 0:
                                    {
                                        return "Rectangular";
                                    }
                                case 1:
                                    {
                                        return "Circle";
                                    }
                                case 2:
                                    {
                                        return "None";
                                    }
                            }
                            return "Other";
                        }
                    case "Underside of Winder":
                        {
                            num = enumVal;
                            switch (num)
                            {
                                case 0:
                                    {
                                        return "Stepped";
                                    }
                                case 1:
                                    {
                                        return "Smooth";
                                    }
                            }
                            return "Other";
                        }
                }
            }
            return null;
        }

        public static bool IsEnum(Parameter param, Element element)
        {
            bool valueName = false;
            //WS: modified:

            //ForgeTypeId temp = param.Definition.GetDataType();

            //WS: 2025-08-25
            //if (param.Definition.ParameterType == ParameterType.Invalid && param.StorageType == StorageType.Integer)
            if (param.Definition.GetDataType() == null && param.StorageType == StorageType.Integer)
            {
                valueName = null != EnumResolver.GetValueName(element.Category.Name, Util.Parameter_getDefinitionName(param), param.AsInteger());
            }
            return valueName;
        }

        public static string MapTypeName(string categoryName, string propertyTypeName)
        {
            categoryName = categoryName ?? "";
            string str = "Autodesk.Revit.DB.Mechanical.";
            string str1 = "Autodesk.Revit.DB.";
            string str2 = "Autodesk.Revit.DB.Electrical.";
            string str3 = "Autodesk.Revit.DB.Plumbing.";
            string str4 = "Autodesk.Revit.DB.Structure.";
            string str5 = "Autodesk.Revit.DB.";
            string str6 = "Autodesk.Revit.DB.";
            string str7 = "Autodesk.Revit.Utility.";
            string str8 = propertyTypeName;
            string str9 = str8;
            if (str8 != null)
            {
                switch (str9)
                {
                    case "ACADVersion":
                        {
                            return string.Concat(str6, "ACADVersion");
                        }
                    case "AnalyzesAsType":
                        {
                            return string.Concat(str6, "AnalyzesAsType");
                        }
                    case "AreaElemType":
                        {
                            return string.Concat(str6, "AreaElemType");
                        }
                    case "AssetPropertyType":
                        {
                            return string.Concat(str7, "AssetPropertyType");
                        }
                    case "AssetType":
                        {
                            return string.Concat(str7, "AssetType");
                        }
                    case "BeamSystemJustifyType":
                        {
                            return string.Concat(str6, "BeamSystemJustifyType");
                        }
                    case "BorderTile":
                        {
                            return string.Concat(str6, "BorderTile");
                        }
                    case "BoxPlacement":
                        {
                            return string.Concat(str6, "BoxPlacement");
                        }
                    case "ColorDepthType":
                        {
                            return string.Concat(str6, "ColorDepthType");
                        }
                    case "ComponentRotation":
                        {
                            return string.Concat(str6, "ComponentRotation");
                        }
                    case "CoordinatePlaneVisibility":
                        {
                            return string.Concat(str6, "CoordinatePlaneVisibility");
                        }
                    case "CriteriaFilterType":
                        {
                            return string.Concat(str6, "CriteriaFilterType");
                        }
                    case "CurtainGridAlignType":
                        {
                            return string.Concat(str6, "CurtainGridAlignType");
                        }
                    case "DecimalSymbolType":
                        {
                            return string.Concat(str6, "DecimalSymbolType");
                        }
                    case "DigitGroupingAmount":
                        {
                            return string.Concat(str6, "DigitGroupingAmount");
                        }
                    case "DigitGroupingSymbolType":
                        {
                            return string.Concat(str6, "DigitGroupingSymbolType");
                        }
                    case "DimensionShape":
                        {
                            return string.Concat(str6, "DimensionShape");
                        }
                    case "DisplayUnit":
                        {
                            return string.Concat(str6, "DisplayUnit");
                        }
                    case "DisplayUnitType":
                        {
                            return string.Concat(str6, "DisplayUnitType");
                        }
                    case "DocumentType":
                        {
                            return string.Concat(str6, "DocumentType");
                        }
                    case "DWFImageFormat":
                        {
                            return string.Concat(str6, "DWFImageFormat");
                        }
                    case "DWFImageQuality":
                        {
                            return string.Concat(str6, "DWFImageQuality");
                        }
                    case "Eave cuts":
                        {
                            return string.Concat(str6, "EaveCutterType");
                        }
                    case "ElementReferenceType":
                        {
                            return string.Concat(str6, "ElementReferenceType");
                        }
                    case "ExportPaperFormat":
                        {
                            return string.Concat(str6, "ExportPaperFormat");
                        }
                    case "ExportUnit":
                        {
                            return string.Concat(str6, "ExportUnit");
                        }
                    case "GridNodeLocation":
                        {
                            return string.Concat(str6, "GridNodeLocation");
                        }
                    case "GridSegmentDirection":
                        {
                            return string.Concat(str6, "GridSegmentDirection");
                        }
                    case "HiddenLineViewsType":
                        {
                            return string.Concat(str6, "HiddenLineViewsType");
                        }
                    case "HorizontalAlign":
                        {
                            return string.Concat(str6, "HorizontalAlign");
                        }
                    case "ImportColorMode":
                        {
                            return string.Concat(str6, "ImportColorMode");
                        }
                    case "ImportExportFileFormat":
                        {
                            return string.Concat(str6, "ImportExportFileFormat");
                        }
                    case "ImportPlacement":
                        {
                            return string.Concat(str6, "ImportPlacement");
                        }
                    case "ImportUnit":
                        {
                            return string.Concat(str6, "ImportUnit");
                        }
                    case "LanguageType":
                        {
                            return string.Concat(str6, "LanguageType");
                        }
                    case "LeaderEndCondition":
                        {
                            return string.Concat(str6, "LeaderEndCondition");
                        }
                    case "LineScaling":
                        {
                            return string.Concat(str6, "LineScaling");
                        }
                    case "MarginType":
                        {
                            return string.Concat(str6, "MarginType");
                        }
                    case "MaterialBehaviourType":
                        {
                            return string.Concat(str6, "MaterialBehaviourType");
                        }
                    case "PageOrientationType":
                        {
                            return string.Concat(str6, "PageOrientationType");
                        }
                    case "PaperPlacementType":
                        {
                            return string.Concat(str6, "PaperPlacementType");
                        }
                    case "PostedErrorType":
                        {
                            return string.Concat(str6, "PostedErrorType");
                        }
                    case "PrintRange":
                        {
                            return string.Concat(str6, "PrintRange");
                        }
                    case "":
                        {
                            return string.Concat(str6, "ProductType");
                        }
                    case "PropOverrideMode":
                        {
                            return string.Concat(str6, "PropOverrideMode");
                        }
                    case "RasterQualityType":
                        {
                            return string.Concat(str6, "RasterQualityType");
                        }
                    case "ReferenceType":
                        {
                            return string.Concat(str6, "ReferenceType");
                        }
                    case "SetComparisonResult":
                        {
                            return string.Concat(str6, "SetComparisonResult");
                        }
                    case "SlabShapeCreaseType":
                        {
                            return string.Concat(str6, "SlabShapeCreaseType");
                        }
                    case "SlabShapeVertexType":
                        {
                            return string.Concat(str6, "SlabShapeVertexType");
                        }
                    case "SlantedOrVerticalColumnType":
                        {
                            return string.Concat(str6, "SlantedOrVerticalColumnType");
                        }
                    case "SolidGeometry":
                        {
                            return string.Concat(str6, "SolidGeometry");
                        }
                    case "SpacingRuleJustification":
                        {
                            return string.Concat(str6, "SpacingRuleJustification");
                        }
                    case "SpacingRuleLayout":
                        {
                            return string.Concat(str6, "SpacingRuleLayout");
                        }
                    case "StickSymbolLocation":
                        {
                            return string.Concat(str6, "StickSymbolLocation");
                        }
                    case "StructuralReleaseType":
                        {
                            return string.Concat(str6, "StructuralReleaseType");
                        }
                    case "TagMode":
                        {
                            return string.Concat(str6, "TagMode");
                        }
                    case "TagOrientation":
                        {
                            return string.Concat(str6, "TagOrientation");
                        }
                    case "TextAlignFlags":
                        {
                            return string.Concat(str6, "TextAlignFlags");
                        }
                    case "TextAlignMask":
                        {
                            return string.Concat(str6, "TextAlignMask");
                        }
                    case "TextElementBackground":
                        {
                            return string.Concat(str6, "TextElementBackground");
                        }
                    case "TextNoteLeaderStyles":
                        {
                            return string.Concat(str6, "TextNoteLeaderStyles");
                        }
                    case "TextNoteLeaderTypes":
                        {
                            return string.Concat(str6, "TextNoteLeaderTypes");
                        }
                    case "TilePatternsBuiltIn":
                        {
                            return string.Concat(str6, "TilePatternsBuiltIn");
                        }
                    case "TrussCurveType":
                        {
                            return string.Concat(str6, "TrussCurveType");
                        }
                    case "TrussMemberType":
                        {
                            return string.Concat(str6, "TrussMemberType");
                        }
                    case "UnitGroup":
                        {
                            return string.Concat(str6, "UnitGroup");
                        }
                    case "UnitSymbolType":
                        {
                            return string.Concat(str6, "UnitSymbolType");
                        }
                    case "UnitType":
                        {
                            return string.Concat(str6, "UnitType");
                        }
                    case "ViewType":
                        {
                            return string.Concat(str6, "ViewType");
                        }
                    case "VirtualPrinterType":
                        {
                            return string.Concat(str6, "VirtualPrinterType");
                        }
                    case "Function":
                        {
                            return string.Concat(str6, "WallFunction");
                        }
                    case "ZoomType":
                        {
                            return string.Concat(str6, "ZoomType");
                        }
                    case "AnalyticalSupportType":
                        {
                            return string.Concat(str4, "AnalyticalSupportType");
                        }
                    case "CompoundStructureDeckUsage":
                        {
                            return string.Concat(str5, "CompoundStructureDeckUsage");
                        }
                    case "CompoundStructureLayerFunction":
                        {
                            return string.Concat(str5, "CompoundStructureLayerFunction");
                        }
                    case "InstanceUsage":
                        {
                            return string.Concat(str4, "InstanceUsage");
                        }
                    case "Material":
                        {
                            return string.Concat(str4, "Material");
                        }
                    case "RebarHookOrientation":
                        {
                            return string.Concat(str4, "RebarHookOrientation");
                        }
                    case "RebarLayoutRule":
                        {
                            return string.Concat(str4, "RebarLayoutRule");
                        }
                    case "RebarShapeArcReferenceType":
                        {
                            return string.Concat(str4, "RebarShapeArcReferenceType");
                        }
                    case "RebarShapeBendAngle":
                        {
                            return string.Concat(str4, "RebarShapeBendAngle");
                        }
                    case "RebarShapeDefinitionByArcType":
                        {
                            return string.Concat(str4, "RebarShapeDefinitionByArcType");
                        }
                    case "RebarStyle":
                        {
                            return string.Concat(str4, "RebarStyle");
                        }
                    case "SlabFoundationType":
                        {
                            return string.Concat(str4, "SlabFoundationType");
                        }
                    case "StructuralType":
                    case "Structural Type":
                        {
                            return string.Concat(str4, "StructuralType");
                        }
                    case "TranslationRotationValue":
                        {
                            return string.Concat(str4, "TranslationRotationValue");
                        }
                    case "StructuralUsage":
                    case "Structural Usage":
                        {
                            string str10 = categoryName;
                            string str11 = str10;
                            if (str10 == null)
                            {
                                break;
                            }
                            if (str11 == "Structural Framing")
                            {
                                return string.Concat(str4, "StructuralInstanceUsage");
                            }
                            if (str11 == "Floors")
                            {
                                return string.Concat(str4, "SlabFoundationType");
                            }
                            if (str11 == "Walls")
                            {
                                return string.Concat(str4, "StructuralWallUsage");
                            }
                            break;
                        }
                    case "BaseLoadOn":
                        {
                            return string.Concat(str2, "BaseLoadOn");
                        }
                    case "ConditionType":
                        {
                            return string.Concat(str, "ConditionType");
                        }
                    case "ConnectorProfileType":
                        {
                            return string.Concat(str1, "ConnectorProfileType");
                        }
                    case "ConnectorType":
                        {
                            return string.Concat(str1, "ConnectorType");
                        }
                    case "Domain":
                        {
                            return string.Concat(str1, "Domain");
                        }
                    case "DuctFlowConfigurationType":
                        {
                            return string.Concat(str, "DuctFlowConfigurationType");
                        }
                    case "DuctLossMethodType":
                        {
                            return string.Concat(str, "DuctLossMethodType");
                        }
                    case "DuctSystemType":
                        {
                            return string.Concat(str, "DuctSystemType");
                        }
                    case "ElectricalPhase":
                        {
                            return string.Concat(str2, "ElectricalPhase");
                        }
                    case "ElectricalPhaseConfiguration":
                        {
                            return string.Concat(str2, "ElectricalPhaseConfiguration");
                        }
                    case "ElectricalSystemType":
                        {
                            return string.Concat(str2, "ElectricalSystemType");
                        }
                    case "FlowDirectionType":
                        {
                            return string.Concat(str1, "FlowDirectionType");
                        }
                    case "JunctionType":
                        {
                            return string.Concat(str1, "JunctionType");
                        }
                    case "LoadClassification":
                        {
                            return string.Concat(str2, "LoadClassification");
                        }
                    case "LoadClassificationType":
                        {
                            return string.Concat(str2, "LoadClassificationType");
                        }
                    case "OccupancyUnit":
                        {
                            return string.Concat(str, "OccupancyUnit");
                        }
                    case "PartType":
                        {
                            return string.Concat(str, "PartType");
                        }
                    case "PipeFlowConfigurationType":
                        {
                            return string.Concat(str3, "PipeFlowConfigurationType");
                        }
                    case "PipeFlowState":
                        {
                            return string.Concat(str3, "PipeFlowState");
                        }
                    case "PipeLossMethodType":
                        {
                            return string.Concat(str3, "PipeLossMethodType");
                        }
                    case "PipeSystemType":
                        {
                            return string.Concat(str3, "PipeSystemType");
                        }
                    case "PowerFactorStateType":
                        {
                            return string.Concat(str2, "PowerFactorStateType");
                        }
                    case "ReturnAirflowType":
                        {
                            return string.Concat(str, "ReturnAirflowType");
                        }
                    case "SpaceType":
                        {
                            return string.Concat(str, "SpaceType");
                        }
                    case "WiringType":
                    case "Wiring Type":
                        {
                            return string.Concat(str2, "WiringType");
                        }
                }
            }
            return null;
        }
    }
}
