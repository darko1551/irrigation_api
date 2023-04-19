using irregation_api.Models.Response;
using System.ComponentModel.DataAnnotations;

namespace irregation_api.Entity
{
    public class IrregationScheduleEntity
    {
        public IrregationScheduleEntity(){}


        [Key]
        public int IrregationScheduleId { get; set; }        
        [Required]
        public DateOnly DateFrom { get; set; }
        [Required]
        public DateOnly DateTo { get; set; }
        [Required]
        public TimeOnly Time { get; set; }
        [Required]
        public double Duration { get; set; }
        [Required]
        public bool Activated { get; set; }
        public int SensorEntityId { get; set; }
        public SensorEntity SensorEntity { get; set; }


        public IrregationScheduleResponse asResponseModel()
        {
            return new IrregationScheduleResponse(this.IrregationScheduleId, this.DateFrom, this.DateTo, this.Time, this.Duration, this.Activated);
        }
    }
}
