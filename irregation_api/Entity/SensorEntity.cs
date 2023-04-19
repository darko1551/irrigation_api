using irregation_api.Models;
using irregation_api.Models.Response;
using System.ComponentModel.DataAnnotations;

namespace irregation_api.Entity
{
    public class SensorEntity
    {
        public SensorEntity(){}

        public SensorEntity(int sensorId,string uuid ,string mac, string name, double latitude, double longitude, double humidityThreshold, double? humidity, double? temperature, bool? state, DateTime? time, DateTime? lastActive, double? waterUsedLast, double? waterUsedAll, IEnumerable<IrregationScheduleEntity> irregationScheduleEntitys, UserEntity userEntity, int userEntityId)
        {
            SensorId = sensorId;
            Uuid = uuid;
            Mac = mac;
            Name = name;
            Latitude = latitude;
            Longitude = longitude;
            HumidityThreshold = humidityThreshold;
            Humidity = humidity;
            Temperature = temperature;
            State = state;
            Time = time;
            LastActive = lastActive;
            WaterUsedLast = waterUsedLast;
            WaterUsedAll = waterUsedAll;
            IrregationScheduleEntitys = irregationScheduleEntitys;
            UserEntity = userEntity;
            UserEntityId = userEntityId;
 
        }

        [Key]
        public int SensorId { get; set; }
        [Required]
        public string Uuid { get; set; }
        [Required]
        public string Mac { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public double Latitude { get; set; }
        [Required]
        public double Longitude { get; set; }
        [Required]
        public double HumidityThreshold { get; set; }
        public double? Humidity { get; set; }
        public double? Temperature { get; set; }
        public bool? State { get; set; }
        public DateTime? Time { get; set; }
        public DateTime? LastActive { get; set; }
        public double? WaterUsedLast { get; set; }
        public double? WaterUsedAll { get; set; }
        public virtual IEnumerable<IrregationScheduleEntity> IrregationScheduleEntitys { get; set; }
        public UserEntity UserEntity { get; set; }
        public int UserEntityId { get; set; }



        

        public SensorResponse asResponseModel()
        {
            return new SensorResponse(
                this.SensorId,
                this.Uuid,
                this.Name,
                this.Latitude,
                this.Longitude,
                this.Mac,
                new UserNoSensorsResponse(
                    this.UserEntity.UserId, 
                    this.UserEntity.Name, 
                    this.UserEntity.Surname, 
                    this.UserEntity.Email),
                new SensorData(
                    this.Temperature,
                    this.Humidity, 
                    this.State, 
                    this.Time),
                this.HumidityThreshold,
                this.LastActive,
                this.WaterUsedLast,
                this.WaterUsedAll,
                asIenumExternalIrregation(this.IrregationScheduleEntitys),
                this.UserEntityId,
                this.State
             
           );
        }

        public SensorNoUserResponse asNoUserResponseModel()
        {
            return new SensorNoUserResponse(
                this.SensorId,
                this.Uuid,
                this.Name,
                this.Latitude,
                this.Longitude,
                this.Mac,
                new SensorData(
                    this.Temperature,
                    this.Humidity,
                    this.State,
                    this.Time),
                this.HumidityThreshold,
                this.LastActive,
                this.WaterUsedLast,
                this.WaterUsedAll,
                asIenumExternalIrregation(this.IrregationScheduleEntitys),
                this.UserEntityId,
                this.State
     
           );
        }



        public IEnumerable<IrregationScheduleResponse> asIenumExternalIrregation(IEnumerable<IrregationScheduleEntity> irregationSchedules)
        {
            IEnumerable<IrregationScheduleResponse> result = new List<IrregationScheduleResponse>();
            foreach (IrregationScheduleEntity schedule in irregationSchedules) {
                result = result.Append(new IrregationScheduleResponse(schedule.IrregationScheduleId, schedule.DateFrom, schedule.DateTo,  schedule.Time, schedule.Duration, schedule.Activated));
            }
            return result;
        }
    }


}
