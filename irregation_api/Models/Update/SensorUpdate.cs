using irregation_api.Models.Response;

namespace irregation_api.Models.Update
{
    public class SensorUpdate
    {
        public SensorUpdate(string name, double latitude, double longitude, double humidityThreshold)
        {
            Name = name;
            Latitude = latitude;
            Longitude = longitude;
            HumidityThreshold = humidityThreshold;
        }

        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double HumidityThreshold { get; set; }
    }
}
