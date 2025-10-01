using AuthApi.DTOs.CategoriaDTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuthApi.Interfaces // ⬅️ Namespace de la carpeta Interfaces
{
    public interface ICategoriaService
    {
        Task<CategoriaResultadoDTO> CrearCategoriaAsync(CategoriaCreacionDTO dto);
        Task<IEnumerable<CategoriaResultadoDTO>> ObtenerTodasAsync();
        Task<CategoriaResultadoDTO?> ObtenerPorIdAsync(int id);
        Task ActualizarCategoriaAsync(int id, CategoriaActualizacionDTO dto);
        Task EliminarCategoriaAsync(int id);
    }
}