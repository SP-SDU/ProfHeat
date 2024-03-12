using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptiHeat.Models;

public class ProductionUnitModel
{
    public int Id { get; set; } // Unit Id
    public string Name { get; set; } // Unit name
    public ProductionUnitType Type { get; set; } // Type of unit
    public double MaximumOutput { get; set; } // MWh
    public double MinimumOutput { get; set; } // MWh
    public double OperationCost { get; set; } // DKK/MWh
    public double CO2Emissions { get; set; } // kg/MWh
    public bool IsElectricityProducer { get; set; } // Produces electricity? (True/False)
    public double ElectricityOutput { get; set; } // MWh (if producer)
    public double ElectricityConsumption { get; set; } // MWh (if consumer)
}