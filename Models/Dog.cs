// Models/Dog.cs
using System.Collections.Generic;

namespace AgilityScore.Models
{
    public class Dog
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public int? HandlerId { get; set; }
        public Handler? Handler { get; set; }

        public string? Breed { get; set; }
        public string? Club { get; set; }
        public string Category { get; set; } = string.Empty; // Mini/Midi/...

        public List<Participant> Participants { get; set; } = new();

        public string DisplayName => $"{Name} ({Handler?.FullName ?? "-"})";
    }
}
