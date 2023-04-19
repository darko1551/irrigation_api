using irregation_api.Models.Response;

namespace irregation_api.Models.Update
{
    public class IrrigationActiveUpdate
    {
        public IrrigationActiveUpdate(bool status)
        {
            Status = status;
 
        }

        public bool Status{ get; set; }

    }
}
