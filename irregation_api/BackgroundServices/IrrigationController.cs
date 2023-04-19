using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using irregation_api.Models.Json;
using irregation_api.Models.Response;
using Microsoft.Owin.Security.Provider;
using SQLitePCL;

namespace irregation_api.Socket
{
    public class IrrigationController : BackgroundService
    {

        private readonly ValveClient _valveClient;

        public IrrigationController(ValveClient valveClient)
        {
            _valveClient = valveClient;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await DoWork(stoppingToken);
        }


        private async Task DoWork(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested) {
                await Task.Delay(10000);
                foreach (KeyValuePair<SensorResponse, List<IrregationScheduleResponse>> entry in GlobalSchedule.Schedules)
                {
                    if (checkHumidity(entry.Key)) {
                        String uuid = entry.Key.Uuid;
                        List<IrregationScheduleResponse> schedules = entry.Value;
                        switch (getCommand(schedules))
                        {
                            case Command.Open:
                                await openValve(uuid);
                                break;
                            case Command.Close:
                                await closeValve(uuid);
                                break;
                            case Command.Nothing:
                                break;
                        }
                    }
                }
            }
        }

        private bool checkHumidity(SensorResponse sensorResponse) {
            if (sensorResponse.SensorData == null) {
                return true;
            }
            if (sensorResponse.SensorData.Humidity == null) {
                return true;
            }
            if (sensorResponse.SensorData!.Humidity < sensorResponse.HumidityThreshold) {
                return true;
            }
            return false;
        }

        private Command getCommand(List<IrregationScheduleResponse> schedules) {
            List<DateTime> irrigarionSchedulesOpen = new List<DateTime>();
            List<DateTime> irrigarionSchedulesClose = new List<DateTime>();
            if (schedules.Count != 0) {
                foreach (IrregationScheduleResponse schedule in schedules) {
                    if (schedule.Activated) {
                        DateOnly dateFrom = schedule.DateFrom;
                        TimeOnly time = schedule.Time;
                        DateOnly dateTo = schedule.DateTo;

                        var dateTimeFromOpen = new DateTime(dateFrom.Year, dateFrom.Month, dateFrom.Day, time.Hour, time.Minute, 0);
                        var dateTimeToOpen = new DateTime(dateTo.Year, dateTo.Month, dateTo.Day, time.Hour, time.Minute, 0);

                        var dateTimeFromClose = new DateTime(dateFrom.Year, dateFrom.Month, dateFrom.Day, time.Hour, time.Minute, 0).AddMinutes(schedule.Duration);
                        var dateTimeToClose = new DateTime(dateTo.Year, dateTo.Month, dateTo.Day, time.Hour, time.Minute, 0).AddMinutes(schedule.Duration);

                        while (dateTimeFromOpen <= dateTimeToOpen)
                        {
                            irrigarionSchedulesOpen.Add(dateTimeFromOpen);
                            irrigarionSchedulesClose.Add(dateTimeFromClose); 
                            
                            dateTimeFromOpen = dateTimeFromOpen.AddDays(1);
                            dateTimeFromClose = dateTimeFromClose.AddDays(1);
                        }
                    }
                }
                irrigarionSchedulesOpen.Sort(((a, b) => a.CompareTo(b)));
                irrigarionSchedulesClose.Sort(((a, b) => a.CompareTo(b)));

                foreach (DateTime date in irrigarionSchedulesOpen) {
                    if (date >= DateTime.Now.AddSeconds(-5) && date <= DateTime.Now.AddSeconds(5)) {
                        return Command.Open;
                    }   
                }
                foreach (DateTime date in irrigarionSchedulesClose)
                {
                    if (date >= DateTime.Now.AddSeconds(-5) && date <= DateTime.Now.AddSeconds(5))
                    {
                        return Command.Close;
                    }
                }
            }
            return Command.Nothing;
        }


        public async Task<bool> openValve(String uuid)
        {
            //ValveClient client = new ValveClient();
            bool result = _valveClient.openValve(uuid);
            if (!result)
            {
                return false;
            }
            return true;

        }

        public async Task<bool> closeValve(String uuid)
        {
           // ValveClient client = new ValveClient();
            bool result = _valveClient.closeValve(uuid);
            if (!result)
            {
                return false;
            }
            return true;
        }
    }

    
    public enum Command{
        Nothing,
        Open,
        Close
    }
}

