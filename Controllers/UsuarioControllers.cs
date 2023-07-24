using AutoMapper;
using Azure;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Tienda.DBContext;
using Tienda.Modelo;
using Tienda.Modelo.DTO;
using Tienda.Repositorio.IRepositorio;

namespace Tienda.Controllers{

    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioControllers : ControllerBase{

        private readonly ILogger<UsuarioControllers> _logger;
        private readonly IUsuarioRepositorio _usuarioRepo;
        private readonly IMapper _mapper;
        protected APIResponse _response;

        public UsuarioControllers(ILogger<UsuarioControllers> logger, IUsuarioRepositorio usuarioRepositorio, IMapper maper){
            _logger = logger;
            _usuarioRepo = usuarioRepositorio;
            _mapper = maper;
            _response = new();
        }

        [Route("UsuariosFull")]
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<APIResponse>> GetUsuariosFull()
        {
            try{
                _logger.LogInformation("Todas las villas se estan opteniendo");

                IEnumerable<Usuario> usuario = await _usuarioRepo.ObtenerTodos();

                _response.Resultado = _mapper.Map<IEnumerable<Usuario>>(usuario);
                _response.statusCode = HttpStatusCode.OK;

                return Ok(_response);
            } catch (Exception ex){
                _response.isExitoso = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }

            return _response;
        }

        [Route("Usuarios")]
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<APIResponse>> GetUsuarios(){
            try{
                var usuarios = await _usuarioRepo.ObtenerTodos();

                _response.Resultado = _mapper.Map<IEnumerable<UsuarioDTO>>(usuarios);
                _response.statusCode = HttpStatusCode.OK;

                return Ok(_response);
            } catch (Exception ex){
                _response.isExitoso = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }

            return _response;
        }

        [HttpGet("UsuarioFullID/{Id:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<APIResponse>> GetUsuarioFullID(int Id){
            try {
                if (Id == 0){
                    _logger.LogError($"Error al traer la villa con Id {Id}");
                    _response.isExitoso = false;
                    _response.statusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var usuario = await _usuarioRepo.Obtener(v => v.UsuarioId == Id);

                if (usuario == null){
                    _response.isExitoso = false;
                    _response.statusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                _response.Resultado = _mapper.Map<Usuario>(usuario);
                _response.statusCode = HttpStatusCode.OK;

                return Ok(_response);
            } catch (Exception ex) { 
                _response.isExitoso = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _response;
        }

        [HttpGet("UsuarioId/{Id:int}", Name = "GetUsuario")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<APIResponse>> GetUsuarioID(int Id){
            try{
                if (Id == 0){
                    _response.isExitoso = false;
                    _response.statusCode = HttpStatusCode.BadRequest;
                    return _response;
                }

                var usuarioID = await _usuarioRepo.Obtener(v => v.UsuarioId == Id);

                if (usuarioID == null){
                    _response.isExitoso = false;
                    _response.statusCode = HttpStatusCode.NotFound;
                    return _response;
                }

                _response.Resultado = _mapper.Map<UsuarioDTO>(usuarioID);
                _response.statusCode = HttpStatusCode.OK;

                return Ok(_response);
            }
            catch (Exception ex) { 
                _response.isExitoso = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return _response;
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<APIResponse>> CrearUsuario([FromBody] CrearUsuarioDTO usuario){

            try {
                //validar mi ModelState
                if (!ModelState.IsValid){
                    return BadRequest(ModelState);
                }

                //Validaciones Personalizadas
                if (await _usuarioRepo.Obtener(v => v.Nombre.ToLower() == usuario.Nombre.ToLower()) != null)
                {
                    ModelState.AddModelError("NombreExistente", "El nombre que ingreso ya existe");
                    return BadRequest(ModelState);
                }

                if (usuario == null){
                    return BadRequest(usuario);
                }

                Usuario modelo = _mapper.Map<Usuario>(usuario);

                //agrego mis datos
                await _usuarioRepo.Crear(modelo);

                _response.Resultado = _mapper.Map<UsuarioDTO>(modelo);
                _response.statusCode = HttpStatusCode.Created;

                return CreatedAtRoute("GetUsuario", new { Id = modelo.UsuarioId }, _response);
            } catch (Exception ex) {
                _response.isExitoso = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return _response;
        }

        [HttpDelete("EliminarUsuario/{Id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> EliminarUsuario(int Id){

            try{
                if (Id == 0){
                    _response.isExitoso = false;
                    _response.statusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response); 
                }

                var usuario = await _usuarioRepo.Obtener(v => v.UsuarioId == Id);

                if (usuario == null){
                    _response.isExitoso = false;
                    _response.statusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                await _usuarioRepo.Remover(usuario);

                _response.statusCode = HttpStatusCode.NoContent;
                return Ok(_response);
            } catch (Exception ex){
                _response.isExitoso = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return BadRequest(_response);
        }

        [HttpPut("UpdateUsuario/{Id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateUsuario(int Id, [FromBody] ActualizarUsuarioDTO usuario){
            try{
                if (usuario == null || Id != usuario.UsuarioId){
                    _response.isExitoso = false;
                    _response.statusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                if (!ModelState.IsValid){
                    return BadRequest(ModelState);
                }

                Usuario modelo = _mapper.Map<Usuario>(usuario);

                await _usuarioRepo.Actualizar(modelo);

                _response.statusCode = HttpStatusCode.NoContent;

                return Ok(_response);
            } catch (Exception ex){
                _response.isExitoso = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return BadRequest(_response);
        }

        [HttpPatch("PatchUsuario/{Id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PatchUsuario(int Id, JsonPatchDocument<ActualizarUsuarioDTO> usuarioDTO){
            try{
                if (usuarioDTO == null || Id == 0){
                    _response.isExitoso = false;
                    _response.statusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var usuarioId = await _usuarioRepo.Obtener(v => v.UsuarioId == Id, tracked: false);

                if (usuarioId == null){
                    return BadRequest(ModelState);
                }

                ActualizarUsuarioDTO modelo = _mapper.Map<ActualizarUsuarioDTO>(usuarioId);

                usuarioDTO.ApplyTo(modelo, ModelState);

                if (!ModelState.IsValid){
                    return BadRequest(ModelState);
                }

                Usuario usuarioListo = _mapper.Map<Usuario>(modelo);

                await _usuarioRepo.Actualizar(usuarioListo);

                _response.statusCode = HttpStatusCode.NoContent;

                return Ok(_response);
            } catch (Exception ex){
                _response.isExitoso = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return BadRequest(_response);
        }

            /*private readonly ILogger<UsuarioControllers> _logger;
            private readonly SistemaVentasContext _usuarioRepo;
            private readonly IMapper _mapper;

            public UsuarioControllers(ILogger<UsuarioControllers> logger, SistemaVentasContext dbContext, IMapper maper) { 
                _logger = logger;
                _usuarioRepo = dbContext;
                _mapper = maper;
            }

            [Route("UsuariosFull")]
            [HttpGet]
            [ProducesResponseType(200)]
            public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuariosFull() {
                _logger.LogInformation("Todas las villas se estan opteniendo");

                IEnumerable<Usuario> usuario = await _usuarioRepo.Usuarios.ToListAsync();

                return Ok(_mapper.Map<IEnumerable<Usuario>>(usuario));
            }

            [Route("Usuarios")]
            [HttpGet]
            [ProducesResponseType(200)]
            public async Task<ActionResult<IEnumerable<UsuarioDTO>>> GetUsuarios(){

                var usuarios = await _usuarioRepo.Usuarios.ToListAsync();

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

                var usuario = await _usuarioRepo.Usuarios.FirstOrDefaultAsync(v => v.UsuarioId == Id);

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

                var usuarioID = await _usuarioRepo.Usuarios.FirstOrDefaultAsync(v => v.UsuarioId == Id);

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
                if (await _usuarioRepo.Usuarios.FirstOrDefaultAsync(v => v.Nombre.ToLower() == usuario.Nombre.ToLower()) != null){
                    ModelState.AddModelError("NombreExistente", "El nombre que ingreso ya existe");
                    return BadRequest(ModelState);
                }

                if (usuario == null){
                    return BadRequest(usuario);
                }

                Usuario modelo = _mapper.Map<Usuario>(usuario);

                //agrego mis datos
                await _usuarioRepo.Usuarios.AddAsync(modelo);
                await _usuarioRepo.SaveChangesAsync();

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

                var usuario = await _usuarioRepo.Usuarios.FirstOrDefaultAsync(v => v.UsuarioId == Id);

                if(usuario == null) {
                    return NotFound();
                }

                _usuarioRepo.Usuarios.Remove(usuario);
                await _usuarioRepo.SaveChangesAsync();

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

                _usuarioRepo.Usuarios.Update(modelo);
                await _usuarioRepo.SaveChangesAsync();

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

                var usuarioId = await _usuarioRepo.Usuarios.AsNoTracking().FirstOrDefaultAsync(v => v.UsuarioId == Id);

                if (usuarioId == null) {
                    return BadRequest(ModelState);
                }

                ActualizarUsuarioDTO modelo = _mapper.Map<ActualizarUsuarioDTO>(usuarioId);

                usuarioDTO.ApplyTo(modelo, ModelState);

                if (!ModelState.IsValid) { 
                    return BadRequest(ModelState);
                }

                Usuario usuarioListo = _mapper.Map<Usuario>(modelo);

                _usuarioRepo.Usuarios.Update(usuarioListo);
                await _usuarioRepo.SaveChangesAsync();

                return NoContent();
            }*/
        }
}