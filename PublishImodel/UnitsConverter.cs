using Autodesk.Revit.DB;
using Bentley.ECObjects.Units;
using System;
using System.Collections.Generic;

namespace PublishImodel
{
    internal static class UnitsConverter
    {
        //private static Dictionary<DisplayUnitType, Unit> revitDisplayUnitTypeConversions;

        private static Dictionary<ForgeTypeId, Unit> revitDisplayUnitTypeConversions;

        //WS: 2025-08-25
        //private static Dictionary<ParameterType, KindOfQuantity> revitParamKindOfQuantity;

        private static Dictionary<ForgeTypeId, KindOfQuantity> revitParamKindOfQuantity;

        static UnitsConverter()
        {
            //WS: Note: Must make sure not duplicate is added into the dictionary, otherwise got error even when calling this class!!
            UnitsConverter.revitDisplayUnitTypeConversions = new Dictionary<ForgeTypeId, Unit>();

            //WS: 2025-08-25
            //UnitsConverter.revitParamKindOfQuantity = new Dictionary<ParameterType, KindOfQuantity>();

            UnitsConverter.revitParamKindOfQuantity = new Dictionary<ForgeTypeId, KindOfQuantity>();

            UnitsLibrary current = UnitsLibrary.Current;

            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.Meters, StandardUnits.METRE);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.Centimeters, StandardUnits.CENTIMETRE);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.Millimeters, StandardUnits.MILLIMETRE);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.Feet, StandardUnits.FOOT);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.FeetFractionalInches, StandardUnits.FOOT);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.FractionalInches, StandardUnits.INCH);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.Inches, StandardUnits.INCH);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.Acres, StandardUnits.ACRE);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.Hectares, StandardUnits.HECTARE);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.MetersCentimeters, StandardUnits.METRE);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.CubicYards, StandardUnits.YARD_CUBED);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.SquareFeet, StandardUnits.FOOT_SQUARED);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.SquareMeters, StandardUnits.METRE_SQUARED);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.CubicFeet, StandardUnits.FOOT_CUBED);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.CubicMeters, StandardUnits.METRE_CUBED);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.Degrees, StandardUnits.DEGREE);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.DegreesMinutes, StandardUnits.DEGREE);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.General, null);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.Fixed, null);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.Percentage, StandardUnits.PERCENT_PERCENT);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.SquareInches, StandardUnits.INCH_SQUARED);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.SquareCentimeters, StandardUnits.CENTIMETRE_SQUARED);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.SquareMillimeters, StandardUnits.MILLIMETRE_SQUARED);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.CubicInches, StandardUnits.INCH_CUBED);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.CubicCentimeters, StandardUnits.CENTIMETRE_CUBED);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.CubicMillimeters, StandardUnits.CENTIMETRE_CUBED);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.Liters, StandardUnits.LITRE);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.UsGallons, StandardUnits.GALLON);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.KilogramsPerCubicMeter, StandardUnits.KILOGRAM_PER_METRE_CUBED);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.PoundsMassPerCubicFoot, StandardUnits.POUND_PER_FOOT_CUBED);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.PoundsMassPerCubicInch, StandardUnits.POUND_PER_INCH_CUBED);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.BritishThermalUnits, StandardUnits.BTU);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.Joules, StandardUnits.JOULE);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.KilowattHours, StandardUnits.KILOWATT_HOUR);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.PascalsPerMeter, StandardUnits.PASCAL_PER_METRE);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.InchesOfWater60DegreesFahrenheitPer100Feet, CustomUnits.INCHES_OF_WATER_PER_100FT);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.Watts, StandardUnits.WATT);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.Kilowatts, StandardUnits.KILOWATT);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.BritishThermalUnitsPerSecond, StandardUnits.BTU_PER_HOUR);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.BritishThermalUnitsPerHour, StandardUnits.BTU_PER_HOUR);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.WattsPerSquareMeter, StandardUnits.WATT_PER_METRE_SQUARED);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.InchesOfWater60DegreesFahrenheit, StandardUnits.INCH_OF_H2O_AT_60_FAHRENHEIT);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.Pascals, StandardUnits.PASCAL);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.Kilopascals, StandardUnits.KILOPASCAL);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.Megapascals, StandardUnits.MEGAPASCAL);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.PoundsForcePerSquareInch, StandardUnits.POUND_FORCE_PER_INCH_SQUARED);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.InchesOfMercury32DegreesFahrenheit, StandardUnits.INCH_OF_HG_AT_32_FAHRENHEIT);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.MillimetersOfMercury, StandardUnits.MILLIMETRE_OF_HG_AT_32_FAHRENHEIT);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.Atmospheres, StandardUnits.ATMOSPHERE);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.Bars, StandardUnits.BAR);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.Fahrenheit, StandardUnits.DEGREE_FAHRENHEIT);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.Celsius, StandardUnits.DEGREE_CELSIUS);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.Kelvin, StandardUnits.DEGREE_KELVIN);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.Rankine, StandardUnits.DEGREE_RANKINE);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.FeetPerMinute, StandardUnits.FOOT_PER_MINUTE);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.MetersPerSecond, StandardUnits.METRE_PER_SECOND);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.CentimetersPerMinute, StandardUnits.CENTIMETRE_PER_MINUTE);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.CubicFeetPerMinute, StandardUnits.FOOT_CUBED_PER_MINUTE);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.LitersPerSecond, StandardUnits.LITRE_PER_SECOND);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.CubicMetersPerSecond, StandardUnits.METRE_CUBED_PER_SECOND);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.CubicMetersPerHour, StandardUnits.METRE_CUBED_PER_HOUR);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.UsGallonsPerMinute, StandardUnits.GALLON_PER_MINUTE);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.UsGallonsPerHour, StandardUnits.GALLON_PER_MINUTE);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.Amperes, StandardUnits.AMPERE);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.KilovoltAmperes, StandardUnits.AMPERE);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.Milliamperes, StandardUnits.AMPERE);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.Volts, StandardUnits.VOLT);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.Kilovolts, StandardUnits.VOLT);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.Millivolts, StandardUnits.VOLT);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.Hertz, StandardUnits.HERTZ);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.CyclesPerSecond, StandardUnits.HERTZ);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.Lux, StandardUnits.LUX);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.Footcandles, StandardUnits.LUMEN_PER_FOOT_SQUARED);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.Lumens, StandardUnits.LUMEN);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.VoltAmperes, StandardUnits.VOLT_AMPERE);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.Horsepower, StandardUnits.HORSEPOWER);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.Newtons, StandardUnits.NEWTON);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.Kilonewtons, StandardUnits.KILONEWTON);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.Meganewtons, StandardUnits.KILONEWTON);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.Kips, StandardUnits.KILOPOUND_FORCE);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.KilogramsForce, StandardUnits.KILOGRAM_FORCE);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.TonnesForce, StandardUnits.KILOGRAM_FORCE);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.PoundsForce, StandardUnits.POUND_FORCE);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.NewtonsPerMeter, StandardUnits.NEWTON_METRE);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.KilonewtonsPerMeter, StandardUnits.NEWTON_METRE);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.MeganewtonsPerMeter, StandardUnits.NEWTON_METRE);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.KipsPerFoot, StandardUnits.POUND_FOOT);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.NewtonsPerSquareMeter, StandardUnits.NEWTON_PER_METRE_SQUARED);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.KilonewtonsPerSquareMeter, StandardUnits.NEWTON_PER_METRE_SQUARED);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.MeganewtonsPerSquareMeter, StandardUnits.NEWTON_PER_METRE_SQUARED);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.KipsPerSquareFoot, StandardUnits.POUND_FORCE_PER_FOOT_SQUARED);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.KilogramsForcePerSquareMeter, StandardUnits.KILOGRAM_FORCE_PER_METRE_SQUARED);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.TonnesForcePerSquareMeter, StandardUnits.KILOGRAM_FORCE_PER_METRE_SQUARED);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.PoundsForcePerSquareFoot, StandardUnits.POUND_FORCE_PER_FOOT_SQUARED);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.NewtonMeters, StandardUnits.NEWTON_METRE);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.KilonewtonMeters, StandardUnits.NEWTON_METRE);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.MeganewtonMeters, StandardUnits.NEWTON_METRE);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.KipFeet, StandardUnits.POUND_FOOT);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.FeetOfWater39_2DegreesFahrenheit, StandardUnits.FOOT_OF_H2O_CONVENTIONAL);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.PascalSeconds, StandardUnits.PASCAL_SECOND);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.Centipoises, StandardUnits.CENTIPOISE);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.FeetPerSecond, StandardUnits.FOOT_PER_SECOND);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.KipsPerSquareInch, StandardUnits.POUND_FORCE_PER_INCH_SQUARED);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.KilonewtonsPerCubicMeter, StandardUnits.KILONEWTON_PER_METRE_CUBED);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.PoundsForcePerCubicFoot, StandardUnits.POUND_FORCE_PER_FOOT_CUBED);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.InverseDegreesFahrenheit, StandardUnits.RECIPROCAL_DELTA_DEGREE_FAHRENHEIT);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.InverseDegreesCelsius, StandardUnits.RECIPROCAL_DELTA_DEGREE_CELSIUS);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.KipsPerInch, StandardUnits.POUND_FORCE_PER_INCH);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.KipsPerCubicFoot, StandardUnits.POUND_FORCE_PER_FOOT_CUBED);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.KilonewtonMetersPerDegree, StandardUnits.NEWTON_METRE_PER_DEGREE);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.BritishThermalUnitsPerHourSquareFootDegreeFahrenheit, StandardUnits.BTU_PER_FOOT_SQUARED_PER_HOUR_PER_DELTA_DEGREE_FAHRENHEIT);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.WattsPerSquareMeterKelvin, StandardUnits.WATT_PER_METRE_SQUARED_PER_DELTA_DEGREE_KELVIN);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.CubicFeetPerMinuteSquareFoot, StandardUnits.FOOT_CUBED_PER_FOOT_SQUARED_PER_MINUTE);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.LitersPerSecondSquareMeter, StandardUnits.LITRE_PER_METRE_SQUARED_PER_SECOND);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.RiseDividedBy12Inches, StandardUnits.FOOT_PER_FOOT);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.RiseDividedBy1Foot, StandardUnits.FOOT_PER_FOOT);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.WattsPerCubicMeter, StandardUnits.WATT_PER_METRE_CUBED);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.BritishThermalUnitsPerHourSquareFoot, StandardUnits.BTU_PER_HOUR_PER_FOOT_SQUARED);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.BritishThermalUnitsPerHourCubicFoot, StandardUnits.BTU_PER_HOUR_PER_FOOT_CUBED);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.Currency, StandardUnits.DOLLAR);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.SquareFeetPer1000BritishThermalUnitsPerHour, StandardUnits.FOOT_SQUARED_HOUR_DELTA_DEGREE_FAHRENHEIT_PER_BTU);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.NewtonsPerSquareMillimeter, StandardUnits.NEWTON_PER_MILLIMETRE_SQUARED);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.KilonewtonsPerSquareMillimeter, StandardUnits.NEWTON_PER_MILLIMETRE_SQUARED);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.OneToRatio, StandardUnits.ONE_OVER_SLOPE);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.HourSquareFootDegreesFahrenheitPerBritishThermalUnit, StandardUnits.FOOT_SQUARED_HOUR_DELTA_DEGREE_FAHRENHEIT_PER_BTU);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.SquareMeterKelvinsPerWatt, StandardUnits.METRE_SQUARED_DELTA_DEGREE_KELVIN_PER_WATT);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.Kilograms, StandardUnits.KILOGRAM);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.UsTonnesMass, StandardUnits.KILOGRAM);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.PoundsMass, StandardUnits.POUND);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.FeetToTheFourthPower, StandardUnits.FOOT_TO_THE_FOURTH);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.InchesToTheFourthPower, StandardUnits.INCH_TO_THE_FOURTH);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.MillimetersToTheFourthPower, StandardUnits.MILLIMETRE_TO_THE_FOURTH);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.CentimetersToTheFourthPower, StandardUnits.CENTIMETRE_TO_THE_FOURTH);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.MetersToTheFourthPower, StandardUnits.METRE_TO_THE_FOURTH);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.FeetToTheSixthPower, StandardUnits.FOOT_TO_THE_SIXTH);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.InchesToTheSixthPower, StandardUnits.INCH_TO_THE_SIXTH);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.MillimetersToTheSixthPower, StandardUnits.MILLIMETRE_TO_THE_SIXTH);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.CentimetersToTheSixthPower, StandardUnits.CENTIMETRE_TO_THE_SIXTH);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.MetersToTheSixthPower, StandardUnits.METRE_TO_THE_SIXTH);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.KilogramsPerMeter, StandardUnits.KILOGRAM_PER_METRE);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.PoundsMassPerFoot, StandardUnits.POUND_MASS_PER_FOOT);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.Radians, StandardUnits.RADIAN);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.Gradians, StandardUnits.GRADIAN);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.RadiansPerSecond, StandardUnits.RADIAN_PER_SECOND);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.Milliseconds, StandardUnits.MILLISECOND);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.Seconds, StandardUnits.SECOND);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.Minutes, StandardUnits.MINUTE);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.Hours, StandardUnits.HOUR);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.KilometersPerHour, StandardUnits.KILOMETRE_PER_HOUR);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.MilesPerHour, StandardUnits.MILE_PER_HOUR);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.Kilojoules, StandardUnits.KILOJOULE);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.KilogramsPerSquareMeter, StandardUnits.KILOGRAM_PER_METRE_SQUARED);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.PoundsMassPerSquareFoot, StandardUnits.POUND_MASS_PER_FOOT_SQUARED);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.WattsPerMeterKelvin, StandardUnits.WATT_PER_METRE_PER_DEGREE_KELVIN);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.BritishThermalUnitsPerPound, StandardUnits.BTU_PER_POUND_MASS);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.GrainsPerHourSquareFootInchMercury, StandardUnits.GRAIN_FORCE_PER_HOUR_PER_FOOT_SQUARED_PER_INCH_HG_CONVENTIONAL);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.Decimeters, StandardUnits.DECIMETRE);
            //UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.UsTonnesMass, StandardUnits.SHORT_TON);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.UsTonnesForce, StandardUnits.SHORT_TON_FORCE);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.LitersPerMinute, StandardUnits.LITRE_PER_MINUTE);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.FahrenheitInterval, StandardUnits.RECIPROCAL_DELTA_DEGREE_FAHRENHEIT);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.CelsiusInterval, StandardUnits.RECIPROCAL_DELTA_DEGREE_CELSIUS);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.KelvinInterval, StandardUnits.RECIPROCAL_DELTA_DEGREE_KELVIN);
            UnitsConverter.revitDisplayUnitTypeConversions.Add(UnitTypeId.RankineInterval, StandardUnits.RECIPROCAL_DELTA_DEGREE_RANKINE);

            //WS: 2025-08-25 Diable old API ParameterType
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.Length, StandardKindOfQuantities.LENGTH);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.Area, StandardKindOfQuantities.AREA);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.Volume, StandardKindOfQuantities.VOLUME);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.Angle, StandardKindOfQuantities.PLANE_ANGLE);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.Force, StandardKindOfQuantities.FORCE);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.LinearForce, StandardKindOfQuantities.MOMENT_OF_FORCE);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.AreaForce, StandardKindOfQuantities.AREA_FORCE);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.Moment, StandardKindOfQuantities.MOMENT_OF_FORCE);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.Currency, StandardKindOfQuantities.CURRENCY);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.ElectricalCurrent, StandardKindOfQuantities.ELECTRIC_CURRENT);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.ElectricalFrequency, StandardKindOfQuantities.FREQUENCY);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.ElectricalIlluminance, StandardKindOfQuantities.ILLUMINANCE);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.ElectricalPotential, StandardKindOfQuantities.ELECTRIC_POTENTIAL);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.ElectricalPower, StandardKindOfQuantities.APPARENT_POWER);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.HVACPressure, StandardKindOfQuantities.PRESSURE);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.HVACRoughness, StandardKindOfQuantities.ROUGHNESS);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.HVACAirflow, StandardKindOfQuantities.FLOW);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.HVACEnergy, StandardKindOfQuantities.ENERGY);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.HVACHeatGain, StandardKindOfQuantities.REACTIVE_POWER);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.HVACVelocity, StandardKindOfQuantities.VELOCITY);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.HVACDuctSize, StandardKindOfQuantities.LENGTH);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.PipeSize, StandardKindOfQuantities.LENGTH);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.HVACFriction, StandardKindOfQuantities.FORCE_PER_VOLUME);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.HVACCoefficientOfHeatTransfer, StandardKindOfQuantities.COEFFICIENT_OF_HEAT_TRANSFER);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.ColorTemperature, StandardKindOfQuantities.TEMPERATURE);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.ElectricalLuminousFlux, StandardKindOfQuantities.LUMINOUS_FLUX);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.PipingPressure, StandardKindOfQuantities.PRESSURE);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.PipingFlow, StandardKindOfQuantities.FLOW);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.PipingVelocity, StandardKindOfQuantities.VELOCITY);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.PipingRoughness, StandardKindOfQuantities.ROUGHNESS);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.PipingFriction, StandardKindOfQuantities.FORCE_PER_VOLUME);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.ElectricalDemandFactor, StandardKindOfQuantities.PERCENT);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.BarDiameter, StandardKindOfQuantities.DIAMETER);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.CrackWidth, StandardKindOfQuantities.LENGTH);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.DisplacementDeflection, StandardKindOfQuantities.LENGTH);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.ElectricalApparentPower, StandardKindOfQuantities.APPARENT_POWER);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.ElectricalCableTraySize, StandardKindOfQuantities.LENGTH);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.ElectricalConduitSize, StandardKindOfQuantities.LENGTH);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.ElectricalEfficacy, StandardKindOfQuantities.LUMINOUS_EFFICACY);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.ElectricalLuminousIntensity, StandardKindOfQuantities.LUMINOUS_INTENSITY);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.ElectricalTemperature, StandardKindOfQuantities.TEMPERATURE);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.ElectricalPowerDensity, StandardKindOfQuantities.POWER_PER_AREA);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.ElectricalWattage, StandardKindOfQuantities.APPARENT_POWER);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.Energy, StandardKindOfQuantities.ENERGY);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.HVACAirflowDensity, StandardKindOfQuantities.AIR_FLOW_RATE);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.HVACDensity, StandardKindOfQuantities.DENSITY_GAS);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.HVACPower, StandardKindOfQuantities.POWER);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.HVACPowerDensity, StandardKindOfQuantities.POWER_PER_AREA);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.HVACSlope, StandardKindOfQuantities.SLOPE);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.HVACSpecificHeat, StandardKindOfQuantities.SPECIFIC_HEAT_CAPACITY);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.HVACTemperature, StandardKindOfQuantities.TEMPERATURE);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.HVACThermalResistance, StandardKindOfQuantities.THERMAL_INSULANCE);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.HVACViscosity, StandardKindOfQuantities.VISCOSITY);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.LinearMoment, StandardKindOfQuantities.LINEAR_MOMENT);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.Mass, StandardKindOfQuantities.MASS);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.MomentOfInertia, StandardKindOfQuantities.AREA_MOMENT_OF_INERTIA);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.PipingDensity, StandardKindOfQuantities.DENSITY);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.PipingSlope, StandardKindOfQuantities.SLOPE);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.PipingTemperature, StandardKindOfQuantities.TEMPERATURE);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.PipingViscosity, StandardKindOfQuantities.VISCOSITY);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.PipingVolume, StandardKindOfQuantities.VOLUME);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.Slope, StandardKindOfQuantities.SLOPE);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.StructuralFrequency, StandardKindOfQuantities.FREQUENCY);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.AreaForcePerLength, StandardKindOfQuantities.AREA_SPRING_CONSTANT);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.ElectricalResistivity, StandardKindOfQuantities.RESISTIVITY);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.ForceLengthPerAngle, StandardKindOfQuantities.ROTATIONAL_SPRING_CONSTANT);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.ForcePerLength, StandardKindOfQuantities.MOMENT_OF_FORCE);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.HVACCrossSection, StandardKindOfQuantities.AREA);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.HVACCoolingLoad, StandardKindOfQuantities.APPARENT_POWER);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.HVACCoolingLoadDividedByArea, StandardKindOfQuantities.POWER_PER_AREA);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.HVACDuctInsulationThickness, StandardKindOfQuantities.LENGTH);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.HVACDuctLiningThickness, StandardKindOfQuantities.LENGTH);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.HVACFactor, StandardKindOfQuantities.PERCENT);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.HVACHeatingLoad, StandardKindOfQuantities.APPARENT_POWER);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.HVACHeatingLoadDividedByArea, StandardKindOfQuantities.POWER_PER_AREA);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.HVACThermalConductivity, StandardKindOfQuantities.THERMAL_CONDUCTIVITY);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.LinearForcePerLength, StandardKindOfQuantities.LINEAR_SPRING_CONSTANT);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.MassPerUnitArea, StandardKindOfQuantities.MASS_PER_AREA);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.MassPerUnitLength, StandardKindOfQuantities.LINEIC_MASS);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.Period, StandardKindOfQuantities.PERIOD);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.PipeDimension, StandardKindOfQuantities.LENGTH);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.PipeInsulationThickness, StandardKindOfQuantities.LENGTH);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.PipeMass, StandardKindOfQuantities.MASS);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.PipeMassPerUnitLength, StandardKindOfQuantities.LINEIC_MASS);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.Pulsation, StandardKindOfQuantities.ANGULAR_FREQUENCY);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.ReinforcementArea, StandardKindOfQuantities.AREA);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.ReinforcementCover, StandardKindOfQuantities.LENGTH);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.ReinforcementLength, StandardKindOfQuantities.LENGTH);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.ReinforcementSpacing, StandardKindOfQuantities.LENGTH);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.ReinforcementVolume, StandardKindOfQuantities.VOLUME);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.Rotation, StandardKindOfQuantities.PLANE_ANGLE);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.SectionArea, StandardKindOfQuantities.AREA);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.SectionDimension, StandardKindOfQuantities.LENGTH);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.SectionModulus, StandardKindOfQuantities.VOLUME);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.SectionProperty, StandardKindOfQuantities.LENGTH);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.Stress, StandardKindOfQuantities.STRESS);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.StructuralVelocity, StandardKindOfQuantities.VELOCITY);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.ThermalExpansion, StandardKindOfQuantities.COEFFICIENT_OF_THERMAL_EXPANSION);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.UnitWeight, StandardKindOfQuantities.SPECIFIC_WEIGHT);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.WarpingConstant, StandardKindOfQuantities.BUCKLING_RESISTANCE);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.Weight, StandardKindOfQuantities.FORCE);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.WireSize, StandardKindOfQuantities.DIAMETER_SMALL);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.ElectricalTemperatureDifference, StandardKindOfQuantities.RECIPROCAL_TEMPERATURE);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.HVACTemperatureDifference, StandardKindOfQuantities.RECIPROCAL_TEMPERATURE);
            //UnitsConverter.revitParamKindOfQuantity.Add(ParameterType.PipingTemperatureDifference, StandardKindOfQuantities.RECIPROCAL_TEMPERATURE);

            //WS: the following is using new API============
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.Length, StandardKindOfQuantities.LENGTH);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.Area, StandardKindOfQuantities.AREA);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.Volume, StandardKindOfQuantities.VOLUME);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.Angle, StandardKindOfQuantities.PLANE_ANGLE);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.Force, StandardKindOfQuantities.FORCE);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.LinearForce, StandardKindOfQuantities.MOMENT_OF_FORCE);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.AreaForce, StandardKindOfQuantities.AREA_FORCE);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.Moment, StandardKindOfQuantities.MOMENT_OF_FORCE);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.Currency, StandardKindOfQuantities.CURRENCY);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.Current, StandardKindOfQuantities.ELECTRIC_CURRENT);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.ElectricalFrequency, StandardKindOfQuantities.FREQUENCY);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.Illuminance, StandardKindOfQuantities.ILLUMINANCE);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.ElectricalPotential, StandardKindOfQuantities.ELECTRIC_POTENTIAL);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.ElectricalPower, StandardKindOfQuantities.APPARENT_POWER);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.HvacPressure, StandardKindOfQuantities.PRESSURE);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.HvacRoughness, StandardKindOfQuantities.ROUGHNESS);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.AirFlow, StandardKindOfQuantities.FLOW);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.HvacEnergy, StandardKindOfQuantities.ENERGY);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.HeatGain, StandardKindOfQuantities.REACTIVE_POWER);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.HvacVelocity, StandardKindOfQuantities.VELOCITY);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.DuctSize, StandardKindOfQuantities.LENGTH);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.PipeSize, StandardKindOfQuantities.LENGTH);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.HvacFriction, StandardKindOfQuantities.FORCE_PER_VOLUME);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.HeatTransferCoefficient, StandardKindOfQuantities.COEFFICIENT_OF_HEAT_TRANSFER);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.ColorTemperature, StandardKindOfQuantities.TEMPERATURE);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.LuminousFlux, StandardKindOfQuantities.LUMINOUS_FLUX);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.PipingPressure, StandardKindOfQuantities.PRESSURE);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.Flow, StandardKindOfQuantities.FLOW);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.PipingVelocity, StandardKindOfQuantities.VELOCITY);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.PipingRoughness, StandardKindOfQuantities.ROUGHNESS);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.PipingFriction, StandardKindOfQuantities.FORCE_PER_VOLUME);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.DemandFactor, StandardKindOfQuantities.PERCENT);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.BarDiameter, StandardKindOfQuantities.DIAMETER);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.CrackWidth, StandardKindOfQuantities.LENGTH);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.Displacement, StandardKindOfQuantities.LENGTH);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.ApparentPower, StandardKindOfQuantities.APPARENT_POWER);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.CableTraySize, StandardKindOfQuantities.LENGTH);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.ConduitSize, StandardKindOfQuantities.LENGTH);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.Efficacy, StandardKindOfQuantities.LUMINOUS_EFFICACY);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.LuminousIntensity, StandardKindOfQuantities.LUMINOUS_INTENSITY);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.ElectricalTemperature, StandardKindOfQuantities.TEMPERATURE);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.ElectricalPowerDensity, StandardKindOfQuantities.POWER_PER_AREA);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.Wattage, StandardKindOfQuantities.APPARENT_POWER);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.Energy, StandardKindOfQuantities.ENERGY);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.AirFlowDensity, StandardKindOfQuantities.AIR_FLOW_RATE);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.HvacDensity, StandardKindOfQuantities.DENSITY_GAS);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.HvacPower, StandardKindOfQuantities.POWER);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.HvacPowerDensity, StandardKindOfQuantities.POWER_PER_AREA);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.HvacSlope, StandardKindOfQuantities.SLOPE);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.SpecificHeat, StandardKindOfQuantities.SPECIFIC_HEAT_CAPACITY);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.HvacTemperature, StandardKindOfQuantities.TEMPERATURE);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.ThermalResistance, StandardKindOfQuantities.THERMAL_INSULANCE);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.HvacViscosity, StandardKindOfQuantities.VISCOSITY);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.LinearMoment, StandardKindOfQuantities.LINEAR_MOMENT);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.Mass, StandardKindOfQuantities.MASS);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.MomentOfInertia, StandardKindOfQuantities.AREA_MOMENT_OF_INERTIA);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.PipingDensity, StandardKindOfQuantities.DENSITY);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.PipingSlope, StandardKindOfQuantities.SLOPE);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.PipingTemperature, StandardKindOfQuantities.TEMPERATURE);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.PipingViscosity, StandardKindOfQuantities.VISCOSITY);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.PipingVolume, StandardKindOfQuantities.VOLUME);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.Slope, StandardKindOfQuantities.SLOPE);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.StructuralFrequency, StandardKindOfQuantities.FREQUENCY);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.AreaForceScale, StandardKindOfQuantities.AREA_SPRING_CONSTANT);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.ElectricalResistivity, StandardKindOfQuantities.RESISTIVITY);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.ForceScale, StandardKindOfQuantities.ROTATIONAL_SPRING_CONSTANT);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.LinearForceScale, StandardKindOfQuantities.MOMENT_OF_FORCE);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.CrossSection, StandardKindOfQuantities.AREA);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.CoolingLoad, StandardKindOfQuantities.APPARENT_POWER);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.CoolingLoadDividedByArea, StandardKindOfQuantities.POWER_PER_AREA);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.DuctInsulationThickness, StandardKindOfQuantities.LENGTH);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.DuctLiningThickness, StandardKindOfQuantities.LENGTH);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.Factor, StandardKindOfQuantities.PERCENT);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.HeatingLoad, StandardKindOfQuantities.APPARENT_POWER);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.HeatingLoadDividedByArea, StandardKindOfQuantities.POWER_PER_AREA);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.ThermalConductivity, StandardKindOfQuantities.THERMAL_CONDUCTIVITY);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.MassPerUnitArea, StandardKindOfQuantities.MASS_PER_AREA);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.MassPerUnitLength, StandardKindOfQuantities.LINEIC_MASS);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.Period, StandardKindOfQuantities.PERIOD);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.PipeDimension, StandardKindOfQuantities.LENGTH);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.PipeInsulationThickness, StandardKindOfQuantities.LENGTH);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.PipeMassPerUnitLength, StandardKindOfQuantities.LINEIC_MASS);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.Pulsation, StandardKindOfQuantities.ANGULAR_FREQUENCY);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.ReinforcementArea, StandardKindOfQuantities.AREA);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.ReinforcementCover, StandardKindOfQuantities.LENGTH);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.ReinforcementLength, StandardKindOfQuantities.LENGTH);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.ReinforcementSpacing, StandardKindOfQuantities.LENGTH);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.ReinforcementVolume, StandardKindOfQuantities.VOLUME);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.Rotation, StandardKindOfQuantities.PLANE_ANGLE);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.SectionArea, StandardKindOfQuantities.AREA);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.SectionDimension, StandardKindOfQuantities.LENGTH);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.SectionModulus, StandardKindOfQuantities.VOLUME);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.SectionProperty, StandardKindOfQuantities.LENGTH);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.Stress, StandardKindOfQuantities.STRESS);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.StructuralVelocity, StandardKindOfQuantities.VELOCITY);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.ThermalExpansionCoefficient, StandardKindOfQuantities.COEFFICIENT_OF_THERMAL_EXPANSION);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.UnitWeight, StandardKindOfQuantities.SPECIFIC_WEIGHT);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.WarpingConstant, StandardKindOfQuantities.BUCKLING_RESISTANCE);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.Weight, StandardKindOfQuantities.FORCE);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.WireDiameter, StandardKindOfQuantities.DIAMETER_SMALL);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.ElectricalTemperatureDifference, StandardKindOfQuantities.RECIPROCAL_TEMPERATURE);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.HvacTemperatureDifference, StandardKindOfQuantities.RECIPROCAL_TEMPERATURE);
            UnitsConverter.revitParamKindOfQuantity.Add(SpecTypeId.PipingTemperatureDifference, StandardKindOfQuantities.RECIPROCAL_TEMPERATURE);
            ////===================


        }

        //WS: 2025-08-25
        public static bool ExportNumberAsValueString(ForgeTypeId type)
        //public static bool ExportNumberAsValueString(ParameterType type)
        {
            return false;
        }

        public static Unit GetRevitParameterDisplayUnit(Parameter param)
        {
            Unit unit;

            //WS: 2025-08-25
            //if (UnitsConverter.HasDisplayUnitType(param.Definition.ParameterType))
            if (UnitsConverter.HasDisplayUnitType(param.Definition.GetDataType()))
            {
                //WS: 2025-08-25 Got error here!, so add catch
                try
                {
                    if (UnitsConverter.revitDisplayUnitTypeConversions.TryGetValue(param.GetUnitTypeId(), out unit))
                    {
                        return unit;
                    }
                }
                catch (Exception )
                {
                    return null;
                }
                //ExpImpLogger.warning(string.Format("Export - no mapped EC units for {0} unit type ({1} parameter type)", param.DisplayUnitType, param.Definition.ParameterType));
            }
            return null;
        }

        public static KindOfQuantity GetRevitParameterKindOfQuantity(Parameter param)
        {
            KindOfQuantity kindOfQuantity;

            //WS: 2025-08-25
            //if (!UnitsConverter.revitParamKindOfQuantity.TryGetValue(param.Definition.ParameterType, out kindOfQuantity))
            if (!UnitsConverter.revitParamKindOfQuantity.TryGetValue(param.Definition.GetDataType(), out kindOfQuantity))
            {
                //ExpImpLogger.warning(string.Format("Export - no mapped EC dimension/KoG for {0} parameter type", param.Definition.ParameterType));
            }
            return kindOfQuantity;
        }

        //WS: 2025-08-25 new API
        public static double GetRevitParameterValueInBentleyUnit(Parameter param)
        {
            Unit revitParameterDisplayUnit;
            //object[] name;
            double num = param.AsDouble();
            Unit fOOT = null;
            //ParameterType parameterType = param.Definition.ParameterType;

            ForgeTypeId parameterType = param.Definition.GetDataType();

            if (parameterType == SpecTypeId.Length)
            {
                fOOT = StandardUnits.FOOT;
            }
            else if (parameterType == SpecTypeId.Area)
            {
                fOOT = StandardUnits.FOOT_SQUARED;
            }
            else if (parameterType == SpecTypeId.Volume)
            {
                fOOT = StandardUnits.FOOT_CUBED;
            }
            else if (parameterType == SpecTypeId.Angle)
            {
                fOOT = StandardUnits.RADIAN;
            }
            else
            {
                if (parameterType == SpecTypeId.HvacDensity || parameterType == SpecTypeId.PipingDensity)
                {
                    fOOT = StandardUnits.KILOGRAM_PER_METRE_CUBED;
                    num = StandardUnits.METRE_CUBED.ConvertFrom(num, StandardUnits.FOOT_CUBED);
                }
                else if (parameterType == SpecTypeId.HvacEnergy || parameterType == SpecTypeId.Energy)
                {
                    fOOT = StandardUnits.JOULE;
                    num = StandardUnits.METRE_SQUARED.ConvertFrom(num, StandardUnits.FOOT_SQUARED);
                }
                else if (parameterType == SpecTypeId.HvacFriction || parameterType == SpecTypeId.PipingFriction)
                {
                    fOOT = CustomUnits.REVIT_PASCAL_PER_FOOT;
                }
                else if (parameterType == SpecTypeId.HvacPower || parameterType == SpecTypeId.HeatGain || parameterType == SpecTypeId.ElectricalPower ||
                         parameterType == SpecTypeId.ApparentPower || parameterType == SpecTypeId.CoolingLoad || parameterType == SpecTypeId.HeatingLoad ||
                         parameterType == SpecTypeId.Wattage)
                {
                    num = StandardUnits.METRE_SQUARED.ConvertFrom(num, StandardUnits.FOOT_SQUARED);

                    if (param.GetUnitTypeId() != UnitTypeId.Horsepower && param.GetUnitTypeId() != UnitTypeId.BritishThermalUnitsPerSecond && param.GetUnitTypeId() != UnitTypeId.BritishThermalUnitsPerHour)
                    {
                        revitParameterDisplayUnit = UnitsConverter.GetRevitParameterDisplayUnit(param);

                        if (revitParameterDisplayUnit != null && fOOT != null)
                        {
                            if (!revitParameterDisplayUnit.CanConvertBetween(fOOT))
                            {
                                //name = new object[] { fOOT.Name, revitParameterDisplayUnit.Name, param.GetUnitTypeId().TypeId, param.Definition.ParameterType };
                                //ExpImpLogger.error(string.Format("Export - incompatible base EC units {0} with {1} mapped for {2} units of {3} parameter type", name));
                            }
                            else
                            {
                                num = revitParameterDisplayUnit.ConvertFrom(num, fOOT);
                            }
                        }
                        return num;
                    }
                    fOOT = StandardUnits.WATT;
                }
                else if (parameterType == SpecTypeId.HvacPressure || parameterType == SpecTypeId.PipingPressure || parameterType == SpecTypeId.Stress)
                {
                    fOOT = StandardUnits.PASCAL;
                    num = StandardUnits.FOOT.ConvertFrom(num, StandardUnits.METRE);
                }
                else if (parameterType == SpecTypeId.HvacTemperature || parameterType == SpecTypeId.PipingTemperature || parameterType == SpecTypeId.ColorTemperature ||
                         parameterType == SpecTypeId.ElectricalTemperature)
                {
                    fOOT = StandardUnits.DEGREE_KELVIN;
                }
                else if (parameterType == SpecTypeId.HvacVelocity || parameterType == SpecTypeId.PipingVelocity)
                {
                    fOOT = StandardUnits.FOOT_PER_SECOND;
                }
                else if (parameterType == SpecTypeId.AirFlow || parameterType == SpecTypeId.Flow)
                {
                    fOOT = StandardUnits.FOOT_CUBED_PER_SECOND;
                }
                else if (parameterType == SpecTypeId.CrossSection || parameterType == SpecTypeId.PipeSize || parameterType == SpecTypeId.WireDiameter ||
                         parameterType == SpecTypeId.CableTraySize || parameterType == SpecTypeId.ConduitSize)
                {
                    fOOT = StandardUnits.FOOT;
                }
                else if (parameterType == SpecTypeId.ElectricalPotential)
                {
                    fOOT = StandardUnits.VOLT;
                    num = StandardUnits.METRE_SQUARED.ConvertFrom(num, StandardUnits.FOOT_SQUARED);
                }
                else if (parameterType == SpecTypeId.HvacRoughness || parameterType == SpecTypeId.PipingRoughness)
                {
                    fOOT = StandardUnits.FOOT;
                }
                else if (parameterType == SpecTypeId.HvacVelocity || parameterType == SpecTypeId.PipingVelocity)
                {
                    fOOT = StandardUnits.PASCAL_SECOND;
                    num = StandardUnits.FOOT.ConvertFrom(num, StandardUnits.METRE);
                }
                else if (parameterType == SpecTypeId.PipingVolume || parameterType == SpecTypeId.ReinforcementVolume || parameterType == SpecTypeId.SectionModulus)
                {
                    fOOT = StandardUnits.FOOT_CUBED;
                }
                else if (parameterType == SpecTypeId.HeatTransferCoefficient)
                {
                    fOOT = StandardUnits.WATT_PER_METRE_SQUARED_PER_DELTA_DEGREE_KELVIN;
                }
                else if (parameterType == SpecTypeId.AirFlowDensity)
                {
                    fOOT = StandardUnits.FOOT_CUBED_PER_FOOT_SQUARED_PER_SECOND;
                }
                else if (parameterType == SpecTypeId.Slope || parameterType == SpecTypeId.HvacSlope || parameterType == SpecTypeId.PipingSlope || parameterType == SpecTypeId.Factor)
                {
                    if (param.GetUnitTypeId() != UnitTypeId.SlopeDegrees)
                    {
                        if (param.GetUnitTypeId() != UnitTypeId.Percentage)
                        {
                            revitParameterDisplayUnit = UnitsConverter.GetRevitParameterDisplayUnit(param);
                            if (revitParameterDisplayUnit != null && fOOT != null)
                            {
                                if (!revitParameterDisplayUnit.CanConvertBetween(fOOT))
                                {
                                    //name = new object[] { fOOT.Name, revitParameterDisplayUnit.Name, param.GetUnitTypeId().TypeId, param.Definition.ParameterType };
                                    //ExpImpLogger.error(string.Format("Export - incompatible base EC units {0} with {1} mapped for {2} units of {3} parameter type", name));
                                }
                                else
                                {
                                    num = revitParameterDisplayUnit.ConvertFrom(num, fOOT);
                                }
                            }
                            return num;
                        }
                        num *= 100;
                    }
                    else
                    {
                        num = StandardUnits.DEGREE.ConvertFrom(num, StandardUnits.RADIAN);
                    }
                }
                else if (parameterType == SpecTypeId.CoolingLoadDividedByArea || parameterType == SpecTypeId.HeatingLoadDividedByArea)
                {
                    fOOT = StandardUnits.WATT_PER_METRE_CUBED;
                    num = StandardUnits.METRE.ConvertFrom(num, StandardUnits.FOOT);
                }
                else if (parameterType == SpecTypeId.ReinforcementLength || parameterType == SpecTypeId.BarDiameter || parameterType == SpecTypeId.CrackWidth ||
                         parameterType == SpecTypeId.Displacement || parameterType == SpecTypeId.ReinforcementCover || parameterType == SpecTypeId.ReinforcementSpacing ||
                         parameterType == SpecTypeId.SectionDimension || parameterType == SpecTypeId.SectionProperty || parameterType == SpecTypeId.PipeDimension)
                {
                    fOOT = StandardUnits.FOOT;
                }
                else if (parameterType == SpecTypeId.DemandFactor)
                {
                    fOOT = StandardUnits.UNITLESS_PERCENT;
                }
                else if (parameterType == SpecTypeId.DuctInsulationThickness || parameterType == SpecTypeId.DuctLiningThickness || parameterType == SpecTypeId.PipeInsulationThickness)
                {
                    fOOT = StandardUnits.FOOT;
                }
                else if (parameterType == SpecTypeId.ThermalResistance)
                {
                    fOOT = StandardUnits.METRE_SQUARED_DELTA_DEGREE_KELVIN_PER_WATT;
                }
                else if (parameterType == SpecTypeId.ThermalMass)
                {
                    num = StandardUnits.METRE_SQUARED.ConvertFrom(num, StandardUnits.FOOT_SQUARED);
                    if (param.GetUnitTypeId() != UnitTypeId.KilojoulesPerKelvin)
                    {
                        //if (param.DisplayUnitType != DisplayUnitType.DUT_BRITISH_THERMAL_UNIT_PER_FAHRENHEIT)
                        if (param.GetUnitTypeId() != UnitTypeId.BritishThermalUnitsPerDegreeFahrenheit)
                        {
                            revitParameterDisplayUnit = UnitsConverter.GetRevitParameterDisplayUnit(param);
                            if (revitParameterDisplayUnit != null && fOOT != null)
                            {
                                if (!revitParameterDisplayUnit.CanConvertBetween(fOOT))
                                {
                                    //name = new object[] { fOOT.Name, revitParameterDisplayUnit.Name, param.GetUnitTypeId().TypeId, param.Definition.ParameterType };
                                    //ExpImpLogger.error(string.Format("Export - incompatible base EC units {0} with {1} mapped for {2} units of {3} parameter type", name));
                                }
                                else
                                {
                                    num = revitParameterDisplayUnit.ConvertFrom(num, fOOT);
                                }
                            }
                            return num;
                        }
                        num /= 1899.09988383089;
                    }
                    else
                    {
                        num /= 1000;
                    }
                }
                else if (parameterType == SpecTypeId.MomentOfInertia)
                {
                    fOOT = StandardUnits.FOOT_TO_THE_FOURTH;
                }
                else if (parameterType == SpecTypeId.Period)
                {
                    fOOT = StandardUnits.SECOND;
                }
                else if (parameterType == SpecTypeId.Pulsation)
                {
                    fOOT = StandardUnits.RADIAN_PER_SECOND;
                }
                else if (parameterType == SpecTypeId.Rotation)
                {
                    fOOT = StandardUnits.RADIAN;
                }
                else if (parameterType == SpecTypeId.WarpingConstant)
                {
                    fOOT = StandardUnits.FOOT_TO_THE_SIXTH;
                }
                else if (parameterType == SpecTypeId.ThermalConductivity)
                {
                    fOOT = StandardUnits.WATT_PER_METRE_PER_DEGREE_KELVIN;
                    num = StandardUnits.METRE.ConvertFrom(num, StandardUnits.FOOT);
                }
            }

            revitParameterDisplayUnit = UnitsConverter.GetRevitParameterDisplayUnit(param);

            if (revitParameterDisplayUnit != null && fOOT != null)
            {
                if (!revitParameterDisplayUnit.CanConvertBetween(fOOT))
                {
                    //name = new object[] { fOOT.Name, revitParameterDisplayUnit.Name, param.GetUnitTypeId().TypeId, param.Definition.ParameterType };
                    //ExpImpLogger.error(string.Format("Export - incompatible base EC units {0} with {1} mapped for {2} units of {3} parameter type", name));
                }
                else
                {
                    num = revitParameterDisplayUnit.ConvertFrom(num, fOOT);
                }
            }
            return num;
        }

        //WS: 2025-08-25 disable old API
        //public static double GetRevitParameterValueInBentleyUnit(Parameter param)
        //{
        //    Unit revitParameterDisplayUnit;
        //    //object[] name;
        //    double num = param.AsDouble();
        //    Unit fOOT = null;
        //    ParameterType parameterType = param.Definition.ParameterType;
        //    switch (parameterType)
        //    {
        //        case ParameterType.Length:
        //            {
        //                //Label2:
        //                fOOT = StandardUnits.FOOT;
        //                break;
        //            }
        //        case ParameterType.Area:
        //            {
        //                //Label0:
        //                fOOT = StandardUnits.FOOT_SQUARED;
        //                break;
        //            }
        //        case ParameterType.Volume:
        //            {
        //                //Label1:
        //                fOOT = StandardUnits.FOOT_CUBED;
        //                break;
        //            }
        //        case ParameterType.Angle:
        //            {
        //                //Label3:
        //                fOOT = StandardUnits.RADIAN;
        //                break;
        //            }
        //        default:
        //            {
        //                switch (parameterType)
        //                {
        //                    case ParameterType.HVACDensity:
        //                    case ParameterType.PipingDensity:
        //                        {
        //                            fOOT = StandardUnits.KILOGRAM_PER_METRE_CUBED;
        //                            num = StandardUnits.METRE_CUBED.ConvertFrom(num, StandardUnits.FOOT_CUBED);
        //                            break;
        //                        }
        //                    case ParameterType.HVACEnergy:
        //                    case ParameterType.Energy:
        //                        {
        //                            fOOT = StandardUnits.JOULE;
        //                            num = StandardUnits.METRE_SQUARED.ConvertFrom(num, StandardUnits.FOOT_SQUARED);
        //                            break;
        //                        }
        //                    case ParameterType.HVACFriction:
        //                    case ParameterType.PipingFriction:
        //                        {
        //                            fOOT = CustomUnits.REVIT_PASCAL_PER_FOOT;
        //                            break;
        //                        }
        //                    case ParameterType.HVACPower:
        //                    case ParameterType.HVACHeatGain:
        //                    case ParameterType.ElectricalPower:
        //                    case ParameterType.ElectricalApparentPower:
        //                    case ParameterType.HVACCoolingLoad:
        //                    case ParameterType.HVACHeatingLoad:
        //                    case ParameterType.ElectricalWattage:
        //                        {
        //                            num = StandardUnits.METRE_SQUARED.ConvertFrom(num, StandardUnits.FOOT_SQUARED);
        //                            if (param.GetUnitTypeId() != UnitTypeId.Horsepower && param.GetUnitTypeId() != UnitTypeId.BritishThermalUnitsPerSecond && param.GetUnitTypeId() != UnitTypeId.BritishThermalUnitsPerHour)
        //                            {
        //                                revitParameterDisplayUnit = UnitsConverter.GetRevitParameterDisplayUnit(param);
        //                                if (revitParameterDisplayUnit != null && fOOT != null)
        //                                {
        //                                    if (!revitParameterDisplayUnit.CanConvertBetween(fOOT))
        //                                    {
        //                                        //name = new object[] { fOOT.Name, revitParameterDisplayUnit.Name, param.DisplayUnitType, param.Definition.ParameterType };
        //                                        //ExpImpLogger.error(string.Format("Export - incompatible base EC units {0} with {1} mapped for {2} units of {3} parameter type", name));
        //                                    }
        //                                    else
        //                                    {
        //                                        num = revitParameterDisplayUnit.ConvertFrom(num, fOOT);
        //                                    }
        //                                }
        //                                return num;
        //                            }
        //                            fOOT = StandardUnits.WATT;
        //                            break;
        //                        }
        //                    case ParameterType.HVACPressure:
        //                    case ParameterType.PipingPressure:
        //                    case ParameterType.Stress:
        //                        {
        //                            fOOT = StandardUnits.PASCAL;
        //                            num = StandardUnits.FOOT.ConvertFrom(num, StandardUnits.METRE);
        //                            break;
        //                        }
        //                    case ParameterType.HVACTemperature:
        //                    case ParameterType.PipingTemperature:
        //                    case ParameterType.ColorTemperature:
        //                    case ParameterType.ElectricalTemperature:
        //                        {
        //                            fOOT = StandardUnits.DEGREE_KELVIN;
        //                            break;
        //                        }
        //                    case ParameterType.HVACVelocity:
        //                    case ParameterType.PipingVelocity:
        //                        {
        //                            fOOT = StandardUnits.FOOT_PER_SECOND;
        //                            break;
        //                        }
        //                    case ParameterType.HVACAirflow:
        //                    case ParameterType.PipingFlow:
        //                        {
        //                            fOOT = StandardUnits.FOOT_CUBED_PER_SECOND;
        //                            break;
        //                        }
        //                    case ParameterType.HVACDuctSize:
        //                    case ParameterType.PipeSize:
        //                    case ParameterType.WireSize:
        //                    case ParameterType.ElectricalCableTraySize:
        //                    case ParameterType.ElectricalConduitSize:
        //                        {
        //                            fOOT = StandardUnits.FOOT;
        //                            break;
        //                        }
        //                    case ParameterType.HVACCrossSection:
        //                    case ParameterType.ReinforcementArea:
        //                    case ParameterType.SectionArea:
        //                        {
        //                            //goto Label0;

        //                            //WS:
        //                            fOOT = StandardUnits.FOOT_SQUARED;
        //                            break;
        //                        }
        //                    case ParameterType.ElectricalPotential:
        //                        {
        //                            fOOT = StandardUnits.VOLT;
        //                            num = StandardUnits.METRE_SQUARED.ConvertFrom(num, StandardUnits.FOOT_SQUARED);
        //                            break;
        //                        }
        //                    case ParameterType.HVACRoughness:
        //                    case ParameterType.PipingRoughness:
        //                        {
        //                            fOOT = StandardUnits.FOOT;
        //                            break;
        //                        }
        //                    case ParameterType.PipingViscosity:
        //                    case ParameterType.HVACViscosity:
        //                        {
        //                            fOOT = StandardUnits.PASCAL_SECOND;
        //                            num = StandardUnits.FOOT.ConvertFrom(num, StandardUnits.METRE);
        //                            break;
        //                        }
        //                    case ParameterType.PipingVolume:
        //                    case ParameterType.ReinforcementVolume:
        //                    case ParameterType.SectionModulus:
        //                        {
        //                            fOOT = StandardUnits.FOOT_CUBED;
        //                            break;
        //                        }
        //                    case ParameterType.HVACCoefficientOfHeatTransfer:
        //                        {
        //                            fOOT = StandardUnits.WATT_PER_METRE_SQUARED_PER_DELTA_DEGREE_KELVIN;
        //                            break;
        //                        }
        //                    case ParameterType.HVACAirflowDensity:
        //                        {
        //                            fOOT = StandardUnits.FOOT_CUBED_PER_FOOT_SQUARED_PER_SECOND;
        //                            break;
        //                        }
        //                    case ParameterType.Slope:
        //                    case ParameterType.HVACSlope:
        //                    case ParameterType.PipingSlope:
        //                    case ParameterType.HVACFactor:
        //                        {
        //                            if (param.GetUnitTypeId() != UnitTypeId.SlopeDegrees)
        //                            {
        //                                if (param.GetUnitTypeId() != UnitTypeId.Percentage)
        //                                {
        //                                    revitParameterDisplayUnit = UnitsConverter.GetRevitParameterDisplayUnit(param);
        //                                    if (revitParameterDisplayUnit != null && fOOT != null)
        //                                    {
        //                                        if (!revitParameterDisplayUnit.CanConvertBetween(fOOT))
        //                                        {
        //                                            //name = new object[] { fOOT.Name, revitParameterDisplayUnit.Name, param.DisplayUnitType, param.Definition.ParameterType };
        //                                            //ExpImpLogger.error(string.Format("Export - incompatible base EC units {0} with {1} mapped for {2} units of {3} parameter type", name));
        //                                        }
        //                                        else
        //                                        {
        //                                            num = revitParameterDisplayUnit.ConvertFrom(num, fOOT);
        //                                        }
        //                                    }
        //                                    return num;
        //                                }
        //                                num *= 100;
        //                            }
        //                            else
        //                            {
        //                                num = StandardUnits.DEGREE.ConvertFrom(num, StandardUnits.RADIAN);
        //                            }
        //                            break;
        //                        }
        //                    case ParameterType.HVACCoolingLoadDividedByArea:
        //                    case ParameterType.HVACHeatingLoadDividedByArea:
        //                        {
        //                            fOOT = StandardUnits.WATT_PER_METRE_SQUARED;
        //                            break;
        //                        }
        //                    case ParameterType.HVACCoolingLoadDividedByVolume:
        //                    case ParameterType.HVACHeatingLoadDividedByVolume:
        //                        {
        //                            fOOT = StandardUnits.WATT_PER_METRE_CUBED;
        //                            num = StandardUnits.METRE.ConvertFrom(num, StandardUnits.FOOT);
        //                            break;
        //                        }
        //                    case ParameterType.ReinforcementLength:
        //                    case ParameterType.BarDiameter:
        //                    case ParameterType.CrackWidth:
        //                    case ParameterType.DisplacementDeflection:
        //                    case ParameterType.ReinforcementCover:
        //                    case ParameterType.ReinforcementSpacing:
        //                    case ParameterType.SectionDimension:
        //                    case ParameterType.SectionProperty:
        //                    case ParameterType.PipeDimension:
        //                        {
        //                            fOOT = StandardUnits.FOOT;
        //                            break;
        //                        }
        //                    case ParameterType.ElectricalDemandFactor:
        //                        {
        //                            fOOT = StandardUnits.UNITLESS_PERCENT;
        //                            break;
        //                        }
        //                    case ParameterType.HVACDuctInsulationThickness:
        //                    case ParameterType.HVACDuctLiningThickness:
        //                    case ParameterType.PipeInsulationThickness:
        //                        {
        //                            fOOT = StandardUnits.FOOT;
        //                            break;
        //                        }
        //                    case ParameterType.HVACThermalResistance:
        //                        {
        //                            fOOT = StandardUnits.METRE_SQUARED_DELTA_DEGREE_KELVIN_PER_WATT;
        //                            break;
        //                        }
        //                    case ParameterType.HVACThermalMass:
        //                        {
        //                            num = StandardUnits.METRE_SQUARED.ConvertFrom(num, StandardUnits.FOOT_SQUARED);
        //                            if (param.GetUnitTypeId() != UnitTypeId.KilojoulesPerKelvin)
        //                            {
        //                                if (param.GetUnitTypeId() != UnitTypeId.BritishThermalUnitsPerDegreeFahrenheit)
        //                                {
        //                                    revitParameterDisplayUnit = UnitsConverter.GetRevitParameterDisplayUnit(param);
        //                                    if (revitParameterDisplayUnit != null && fOOT != null)
        //                                    {
        //                                        if (!revitParameterDisplayUnit.CanConvertBetween(fOOT))
        //                                        {
        //                                            //name = new object[] { fOOT.Name, revitParameterDisplayUnit.Name, param.DisplayUnitType, param.Definition.ParameterType };
        //                                            //ExpImpLogger.error(string.Format("Export - incompatible base EC units {0} with {1} mapped for {2} units of {3} parameter type", name));
        //                                        }
        //                                        else
        //                                        {
        //                                            num = revitParameterDisplayUnit.ConvertFrom(num, fOOT);
        //                                        }
        //                                    }
        //                                    return num;
        //                                }
        //                                num /= 1899.09988383089;
        //                            }
        //                            else
        //                            {
        //                                num /= 1000;
        //                            }
        //                            break;
        //                        }
        //                    case ParameterType.MomentOfInertia:
        //                        {
        //                            fOOT = StandardUnits.FOOT_TO_THE_FOURTH;
        //                            break;
        //                        }
        //                    case ParameterType.Period:
        //                        {
        //                            fOOT = StandardUnits.SECOND;
        //                            break;
        //                        }
        //                    case ParameterType.Pulsation:
        //                        {
        //                            fOOT = StandardUnits.RADIAN_PER_SECOND;
        //                            break;
        //                        }
        //                    case ParameterType.Rotation:
        //                        {
        //                            fOOT = StandardUnits.RADIAN;
        //                            break;
        //                        }
        //                    case ParameterType.WarpingConstant:
        //                        {
        //                            fOOT = StandardUnits.FOOT_TO_THE_SIXTH;
        //                            break;
        //                        }
        //                    case ParameterType.HVACThermalConductivity:
        //                        {
        //                            fOOT = StandardUnits.WATT_PER_METRE_PER_DEGREE_KELVIN;
        //                            num = StandardUnits.METRE.ConvertFrom(num, StandardUnits.FOOT);
        //                            break;
        //                        }
        //                }
        //                break;
        //            }
        //    }

        //    revitParameterDisplayUnit = UnitsConverter.GetRevitParameterDisplayUnit(param);
        //    if (revitParameterDisplayUnit != null && fOOT != null)
        //    {
        //        if (!revitParameterDisplayUnit.CanConvertBetween(fOOT))
        //        {
        //            //name = new object[] { fOOT.Name, revitParameterDisplayUnit.Name, param.DisplayUnitType, param.Definition.ParameterType };
        //            //ExpImpLogger.error(string.Format("Export - incompatible base EC units {0} with {1} mapped for {2} units of {3} parameter type", name));
        //        }
        //        else
        //        {
        //            num = revitParameterDisplayUnit.ConvertFrom(num, fOOT);
        //        }
        //    }
        //    return num;
        //}

        //WS: 2025-08-25 new API
        private static bool HasDisplayUnitType(ForgeTypeId paramType)
        {
            if (paramType == SpecTypeId.Number || paramType == SpecTypeId.Length || paramType == SpecTypeId.Area ||
                paramType == SpecTypeId.Volume || paramType == SpecTypeId.Angle || paramType == SpecTypeId.Force ||
                paramType == SpecTypeId.LinearForce || paramType == SpecTypeId.AreaForce || paramType == SpecTypeId.Moment)
            {
                return true;
            }
            else if (paramType == SpecTypeId.Reference.Image || paramType == SpecTypeId.Reference.Material || paramType == SpecTypeId.Boolean.YesNo ||
                     paramType == SpecTypeId.String.Url || paramType == SpecTypeId.Int.NumberOfPoles || paramType == SpecTypeId.Reference.LoadClassification ||
                     paramType == SpecTypeId.String.Text || paramType == SpecTypeId.String.MultilineText)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        //WS: 2025-08-25 disable old API
        //private static bool HasDisplayUnitType(ParameterType paramType)
        //{
        //    switch (paramType)
        //    {
        //        case ParameterType.Number:
        //        case ParameterType.Length:
        //        case ParameterType.Area:
        //        case ParameterType.Volume:
        //        case ParameterType.Angle:
        //        case ParameterType.Force:
        //        case ParameterType.LinearForce:
        //        case ParameterType.AreaForce:
        //        case ParameterType.Moment:
        //            {
        //                return true;
        //            }
        //        case ParameterType.URL:
        //        case ParameterType.Material:
        //        case ParameterType.YesNo:
        //        case ParameterType.NumberOfPoles:
        //        case ParameterType.FixtureUnit:
        //        case ParameterType.FamilyType:
        //        case ParameterType.LoadClassification:
        //            {
        //                return 100 < (int)paramType;
        //            }
        //        case ParameterType.Image:
        //            {
        //                return false;
        //            }
        //        default:
        //            {
        //                return 100 < (int)paramType;
        //            }
        //    }
        //}
    }
}
