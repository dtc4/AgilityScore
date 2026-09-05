using AgilityScore.Data;
using AgilityScore.Models;
using Microsoft.EntityFrameworkCore;

namespace AgilityScore.Services
{
    public class CompetitionService
    {
        private readonly AppDbContext _db;

        public CompetitionService(AppDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Devuelve todas las competiciones de una jornada.
        /// </summary>
        public async Task<List<Competition>> GetByEventDayAsync(int eventDayId)
        {
            return await _db.Competitions
                .Where(c => c.EventDayId == eventDayId)
                .Include(c => c.Participants)
                    .ThenInclude(p => p.Dog)
                        .ThenInclude(d => d.Handler)
                .OrderBy(c => c.Level)
                .ThenBy(c => c.Size)
                .ThenBy(c => c.Type)
                .ToListAsync();
        }

        /// <summary>
        /// Devuelve una competición con sus participantes.
        /// </summary>
        public async Task<Competition?> GetByIdAsync(int id)
        {
            return await _db.Competitions
                .Include(c => c.Participants)
                    .ThenInclude(p => p.Dog)
                        .ThenInclude(d => d.Handler)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        /// <summary>
        /// Guarda o actualiza una competición (longitud, velocidad, factor TRM).
        /// </summary>
        public async Task SaveAsync(Competition competition)
        {
            var existing = await _db.Competitions.FindAsync(competition.Id);
            if (existing != null)
            {
                existing.LengthMeters = competition.LengthMeters;
                existing.ChosenSpeedMps = competition.ChosenSpeedMps;
                existing.TRMFactor = competition.TRMFactor;
            }
            else
            {
                _db.Competitions.Add(competition);
            }

            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Elimina una competición completa (incluye participantes).
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            var competition = await _db.Competitions
                .Include(c => c.Participants)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (competition != null)
            {
                _db.Competitions.Remove(competition);
                await _db.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Recalcula los TRS y TRM si cambian longitud o velocidad.
        /// </summary>
        public async Task RecalculateTimesAsync(int competitionId)
        {
            var comp = await _db.Competitions.FindAsync(competitionId);
            if (comp == null) return;

            if (comp.LengthMeters != null && comp.ChosenSpeedMps != null && comp.ChosenSpeedMps > 0)
            {
                // TRS y TRM se calculan dinámicamente en las propiedades del modelo,
                // por lo tanto no hace falta guardar valores persistentes.
                await _db.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Devuelve todos los tipos, niveles y tamaños disponibles (para menús).
        /// </summary>
        public List<(LevelType Level, SizeType Size, TrialType Type)> GetDefaultCombinations()
        {
            var list = new List<(LevelType, SizeType, TrialType)>();
            foreach (LevelType level in Enum.GetValues(typeof(LevelType)))
            {
                foreach (SizeType size in Enum.GetValues(typeof(SizeType)))
                {
                    foreach (TrialType type in Enum.GetValues(typeof(TrialType)))
                    {
                        list.Add((level, size, type));
                    }
                }
            }
            return list;
        }
    }
}
