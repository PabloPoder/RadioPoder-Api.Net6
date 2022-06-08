using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RadioPoder_2022.Models
{
    public class Participacion
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public int UsuarioId { get; set; }
        [ForeignKey(nameof(UsuarioId))]
        public Usuario? Usuario { get; set; }
        public int SorteoId { get; set; }
        [ForeignKey(nameof(SorteoId))]
        public Sorteo? Sorteo { get; set; }
    }
}
