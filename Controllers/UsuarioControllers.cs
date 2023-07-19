using Azure;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tienda.DBContext;
using Tienda.Modelo.DTO;

namespace Tienda.Controllers{

    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioControllers : ControllerBase{

        private readonly ILogger<UsuarioControllers> _logger;
        private readonly SistemaVentasContext _dbContext;

        public UsuarioControllers(ILogger<UsuarioControllers> logger, SistemaVentasContext dbContext) { 
            _logger = logger;
            _dbContext = dbContext;
        }

        [Route("UsuariosFull")]
        [HttpGet]
        [ProducesResponseType(200)]
        public ActionResult<IEnumerable<Usuario>> GetUsuariosFull() {
            _logger.LogInformation("Todas las villas se estan opteniendo");
            return Ok(_dbContext.Usuarios.ToList());
        }

        [Route("Usuarios")]
        [HttpGet]
        [ProducesResponseType(200)]
        public ActionResult<IEnumerable<UsuarioDTO>> GetUsuarios(){

            var usuarios = _dbContext.Usuarios.ToList();

            var usuariosDTO = usuarios.Select(usuario => new UsuarioDTO
            {
                UsuarioId = usuario.UsuarioId,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Contrasena = usuario.Contrasena
            }).ToList();

            return Ok(usuariosDTO);
        }

        [HttpGet("UsuarioFullID/{Id:int}", Name = "GetUsuario")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult<Usuario> GetUsuarioFullID(int Id) { 
            if(Id == 0) {
                _logger.LogError($"Error al traer la villa con Id {Id}");
                return BadRequest();
            }

            var usuario = _dbContext.Usuarios.FirstOrDefault(v => v.UsuarioId == Id);

            if(usuario == null) { 
                return NotFound();
            }

            return Ok(usuario);
        }

        [HttpGet("UsuarioId/{Id:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult<UsuarioDTO> GetUsuarioID(int Id) { 
            if(Id == 0) {
                return BadRequest();
            }

            var usuario = _dbContext.Usuarios.ToList();

            var usuarioDTO = usuario.Select(usuarios => new UsuarioDTO {
                UsuarioId = usuarios.UsuarioId,
                Nombre = usuarios.Nombre,
                Apellido = usuarios.Apellido,
                Contrasena = usuarios.Contrasena
            }).ToList();

            var usuarioID = usuarioDTO.FirstOrDefault(v => v.UsuarioId == Id);

            if (usuarioID == null) { 
                return NotFound();
            }

            return Ok(usuarioID);
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public ActionResult<UsuarioDTO> CrearUsuario([FromBody] UsuarioDTO usuario) {

            //validar mi ModelState
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            //Validaciones Personalizadas
            if (_dbContext.Usuarios.FirstOrDefault(v => v.Nombre.ToLower() == usuario.Nombre.ToLower()) != null) {
                ModelState.AddModelError("NombreExistente","El nombre que ingreso ya existe");
                return BadRequest(ModelState);
            }
            
            if (usuario == null) {
                return BadRequest(usuario);
            }

            if (usuario.UsuarioId > 0) {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            usuario.UsuarioId = _dbContext.Usuarios.OrderByDescending(v => v.UsuarioId).FirstOrDefault().UsuarioId + 1;
            
            //Creo mi modelo
            Usuario modelo = new(){
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Contrasena = usuario.Contrasena
            };

            //agrego mis datos
            _dbContext.Usuarios.Add(modelo);
            _dbContext.SaveChanges();

            return CreatedAtRoute("GetUsuario", new {Id = usuario.UsuarioId}, usuario);
        }

        [HttpDelete("EliminarUsuario/{Id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult EliminarUsuario(int Id) {
            if (Id == 0) { 
                return BadRequest();
            }

            var usuario = _dbContext.Usuarios.FirstOrDefault(v => v.UsuarioId == Id);

            if(usuario == null) {
                return NotFound();
            }

            _dbContext.Usuarios.Remove(usuario);
            _dbContext.SaveChanges();

            return NoContent();
        }

        [HttpPut("UpdateUsuario/{Id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult UpdateUsuario(int Id, [FromBody] UsuarioDTO usuario) { 
            if(usuario == null || Id != usuario.UsuarioId) {
                return BadRequest();
            }

            Usuario modelo = new() { 
                UsuarioId = usuario.UsuarioId,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Contrasena = usuario.Contrasena,
            };

            _dbContext.Usuarios.Update(modelo);
            _dbContext.SaveChanges();

            return NoContent();
        }

        [HttpPatch("PatchUsuario/{Id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult PatchUsuario(int Id, JsonPatchDocument<UsuarioDTO> usuarioDTO)
        {
            if (usuarioDTO == null || Id == 0)
            {
                return BadRequest();
            }

            var usuarioId = _dbContext.Usuarios.AsNoTracking().FirstOrDefault(v => v.UsuarioId == Id);

            if (usuarioId == null) {
                return BadRequest(ModelState);
            }

            UsuarioDTO modelo = new(){
                UsuarioId = usuarioId.UsuarioId,
                Nombre = usuarioId.Nombre,
                Apellido = usuarioId.Apellido,
                Contrasena = usuarioId.Contrasena
            };

            if (usuarioId == null) {
                return BadRequest();
            }

            usuarioDTO.ApplyTo(modelo, ModelState);

            if (!ModelState.IsValid) { 
                return BadRequest(ModelState);
            }

            Usuario usuarioListo = new() {
                UsuarioId = modelo.UsuarioId,
                Nombre = modelo.Nombre,
                Apellido = modelo.Apellido,
                Contrasena = modelo.Contrasena
            };

            try { 
                _dbContext.Usuarios.Update(usuarioListo);
            }catch(Exception ex) {
                return BadRequest();
            }
            _dbContext.SaveChanges();

            return NoContent();
        }
    }
}
