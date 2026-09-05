using System.Windows;
using System.Windows.Controls;
using AgilityScore.Data;
using AgilityScore.Models;
using AgilityScore.Services;

namespace AgilityScore.Views.Configuration
{
    public partial class EventDaysMenuView : UserControl
    {
        private readonly AppDbContext _db;
        private readonly SeasonService _seasonService;

        public EventDaysMenuView()
        {
            InitializeComponent();
            _db = new AppDbContext();
            _seasonService = new SeasonService(_db);
            Loaded += EventDaysMenuView_Loaded;
        }

        private async void EventDaysMenuView_Loaded(object sender, RoutedEventArgs e)
        {
            var seasons = await _seasonService.GetAllAsync();
            SeasonsGrid.ItemsSource = seasons;
        }

        private void BtnViewEventDays_Click(object sender, RoutedEventArgs e)
        {
            var season = (sender as FrameworkElement)?.DataContext as Season;
            if (season == null) return;

            var mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow!.MainContent.Content = new EventDaysView(season.Id, season.Name);
        }
    }
}
