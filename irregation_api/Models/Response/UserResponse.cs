using irregation_api.Entity;

namespace irregation_api.Models.Response
{
    public class UserResponse
    {
        public UserResponse(int userid, string name, string surname, string email, IEnumerable<SensorNoUserResponse> sensor)
        {
            UserId = userid;
            Name = name;
            Surname = surname;
            Email = email;
            Sensors = sensor;
        }

        public int UserId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public virtual IEnumerable<SensorNoUserResponse> Sensors { get; set; }
    }
}
