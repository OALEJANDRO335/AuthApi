namespace AuthApi.DTOs.CategoriaDTOs
{
    public class CategoriaResultadoDTO
    {
        // 1. Id: Es crucial para que el cliente pueda referenciar y usar la categoría después de que se crea o se obtiene.
        public int Id { get; set; }

        // 2. Nombre
        public string Nombre { get; set; } = string.Empty;

        // 3. Descripción
        public string Descripcion { get; set; } = string.Empty;

        // Opcional: Podrías añadir datos derivados o de solo lectura
        // public DateTime FechaCreacion { get; set; }
        // public int TotalProductos { get; set; } // Si lo calculás en el Servicio
    }
}
