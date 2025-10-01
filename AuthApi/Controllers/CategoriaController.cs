using Microsoft.AspNetCore.Mvc;
using AuthApi.Interfaces;
using AuthApi.DTOs.CategoriaDTOs;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System; // Necesario para Exception

namespace AuthApi.Controllers
{
    [Authorize]
    [ApiController] // ⬅️ DEBE estar presente para ser un API Controller
    [Route("api/[controller]")]
    // 🛑 Nota: Aquí la clase debe heredar de ControllerBase.
    // public class CategoriasController : Controller <--- ANTES
    public class CategoriasController : ControllerBase // ⬅️ CAMBIO: Hereda de ControllerBase
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriasController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        // -----------------------------------------------------------------
        // 1. OBTENER TODAS (GET: api/Categorias)
        // -----------------------------------------------------------------
        [HttpGet] // ⬅️ SOLUCIÓN AL ERROR DE AMBIGÜEDAD (Swagger)
        public async Task<ActionResult<IEnumerable<CategoriaResultadoDTO>>> ObtenerTodas()
        {
            var categorias = await _categoriaService.ObtenerTodasAsync();
            // Retorna 200 OK con el cuerpo JSON de las categorías
            return Ok(categorias);
        }

        // -----------------------------------------------------------------
        // 2. CREAR (POST: api/Categorias)
        // -----------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> Crear(CategoriaCreacionDTO dto)
        {
            // La validación del ModelState ocurre automáticamente con [ApiController]

            try
            {
                var nuevaCategoria = await _categoriaService.CrearCategoriaAsync(dto);

                // Retorna 201 Created y la ubicación del nuevo recurso
                return CreatedAtAction(
                    nameof(ObtenerPorId), // Nombre del método GET para el nuevo recurso
                    new { id = nuevaCategoria.Id }, // Parámetros de ruta
                    nuevaCategoria); // Cuerpo de la respuesta (el objeto creado)
            }
            catch (Exception ex)
            {
                // Manejo básico de errores de negocio o base de datos
                return BadRequest(new { Error = "Error al crear la categoría.", Detalle = ex.Message });
            }
        }

        // -----------------------------------------------------------------
        // 3. OBTENER POR ID (GET: api/Categorias/{id})
        // -----------------------------------------------------------------
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CategoriaResultadoDTO>> ObtenerPorId(int id)
        {
            var categoria = await _categoriaService.ObtenerPorIdAsync(id);

            if (categoria == null)
            {
                return NotFound(); // Retorna 404 Not Found si no existe
            }

            return Ok(categoria); // Retorna 200 OK
        }
    }
}