using AuthApi.Entidades; // Necesario para la Entidad CategoriaMC
using AuthApi.DTOs.CategoriaDTOs;
using AuthApi.Interfaces; // Necesario para ICategoriaService y ICategoriaRepository
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq; // Necesario para usar Linq si fuera necesario

namespace AuthApi.Servicios // ⬅️ Namespace de la capa de Servicio
{
    // Clase concreta que implementa el contrato (ICategoriaService)
    public class CategoriaService : ICategoriaService
    {
        private readonly ICategoriaRepository _repository;
        private readonly IMapper _mapper;

        public CategoriaService(ICategoriaRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // -----------------------------------------------------
        // IMPLEMENTACIONES
        // -----------------------------------------------------

        // CREATE: Implementación que resuelve ExistsByNameAsync
        public async Task<CategoriaResultadoDTO> CrearCategoriaAsync(CategoriaCreacionDTO dto)
        {
            // Lógica de Negocio: Verifica si ya existe el nombre
            if (await _repository.ExistsByNameAsync(dto.Nombre))
            {
                // Deberías usar InvalidOperationException o una custom exception de negocio
                throw new InvalidOperationException($"Ya existe una categoría con el nombre '{dto.Nombre}'.");
            }

            // Mapeo DTO -> Entidad
            var categoriaEntidad = _mapper.Map<Categoria>(dto);

            // Persistencia
            await _repository.AddAsync(categoriaEntidad);

            // Mapeo Entidad -> DTO de resultado
            return _mapper.Map<CategoriaResultadoDTO>(categoriaEntidad);
        }

        // READ ALL: Implementación
        public async Task<IEnumerable<CategoriaResultadoDTO>> ObtenerTodasAsync()
        {
            var entidades = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<CategoriaResultadoDTO>>(entidades);
        }

        // READ BY ID: Implementación (Resuelve parte del error CS0535)
        public async Task<CategoriaResultadoDTO?> ObtenerPorIdAsync(int id)
        {
            var entidad = await _repository.GetByIdAsync(id);
            // Mapea a DTO, si la entidad es null, el DTO también será null (por el '?' en el Task)
            return _mapper.Map<CategoriaResultadoDTO>(entidad);
        }

        // UPDATE: Implementación (Resuelve parte del error CS0535)
        public async Task ActualizarCategoriaAsync(int id, CategoriaActualizacionDTO dto)
        {
            var entidadExistente = await _repository.GetByIdAsync(id);
            if (entidadExistente == null)
            {
                // Se lanza una excepción si no se encuentra para que el controlador pueda manejar el 404
                throw new KeyNotFoundException($"Categoría con ID {id} no encontrada para actualizar.");
            }

            // Mapea los datos del DTO a la entidad existente (actualiza los campos)
            _mapper.Map(dto, entidadExistente);

            await _repository.UpdateAsync(entidadExistente);
        }

        // DELETE: Implementación (Resuelve parte del error CS0535)
        public async Task EliminarCategoriaAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}