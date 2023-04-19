using irregation_api.Entity;
using irregation_api.Models.Response;

namespace irregation_api.Models.Response
{
    public class SensorResponse
    {
        public SensorResponse(int sensorId,string uuid ,string name, double latitude, double longitude, string mac, UserNoSensorsResponse user, SensorData? sensorData, double? humidityThreshold, DateTime? lastActive, double? waterUsageLast, double? waterUsageAllTime, IEnumerable<IrregationScheduleResponse> irregationSchedules, int userId, bool? state)
        {
            SensorId = sensorId;
            Uuid = uuid;
            Name = name;
            Latitude = latitude;
            Longitude = longitude;
            Mac = mac;
            User = user;
            SensorData = sensorData;
            HumidityThreshold = humidityThreshold;
            LastActive = lastActive;
            WaterUsageLast = waterUsageLast;
            WaterUsageAllTime = waterUsageAllTime;
            IrregationSchedules = irregationSchedules;
            UserId = userId;
            State = state;
   
        }

        public int SensorId { get; set; }
        public string Uuid { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Mac { get; set; }
        public UserNoSensorsResponse? User { get; set; }
        public SensorData? SensorData { get; set; }
        public double? HumidityThreshold { get; set; }

        public DateTime? LastActive { get; set; }
        public double? WaterUsageLast { get; set; }
        public double? WaterUsageAllTime { get; set; }
        public IEnumerable<IrregationScheduleResponse> IrregationSchedules { get; set; }
        public int UserId { get; set; }
        public bool? State { get; set; }

    }
}
