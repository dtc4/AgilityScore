using AgilityScore.Data;
using AgilityScore.Models;
using Microsoft.EntityFrameworkCore;

namespace AgilityScore.Services
{
    public class DogService
    {
        private readonly AppDbContext _db;

        public DogService(AppDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Obtiene todos los perros con su guía.
        /// </summary>
        public async Task<List<Dog>> GetAllAsync()
        {
            return await _db.Dogs
                .Include(d => d.Handler)
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene los perros de un guía.
        /// </summary>
        public async Task<List<Dog>> GetByHandlerAsync(int handlerId)
        {
            return await _db.Dogs
                .Where(d => d.HandlerId == handlerId)
                .Include(d => d.Handler)
                .ToListAsync();
        }

        /// <summary>
        /// Guarda o actualiza un perro.
        /// </summary>
        public async Task SaveAsync(Dog dog)
        {
            if (dog.Id == 0)
                _db.Dogs.Add(dog);
            else
                _db.Dogs.Update(dog);

            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Elimina un perro (y sus participaciones).
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            var dog = await _db.Dogs
                .Include(d => d.Participants)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (dog != null)
            {
                _db.Participants.RemoveRange(dog.Participants);
                _db.Dogs.Remove(dog);
                await _db.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Obtiene un perro por ID con su guía.
        /// </summary>
        public async Task<Dog?> GetByIdAsync(int id)
        {
            return await _db.Dogs
                .Include(d => d.Handler)
                .FirstOrDefaultAsync(d => d.Id == id);
        }
    }
}
