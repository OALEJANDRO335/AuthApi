using AuthApi.Entidades;
using Microsoft.EntityFrameworkCore;
using AuthApi.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq; // Necesario para AnyAsync en ExistsByNameAsync

namespace AuthApi.Repositorios
{
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly AppDbContext _context;

        public CategoriaRepository(AppDbContext context)
        {
            _context = context;
        }

        // -----------------------------------------------------
        // 1. CREATE (Crear)
        // -----------------------------------------------------
        public async Task AddAsync(Categoria categoria)
        {
            await _context.Categorias.AddAsync(categoria);
            await _context.SaveChangesAsync();
        }

        // -----------------------------------------------------
        // 2. READ (Obtener por Id)
        // CORREGIDO: El tipo de retorno coincide con la interfaz: Task<Categoria?>
        // -----------------------------------------------------
        public async Task<Categoria?> GetByIdAsync(int id)
        {
            // Nota: Se usó FindAsync() ya que es el método más eficiente para buscar por PK.
            // FindAsync incluye automáticamente el .Include() si el ORM lo necesita
            // y es la forma recomendada en repositorios simples.
            return await _context.Categorias.FindAsync(id);

            // Si quieres la relación Products, usa este (pero recuerda que Producto aún no existe):
            /*
            return await _context.Categorias
                                 .Include(c => c.Productos) 
                                 .FirstOrDefaultAsync(c => c.Id == id);
            */
        }

        // -----------------------------------------------------
        // 3. READ (Obtener todos)
        // -----------------------------------------------------
        public async Task<IEnumerable<Categoria>> GetAllAsync()
        {
            // Devuelve todas las categorías
            return await _context.Categorias.ToListAsync();
        }

        // -----------------------------------------------------
        // 4. UPDATE (Actualizar)
        // -----------------------------------------------------
        public async Task UpdateAsync(Categoria categoria)
        {
            _context.Categorias.Update(categoria);
            await _context.SaveChangesAsync();
        }

        // -----------------------------------------------------
        // 5. DELETE (Eliminar)
        // -----------------------------------------------------
        public async Task DeleteAsync(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria != null)
            {
                _context.Categorias.Remove(categoria);
                await _context.SaveChangesAsync();
            }
        }

        // -----------------------------------------------------
        // 6. EXISTE POR NOMBRE (Implementación para CS0535)
        // -----------------------------------------------------
        public async Task<bool> ExistsByNameAsync(string name)
        {
            // Verifica si existe alguna categoría con ese nombre
            return await _context.Categorias.AnyAsync(c => c.Nombre == name);
        }
    }
}