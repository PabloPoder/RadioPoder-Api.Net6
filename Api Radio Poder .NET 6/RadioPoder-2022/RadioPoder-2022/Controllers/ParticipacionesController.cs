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

        // Get: api/<controller>/obtenerTodos
        [HttpGet("GetAll")]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                return Ok(await context.Participaciones.Include(x => x.Usuario).Include(x=>x.Sorteo).ToListAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
