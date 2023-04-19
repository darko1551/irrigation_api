using irregation_api.Models.Response;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace irregation_api.Entity
{
    public class UserEntity
    {
        public UserEntity()
        {
        }

        public UserEntity(int userId, string name, string surname, string email, IEnumerable<SensorEntity> sensorEntitys)
        {
            UserId = userId;
            Name = name;
            Surname = surname;
            Email = email;
            SensorEntitys = sensorEntitys;
        }

        [Key]
        public int UserId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public string Email { get; set; }
        public virtual IEnumerable<SensorEntity> SensorEntitys { get; set; }

       
        
        
        public UserResponse asResponseModel()
        {
            return new UserResponse(this.UserId, this.Name,this.Surname,this.Email,  this.SensorEntitys != null? this.SensorEntitys.Select(e => e.asNoUserResponseModel()).ToList() :new List<SensorNoUserResponse>());
        }
    }
}
