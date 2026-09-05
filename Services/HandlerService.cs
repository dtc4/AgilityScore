using AgilityScore.Data;
using AgilityScore.Models;
using Microsoft.EntityFrameworkCore;

namespace AgilityScore.Services
{
    public class HandlerService
    {
        private readonly AppDbContext _db;

        public HandlerService(AppDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Obtiene todos los guías.
        /// </summary>
        public async Task<List<Handler>> GetAllAsync()
        {
            return await _db.Handlers
                .Include(h => h.Dogs)
                .OrderBy(h => h.FullName)
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene un guía por ID.
        /// </summary>
        public async Task<Handler?> GetByIdAsync(int id)
        {
            return await _db.Handlers
                .Include(h => h.Dogs)
                .FirstOrDefaultAsync(h => h.Id == id);
        }

        /// <summary>
        /// Guarda o actualiza un guía.
        /// </summary>
        public async Task SaveAsync(Handler handler)
        {
            if (handler.Id == 0)
                _db.Handlers.Add(handler);
            else
                _db.Handlers.Update(handler);

            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Elimina un guía (y desvincula sus perros).
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            var handler = await _db.Handlers
                .Include(h => h.Dogs)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (handler != null)
            {
                foreach (var dog in handler.Dogs)
                    dog.HandlerId = null; // los dejamos sin guía

                _db.Handlers.Remove(handler);
                await _db.SaveChangesAsync();
            }
        }
    }
}
