namespace AuthApi.DTOs.CategoriaDTOs
{
    public class CategoriaActualizacionDTO
    {
        // 1. Nombre: Generalmente permitido actualizar.
        // Aunque a veces se requieren validaciones [Required] si la regla de negocio
        // dice que nunca puede estar vacío.
        public string Nombre { get; set; } = string.Empty;

        // 2. Descripción: Definitivamente permitida actualizar.
        public string Descripcion { get; set; } = string.Empty;

        // NOTA: El 'Id' de la categoría NO va aquí. 
        // Se pasa por la URL del controlador (ej: PUT /api/categorias/{id}).
    }
}
