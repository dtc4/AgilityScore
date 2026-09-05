// Models/EventDay.cs
using AgilityScore.Models;
using System;
using System.Collections.Generic;

namespace AgilityScore.Models
{
    public class EventDay
    {
        public int Id { get; set; }

        public int SeasonId { get; set; }
        public Season? Season { get; set; }

        public string Name { get; set; } = string.Empty; // "Jornada 1"
        public DateTime Date { get; set; } = DateTime.Now;

        public string Organizer { get; set; } = string.Empty; // Ej: "E.A. Almussafes"
        public string Location { get; set; } = string.Empty;  // Ej: "Almussafes"

        public string StartOrder { get; set; } = string.Empty; // Ej: "Nivel II - Mini / Nivel III - Maxi"

        public string Judge { get; set; } = string.Empty;

        // Todas las competiciones (cada combinación Level × Size × TrialType)
        public List<Competition> Competitions { get; set; } = new();

        /// <summary>
        /// Helper: crear por defecto todas las competiciones (niveles × tamaños × tipos).
        /// Llamar desde la UI o desde la lógica al crear la jornada.
        /// </summary>
        public void EnsureDefaultCompetitions()
        {
            foreach (LevelType level in Enum.GetValues(typeof(LevelType)))
            {
                foreach (SizeType size in Enum.GetValues(typeof(SizeType)))
                {
                    foreach (TrialType type in Enum.GetValues(typeof(TrialType)))
                    {
                        // evita duplicados
                        if (!Competitions.Any(c => c.Level == level && c.Size == size && c.Type == type))
                        {
                            Competitions.Add(new Competition
                            {
                                Level = level,
                                Size = size,
                                Type = type
                            });
                        }
                    }
                }
            }
        }
    }
}
