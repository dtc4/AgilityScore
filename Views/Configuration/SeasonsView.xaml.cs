using System.Windows;
using System.Windows.Controls;
using AgilityScore.Data;
using AgilityScore.Models;
using AgilityScore.Services;

namespace AgilityScore.Views.Configuration
{
    public partial class SeasonsView : UserControl
    {
        private readonly SeasonService _seasonService;
        private readonly AppDbContext _db;

        public SeasonsView()
        {
            InitializeComponent();
            _db = new AppDbContext();
            _seasonService = new SeasonService(_db);
            Loaded += SeasonsView_Loaded;
        }

        private async void SeasonsView_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadSeasonsAsync();
        }

        private async Task LoadSeasonsAsync()
        {
            var seasons = await _seasonService.GetAllAsync();
            SeasonsGrid.ItemsSource = seasons;
        }

        private async void BtnAddSeason_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddSeasonWindow();
            if (window.ShowDialog() == true)
            {
                // Creamos la temporada completa (con jornadas y competiciones)
                await _seasonService.CreateSeasonAsync(
                    window.Season.Name,
                    window.Season.StartDate,
                    window.Season.EndDate,
                    window.Season.EventDaysCount
                );

                await LoadSeasonsAsync();
            }
        }


        private async void BtnEditSeason_Click(object sender, RoutedEventArgs e)
        {
            var season = (sender as FrameworkElement)?.DataContext as Season;
            if (season == null) return;

            var window = new AddSeasonWindow(season);
            if (window.ShowDialog() == true)
            {
                await _seasonService.SaveAsync(window.Season);
                await LoadSeasonsAsync();
            }
        }

        private async void BtnDeleteSeason_Click(object sender, RoutedEventArgs e)
        {
            var season = (sender as FrameworkElement)?.DataContext as Season;
            if (season == null) return;

            if (MessageBox.Show($"¿Seguro que deseas eliminar la temporada '{season.Name}'?",
                                "Confirmar eliminación",
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                await _seasonService.DeleteAsync(season.Id);
                await LoadSeasonsAsync();
            }
        }
    }
}
