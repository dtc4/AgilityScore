using System;
using System.Windows;
using AgilityScore.Models;

namespace AgilityScore.Views.Configuration
{
    public partial class AddSeasonWindow : Window
    {
        public Season Season { get; private set; }

        public AddSeasonWindow()
        {
            InitializeComponent();
            Season = new Season();
        }

        public AddSeasonWindow(Season season) : this()
        {
            Season = season;
            TxtName.Text = season.Name;
            DpStart.SelectedDate = season.StartDate;
            DpEnd.SelectedDate = season.EndDate;
            TxtDays.Text = season.EventDaysCount.ToString();
            Title = "Editar Temporada";
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtName.Text))
            {
                MessageBox.Show("Por favor, introduce un nombre para la temporada.", "Atención",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!DpStart.SelectedDate.HasValue || !DpEnd.SelectedDate.HasValue)
            {
                MessageBox.Show("Debes seleccionar las fechas de inicio y fin.", "Atención",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Season.Name = TxtName.Text.Trim();
            Season.StartDate = DpStart.SelectedDate.Value;
            Season.EndDate = DpEnd.SelectedDate.Value;
            Season.EventDaysCount = int.TryParse(TxtDays.Text, out var n) ? n : 0;

            DialogResult = true;
        }

    }
}
