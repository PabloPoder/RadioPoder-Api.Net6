using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RadioPoder_2022.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RadioPoder_2022.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class SorteosController : ControllerBase
    {

        private readonly DataContext context;
        private readonly IConfiguration config;
        private readonly IWebHostEnvironment environment;

        public SorteosController(DataContext context, IConfiguration config, IWebHostEnvironment environment)
        {
            this.context = context;
            this.config = config;
            this.environment = environment;
        }

        // Get: api/<controller>/obtenerTodos
        [HttpGet("GetAll")]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                return Ok(await context.Sorteos.ToListAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET api/<NoticiasController>/5
        [HttpGet("GetGanador/{id}")]
        public async Task<ActionResult> GetGanador(int id)
        {
            try
            {
                Sorteo sorteo = await context.Sorteos.SingleOrDefaultAsync(item => item.Id == id);
                if(sorteo == null) return NotFound();
                if (sorteo.GanadorId != null) return BadRequest("Ya hay un ganador");

                List<Participacion> participaciones = context.Participaciones.Where(x => x.SorteoId == sorteo.Id).ToList();

                Random R = new Random();
                int numeroRandom = R.Next(0, participaciones.Count());


                var ganadorId = participaciones.ElementAt(numeroRandom).UsuarioId;

                Usuario ganador = await context.Usuarios.SingleOrDefaultAsync(item => item.Id == ganadorId);

                sorteo.GanadorId = ganadorId;
                // sorteo.Estado = false;

                context.Sorteos.Update(sorteo);
                await context.SaveChangesAsync();
                return Ok(sorteo);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET api/<NoticiasController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Noticia>> Get(int id)
        {
            try
            {
                var sorteo = await context.Sorteos.SingleOrDefaultAsync(item => item.Id == id);
                return sorteo != null ? Ok(sorteo) : NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // POST api/<controller>
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] Sorteo sorteo)
        {
            try
            {

                string wwwPath = environment.WebRootPath;
                string path = Path.Combine(wwwPath, "uploads");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                //Path.GetFileName(u.AvatarFile.FileName);//este nombre se puede repetir
                string fileName = "foto_sorteos_" + sorteo.Titulo + Path.GetExtension(sorteo.FotoFile.FileName);
                string pathCompleto = Path.Combine(path, fileName);
                sorteo.Foto = Path.Combine("/uploads", fileName);

                // Esta operación guarda la foto en memoria en la ruta que necesitamos
                using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                {
                    sorteo.FotoFile.CopyTo(stream);
                }

                sorteo.FechaInicio = DateTime.Now;

                await context.Sorteos.AddAsync(sorteo);
                context.SaveChanges();
                return CreatedAtAction(nameof(Get), new { id = sorteo.Id }, sorteo);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        /*
        // DELETE api/<controller>/5
        [HttpDelete("BajaLogica/{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> BajaLogica(int id)
        {
            try
            {
                var entidad = context.Sorteos.FirstOrDefault(e => e.Id == id);
                if (entidad != null)
                {
                    context.Sorteos.Update(entidad);
                    context.SaveChanges();
                    return Ok();
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        */
    }
}
