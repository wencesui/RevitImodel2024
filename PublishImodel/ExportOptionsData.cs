using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PublishImodel
{
    public class ExportOptionsData
    {
        private List<string> m_layersAndProperties;

        private List<PropOverrideMode> m_enumLayersAndProperties;

        private List<string> m_layerMapping;

        private List<string> m_enumLayerMapping;

        private string m_exportLayerMapping;

        private List<string> m_lineScaling;

        private PropOverrideMode m_exportLayersAndProperties;

        private List<LineScaling> m_enumLineScaling;

        private LineScaling m_exportLineScaling;

        private List<string> m_coorSystem;

        private List<bool> m_enumCoorSystem;

        private bool m_exportCoorSystem;

        private List<string> m_units;

        private List<ExportUnit> m_enumUnits;

        private ExportUnit m_exportUnit;

        private List<string> m_solids;

        private List<SolidGeometry> m_enumSolids;

        private SolidGeometry m_exportSolid;

        private bool m_exportAreas;

        public ReadOnlyCollection<string> CoorSystem
        {
            get
            {
                return new ReadOnlyCollection<string>(this.m_coorSystem);
            }
        }

        public ReadOnlyCollection<bool> EnumCoorSystem
        {
            get
            {
                return new ReadOnlyCollection<bool>(this.m_enumCoorSystem);
            }
        }

        public ReadOnlyCollection<string> EnumLayerMapping
        {
            get
            {
                return new ReadOnlyCollection<string>(this.m_enumLayerMapping);
            }
        }

        public ReadOnlyCollection<PropOverrideMode> EnumLayersAndProperties
        {
            get
            {
                return new ReadOnlyCollection<PropOverrideMode>(this.m_enumLayersAndProperties);
            }
        }

        public ReadOnlyCollection<LineScaling> EnumLineScaling
        {
            get
            {
                return new ReadOnlyCollection<LineScaling>(this.m_enumLineScaling);
            }
        }

        public ReadOnlyCollection<SolidGeometry> EnumSolids
        {
            get
            {
                return new ReadOnlyCollection<SolidGeometry>(this.m_enumSolids);
            }
        }

        public ReadOnlyCollection<ExportUnit> EnumUnits
        {
            get
            {
                return new ReadOnlyCollection<ExportUnit>(this.m_enumUnits);
            }
        }

        public bool ExportAreas
        {
            get
            {
                return this.m_exportAreas;
            }
            set
            {
                this.m_exportAreas = value;
            }
        }

        public bool ExportCoorSystem
        {
            get
            {
                return this.m_exportCoorSystem;
            }
            set
            {
                this.m_exportCoorSystem = value;
            }
        }

        public string ExportLayerMapping
        {
            get
            {
                return this.m_exportLayerMapping;
            }
            set
            {
                this.m_exportLayerMapping = value;
            }
        }

        public PropOverrideMode ExportLayersAndProperties
        {
            get
            {
                return this.m_exportLayersAndProperties;
            }
            set
            {
                this.m_exportLayersAndProperties = value;
            }
        }

        public LineScaling ExportLineScaling
        {
            get
            {
                return this.m_exportLineScaling;
            }
            set
            {
                this.m_exportLineScaling = value;
            }
        }

        public SolidGeometry ExportSolid
        {
            get
            {
                return this.m_exportSolid;
            }
            set
            {
                this.m_exportSolid = value;
            }
        }

        public ExportUnit ExportUnit
        {
            get
            {
                return this.m_exportUnit;
            }
            set
            {
                this.m_exportUnit = value;
            }
        }

        public ReadOnlyCollection<string> LayerMapping
        {
            get
            {
                return new ReadOnlyCollection<string>(this.m_layerMapping);
            }
        }

        public ReadOnlyCollection<string> LayersAndProperties
        {
            get
            {
                return new ReadOnlyCollection<string>(this.m_layersAndProperties);
            }
        }

        public ReadOnlyCollection<string> LineScaling
        {
            get
            {
                return new ReadOnlyCollection<string>(this.m_lineScaling);
            }
        }

        public ReadOnlyCollection<string> Solids
        {
            get
            {
                return new ReadOnlyCollection<string>(this.m_solids);
            }
        }

        public ReadOnlyCollection<string> Units
        {
            get
            {
                return new ReadOnlyCollection<string>(this.m_units);
            }
        }

        public ExportOptionsData(Document activeDocument)
        {
            this.Initialize();
            //WS:
            //switch (activeDocument.GetUnits().GetFormatOptions(UnitType.UT_Length).DisplayUnits)


            switch (activeDocument.GetUnits().GetFormatOptions(Autodesk.Revit.DB.SpecTypeId.Length).GetUnitTypeId().TypeId.ToUpper())
            {
                //case DisplayUnitType.DUT_METERS:
                case "M":
                    {
                        this.m_exportUnit = ExportUnit.Meter;
                        return;
                    }
                case "CM":
                    {
                        this.m_exportUnit = ExportUnit.Centimeter;
                        return;
                    }
                case "MM":
                    {
                        this.m_exportUnit = ExportUnit.Millimeter;
                        return;
                    }
                case "FEET":
                    {
                        this.m_exportUnit = ExportUnit.Foot;
                        return;
                    }
                //case DisplayUnitType.DUT_FEET_FRACTIONAL_INCHES:
                //case DisplayUnitType.DUT_FRACTIONAL_INCHES:
                //case DisplayUnitType.DUT_DECIMAL_INCHES:
                //    {
                //        this.m_exportUnit = ExportUnit.Inch;
                //        return;
                //    }
            }
            this.m_exportUnit = ExportUnit.Default;
        }

        private void Initialize()
        {
            this.m_layersAndProperties = new List<string>();
            this.m_enumLayersAndProperties = new List<PropOverrideMode>();
            this.m_layersAndProperties.Add("Category properties BYLAYER, overrides BYENTITY");
            this.m_enumLayersAndProperties.Add(PropOverrideMode.ByEntity);
            this.m_layersAndProperties.Add("All properties BYLAYER, no overrides");
            this.m_enumLayersAndProperties.Add(PropOverrideMode.ByLayer);
            this.m_layersAndProperties.Add("All properties BYLAYER, New Layers for overrides");
            this.m_enumLayersAndProperties.Add(PropOverrideMode.NewLayer);
            this.m_exportLayersAndProperties = PropOverrideMode.ByEntity;
            this.m_layerMapping = new List<string>();
            this.m_enumLayerMapping = new List<string>();
            this.m_layerMapping.Add("AIA - American Institute of Architects standard");
            this.m_enumLayerMapping.Add("AIA");
            this.m_layerMapping.Add("ISO13567 - ISO standard 13567");
            this.m_enumLayerMapping.Add("ISO13567");
            this.m_layerMapping.Add("CP83 - Singapore standard 83");
            this.m_enumLayerMapping.Add("CP83");
            this.m_layerMapping.Add("BS1192 - British Standard 1192");
            this.m_enumLayerMapping.Add("BS1192");
            this.m_exportLayerMapping = "AIA";
            this.m_lineScaling = new List<string>();
            this.m_enumLineScaling = new List<LineScaling>();
            this.m_lineScaling.Add("Scaled Linetype definitions");
            this.m_lineScaling.Add("ModelSpace (PSLTSCALE = 0)");
            this.m_lineScaling.Add("Paperspace (PSLTSCALE = 1)");
            this.m_coorSystem = new List<string>();
            this.m_enumCoorSystem = new List<bool>();
            this.m_coorSystem.Add("Project Internal");
            this.m_enumCoorSystem.Add(false);
            this.m_coorSystem.Add("Shared");
            this.m_enumCoorSystem.Add(true);
            this.m_units = new List<string>();
            this.m_enumUnits = new List<ExportUnit>();
            this.m_units.Add(ExportUnit.Foot.ToString().ToLower());
            this.m_enumUnits.Add(ExportUnit.Foot);
            this.m_units.Add(ExportUnit.Inch.ToString().ToLower());
            this.m_enumUnits.Add(ExportUnit.Inch);
            this.m_units.Add(ExportUnit.Meter.ToString().ToLower());
            this.m_enumUnits.Add(ExportUnit.Meter);
            this.m_units.Add(ExportUnit.Centimeter.ToString().ToLower());
            this.m_enumUnits.Add(ExportUnit.Centimeter);
            this.m_units.Add(ExportUnit.Millimeter.ToString().ToLower());
            this.m_enumUnits.Add(ExportUnit.Millimeter);
            this.m_solids = new List<string>();
            this.m_enumSolids = new List<SolidGeometry>();
            this.m_solids.Add("Export as polymesh");
            this.m_enumSolids.Add(SolidGeometry.Polymesh);
            this.m_solids.Add("Export as ACIS solids");
            this.m_enumSolids.Add(SolidGeometry.ACIS);
        }
    }
}