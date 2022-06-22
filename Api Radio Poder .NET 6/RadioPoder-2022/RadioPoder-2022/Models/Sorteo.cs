using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RadioPoder_2022.Models
{
    public class Sorteo
    {
        public int Id { get; set; }
        [Required]
        public string Titulo { get; set; }
        [Required]
        public string Texto { get; set; }
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }
        [DataType(DataType.Date)]
        [Required]
        public DateTime FechaFin { get; set; }
        public string? Foto { get; set; }
        [Display(Name = "Foto")]
        [NotMapped]
        public IFormFile FotoFile { get; set; }
        public bool? Estado { get; set; }
        public int? GanadorId { get; set; }
        [ForeignKey(nameof(GanadorId))]
        public Usuario? Ganador { get; set; }
    }
}