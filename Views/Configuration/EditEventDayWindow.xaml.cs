using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using AgilityScore.Models;
using System.Globalization;
using System.Windows.Input;


namespace AgilityScore.Views.Configuration
{
    public partial class EditEventDayWindow : Window
    {
        public EventDay EventDay { get; private set; }

        public EditEventDayWindow()
        {
            InitializeComponent();
            EventDay = new EventDay();
        }

        public EditEventDayWindow(EventDay eventDay) : this()
        {
            EventDay = eventDay;

            TxtName.Text = eventDay.Name;
            DpDate.SelectedDate = eventDay.Date;
            TxtOrganizer.Text = eventDay.Organizer;
            TxtLocation.Text = eventDay.Location;
            TxtJudge.Text = eventDay.Judge;
            TxtOrder.Text = eventDay.StartOrder;

            CompetitionsGrid.ItemsSource = EventDay.Competitions;
            Title = $"Editar Jornada: {eventDay.Name}";
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            EventDay.Name = TxtName.Text.Trim();
            EventDay.Date = DpDate.SelectedDate ?? DateTime.Now;
            EventDay.Organizer = TxtOrganizer.Text.Trim();
            EventDay.Location = TxtLocation.Text.Trim();
            EventDay.Judge = TxtJudge.Text.Trim();
            EventDay.StartOrder = TxtOrder.Text.Trim();

            DialogResult = true;
        }

        // 🔹 Recalcula automáticamente TRS y TRM cuando se edita una celda
        private void CompetitionsGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var comp = e.Row.Item as Competition;
            if (comp == null) return;

            // Guardamos los valores y recalculamos
            if (comp.LengthMeters.HasValue && comp.ChosenSpeedMps.HasValue && comp.ChosenSpeedMps > 0)
            {
                double trs = comp.LengthMeters.Value / comp.ChosenSpeedMps.Value;
                double trm = trs * Math.Clamp(comp.TRMFactor, 1.5, 2.0);

                // No se asignan porque TRS/TRM son propiedades calculadas, 
                // pero forzamos el refresco visual después del commit:
                Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    CompetitionsGrid.Items.Refresh();
                }));
            }
        }

        private void CompetitionsGrid_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            if (e.EditingElement is TextBox tb)
            {
                tb.PreviewTextInput += (s, ev) =>
                {
                    char ch = ev.Text[0];
                    // Permitir solo números y separadores decimales , o .
                    if (!char.IsDigit(ch) && ch != ',' && ch != '.')
                        ev.Handled = true;
                };

                tb.PreviewKeyDown += (s, ev) =>
                {
                    // Permitir teclas de control: borrado, flechas, tabulador
                    if (ev.Key == Key.Back || ev.Key == Key.Delete || ev.Key == Key.Left || ev.Key == Key.Right || ev.Key == Key.Tab)
                        ev.Handled = false;
                };

                // Corregir automáticamente punto a coma solo si la cultura usa coma
                tb.TextChanged += (s, ev) =>
                {
                    if (System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator == ",")
                    {
                        var caret = tb.CaretIndex;
                        var newText = tb.Text.Replace('.', ',');
                        if (newText != tb.Text)
                        {
                            tb.Text = newText;
                            tb.CaretIndex = Math.Min(caret + 1, tb.Text.Length);
                        }
                    }
                };
            }
        }


    }
}
