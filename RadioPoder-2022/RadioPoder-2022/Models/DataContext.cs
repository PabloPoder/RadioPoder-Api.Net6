using Microsoft.EntityFrameworkCore;

namespace RadioPoder_2022.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Noticia> Noticias{ get; set; }
        public DbSet<Comentario> Comentarios { get; set; }
        public DbSet<Sorteo> Sorteos { get; set; }
        public DbSet<Participacion> Participaciones { get; set; }

    }
}
