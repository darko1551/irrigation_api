using IdentityModel;
using irregation_api.Data;
using irregation_api.Entity;
using irregation_api.Models;
using irregation_api.Models.Json;
using irregation_api.Models.Request;
using irregation_api.Models.Response;
using irregation_api.Models.Update;
using irregation_api.Socket;
using irregation_api.Static;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace irregation_api.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class SensorDataController : ControllerBase
    {
  
        private readonly ValveClient _valveClient;
        private Dao _dao;




        public SensorDataController( ValveClient valveClient, Dao dao)
        {
            _valveClient = valveClient;
            _dao = dao;
            refreshGlobalSchedule();
        }

        [HttpGet("sensors/{userId}")]
        public ActionResult<List<SensorResponse>> GetSensors(int userId)
        {
            if (_dao.getUser(userId) == null) {
                return BadRequest(ExceptionStrings.userDoesNotExist);
            }
            List<SensorResponse> sensors = new List<SensorResponse>();
            sensors = _dao.getSensors(userId);
            return sensors;
        }

        
        [HttpGet("sensors/{userId}/{mac}")]
        public ActionResult<SensorResponse> GetSensorByMac(int userId ,String mac)
         {
            if (_dao.getUser(userId) == null)
            {
                return BadRequest(ExceptionStrings.userDoesNotExist);
            }
            SensorResponse? retrievedSensor = _dao.getSensorByMac(userId, mac);
            if (retrievedSensor != null) {
                return retrievedSensor;
            }
            return BadRequest(ExceptionStrings.sensorDoesNotExist);
        }
        
        
        [HttpPost("sensors")]
        public async Task<ActionResult<int>> AddSensor([FromBody] SensorRequest sensor)
        {
            if (_dao.getUser(sensor.UserId) == null)
            {
                return BadRequest(ExceptionStrings.userDoesNotExist);
            }

            var retrievedSensorMac = _dao.getSensorByMacAllUsers(sensor.Mac);
            if (retrievedSensorMac != null)
            {
                return BadRequest(ExceptionStrings.sensorMacAlreadyExists);
            }

            var retrievedSensorName = _dao.getSensorByName(sensor.UserId, sensor.Name);
            if(retrievedSensorName != null){
                return BadRequest(ExceptionStrings.sensorNameAlreadyExists);
            }

            string? uuid = await _valveClient.getUuid(sensor.Mac);
            if (uuid == null)
            {
                return BadRequest(ExceptionStrings.macNotValid);
            }

            var newSensorId = _dao.addSensor(sensor, uuid);
            if (newSensorId != null) {
                refreshGlobalUuid();
                return Ok(newSensorId);
            }
            return BadRequest(ExceptionStrings.somethingWentWrong);
            
        }
        
        
        
       [HttpPut("sensors/{userId}/{sensorId}")]
        public ActionResult<int> UpdateSensor(int userId, int sensorId, [FromBody] SensorUpdate sensorUpdate)
        {
            if (_dao.getUser(userId) == null)
            {
                return BadRequest(ExceptionStrings.userDoesNotExist);
            }

            var retrievedSensor = _dao.getSensorById(userId, sensorId);
            if (retrievedSensor == null)
            {
                return BadRequest(ExceptionStrings.sensorDoesNotExist);
            }


            var retrievedSensorName = _dao.getSensorByName(userId, sensorUpdate.Name);
            if (retrievedSensorName != null && retrievedSensorName.SensorId != sensorId)
            {
                return BadRequest(ExceptionStrings.sensorNameAlreadyExists);
            }

            var updatedSensorId = _dao.updateSensor(userId, sensorId, sensorUpdate);
            return Ok(updatedSensorId);
        }


        [HttpDelete("sensors/{userId}/{sensorId}")]
        public ActionResult<IHttpActivityFeature> DeleteSensor(int userId, int sensorId)
        {
            if (_dao.getUser(userId) == null)
            {
                return BadRequest(ExceptionStrings.userDoesNotExist);
            }

            var retrievedSensor = _dao.getSensorById(userId, sensorId);
            if (retrievedSensor == null)
            {
                return BadRequest(ExceptionStrings.sensorDoesNotExist);
            }

            _dao.deleteSensor(userId, sensorId);
            refreshGlobalUuid();
            return Ok();
        }



       
        [HttpGet("schedules/{userId}/{sensorId}")]
         public ActionResult<List<IrregationScheduleResponse>> GetSchedules(int userId, int sensorId)
         {
             List<IrregationScheduleResponse> schedules = new List<IrregationScheduleResponse>();

            if (_dao.getUser(userId) == null)
            {
                return BadRequest(ExceptionStrings.userDoesNotExist);
            }

            var retrievedSensor = _dao.getSensorById(userId, sensorId);
            if (retrievedSensor == null)
            {
                return BadRequest(ExceptionStrings.sensorDoesNotExist);
            }

            schedules = _dao.getSchedules(userId, sensorId);
            return schedules;
         }




        [HttpPost("schedules/{userId}/{sensorId}")]
        public ActionResult<int> AddSchedule(int userId, int sensorId, [FromBody] IrregationScheduleRequest schedule)
        {
            if (_dao.getUser(userId) == null)
            {
                return BadRequest(ExceptionStrings.userDoesNotExist);
            }

            var retrievedSensor = _dao.getSensorById(userId, sensorId);
            if (retrievedSensor == null)
            {
                return BadRequest(ExceptionStrings.sensorDoesNotExist);
            }

            List<IrregationScheduleResponse> scheduleList = _dao.getSchedules(userId, sensorId);

            if (checkScheduleOverlapp(scheduleList, schedule)) {
                return BadRequest(ExceptionStrings.scheduleOverlap);
            }

            int? addedScheduleId = _dao.addSchedule(userId, sensorId, schedule);

            if (addedScheduleId != null) {
                refreshGlobalSchedule();
                return Ok(addedScheduleId);
            }

            return BadRequest(ExceptionStrings.somethingWentWrong);
        }



        [HttpDelete("schedules/{userId}/{scheduleId}")]
         public ActionResult<IHttpActivityFeature> DeleteSchedule(int userId, int scheduleId)
         {
            if (_dao.getUser(userId) == null)
            {
                return BadRequest(ExceptionStrings.userDoesNotExist);
            }
            var retrievedSchedule = _dao.getScheduleById(userId, scheduleId);
            if (retrievedSchedule == null)
            {
                return BadRequest(ExceptionStrings.scheduleDoesNotExist);
            }

            _dao.deleteSchedule(userId, scheduleId);
            refreshGlobalSchedule();
            return Ok();
         }


        [HttpPut("schedules/{userId}/{scheduleId}")]
          public ActionResult<int> UpdateSchedule(int userId,int sensorId, int scheduleId, [FromBody] IrregationScheduleRequest scheduleUpdate)
          {
            if (_dao.getUser(userId) == null)
            {
                return BadRequest(ExceptionStrings.userDoesNotExist);
            }

            var retrievedSchedule = _dao.getScheduleById(userId, scheduleId);
              if (retrievedSchedule == null)
              {
                  return BadRequest(ExceptionStrings.scheduleDoesNotExist);
              }


            List<IrregationScheduleResponse> scheduleList = _dao.getSchedules(userId, sensorId);
            scheduleList.RemoveAll(e => e.Id == scheduleId);

            if (checkScheduleOverlapp(scheduleList, scheduleUpdate))
            {
                return BadRequest(ExceptionStrings.scheduleOverlap);
            }

            int? scheduleUpdateId = _dao.updateSchedule(userId, scheduleId, scheduleUpdate);

            if (scheduleUpdateId != null) {
                refreshGlobalSchedule();
                return Ok(scheduleUpdateId);
            }
            return BadRequest(ExceptionStrings.somethingWentWrong);
          }




        [HttpPut("schedules/activation/{userId}/{scheduleId}")]
        public ActionResult<int> ActivationActivationUpdate(int userId,int scheduleId, [FromBody] bool status)
        {
            if (_dao.getUser(userId) == null)
            {
                return BadRequest(ExceptionStrings.userDoesNotExist);
            }

            var retrievedSchedule = _dao.getScheduleById(userId, scheduleId);
            if (retrievedSchedule == null)
            {
                return BadRequest(ExceptionStrings.scheduleDoesNotExist);
            }

            int? scheduleUpdateId = _dao.changeActivationStatus(userId, scheduleId, status);
            if (scheduleUpdateId != null) {
                refreshGlobalSchedule();
                return Ok(scheduleUpdateId);
            }
            return BadRequest(ExceptionStrings.somethingWentWrong);
            
        }

        [HttpGet("users")]
         public ActionResult<List<UserResponse>> GetUsers()
         {
            List<UserResponse> users = _dao.getUsers();
            return users;
         }



        [HttpPost("valve/open/{uuid}")]
        public ActionResult<IHttpActivityFeature> OpenValve(string uuid)
        {
            //ValveClient client = new ValveClient();
            bool result = _valveClient.openValve(uuid);
            if (!result) { 
                return BadRequest(ExceptionStrings.somethingWentWrong);
            }
            return Ok();
        }

        [HttpPost("valve/close/{uuid}")]
        public ActionResult<IHttpActivityFeature> CloseValve(string uuid)
        {
            //ValveClient client = new ValveClient();
            bool result = _valveClient.closeValve(uuid);
            if (!result)
            {
                return BadRequest(ExceptionStrings.somethingWentWrong);
            }
            return Ok();
        }


        private void refreshGlobalSchedule() {
            Dictionary<SensorResponse, List<IrregationScheduleResponse>> schedules = new Dictionary<SensorResponse, List<IrregationScheduleResponse>>();
            foreach (SensorResponse sensor in _dao.getSensorsAllUsers())
            {
                if (sensor.IrregationSchedules != null) {
                    schedules.Add(sensor, sensor.IrregationSchedules.ToList());
                }
            }
            GlobalSchedule.setSchedule(schedules);
        }

        private void refreshGlobalUuid()
        {
            List<String> deviceUuids = new List<String>();
            foreach (SensorResponse sensor in _dao.getSensorsAllUsers())
            {
                if (sensor.Uuid != null) {
                    deviceUuids.Add(sensor.Uuid);
                }
            }
            GlobalUuid.setList(deviceUuids);    
        }



        private bool checkScheduleOverlapp(List<IrregationScheduleResponse> scheduleList, IrregationScheduleRequest newSchedule) {
            // Check for overlap for each day between DateFrom and DateTo
            for (DateTime date = newSchedule.DateFrom.ToDateTime(newSchedule.Time); date <= newSchedule.DateTo.ToDateTime(newSchedule.Time); date = date.AddDays(1))
            {
                var startTime = newSchedule.Time;
                var endTime = startTime.Add(TimeSpan.FromMinutes(newSchedule.Duration));

                foreach (var schedule in scheduleList)
                {
                    // Convert the DateOnly instances to DateTime instances for comparison
                    var scheduleStartDateTime = schedule.DateFrom.ToDateTime(schedule.Time);
                    var scheduleEndDateTime = schedule.DateTo.ToDateTime(schedule.Time);

                    if (scheduleStartDateTime <= date && scheduleEndDateTime >= date)
                    {
                        // If the current schedule overlaps with the current day, check for time overlap
                        var currStartTime = schedule.Time;
                        var currEndTime = currStartTime.Add(TimeSpan.FromMinutes(schedule.Duration));

                        if (startTime < currEndTime && currStartTime < endTime)
                        {
                            // The schedules overlap
                            return true;
                        }
                    }
                }
            }

            // The new schedule does not overlap with any existing schedules
            return false;

        }
    }
}
