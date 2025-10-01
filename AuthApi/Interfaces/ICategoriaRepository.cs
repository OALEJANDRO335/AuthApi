using AuthApi.Entidades;

namespace AuthApi.Interfaces
{
    public interface ICategoriaRepository
    {
        // READ (Leer)
        // Obtiene una categoría por su clave primaria (Id).
        Task<Categoria?> GetByIdAsync(int id);

        // Obtiene todas las categorías.
        Task<IEnumerable<Categoria>> GetAllAsync();

        // CREATE (Crear)
        // Agrega una nueva categoría a la base de datos.
        Task AddAsync(Categoria categoria);

        // UPDATE (Actualizar)
        // Actualiza los datos de una categoría existente.
        Task UpdateAsync(Categoria categoria);

        // DELETE (Eliminar)
        // Elimina una categoría por su Id.
        Task DeleteAsync(int id);

        // Revisa si una categoría existe por nombre.
        Task<bool> ExistsByNameAsync(string name);
    }
}