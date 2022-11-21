using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RadioPoder_2022.Models
{
    public class Noticia
    {
        public int Id { get; set; }
        [Required]
        public string Titulo { get; set; }
        [Required]
        public string Texto { get; set; }
        [DataType(DataType.Date)]
        public DateTime? Fecha { get; set; }
        public string? Foto { get; set; }
        [Display(Name = "Foto")]
        [NotMapped]
        public IFormFile FotoFile { get; set; }
        [Required]
        public string Autor { get; set; }
        public int? Tiempo { get; set; }
    }
}
