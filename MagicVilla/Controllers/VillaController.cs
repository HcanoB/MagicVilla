using MagicVilla.Datos;
using MagicVilla.Modelos;
using MagicVilla.Modelos.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        private readonly ApplicationDbContext _db;

        public VillaController(ILogger<VillaController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet]
        public ActionResult<IEnumerable<VillaDto>> GetVillas()
        {
            _logger.LogInformation("Obtener las villas");
            //return Ok(VillaStore.VillaList);
            return Ok(_db.Villas.ToList());
        }

        [HttpGet("id:int", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<VillaDto> GetVilla(int id)
        {
            if (id == 0)
            {
                _logger.LogError("Error al traer villa con id: " + id);
                return BadRequest();
            }
            //var villa = VillaStore.VillaList.FirstOrDefault(v => v.Id == id);
            var villa = _db.Villas.FirstOrDefault(v => v.Id == id);

            if (villa == null)
            {
                return NotFound();
            }
            return Ok(villa);
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<VillaDto> CrearVilla([FromBody] VillaDto villadto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // if (VillaStore.VillaList.FirstOrDefault(v => v.Nombre.ToLower() == villadto.Nombre.ToLower()) != null)
            if (_db.Villas.FirstOrDefault(v => v.Nombre.ToLower() == villadto.Nombre.ToLower()) != null)
            {
                ModelState.AddModelError("NombreExiste", "La Villa con ese nombre ya existe");
                return BadRequest(ModelState);
            }

            if (villadto == null)
            {
                return BadRequest();
            }
            if (villadto.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            //villadto.Id = VillaStore.VillaList.OrderByDescending(v => v.Id).FirstOrDefault().Id + 1;
            //VillaStore.VillaList.Add(villadto);

            Villa modelo = new()
            {
                Nombre = villadto.Nombre,
                Detalle = villadto.Detalle,
                ImageUrl = villadto.ImageUrl,
                Ocupantes = villadto.Ocupantes,
                Tarifa = villadto.Tarifa,
                MetrosCuadrados = villadto.MetrosCuadrados,
                Amenidad = villadto.Amenidad,
            };

            _db.Villas.Add(modelo);
            _db.SaveChanges();

            //return Ok(villadto);
            return CreatedAtRoute("GetVilla", new { id = villadto.Id }, villadto);
        }

        [HttpDelete("id:int")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            //var Villa = VillaStore.VillaList.FirstOrDefault(v => v.Id == id);
            var Villa = _db.Villas.FirstOrDefault(v => v.Id == id);
            if (Villa == null)
            {
                return NotFound();
            }
            //VillaStore.VillaList.Remove(Villa);
            _db.Villas.Remove(Villa);
            _db.SaveChanges();

            return NoContent();
        }

        [HttpPut("id:int")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdateVilla(int id, [FromBody] VillaDto villaDto)
        {
            if (villaDto == null || id != villaDto.Id)
            {
                return BadRequest();
            }
            //var villa = VillaStore.VillaList.FirstOrDefault(v => v.Id == id);

            //villa.Nombre = villaDto.Nombre;
            //villa.Ocupantes = villaDto.Ocupantes;
            //villa.MetrosCuadrados = villaDto.MetrosCuadrados;

            Villa modelo = new()
            {
                Id = villaDto.Id,
                Nombre = villaDto.Nombre,
                Detalle = villaDto.Detalle,
                ImageUrl = villaDto.ImageUrl,
                Ocupantes = villaDto.Ocupantes,
                Tarifa = villaDto.Tarifa,
                MetrosCuadrados = villaDto.MetrosCuadrados,
                Amenidad = villaDto.Amenidad,
            };
            _db.Villas.Update(modelo);
            _db.SaveChanges();

            return NoContent();
        }

        [HttpPatch("id:int")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDto> patchDto)
        {
            if (patchDto == null || id == 0)
            {
                return BadRequest();
            }

            //var villa = VillaStore.VillaList.FirstOrDefault(v => v.Id == id);
            var villa = _db.Villas.AsNoTracking().FirstOrDefault(v => v.Id == id);

            if (villa == null)
            {
                return BadRequest();
            }

            VillaDto villaDto = new()
            {
                Id = villa.Id,
                Nombre = villa.Nombre,
                Detalle = villa.Detalle,
                ImageUrl = villa.ImageUrl,
                Ocupantes = villa.Ocupantes,
                Tarifa = villa.Tarifa,
                MetrosCuadrados = villa.MetrosCuadrados,
                Amenidad = villa.Amenidad,
            };

            patchDto.ApplyTo(villaDto, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Villa modelo = new()
            {
                Id = villaDto.Id,
                Nombre = villaDto.Nombre,
                Detalle = villaDto.Detalle,
                ImageUrl = villaDto.ImageUrl,
                Ocupantes = villaDto.Ocupantes,
                Tarifa = villaDto.Tarifa,
                MetrosCuadrados = villaDto.MetrosCuadrados,
                Amenidad = villaDto.Amenidad,
            };

            _db.Villas.Update(modelo);
            _db.SaveChanges();

            return NoContent();
        }

    }
}
