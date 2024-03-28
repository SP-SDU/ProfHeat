// Ignore Spelling: Timestamp

using System;

namespace OptiHeat.Models;

public class ResultDataModel
{
    public int Id { get; set; } // Id
    public DateTime Timestamp { get; set; } // Date and Time
    public double HeatProduced { get; set; } // MWh
    public double ElectricityProduced { get; set; } // MWh
    public double ElectricityConsumed { get; set; } // MWh
    public double OperatingExpenses { get; set; } // DKK
    public double Profits { get; set; } // DKK
    public double CO2Emissions { get; set; } // Tons
}