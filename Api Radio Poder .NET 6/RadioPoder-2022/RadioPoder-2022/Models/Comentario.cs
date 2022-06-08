using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RadioPoder_2022.Models
{
    public class Comentario 
    {
        public int Id { get; set; }
        [Required]
        public string Texto { get; set; }
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }
        public int UsuarioId { get; set; }
        [ForeignKey(nameof(UsuarioId))]
        public Usuario? Usuario { get; set; }
        [Required]
        public int NoticiaId { get; set; }
    }
}
