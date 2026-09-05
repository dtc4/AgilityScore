using AgilityScore.Data;
using AgilityScore.Models;
using Microsoft.EntityFrameworkCore;

namespace AgilityScore.Services
{
    public class SeasonService
    {
        private readonly AppDbContext _db;

        public SeasonService(AppDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Devuelve todas las temporadas ordenadas por fecha de inicio descendente.
        /// </summary>
        public async Task<List<Season>> GetAllAsync()
        {
            return await _db.Seasons
                .Include(s => s.EventDays)
                .OrderByDescending(s => s.StartDate)
                .ToListAsync();
        }

        /// <summary>
        /// Devuelve una temporada con todas sus jornadas y competiciones.
        /// </summary>
        public async Task<Season?> GetByIdAsync(int id)
        {
            return await _db.Seasons
                .Include(s => s.EventDays)
                    .ThenInclude(e => e.Competitions)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        /// <summary>
        /// Crea una nueva temporada con el número de jornadas especificado.
        /// </summary>
        public async Task<Season> CreateSeasonAsync(string name, DateTime start, DateTime end, int numEventDays)
        {
            var season = new Season
            {
                Name = name,
                StartDate = start,
                EndDate = end,
                EventDaysCount = numEventDays
            };

            // Crear jornadas vacías por defecto
            for (int i = 1; i <= numEventDays; i++)
            {
                var eventDay = new EventDay
                {
                    Name = $"Jornada {i}",
                    Date = start.AddDays((i - 1) * 7), // una semana de separación por defecto
                    Judge = string.Empty
                };
                eventDay.EnsureDefaultCompetitions();
                season.EventDays.Add(eventDay);
            }

            _db.Seasons.Add(season);
            await _db.SaveChangesAsync();
            return season;
        }

        /// <summary>
        /// Guarda (crea o actualiza) una temporada existente.
        /// </summary>
        public async Task SaveAsync(Season season)
        {
            if (season.Id == 0)
            {
                _db.Seasons.Add(season);
            }
            else
            {
                _db.Seasons.Update(season);
            }

            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Elimina una temporada completa (con jornadas, competiciones y participantes).
        /// </summary>
        public async Task DeleteAsync(int seasonId)
        {
            var season = await _db.Seasons
                .Include(s => s.EventDays)
                .ThenInclude(e => e.Competitions)
                .ThenInclude(c => c.Participants)
                .FirstOrDefaultAsync(s => s.Id == seasonId);

            if (season != null)
            {
                _db.Seasons.Remove(season);
                await _db.SaveChangesAsync();
            }
        }
    }
}
