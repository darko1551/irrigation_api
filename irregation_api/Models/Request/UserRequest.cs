using irregation_api.Entity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Diagnostics.Tracing;

namespace irregation_api.Models
{
    public class UserRequest
    {
   
        public UserRequest( string name,string surname, string email)
        {
            Name = name;
            Surname = surname;
            Email = email;
        }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
    }
}
