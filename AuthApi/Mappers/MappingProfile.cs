using AutoMapper;
using AuthApi.Entidades;
using AuthApi.DTOs.CategoriaDTOs;

namespace AuthApi.Mappers
{
    // Hereda de Profile
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapeo 1: DTO de Creación a la Entidad
            CreateMap<CategoriaCreacionDTO, Categoria>();

            // Mapeo 2: Entidad a DTO de Resultado
            CreateMap<Categoria, CategoriaResultadoDTO>();

            // Mapeo 3: DTO de Actualización a la Entidad
            CreateMap<CategoriaActualizacionDTO, Categoria>();
        }
    }
}
