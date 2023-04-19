using irregation_api.Entity;
using irregation_api.Models.Request;
using irregation_api.Models.Response;
using irregation_api.Models.Update;
using irregation_api.Static;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using System;
using System.Security.Cryptography;

namespace irregation_api.Data
{
    public class Dao
    {
        ApplicationDbContext context;
        public Dao(ApplicationDbContext ctx) {
            context = ctx;
        }

        public UserResponse? getUser(int userId) {
            UserEntity? user = context.Users.SingleOrDefault(e => e.UserId == userId);
            if (user != null) {
                return user.asResponseModel();
            }
            return null;
        }

        public List<SensorResponse> getSensors(int userId) {
            List<SensorResponse> sensors = new List<SensorResponse>();
            foreach (SensorEntity sensor in context.Sensors.Include(a => a.IrregationScheduleEntitys).Include(b => b.UserEntity!).Where(e => e.UserEntityId == userId).ToList())
            {
                sensors.Add(sensor.asResponseModel());
            }
            return sensors;
        }

        public List<SensorResponse> getSensorsAllUsers()
        {
            List<SensorResponse> sensors = new List<SensorResponse>();
            foreach (SensorEntity sensor in context.Sensors.Include(a => a.IrregationScheduleEntitys).Include(b => b.UserEntity!).ToList())
            {
                sensors.Add(sensor.asResponseModel());
            }
            return sensors;
        }

        public SensorResponse? getSensorByMac(int userId, String mac) {
            SensorEntity? retrievedSensor = context.Sensors.Where(x => x.Mac.Equals(mac)).Include(x => x.UserEntity).Include(y => y.IrregationScheduleEntitys).Where(e => e.UserEntityId == userId).SingleOrDefault();
            if (retrievedSensor != null)
            {
                return retrievedSensor.asResponseModel();
            }
            return null;
        }

        public SensorResponse? getSensorByMacAllUsers(String mac) {
            var retrievedSensor = context.Sensors.SingleOrDefault(e => e.Mac == mac);
            if (retrievedSensor != null)
            {
                retrievedSensor.asResponseModel();
            }
            return null;
        }

        public SensorResponse? getSensorByName(int userId, String name) {
            var retrievedSensor = context.Sensors.SingleOrDefault(e => e.Name == name && e.UserEntityId == userId);
            if (retrievedSensor != null) {
                return retrievedSensor.asResponseModel();
            }
            return null;
        }

        public SensorResponse? getSensorByIdAllUsers(int sensorId) {
            var retrievedSensor = context.Sensors.SingleOrDefault(e => e.SensorId == sensorId);
            if (retrievedSensor != null) {
                return retrievedSensor.asResponseModel();
            }
            return null;


        }
        public SensorResponse? getSensorById(int userId, int sensorId)
        {
            var retrievedSensor = context.Sensors.Where(x => x.SensorId == sensorId).Include(x => x.UserEntity).Include(y => y.IrregationScheduleEntitys).SingleOrDefault(e => e.UserEntityId == userId);
            if (retrievedSensor != null)
            {
                return retrievedSensor.asResponseModel();
            }
            return null;
        }

        public int? addSensor(SensorRequest sensorRequest, String uuid) {

            SensorEntity entity;

            try {
                entity = new SensorEntity()
                {
                    Name = sensorRequest.Name,
                    Uuid = uuid,
                    UserEntityId = sensorRequest.UserId,
                    Latitude = sensorRequest.Latitude,
                    Longitude = sensorRequest.Longitude,
                    Mac = sensorRequest.Mac,
                    HumidityThreshold = sensorRequest.HumidityThreshold,
                    State = null
                };
            }
            catch {
                return null;
            }
            
            context.Sensors.Add(entity);
            context.SaveChanges();
            return entity.SensorId;
        }

        public int? updateSensor(int userId, int sensorId, SensorUpdate sensorUpdate) {
            var retrievedSensor = context.Sensors.Where(x => x.SensorId == sensorId).Include(x => x.UserEntity).Include(y => y.IrregationScheduleEntitys).SingleOrDefault(e => e.UserEntityId == userId);

            if (retrievedSensor != null)
            {
                retrievedSensor.Name = sensorUpdate.Name;
                retrievedSensor.Latitude = sensorUpdate.Latitude;
                retrievedSensor.Longitude = sensorUpdate.Longitude;
                retrievedSensor.HumidityThreshold = sensorUpdate.HumidityThreshold;
            }
            else {
                return null;
            }
            context.SaveChanges();
            return retrievedSensor.SensorId;
        }

        public void deleteSensor(int userId, int sensorId) {
            var retrievedSensor = context.Sensors.Where(x => x.SensorId == sensorId).Include(x => x.UserEntity).Include(y => y.IrregationScheduleEntitys).SingleOrDefault(e => e.UserEntityId == userId);
            if (retrievedSensor != null)
            {
                context.Remove(retrievedSensor);
                context.SaveChanges();
            }
        }

        public List<IrregationScheduleResponse> getSchedules(int userId, int sensorId) {
            List<IrregationScheduleResponse> schedules = new List<IrregationScheduleResponse>();
            foreach (IrregationScheduleEntity schedule in context.IrregationSchedules.Where(x => x.SensorEntityId == sensorId).Include(y => y.SensorEntity).Where(z => z.SensorEntity.UserEntityId == userId).ToList())
            {
                schedules.Add(schedule.asResponseModel());
            }
            return schedules;
        }

        public IrregationScheduleResponse? getScheduleById(int userId, int scheduleId) {
            var entity = context.IrregationSchedules.SingleOrDefault(e => e.IrregationScheduleId == scheduleId && e.SensorEntity.UserEntityId == userId);
            if (entity != null) {
                return entity.asResponseModel();
            }
            return null;
        }

        public int? addSchedule(int userId, int sensorId, IrregationScheduleRequest schedule) {
            var retrievedSensor = context.Sensors.Where(x => x.SensorId == sensorId).Include(x => x.UserEntity).Include(y => y.IrregationScheduleEntitys).SingleOrDefault(e => e.UserEntityId == userId);
            if (retrievedSensor != null)
            {
                var entity = new IrregationScheduleEntity()
                {
                    DateFrom = schedule.DateFrom,
                    DateTo = schedule.DateTo,
                    Time = schedule.Time,
                    Duration = schedule.Duration,
                    Activated = true,
                    SensorEntityId = sensorId,
                    SensorEntity = retrievedSensor
                };
                context.IrregationSchedules.Add(entity);
                context.SaveChanges();
                return entity.IrregationScheduleId;
            }
            return null;
        }

        public void deleteSchedule(int userId, int scheduleId) {
            var schedule = context.IrregationSchedules.SingleOrDefault(e => e.IrregationScheduleId == scheduleId && e.SensorEntity.UserEntityId == userId);
            if (schedule != null)
            {
                context.Remove(schedule);
                context.SaveChanges();
            }
        }

        public int? updateSchedule(int userId, int scheduleId, IrregationScheduleRequest scheduleUpdate) {
            var retrievedSchedule = context.IrregationSchedules.SingleOrDefault(e => e.IrregationScheduleId == scheduleId && e.SensorEntity.UserEntityId == userId);
            if (retrievedSchedule != null)
            {
                retrievedSchedule.DateFrom = scheduleUpdate.DateFrom;
                retrievedSchedule.DateTo = scheduleUpdate.DateTo;
                retrievedSchedule.Time = scheduleUpdate.Time;
                retrievedSchedule.Duration = scheduleUpdate.Duration;
                context.SaveChanges();
                return retrievedSchedule.IrregationScheduleId;
            }
            return null;
        }

        public int? changeActivationStatus(int userId, int scheduleId, bool status) {
            var retrievedSchedule = context.IrregationSchedules.SingleOrDefault(e => e.IrregationScheduleId == scheduleId && e.SensorEntity.UserEntityId == userId);
            if (retrievedSchedule != null)
            {
                retrievedSchedule.Activated = status;
                context.SaveChanges();
                return retrievedSchedule.IrregationScheduleId;
            }
            return null;
        }

        public List<UserResponse> getUsers()
        {
            List<UserResponse> users = new List<UserResponse>();
            foreach (UserEntity group in context.Users.Include(a => a.SensorEntitys).Include(b => b.SensorEntitys).ThenInclude(s => s.IrregationScheduleEntitys))
            {
                users.Add(group.asResponseModel());
            }
            return users;
        }

    }
}
