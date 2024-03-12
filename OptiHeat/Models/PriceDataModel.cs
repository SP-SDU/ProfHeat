using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptiHeat.Models;

public class PriceDataModel
{
    public int Id { get; set; } // Id
    public DateTime Timestamp { get; set; } // Date and Time
    public double PricePerMWh { get; set; } // DKK/MWh (I assume DKK because DanFoss is a Danish company)
}