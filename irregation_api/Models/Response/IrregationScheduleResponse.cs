namespace irregation_api.Models.Response
{
    public class IrregationScheduleResponse
    {
        public IrregationScheduleResponse(int id, DateOnly dateFrom, DateOnly dateTo, TimeOnly time, double duration, bool activated)
        {
            Id = id;
            DateFrom = dateFrom;
            DateTo = dateTo;
            Time = time;
            Duration = duration;
            Activated = activated;
        }

        public int Id { get; set; }
        public DateOnly DateFrom { get; set; }
        public DateOnly DateTo { get; set; }
        public TimeOnly Time { get; set; }
        public double Duration { get; set; }
        public bool Activated { get; set; }

    }
}
