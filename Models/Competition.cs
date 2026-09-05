using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AgilityScore.Models
{
    public class Competition : INotifyPropertyChanged
    {
        public int Id { get; set; }

        public int EventDayId { get; set; }
        public EventDay? EventDay { get; set; }

        public LevelType Level { get; set; }
        public SizeType Size { get; set; }
        public TrialType Type { get; set; }

        private double? _lengthMeters;
        public double? LengthMeters
        {
            get => _lengthMeters;
            set
            {
                if (_lengthMeters != value)
                {
                    _lengthMeters = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TRS));
                    OnPropertyChanged(nameof(TRM));
                }
            }
        }

        private double? _chosenSpeedMps;
        public double? ChosenSpeedMps
        {
            get => _chosenSpeedMps;
            set
            {
                if (_chosenSpeedMps != value)
                {
                    _chosenSpeedMps = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TRS));
                    OnPropertyChanged(nameof(TRM));
                }
            }
        }

        private double _trmFactor = 2.0;
        public double TRMFactor
        {
            get => _trmFactor;
            set
            {
                if (_trmFactor != value)
                {
                    _trmFactor = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TRM));
                }
            }
        }

        public double? TRS =>
            (LengthMeters == null || ChosenSpeedMps == null || ChosenSpeedMps == 0)
                ? null
                : LengthMeters / ChosenSpeedMps;

        public double? TRM =>
            TRS == null ? null : TRS * Math.Clamp(TRMFactor, 1.5, 2.0);

        public List<Participant> Participants { get; set; } = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
