// Models/Handler.cs
using AgilityScore.Models;
using System.Collections.Generic;

namespace AgilityScore.Models
{
    public class Handler
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;        // nombre
        public string FirstName { get; set; } = string.Empty;   // primer apellido / segundo nombre
        public string LastName { get; set; } = string.Empty;    // apellido principal

        public List<Dog> Dogs { get; set; } = new();

        public string FullName => string.Join(" ", new[] { Name, FirstName, LastName }.Where(s => !string.IsNullOrWhiteSpace(s)));
        public override string ToString() => FullName;
    }
}
