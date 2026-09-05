// Models/Season.cs
using System;
using System.Collections.Generic;

namespace AgilityScore.Models
{
    public class Season
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // "2025-26"
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // número de jornadas que se esperan (opcional)
        public int EventDaysCount { get; set; }

        // Relaciones
        public List<EventDay> EventDays { get; set; } = new();
    }
}
