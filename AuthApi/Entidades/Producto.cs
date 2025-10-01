namespace AuthApi.Entidades
{
    namespace AuthApi.Entidades
    {
        public class Producto
        {
            // Corresponde a la columna Id, PK
            public int Id { get; set; }

            public string Nombre { get; set; }

            public decimal Precio { get; set; }

            // Clave Foránea (FK)
            public int CategoriaId { get; set; }

            // Propiedad de Navegación
            public Categoria Categoria { get; set; } = null!;
        }
    }
}
