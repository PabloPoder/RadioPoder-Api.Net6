using System.ComponentModel.DataAnnotations;

namespace RadioPoder_2022.Models
{
    public class Login
    {
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
