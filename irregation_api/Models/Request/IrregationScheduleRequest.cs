namespace irregation_api.Models.Response
{
    public class IrregationScheduleRequest
    {
        public IrregationScheduleRequest(DateOnly dateFrom, DateOnly dateTo, TimeOnly time, double duration, bool activated)
        {
            
            DateFrom = dateFrom;
            DateTo = dateTo;
            Time = time;
            Duration = duration;
            Activated = activated;
        }

       
        public DateOnly DateFrom { get; set; }
        public DateOnly DateTo { get; set; }
        public TimeOnly Time { get; set; }
        public double Duration { get; set; }
        public bool Activated { get; set; }

    }
}
