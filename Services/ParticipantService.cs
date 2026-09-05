using AgilityScore.Data;
using AgilityScore.Models;
using Microsoft.EntityFrameworkCore;

namespace AgilityScore.Services
{
    public class ParticipantService
    {
        private readonly AppDbContext _db;

        public ParticipantService(AppDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Obtiene todos los participantes de una competición.
        /// </summary>
        public async Task<List<Participant>> GetByCompetitionAsync(int competitionId)
        {
            return await _db.Participants
                .Include(p => p.Dog)
                    .ThenInclude(d => d.Handler)
                .Where(p => p.CompetitionId == competitionId)
                .OrderBy(p => p.Dorsal)
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene un participante por Id con su perro y guía.
        /// </summary>
        public async Task<Participant?> GetByIdAsync(int id)
        {
            return await _db.Participants
                .Include(p => p.Dog)
                    .ThenInclude(d => d.Handler)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        /// <summary>
        /// Añade o actualiza un participante (inscripción).
        /// </summary>
        public async Task SaveAsync(Participant participant)
        {
            if (participant.Id == 0)
                _db.Participants.Add(participant);
            else
                _db.Participants.Update(participant);

            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Actualiza resultados del participante.
        /// </summary>
        public async Task UpdateResultsAsync(int participantId, int faults, int refusals, TimeSpan time, bool eliminated)
        {
            var p = await _db.Participants.FindAsync(participantId);
            if (p == null) return;

            p.Faults = faults;
            p.Refusals = refusals;
            p.TimeReal = time;
            p.Eliminated = eliminated || p.Refusals >= 3 || p.Faults >= 4;

            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Elimina un participante.
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            var p = await _db.Participants.FindAsync(id);
            if (p != null)
            {
                _db.Participants.Remove(p);
                await _db.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Asigna dorsales automáticos (si faltan).
        /// </summary>
        public async Task AutoAssignDorsalsAsync(int competitionId)
        {
            var participants = await _db.Participants
                .Where(p => p.CompetitionId == competitionId)
                .OrderBy(p => p.Id)
                .ToListAsync();

            int dorsal = 1;
            foreach (var p in participants)
            {
                if (p.Dorsal == 0)
                {
                    p.Dorsal = dorsal++;
                }
                else
                {
                    dorsal = Math.Max(dorsal, p.Dorsal + 1);
                }
            }

            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Recalcula y guarda el ranking dentro de una competición.
        /// </summary>
        public async Task RecalculateRankingAsync(int competitionId)
        {
            var participants = await _db.Participants
                .Include(p => p.Competition)
                .Where(p => p.CompetitionId == competitionId)
                .ToListAsync();

            // Calcula posiciones incluso para eliminados
            var ordered = participants
                .OrderBy(p => p.Eliminated)
                .ThenBy(p => p.PenaltyTotal ?? double.MaxValue)
                .ThenBy(p => p.TimeReal ?? TimeSpan.MaxValue)
                .ToList();

            for (int i = 0; i < ordered.Count; i++)
                ordered[i].Rank = i + 1;

            await _db.SaveChangesAsync();
        }
    }
}
