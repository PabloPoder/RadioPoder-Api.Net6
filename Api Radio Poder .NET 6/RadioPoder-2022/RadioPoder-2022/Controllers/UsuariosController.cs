using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RadioPoder_2022.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RadioPoder_2022.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class UsuariosController : ControllerBase
    {

        private readonly DataContext context;
        private readonly IConfiguration config;

        public UsuariosController(DataContext context, IConfiguration config)
        {
            this.context = context;
            this.config = config;
        }


        // Get: api/<controller>/obtenerTodos
        [HttpGet("GetAll")]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                return Ok(await context.Usuarios.ToListAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET api/<UsuariosController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> Get(int id)
        {
            try
            {
                var usuario = await context.Usuarios.SingleOrDefaultAsync(item => item.Id == id);
                return usuario != null ? Ok(usuario) : NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET api/<UsuariosController>/5
        [HttpGet("GetPorEmail{email}")]
        [AllowAnonymous]
        public async Task<ActionResult<Usuario>> GetPorEmail(string email)
        {
            try
            {
                var usuario = await context.Usuarios.SingleOrDefaultAsync(item => item.Email == email);
                return usuario != null ? Ok() : NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        [HttpGet("UsuarioLogeado")]
        public async Task<ActionResult> UsuarioLogeado()
        {
            try
            {
                var email = User.Identity.Name;
                var usuario = await context.Usuarios.SingleOrDefaultAsync(x => x.Email == email);

                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut("EditarUsuario/{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] Usuario usuario)
        {
            // No permitir editar email para no modificar el claim de email
            try
            {
                var u = context.Usuarios.AsNoTracking().SingleOrDefault(x => x.Id == id && x.Email == User.Identity.Name);
                if (ModelState.IsValid && u != null)
                {
                    

                    string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: usuario.Password,
                        salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));

                    u.Password = hashed;
                    u.Nombre = usuario.Nombre;
                    u.Apellido = usuario.Apellido;

                    context.Usuarios.Update(u);
                    await context.SaveChangesAsync();
                    return Ok(usuario);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut("EditarUsuarioGoogle/{id}")]
        public async Task<ActionResult> EditarUsuarioGoogle (int id, [FromBody] Usuario usuario)
        {
            // No permitir editar email para no modificar el claim de email
            try
            {
                var u = context.Usuarios.AsNoTracking().SingleOrDefault(x => x.Id == id && x.Email == User.Identity.Name);
                if (ModelState.IsValid && u != null)
                {

                    u.Nombre = usuario.Nombre;
                    u.Apellido = usuario.Apellido;

                    context.Usuarios.Update(u);
                    await context.SaveChangesAsync();
                    return Ok(usuario);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        /*
        [HttpPatch("EditarUsuario/{id}")]
        public async Task<IActionResult> Patch(int id, [FromBody] string newNombre, string newApellido, string newPassword)
        {
            try
            {
                var usuario = context.Usuarios.FirstOrDefault(i => i.Id == id);
                if (usuario != null)
                {
                    usuario.Nombre = newNombre;
                    usuario.Apellido = newApellido;

                    string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                       password: newPassword,
                       salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
                       prf: KeyDerivationPrf.HMACSHA1,
                       iterationCount: 1000,
                       numBytesRequested: 256 / 8));

                    usuario.Password = hashed;

                    context.Usuarios.Update(usuario);
                    context.SaveChanges();
                    return Ok();
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        } */


        // POST api/<controller>/login
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] Login loginView)
        {
            try
            {
                // Hasheo de clave
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: loginView.Password,
                    salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 1000,
                    numBytesRequested: 256 / 8));

                // Retorno el usuario que tenga el mismo email que ingreso
                var usuario = await context.Usuarios.FirstOrDefaultAsync(x => x.Email == loginView.Email);

                // Compruebo que la clave sea la misma y que el usuario no sea null
                if (usuario == null || usuario.Password != hashed)
                {
                    return BadRequest("Email y/o clave incorrecta!");
                }
                else
                {
                    // Creacion de claims y token
                    var key = new SymmetricSecurityKey(
                        System.Text.Encoding.ASCII.GetBytes(config["TokenAuthentication:SecretKey"]));
                    var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, usuario.Email),
                        new Claim("FullName", usuario.Nombre + " " + usuario.Apellido),
                        new Claim(ClaimTypes.Role, usuario.RolNombre),
                    };

                    var token = new JwtSecurityToken(
                        issuer: config["TokenAuthentication:Issuer"],
                        audience: config["TokenAuthentication:Audience"],
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(60),
                        signingCredentials: credenciales
                    );
                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // POST api/<controller>
        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Post([FromBody] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                var usuarioExistente = await context.Usuarios.SingleOrDefaultAsync(item => item.Email == usuario.Email);

                if (usuarioExistente == null)
                {
                    // Hasheo de clave
                    string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: usuario.Password,
                        salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));

                    usuario.Password = hashed;
                    usuario.Rol = 0;

                    await context.Usuarios.AddAsync(usuario);
                    context.SaveChanges();
                    return CreatedAtAction(nameof(Get), new { id = usuario.Id }, usuario);

                }
                else { return BadRequest("Email ya existente!");
                }
            }
            else {
                return BadRequest();
            }
        }

    }
}

