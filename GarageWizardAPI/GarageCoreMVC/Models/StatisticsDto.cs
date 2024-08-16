namespace GarageCoreAPI.Models
{
    public class StatisticsDto
    {
        public int TwoWheelers { get; set; }
        public int FourWheelers { get; set; }
        public int SpecialWheelers { get; set; }
        public int VehiclesOnRepair { get; set; }
        public int VacantSpots { get; set; }
        public double RevenueFromToll { get; set; }
        public double RevenueFromRepairs { get; set; }
    }
}
