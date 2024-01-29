using AutoMapper;
using MagicVilla.Modelos;
using MagicVilla.Modelos.Dto;
using MagicVilla.Modelos.Especificaciones;
using MagicVilla.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVilla.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class VillaController : ControllerBase
    {
        /*[HttpGet]
        public IEnumerable<VillaDto> GetVillas()
        {
            return new List<VillaDto>
            {
                new VillaDto {Id=1, Nombre="Vista a la Piscina"},
                new VillaDto {Id=2, Nombre="Vista a la Playa"},
            };
        }*/

        private readonly ILogger<VillaController> _logger;
        //private readonly ApplicationDbContext _db;
        private readonly IVillaRepositorio _villaRepo;
        private readonly IMapper _mapper;
        protected APIResponse _response;

        public VillaController(ILogger<VillaController> logger, IVillaRepositorio villaRepo, IMapper mapper)
        {
            _logger = logger;
            //_db = db;
            _villaRepo = villaRepo;
            _mapper = mapper;
            _response = new();
        }

        [HttpGet]
        [ResponseCache(CacheProfileName = "Default30")]
        [Authorize(Roles = "admin", AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        //public async Task<ActionResult<IEnumerable<VillaDto>>> GetVillas()
        public async Task<ActionResult<APIResponse>> GetVillas()
        {
            try
            {
                _logger.LogInformation("Obtener las villas");
                IEnumerable<Villa> villaList = await _villaRepo.ObtenerTodos();
                //return Ok(_mapper.Map<IEnumerable<VillaDto>>(villaList));
                _response.Resultado = _mapper.Map<IEnumerable<VillaDto>>(villaList);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsExitoso = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }


        [HttpGet("VillasPaginado")]
        [ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        //public async Task<ActionResult<IEnumerable<VillaDto>>> GetVillas()
        public ActionResult<APIResponse> GetVillasPaginado([FromQuery] Parametros parametros)
        {
            try
            {

                var villaList = _villaRepo.ObtenerTodosPaginado(parametros);
                //return Ok(_mapper.Map<IEnumerable<VillaDto>>(villaList));
                _response.Resultado = _mapper.Map<IEnumerable<VillaDto>>(villaList);
                _response.StatusCode = HttpStatusCode.OK;
                _response.TotalPaginas = villaList.MetaData.TotalPages;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsExitoso = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpGet("{id:int}", Name = "GetVilla")]
        [Authorize(Roles = "admin", AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //public async Task<ActionResult<VillaDto>> GetVilla(int id)
        public async Task<ActionResult<APIResponse>> GetVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    _logger.LogError("Error al traer villa con id: " + id);
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsExitoso = false;
                    return BadRequest(_response);
                }

                var villa = await _villaRepo.Obtener(v => v.Id == id);

                if (villa == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsExitoso = false;
                    return NotFound(_response);
                }

                //return Ok(_mapper.Map<VillaDto>(villa));
                _response.Resultado = _mapper.Map<VillaDto>(villa);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsExitoso = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };

            }
            return _response;
        }

        [HttpPost]
        [Authorize(Roles = "admin", AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public async Task<ActionResult<VillaDto>> CrearVilla([FromBody] VillaCreateDto createdto)
        public async Task<ActionResult<APIResponse>> CrearVilla([FromBody] VillaCreateDto createdto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (await _villaRepo.Obtener(v => v.Nombre.ToLower() == createdto.Nombre.ToLower()) != null)
                {
                    ModelState.AddModelError("ErrorMessages", "La Villa con ese nombre ya existe");
                    return BadRequest(ModelState);
                }

                if (createdto == null)
                {
                    return BadRequest();
                }

                Villa modelo = _mapper.Map<Villa>(createdto);
                modelo.FechaCreacion = DateTime.Now;
                modelo.FechaActualizacion = DateTime.Now;

                //_db.Villas.AddAsync(modelo);
                //await _db.SaveChangesAsync();
                await _villaRepo.Crear(modelo);
                _response.Resultado = modelo;
                _response.StatusCode = HttpStatusCode.Created;

                //return CreatedAtRoute("GetVilla", new { id = modelo.Id }, modelo);
                return CreatedAtRoute("GetVilla", new { id = modelo.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.IsExitoso = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };

            }
            return _response;

        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "admin", AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsExitoso = false;
                    return BadRequest(_response);
                }

                var Villa = await _villaRepo.Obtener(v => v.Id == id);
                if (Villa == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsExitoso = false;
                    return NotFound(_response);
                }

                await _villaRepo.Remover(Villa);
                _response.StatusCode = HttpStatusCode.NoContent;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsExitoso = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };

            }
            return BadRequest(_response);

        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "admin", AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDto updateDto)
        {
            try
            {
                if (updateDto == null || id != updateDto.Id)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsExitoso = false;
                    return BadRequest(_response);
                }

                Villa modelo = _mapper.Map<Villa>(updateDto);

                await _villaRepo.Actualizar(modelo);
                _response.StatusCode = HttpStatusCode.NoContent;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsExitoso = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };

            }
            return BadRequest(_response);

        }

        [HttpPatch("{id:int}")]
        [Authorize(Roles = "admin", AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDto> patchDto)
        {
            try
            {
                if (patchDto == null || id == 0)
                {
                    return BadRequest();
                }
                var villa = await _villaRepo.Obtener(v => v.Id == id, tracked: false);

                if (villa == null)
                {
                    return BadRequest();
                }

                VillaUpdateDto villaDto = _mapper.Map<VillaUpdateDto>(villa);

                patchDto.ApplyTo(villaDto, ModelState);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                Villa modelo = _mapper.Map<Villa>(villaDto);

                await _villaRepo.Actualizar(modelo);
                _response.StatusCode = HttpStatusCode.NoContent;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsExitoso = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };

            }
            return BadRequest(_response);

        }

    }
}
