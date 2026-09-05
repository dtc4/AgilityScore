using System.Windows;
using System.Windows.Controls;
using AgilityScore.Data;
using AgilityScore.Models;
using AgilityScore.Services;

namespace AgilityScore.Views.Configuration
{
    public partial class EventDaysView : UserControl
    {
        private readonly EventDayService _eventDayService;
        private readonly AppDbContext _db;
        private readonly int _seasonId;
        private readonly string _seasonName;

        public EventDaysView(int seasonId, string seasonName)
        {
            InitializeComponent();
            _db = new AppDbContext();
            _eventDayService = new EventDayService(_db);
            _seasonId = seasonId;
            _seasonName = seasonName;
            Loaded += EventDaysView_Loaded;
        }

        private async void EventDaysView_Loaded(object sender, RoutedEventArgs e)
        {
            TitleText.Text = $"📆 Jornadas de la temporada {_seasonName}";
            var days = await _eventDayService.GetBySeasonAsync(_seasonId);
            EventDaysGrid.ItemsSource = days;
        }

        private async void BtnEditEventDay_Click(object sender, RoutedEventArgs e)
        {
            var day = (sender as FrameworkElement)?.DataContext as EventDay;
            if (day == null) return;

            var window = new EditEventDayWindow(day);
            if (window.ShowDialog() == true)
            {
                await _eventDayService.UpdateAsync(window.EventDay);
                var days = await _eventDayService.GetBySeasonAsync(_seasonId);
                EventDaysGrid.ItemsSource = days;
            }
        }

        private async void BtnDeleteEventDay_Click(object sender, RoutedEventArgs e)
        {
            var day = (sender as FrameworkElement)?.DataContext as EventDay;
            if (day == null) return;

            if (MessageBox.Show($"¿Seguro que deseas eliminar la jornada '{day.Name}'?",
                                "Confirmar eliminación",
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                await _eventDayService.DeleteAsync(day.Id);
                var days = await _eventDayService.GetBySeasonAsync(_seasonId);
                EventDaysGrid.ItemsSource = days;
            }
        }
    }
}
