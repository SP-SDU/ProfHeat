using System;
using System.Collections.Generic;

namespace OptiHeat.Models;

// Maybe should be a service instead of a model, loading the csv files
public class SourceDataManagerBusinessModel
{
    public List<string> FetchHeatDemandData()
    {
        // Fetch heat demand data
        throw new NotImplementedException();
    }

    public List<string> FetchElectricityPriceData()
    {
        // Fetch electricity price data
        throw new NotImplementedException();
    }

    public void UpdateData()
    {
        // Update data
        throw new NotImplementedException();
    }
}
