using AutoMapper;
using MagicVilla_Utilidad;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Models.ViewModel;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using NuGet.Protocol;

namespace MagicVilla_Web.Controllers
{
    public class NumeroVillaController : Controller
    {
        private readonly INumeroVIllaService _numeroVIllaService;
        private readonly IVIllaService _VillaService;
        private readonly IMapper _mapper;

        public NumeroVillaController(INumeroVIllaService numeroVIllaService, IVIllaService vIllaService, IMapper mapper)
        {
            _numeroVIllaService = numeroVIllaService;
            _mapper = mapper;
            _VillaService = vIllaService;

        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> IndexNumeroVilla()
        {
            List<NumeroVillaDto> numeroVillaList = new();

            var response = await _numeroVIllaService.ObtenerTodos<APIResponse>(HttpContext.Session.GetString(DS.Sessiontoken));

            if (response != null && response.IsExitoso)
            {
                numeroVillaList = JsonConvert.DeserializeObject<List<NumeroVillaDto>>(Convert.ToString(response.Resultado));

            }
            return View(numeroVillaList);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CrearNumeroVilla()
        {
            NumeroVillaViewModel numeroVillaVM = new();
            var response = await _VillaService.ObtenerTodos<APIResponse>(HttpContext.Session.GetString(DS.Sessiontoken));
            if (response != null && response.IsExitoso)
            {
                numeroVillaVM.VillaList = JsonConvert.DeserializeObject<List<VillaDto>>(Convert.ToString(response.Resultado))
                    .Select(v => new SelectListItem
                    {
                        Text = v.Nombre,
                        Value = v.Id.ToString(),
                    });
            }
            return View(numeroVillaVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearNumeroVilla(NumeroVillaViewModel modelo)
        {
            if (ModelState.IsValid)
            {
                var response = await _numeroVIllaService.Crear<APIResponse>(modelo.NumeroVilla, HttpContext.Session.GetString(DS.Sessiontoken));
                if (response != null && response.IsExitoso)
                {
                    TempData["exitoso"] = "Numero Villa Creada Exitosamente";
                    return RedirectToAction(nameof(IndexNumeroVilla));
                }
                else
                {
                    if (response.ErrorMessages.Count > 0)
                    {
                        ModelState.AddModelError("ErrorMessages", response.ErrorMessages.FirstOrDefault());
                    }
                }
            }

            var res = await _VillaService.ObtenerTodos<APIResponse>(HttpContext.Session.GetString(DS.Sessiontoken));
            if (res != null && res.IsExitoso)
            {
                modelo.VillaList = JsonConvert.DeserializeObject<List<VillaDto>>(Convert.ToString(res.Resultado))
                    .Select(v => new SelectListItem
                    {
                        Text = v.Nombre,
                        Value = v.Id.ToString(),
                    });
            }

            return View(modelo);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ActualizarNumeroVilla(int villaNo)
        {
            NumeroVillaUpdateViewModel numeroVillaVM = new();

            var response = await _numeroVIllaService.Obtener<APIResponse>(villaNo, HttpContext.Session.GetString(DS.Sessiontoken));

            if (response != null && response.IsExitoso)
            {
                NumeroVillaDto modelo = JsonConvert.DeserializeObject<NumeroVillaDto>(Convert.ToString(response.Resultado));
                numeroVillaVM.NumeroVilla = _mapper.Map<NumeroVillaUpdateDto>(modelo);
            }

            response = await _VillaService.ObtenerTodos<APIResponse>(HttpContext.Session.GetString(DS.Sessiontoken));
            if (response != null && response.IsExitoso)
            {
                numeroVillaVM.VillaList = JsonConvert.DeserializeObject<List<VillaDto>>(Convert.ToString(response.Resultado))
                    .Select(v => new SelectListItem
                    {
                        Text = v.Nombre,
                        Value = v.Id.ToString(),
                    });
                return View(numeroVillaVM);
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarNumeroVilla(NumeroVillaUpdateViewModel modelo)
        {
            if (ModelState.IsValid)
            {
                var response = await _numeroVIllaService.Actualizar<APIResponse>(modelo.NumeroVilla, HttpContext.Session.GetString(DS.Sessiontoken));
                if (response != null && response.IsExitoso)
                {
                    TempData["exitoso"] = "Numero Villa actualizada Exitosamente";
                    return RedirectToAction(nameof(IndexNumeroVilla));
                }
                else
                {
                    if (response.ErrorMessages.Count > 0)
                    {
                        ModelState.AddModelError("ErrorMessages", response.ErrorMessages.FirstOrDefault());
                    }
                }
            }

            var res = await _VillaService.ObtenerTodos<APIResponse>(HttpContext.Session.GetString(DS.Sessiontoken));
            if (res != null && res.IsExitoso)
            {
                modelo.VillaList = JsonConvert.DeserializeObject<List<VillaDto>>(Convert.ToString(res.Resultado))
                    .Select(v => new SelectListItem
                    {
                        Text = v.Nombre,
                        Value = v.Id.ToString(),
                    });
            }

            return View(modelo);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> RemoverNumeroVilla(int villaNo)
        {
            NumeroVillaDeleteViewModel numeroVillaVM = new();

            var response = await _numeroVIllaService.Obtener<APIResponse>(villaNo, HttpContext.Session.GetString(DS.Sessiontoken));

            if (response != null && response.IsExitoso)
            {
                NumeroVillaDto modelo = JsonConvert.DeserializeObject<NumeroVillaDto>(Convert.ToString(response.Resultado));
                numeroVillaVM.NumeroVilla = modelo;
            }

            response = await _VillaService.ObtenerTodos<APIResponse>(HttpContext.Session.GetString(DS.Sessiontoken));
            if (response != null && response.IsExitoso)
            {
                numeroVillaVM.VillaList = JsonConvert.DeserializeObject<List<VillaDto>>(Convert.ToString(response.Resultado))
                    .Select(v => new SelectListItem
                    {
                        Text = v.Nombre,
                        Value = v.Id.ToString(),
                    });
                return View(numeroVillaVM);
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoverNumeroVilla(NumeroVillaDeleteViewModel modelo)
        {
            var response = await _numeroVIllaService.Remover<APIResponse>(modelo.NumeroVilla.VillaNo, HttpContext.Session.GetString(DS.Sessiontoken));

            if (response != null && response.IsExitoso)
            {
                TempData["exitoso"] = "Numero Villa eliminada Exitosamente";
                return RedirectToAction(nameof(IndexNumeroVilla));
            }
            TempData["error"] = "Un error ocurrio al intentar eliminar";

            return View(modelo);
        }
    }
}