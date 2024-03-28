// Ignore Spelling: Timestamp Wh

using System;

namespace OptiHeat.Models;

public class DemandDataModel
{
    public int Id { get; set; } // Id
    public DateTime Timestamp { get; set; } // Date and Time
    public double DemandInMWh { get; set; } // MWh
}