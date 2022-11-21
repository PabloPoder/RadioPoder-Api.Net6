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
    public class ComentariosController : Controller
    {
        private readonly DataContext context;
        private readonly IConfiguration config;
        private readonly IWebHostEnvironment environment;

        public ComentariosController(DataContext context, IConfiguration config, IWebHostEnvironment environment)
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
                return Ok(context.Comentarios.Include(x => x.Usuario)
                                             .Where(x => x.NoticiaId == id && x.Estado == true));

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // POST api/<controller>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Comentario comentario)
        {
            try
            {
                var email = User.Identity.Name;

                var usuario = await context.Usuarios.SingleOrDefaultAsync(x => x.Email == email);

                comentario.Usuario = usuario;
                comentario.UsuarioId = usuario.Id;
                comentario.Estado = true;
                comentario.Fecha = DateTime.Now;

                await context.Comentarios.AddAsync(comentario);
                context.SaveChanges();
                return CreatedAtAction(nameof(Get), new { id = comentario.Id }, comentario);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        /*
        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var entidad = context.Comentarios.Include(e => e.Usuario)
                                                 .FirstOrDefault(e => e.Id == id && e.Usuario.Email == User.Identity.Name);
                if (entidad != null)
                {
                    context.Comentarios.Remove(entidad);
                    context.SaveChanges();
                    return Ok();
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }*/

        
        // DELETE api/<controller>/5
        [HttpDelete("BajaLogica/{id}")]
        public async Task<IActionResult> BajaLogica(int id)
        {
            try
            {
                var entidad = context.Comentarios.Include(e => e.Usuario).FirstOrDefault(e => e.Id == id && e.Usuario.Email == User.Identity.Name);
                if (entidad != null)
                {
                    entidad.Estado = false;
                    context.Comentarios.Update(entidad);
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
        

    }
}
