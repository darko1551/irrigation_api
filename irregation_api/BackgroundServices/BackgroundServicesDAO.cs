using irregation_api.Data;
using irregation_api.Entity;
using irregation_api.Models.Json;
using irregation_api.Models.Response;
using SQLitePCL;

namespace irregation_api.Socket
{
    public class BackgroundServicesDAO
    {
        private ApplicationDbContext context;

        public BackgroundServicesDAO(ApplicationDbContext ctx)
        {
            context = ctx;
        }

        public void updateDatabase(SensorReading sensorReading) {
            var retrievedSensorMac = context.Sensors.SingleOrDefault(e => e.Mac == sensorReading.Mac);
            if (retrievedSensorMac != null)
            {
                /* if (sensorReading.Temperature.IsValid) {
                     retrievedSensorMac.Temperature = sensorReading.Temperature.Temperature;
                 }
                 if (sensorReading.Humidity.IsValid)
                 {
                     retrievedSensorMac.Humidity = sensorReading.Humidity.Humidity;
                 }*/
                //retrievedSensorMac.State = sensorReading.State;

                handleWaterUsage(sensorReading);

                retrievedSensorMac.LastActive = DateTime.Now;
                context.SaveChanges();
            }
        }
       
        public Dictionary<string, List<IrregationScheduleResponse>> getSchedules() {
            Dictionary<string, List<IrregationScheduleResponse>> schedules = new Dictionary<string, List<IrregationScheduleResponse>>();
            foreach (SensorEntity sensorEntity in context.Sensors) {
                schedules.Add(sensorEntity.Uuid, sensorEntity.IrregationScheduleEntitys.Select(e => e.asResponseModel()).ToList());
            }
            return schedules;
        }

        public void updateUuids() {
            List<String> deviceUuids = new List<String>();
            foreach (SensorEntity sensorEntity in context.Sensors)
            {
                if (sensorEntity.Uuid != null)
                {
                    deviceUuids.Add(sensorEntity.Uuid);
                }
            }
            GlobalUuid.setList(deviceUuids);
        }


        private void handleWaterUsage(SensorReading sensorReading) {
            var sensorEntity = context.Sensors.SingleOrDefault(e => e.Mac == sensorReading.Mac);
            bool? storedState = sensorEntity!.State;


            if (storedState == null || storedState == false)
            {
                if (sensorReading.State.State == 2)
                {
                    sensorEntity.State = false;
                }
                else if (sensorReading.State.State == 1)
                {
                    sensorEntity.Time = DateTime.Now;
                    sensorEntity.State = true;
                }
                else
                {
                    sensorEntity.State = null;
                }
            }
            else {
                if (sensorReading.State.State == 1)
                {
                    //No changes
                }
                else if (sensorReading.State.State == 2) {
                    if (sensorEntity.Time != null) {
                        double? timeDifferenceInMin = DateTime.Now.Subtract((DateTime)sensorEntity.Time).TotalMinutes;
                        //change based on Valve water flow
                        double? waterUsed = timeDifferenceInMin * 25;
                        sensorEntity.WaterUsedLast = waterUsed;
                        if (sensorEntity.WaterUsedAll != null)
                        {
                            sensorEntity.WaterUsedAll += waterUsed;
                        }
                        else {
                            sensorEntity.WaterUsedAll = waterUsed;
                        }
                            
                        sensorEntity.Time = null;
                        sensorEntity.State = false;
                    }
                }
            }
            context.SaveChanges();
        }
    }

}
