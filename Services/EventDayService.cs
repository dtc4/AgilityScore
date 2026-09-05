using AgilityScore.Data;
using AgilityScore.Models;
using Microsoft.EntityFrameworkCore;

namespace AgilityScore.Services
{
    public class EventDayService
    {
        private readonly AppDbContext _db;

        public EventDayService(AppDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Devuelve todas las jornadas de una temporada.
        /// </summary>
        public async Task<List<EventDay>> GetBySeasonAsync(int seasonId)
        {
            return await _db.EventDays
                .Where(e => e.SeasonId == seasonId)
                .Include(e => e.Competitions)
                .OrderBy(e => e.Date)
                .ToListAsync();
        }

        /// <summary>
        /// Devuelve una jornada con todas sus competiciones y participantes.
        /// </summary>
        public async Task<EventDay?> GetByIdAsync(int id)
        {
            return await _db.EventDays
                .Include(e => e.Competitions)
                    .ThenInclude(c => c.Participants)
                        .ThenInclude(p => p.Dog)
                            .ThenInclude(d => d.Handler)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        /// <summary>
        /// Crea una nueva jornada manualmente.
        /// </summary>
        public async Task<EventDay> CreateAsync(int seasonId, string name, DateTime date, string judge)
        {
            var eventDay = new EventDay
            {
                SeasonId = seasonId,
                Name = name,
                Date = date,
                Judge = judge
            };

            eventDay.EnsureDefaultCompetitions();

            _db.EventDays.Add(eventDay);
            await _db.SaveChangesAsync();
            return eventDay;
        }

        /// <summary>
        /// Devuelve todas las jornadas (de todas las temporadas).
        /// </summary>
        public async Task<List<EventDay>> GetAllAsync()
        {
            return await _db.EventDays
                .Include(e => e.Season)
                .OrderBy(e => e.Date)
                .ToListAsync();
        }

        /// <summary>
        /// Guarda (crea o actualiza) una jornada.
        /// </summary>
        public async Task SaveAsync(EventDay eventDay)
        {
            if (eventDay.Id == 0)
            {
                // Si no existe, creamos y añadimos competiciones por defecto
                eventDay.EnsureDefaultCompetitions();
                _db.EventDays.Add(eventDay);
            }
            else
            {
                var existing = await _db.EventDays.FindAsync(eventDay.Id);
                if (existing != null)
                {
                    existing.Name = eventDay.Name;
                    existing.Date = eventDay.Date;
                    existing.Organizer = eventDay.Organizer;
                    existing.Location = eventDay.Location;
                    existing.Judge = eventDay.Judge;
                    existing.StartOrder = eventDay.StartOrder;
                }
            }

            await _db.SaveChangesAsync();
        }


        /// <summary>
        /// Actualiza los datos principales de la jornada (nombre, fecha, juez).
        /// </summary>
        public async Task UpdateAsync(EventDay eventDay)
        {
            var existing = await _db.EventDays.FindAsync(eventDay.Id);
            if (existing == null) return;

            existing.Name = eventDay.Name;
            existing.Date = eventDay.Date;
            existing.Judge = eventDay.Judge;

            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Regenera todas las competiciones de una jornada (niveles × tamaños × tipos).
        /// Elimina las existentes primero.
        /// </summary>
        public async Task RegenerateCompetitionsAsync(int eventDayId)
        {
            var eventDay = await _db.EventDays
                .Include(e => e.Competitions)
                .FirstOrDefaultAsync(e => e.Id == eventDayId);

            if (eventDay == null) return;

            _db.Competitions.RemoveRange(eventDay.Competitions);
            await _db.SaveChangesAsync();

            eventDay.Competitions.Clear();
            eventDay.EnsureDefaultCompetitions();

            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Elimina una jornada completa.
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            var eventDay = await _db.EventDays
                .Include(e => e.Competitions)
                .ThenInclude(c => c.Participants)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (eventDay != null)
            {
                _db.EventDays.Remove(eventDay);
                await _db.SaveChangesAsync();
            }
        }
    }
}
