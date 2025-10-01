using AuthApi.Entidades.AuthApi.Entidades;

namespace AuthApi.Entidades
{
    public class Categoria
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = "";

        public ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}
