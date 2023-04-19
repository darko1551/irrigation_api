using irregation_api.Entity;
using irregation_api.Models.Response;

namespace irregation_api.Models.Request
{
    public class SensorRequest
    {
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Mac { get; set; }
        public double HumidityThreshold { get; set; }
        public int UserId { get; set; }
    }
}
