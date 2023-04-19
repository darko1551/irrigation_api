namespace irregation_api.Models
{
    public class SensorData
    {
        public SensorData(double? temperature, double? humidity, bool? state, DateTime? time)
        {
            Temperature = temperature;
            Humidity = humidity;
            State = state;
            Time = time;
        }


        public double? Temperature { get; set; }
        public double? Humidity { get; set; }
        public bool? State { get; set; }
        public DateTime? Time { get; set; }
    }
}
