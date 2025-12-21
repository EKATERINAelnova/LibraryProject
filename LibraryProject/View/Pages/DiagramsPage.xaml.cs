using LibraryProject.Service;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.DataVisualization.Charting;

namespace LibraryProject.View.Pages
{
    public partial class DiagramsPage : System.Windows.Controls.UserControl
    {
        private readonly MainWindow _main;
        private readonly UserService _userService;

        private bool _isUpdating;
        private string _currentStatType = "Workers";

        private const string WorkersSeriesName = "Выдачи книг";
        private const string ReadersSeriesName = "Запросы читателей";

        public DiagramsPage(MainWindow main)
        {
            InitializeComponent();
            _main = main;
            _userService = _main.UserService;
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeComboBoxes();
            await UpdateChartAsync();
        }

        private void InitializeComboBoxes()
        {
            ChartTypeComboBox.Items.Clear();

            var chartTypes = new List<KeyValuePair<string, SeriesChartType>>
            {
                new("Столбчатая диаграмма", SeriesChartType.Column),
                new("Линейчатая диаграмма", SeriesChartType.Bar),
                new("Круговая диаграмма", SeriesChartType.Pie),
                new("Диаграмма с областями", SeriesChartType.Area)
            };

            foreach (var type in chartTypes)
            {
                ChartTypeComboBox.Items.Add(new ComboBoxItem
                {
                    Content = type.Key,
                    Tag = type.Value
                });
            }

            ChartTypeComboBox.SelectedIndex = 0;
            StatTypeComboBox.SelectedIndex = 0;
        }

        private async Task UpdateChartAsync()
        {
            if (_isUpdating)
                return;

            _isUpdating = true;

            try
            {
                if (ChartTypeComboBox.SelectedItem == null ||
                    StatTypeComboBox.SelectedItem == null)
                    return;

                _currentStatType =
                    (StatTypeComboBox.SelectedItem as ComboBoxItem)?.Tag?.ToString();

                var chartType =
                    (ChartTypeComboBox.SelectedItem as ComboBoxItem)?.Tag as SeriesChartType?;

                if (string.IsNullOrWhiteSpace(_currentStatType) || chartType == null)
                    return;

                SetLoadingState(true);

                switch (_currentStatType)
                {
                    case "Workers":
                        await BuildWorkersChartAsync(chartType.Value);
                        break;

                    case "Readers":
                        await BuildReadersChartAsync(chartType.Value);
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Ошибка при построении графика:\n{ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                SetLoadingState(false);
                _isUpdating = false;
            }
        }

        private async Task BuildWorkersChartAsync(SeriesChartType chartType)
        {
            var workers = (await _userService.GetWorkersStatsAsync()).ToList();

            MainChart.Series.Clear();
            MainChart.Titles.Clear();

            if (!workers.Any())
            {
                ShowNoDataMessage();
                return;
            }

            var series = new Series(WorkersSeriesName)
            {
                ChartType = chartType,
                Color = System.Drawing.Color.SteelBlue,
                IsValueShownAsLabel = chartType != SeriesChartType.Pie,
                LabelFormat = "N0"
            };

            int totalLoans = 0;

            foreach (var worker in workers)
            {
                series.Points.AddXY(worker.FullName, worker.LoansCount);
                totalLoans += worker.LoansCount;

                if (chartType == SeriesChartType.Pie)
                {
                    series.Points.Last().Label =
                        $"{worker.FullName}\n{worker.LoansCount}";
                }
            }

            MainChart.Series.Add(series);

            ConfigureChart(
                "Статистика работников",
                "Работники",
                "Количество выдач",
                chartType);

            UpdateStatsSummary(
                $"Всего работников: {workers.Count}\n" +
                $"Общее количество выдач: {totalLoans}\n" +
                $"Среднее: {Math.Round((double)totalLoans / workers.Count, 1)}");
        }

        private async Task BuildReadersChartAsync(SeriesChartType chartType)
        {
            var readers = (await _userService.GetReadersStatsAsync()).ToList();

            MainChart.Series.Clear();
            MainChart.Titles.Clear();

            if (!readers.Any())
            {
                ShowNoDataMessage();
                return;
            }

            var series = new Series(ReadersSeriesName)
            {
                ChartType = chartType,
                Color = System.Drawing.Color.SeaGreen,
                IsValueShownAsLabel = chartType != SeriesChartType.Pie,
                LabelFormat = "N0"
            };

            int total = 0;
            int approved = 0;

            foreach (var reader in readers)
            {
                series.Points.AddXY(reader.Category, reader.Count);
                total += reader.Count;

                if (reader.Category.Contains("подтверж"))
                    approved += reader.Count;

                if (chartType == SeriesChartType.Pie)
                {
                    series.Points.Last().Label =
                        $"{reader.Category}\n{reader.Count}";
                }
            }

            MainChart.Series.Add(series);

            ConfigureChart(
                "Статистика запросов читателей",
                "Тип запросов",
                "Количество",
                chartType);

            UpdateStatsSummary(
                $"Всего запросов: {total}\n" +
                $"Подтверждено: {approved}\n" +
                $"Процент: {Math.Round((double)approved / total * 100, 1)}%");
        }

        private void ConfigureChart(
            string title,
            string xAxisTitle,
            string yAxisTitle,
            SeriesChartType chartType)
        {
            MainChart.Titles.Add(new Title(title)
            {
                Font = new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Bold)
            });

            var area = MainChart.ChartAreas["MainArea"];

            if (chartType != SeriesChartType.Pie)
            {
                area.AxisX.Title = xAxisTitle;
                area.AxisY.Title = yAxisTitle;
                area.AxisX.Interval = 1;
                area.AxisX.LabelStyle.Angle =
                    chartType is SeriesChartType.Column or SeriesChartType.Bar ? -45 : 0;
            }
            else
            {
                area.AxisX.Enabled = AxisEnabled.False;
                area.AxisY.Enabled = AxisEnabled.False;
            }
        }

        private void UpdateStatsSummary(string summary)
        {
            StatsSummaryText.Text = summary;
        }

        private void ShowNoDataMessage(string message = "Нет данных для отображения")
        {
            MainChart.Titles.Clear();
            MainChart.Titles.Add(new Title(message)
            {
                Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Italic),
                ForeColor = System.Drawing.Color.Gray
            });

            StatsSummaryText.Text = "";
        }

        private void SetLoadingState(bool isLoading)
        {
            BackButton.IsEnabled = !isLoading;
            StatTypeComboBox.IsEnabled = !isLoading;
            ChartTypeComboBox.IsEnabled = !isLoading;

            if (isLoading)
                StatsSummaryText.Text = "Загрузка данных...";
        }

        private async void StatTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
                await UpdateChartAsync();
        }

        private async void ChartTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
                await UpdateChartAsync();
        }

        private async void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _main.MainFrame.GoBack();
        }
    }
}
