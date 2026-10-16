using Bentley.ECObjects.Units;
using System;

namespace PublishImodel
{
    internal static class CustomUnits
    {
        public readonly static UnitSystem ImprSystem;

        public static Unit REVIT_PASCAL_PER_FOOT;

        public static Unit INCHES_OF_WATER_PER_100FT;

        static CustomUnits()
        {
            CustomUnits.ImprSystem = new UnitSystem("IMPR");
            CustomUnits.REVIT_PASCAL_PER_FOOT = new Unit("REVIT_PASCAL_PER_FOOT", StandardUnits.PASCAL_PER_METRE.Dimension, null, null, CustomUnits.ImprSystem, StandardUnits.PASCAL_PER_METRE, 0.092903);
            CustomUnits.INCHES_OF_WATER_PER_100FT = new Unit("INCHES_OF_WATER_PER_100FT", StandardUnits.PASCAL_PER_METRE.Dimension, null, null, CustomUnits.ImprSystem, StandardUnits.PASCAL_PER_METRE, 0.122365946357769);
        }
    }
}