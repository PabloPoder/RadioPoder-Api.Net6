using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RadioPoder_2022.Models
{
    public enum enRoles
    {
        Usuario = 0,
        Administrador = 1
    }

    public class Usuario
    {
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Apellido { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
        public int Rol { get; set; }
        [NotMapped]
        public string RolNombre => Rol >= 0 ? ((enRoles)Rol).ToString() : "";

        /*[DatabaseGenerated(DatabaseGeneratedOption.Computed)]*/

    }
}
