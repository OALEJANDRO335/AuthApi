namespace AuthApi.DTOs.CategoriaDTOs
{
    public class CategoriaCreacionDTO
    {
        // 1. Nombre: Obligatorio para crear la entidad
        // (Aquí agregarías las Data Annotations para validación si usas ASP.NET Core)
        // [Required(ErrorMessage = "El nombre es obligatorio.")]
        // [MaxLength(50)]
        public string Nombre { get; set; } = string.Empty;

        // 2. Descripción: Opcional, pero se acepta en el input
        // [MaxLength(255)]
        public string Descripcion { get; set; } = string.Empty;
    }
}
