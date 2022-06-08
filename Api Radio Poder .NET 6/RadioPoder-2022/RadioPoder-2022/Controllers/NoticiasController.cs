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
    public class NoticiasController : ControllerBase
    {

        private readonly DataContext context;
        private readonly IConfiguration config;
        private readonly IWebHostEnvironment environment;

        public NoticiasController(DataContext context, IConfiguration config, IWebHostEnvironment environment)
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
                return Ok(await context.Noticias.ToListAsync());
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
                var noticia = await context.Noticias.SingleOrDefaultAsync(item => item.Id == id);
                return noticia != null ? Ok(noticia) : NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // POST api/<controller>
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Post([FromForm] Noticia noticia)
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
                    string fileName = "foto_" + noticia.Id + Path.GetExtension(noticia.FotoFile.FileName);
                    string pathCompleto = Path.Combine(path, fileName);
                    noticia.Foto = Path.Combine("/uploads", fileName);

                    // Esta operación guarda la foto en memoria en la ruta que necesitamos
                    using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                    {
                        noticia.FotoFile.CopyTo(stream);
                    }

                    noticia.Fecha = DateTime.Now;

                    await context.Noticias.AddAsync(noticia);
                    context.SaveChanges();
                    return CreatedAtAction(nameof(Get), new { id = noticia.Id }, noticia);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


    }
}
