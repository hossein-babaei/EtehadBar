using System.Collections.Generic;

namespace EtehadBar.Domain
{
    public class PriceRangeModel
    {
        public double Maximum { get; set; }
        public double Minimum { get; set; }
        public int Divider { get; set; }
    }

    public class OriginModel
    {
        public string Name { get; set; }
    }

    public class DestinationModel
    {
        public string Name { get; set; }
    }

    public class LoadFactorModel
    {
        public long VehicleId { get; set; }
        public string VehicleLeftNumber { get; set; }
        public string VehicleRightNumber { get; set; }
        public string DriverName { get; set; }
        public string VehicleNumber { get; set; }
        public double Amount { get; set; }
        public string CustomerName { get; set; }
        public List<LoadFactorDetailModel> Details { get; set; } = new List<LoadFactorDetailModel>();
    }

    public class LoadFactorDetailModel 
    {
        public int Day { get; set; }
        public string Date { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string LoadFactorNumber { get; set; }
        public double Amount { get; set; }
    }
}
