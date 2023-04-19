using irregation_api.Models.Response;

public static class GlobalSchedule
{

    static Dictionary<SensorResponse, List<IrregationScheduleResponse>> _schedules = new Dictionary<SensorResponse, List<IrregationScheduleResponse>>();


    public static Dictionary<SensorResponse, List<IrregationScheduleResponse>> Schedules { get { return _schedules; } }

    public static void setSchedule(Dictionary<SensorResponse, List<IrregationScheduleResponse>> schedule) { 
        _schedules = schedule;
    }    
}
    

    




