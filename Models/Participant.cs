// Models/Participant.cs  (ligeramente adaptado)
using AgilityScore.Models;
using System;

namespace AgilityScore.Models
{
    public class Participant
    {
        public int Id { get; set; }

        public int Dorsal { get; set; }

        public int DogId { get; set; }
        public Dog? Dog { get; set; }

        public int CompetitionId { get; set; }
        public Competition? Competition { get; set; }

        public int Faults { get; set; } = 0;
        public int Refusals { get; set; } = 0;
        public bool Eliminated { get; set; } = false;
        public TimeSpan? TimeReal { get; set; }

        public const double PenaltyPerFault = 5.0;
        public const double PenaltyPerRefusal = 5.0;
        public const double PenaltyElimination = 50.0;

        public double FaultsPenalty => Faults * PenaltyPerFault;
        public double RefusalsPenalty => Refusals * PenaltyPerRefusal;

        public double? TimePenalty
        {
            get
            {
                if (TimeReal == null || Competition?.TRS == null) return null;
                var extra = TimeReal.Value.TotalSeconds - Competition.TRS.Value;
                return extra > 0 ? extra : 0.0;
            }
        }

        public bool? IsOverTRM
        {
            get
            {
                if (TimeReal == null || Competition?.TRM == null) return null;
                return TimeReal.Value.TotalSeconds > Competition.TRM.Value;
            }
        }

        public double? PenaltyTotal
        {
            get
            {
                if (Eliminated) return PenaltyElimination;
                double timePen = TimePenalty ?? 0.0;
                return FaultsPenalty + RefusalsPenalty + timePen;
            }
        }

        public string TimePenaltyDisplay => TimePenalty.HasValue ? TimePenalty.Value.ToString("0.00") : "-";
        public string PenaltyTotalDisplay => PenaltyTotal.HasValue ? PenaltyTotal.Value.ToString("0.00") : "-";

        public string FinalTimeDisplay
        {
            get
            {
                if (TimeReal == null) return "-";
                double totalSeconds = TimeReal.Value.TotalSeconds + (PenaltyTotal ?? 0.0);
                var ts = TimeSpan.FromSeconds(totalSeconds);
                return ts.ToString(@"mm\:ss\.ff");
            }
        }

        public string GradeDisplay
        {
            get
            {
                if (Eliminated) return "Eliminado";
                var total = PenaltyTotal ?? 0.0;
                if (total == 0) return "Excelente";
                else if (total <= 5) return "Muy Bueno";
                else if (total <= 10) return "Bueno";
                else if (total <= 15) return "Suficiente";
                else return "No Clasificado";
            }
        }

        public int? Rank { get; set; }
    }
}
