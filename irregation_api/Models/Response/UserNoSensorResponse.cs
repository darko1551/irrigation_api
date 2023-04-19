using irregation_api.Entity;

namespace irregation_api.Models.Response
{
    public class UserNoSensorsResponse
    {
        public UserNoSensorsResponse()
        {

        }

        public UserNoSensorsResponse(int userId, string name, string surname, string email)
        {
            UserId = userId;
            Name = name;
            Surname = surname;
            Email = email;
        }

        public int UserId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
    }
}
