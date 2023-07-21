using AutoMapper;
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
        private readonly IMapper _mapper;

        public UsuarioControllers(ILogger<UsuarioControllers> logger, SistemaVentasContext dbContext, IMapper maper) { 
            _logger = logger;
            _dbContext = dbContext;
            _mapper = maper;
        }

        [Route("UsuariosFull")]
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuariosFull() {
            _logger.LogInformation("Todas las villas se estan opteniendo");

            IEnumerable<Usuario> usuario = await _dbContext.Usuarios.ToListAsync();

            return Ok(_mapper.Map<IEnumerable<Usuario>>(usuario));
        }

        [Route("Usuarios")]
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<UsuarioDTO>>> GetUsuarios(){

            var usuarios = await _dbContext.Usuarios.ToListAsync();

            return Ok(_mapper.Map<IEnumerable<UsuarioDTO>>(usuarios));
        }

        [HttpGet("UsuarioFullID/{Id:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Usuario>> GetUsuarioFullID(int Id) { 
            if(Id == 0) {
                _logger.LogError($"Error al traer la villa con Id {Id}");
                return BadRequest();
            }

            var usuario = await _dbContext.Usuarios.FirstOrDefaultAsync(v => v.UsuarioId == Id);

            if(usuario == null) { 
                return NotFound();
            }

            return Ok(_mapper.Map<Usuario>(usuario));
        }

        [HttpGet("UsuarioId/{Id:int}", Name = "GetUsuario")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<UsuarioDTO>> GetUsuarioID(int Id) { 
            if(Id == 0) {
                return BadRequest();
            }

            var usuarioID = await _dbContext.Usuarios.FirstOrDefaultAsync(v => v.UsuarioId == Id);

            if (usuarioID == null) { 
                return NotFound();
            }

            return Ok(_mapper.Map<UsuarioDTO>(usuarioID));
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<UsuarioDTO>> CrearUsuario([FromBody] CrearUsuarioDTO usuario){

            //validar mi ModelState
            if (!ModelState.IsValid){
                return BadRequest(ModelState);
            }

            //Validaciones Personalizadas
            if (await _dbContext.Usuarios.FirstOrDefaultAsync(v => v.Nombre.ToLower() == usuario.Nombre.ToLower()) != null){
                ModelState.AddModelError("NombreExistente", "El nombre que ingreso ya existe");
                return BadRequest(ModelState);
            }

            if (usuario == null){
                return BadRequest(usuario);
            }

            Usuario modelo = _mapper.Map<Usuario>(usuario);

            //agrego mis datos
            await _dbContext.Usuarios.AddAsync(modelo);
            await _dbContext.SaveChangesAsync();

            return CreatedAtRoute("GetUsuario", new { Id = modelo.UsuarioId }, _mapper.Map<UsuarioDTO>(modelo));
        }

        [HttpDelete("EliminarUsuario/{Id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> EliminarUsuario(int Id) {
            if (Id == 0) { 
                return BadRequest();
            }

            var usuario = await _dbContext.Usuarios.FirstOrDefaultAsync(v => v.UsuarioId == Id);

            if(usuario == null) {
                return NotFound();
            }

            _dbContext.Usuarios.Remove(usuario);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("UpdateUsuario/{Id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateUsuario(int Id, [FromBody] ActualizarUsuarioDTO usuario) { 
            if(usuario == null || Id != usuario.UsuarioId) {
                return BadRequest();
            }

            if (!ModelState.IsValid) { 
                return BadRequest(ModelState);
            }

            Usuario modelo = _mapper.Map<Usuario>(usuario);

            _dbContext.Usuarios.Update(modelo);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("PatchUsuario/{Id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PatchUsuario(int Id, JsonPatchDocument<ActualizarUsuarioDTO> usuarioDTO)
        {
            if (usuarioDTO == null || Id == 0)
            {
                return BadRequest();
            }

            var usuarioId = await _dbContext.Usuarios.AsNoTracking().FirstOrDefaultAsync(v => v.UsuarioId == Id);

            if (usuarioId == null) {
                return BadRequest(ModelState);
            }

            ActualizarUsuarioDTO modelo = _mapper.Map<ActualizarUsuarioDTO>(usuarioId);

            usuarioDTO.ApplyTo(modelo, ModelState);

            if (!ModelState.IsValid) { 
                return BadRequest(ModelState);
            }

            Usuario usuarioListo = _mapper.Map<Usuario>(modelo);

            _dbContext.Usuarios.Update(usuarioListo);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}