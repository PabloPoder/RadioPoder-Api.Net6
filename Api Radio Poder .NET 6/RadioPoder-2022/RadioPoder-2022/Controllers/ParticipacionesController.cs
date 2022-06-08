using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RadioPoder_2022.Models;

namespace RadioPoder_2022.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class ParticipacionesController : Controller
    {
        private readonly DataContext context;
        private readonly IConfiguration config;
        private readonly IWebHostEnvironment environment;

        public ParticipacionesController(DataContext context, IConfiguration config, IWebHostEnvironment environment)
        {
            this.context = context;
            this.config = config;
            this.environment = environment;
        }


        // GET api/<ComentariosController>/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Comentario>> Get(int id)
        {
            try
            {

                return Ok(context.Participaciones.Include(x => x.Usuario)
                                                 .Include(x => x.Sorteo)
                                                 .Where(x => x.Id == id));

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // Get: api/<controller>/obtenerTodos
        [HttpGet("GetAll")]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                var email = User.Identity.Name;

                return Ok(await context.Participaciones.Include(x => x.Usuario)
                                                        .Include(x=>x.Sorteo)
                                                        .Where(x => x.Usuario.Email == email)
                                                        .ToListAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Participacion participacion)
        {
            try
            {
                var email = User.Identity.Name;

                var usuario = await context.Usuarios.SingleOrDefaultAsync(x => x.Email == email);

                participacion.Usuario = usuario;
                participacion.UsuarioId = usuario.Id;
                participacion.Fecha = DateTime.Now;

                await context.Participaciones.AddAsync(participacion);
                context.SaveChanges();
                return CreatedAtAction(nameof(Get), new { id = participacion.Id }, participacion);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var entidad = context.Participaciones.Include(e => e.Usuario)
                                                     .Include(e => e.Sorteo)
                                                     .FirstOrDefault(e => e.Id == id && e.Usuario.Email == User.Identity.Name);
                if (entidad != null)
                {
                    context.Participaciones.Remove(entidad);
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

        /*
        // DELETE api/<controller>/5
        [HttpDelete("BajaLogica/{id}")]
        public async Task<IActionResult> BajaLogica(int id)
        {
            try
            {
                var entidad = context.Participaciones.Include(e => e.Usuario)
                                                     .Include(e => e.Sorteo)
                                                     .FirstOrDefault(e => e.Id == id && e.Usuario.Email == User.Identity.Name);
                if (entidad != null)
                {
                    entidad.Estado = false;
                    context.Participaciones.Update(entidad);
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
