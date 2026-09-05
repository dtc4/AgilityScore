using System.Windows;
using AgilityScore.Views.Configuration;

namespace AgilityScore
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainContent.Content = new SeasonsView(); // Vista inicial
        }

        // --- CONFIGURACIÓN ---
        private void BtnSeasons_Click(object sender, RoutedEventArgs e) => MainContent.Content = new SeasonsView();
        private void BtnEventDays_Click(object sender, RoutedEventArgs e) => MainContent.Content = new EventDaysMenuView();
        //private void BtnCompetitions_Click(object sender, RoutedEventArgs e) => MainContent.Content = new CompetitionsMenuView();
        //private void BtnHandlers_Click(object sender, RoutedEventArgs e) => MainContent.Content = new HandlersView();
        //private void BtnDogs_Click(object sender, RoutedEventArgs e) => MainContent.Content = new DogsView();

        // --- COMPETICIÓN ---
        //private void BtnScoring_Click(object sender, RoutedEventArgs e) => MainContent.Content = new ScoringMenuView();
        //private void BtnRanking_Click(object sender, RoutedEventArgs e) => MainContent.Content = new RankingMenuView();
    }
}
