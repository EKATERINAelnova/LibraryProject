using LibraryProject.Model;
using LibraryProject.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Excel = Microsoft.Office.Interop.Excel;
using System.Linq;


namespace LibraryProject.View.Pages
{
    public partial class LoansPage : Page
    {
        private readonly LoanService _loanService;
        private readonly MainWindow _mainWindow;

        private List<BookLoanInfo> _allLoans = new();
        private List<BookLoanInfo> _filteredLoans = new();

        public LoansPage(MainWindow main)
        {
            _mainWindow = main;
            _loanService = _mainWindow.LoanService;
            InitializeComponent();
            Loaded += LoansPage_Loaded;
        }

        private async void LoansPage_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadLoans();
            FillStatusCombo();
        }

        private async Task LoadLoans()
        {
            _allLoans = await _loanService.GetLoansAsync();
            _filteredLoans = new List<BookLoanInfo>(_allLoans);

            LoansGrid.ItemsSource = _filteredLoans;
        }

        private void FillStatusCombo()
        {
            StatusFilter.Items.Clear();
            StatusFilter.Items.Add("Все");
            StatusFilter.Items.Add("выдана");
            StatusFilter.Items.Add("просрочена");
            StatusFilter.Items.Add("возвращена");
            StatusFilter.SelectedIndex = 0;
        }

        private void StatusFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selected = StatusFilter.SelectedItem.ToString();

            if (selected == "Все")
                _filteredLoans = new List<BookLoanInfo>(_allLoans);
            else
                _filteredLoans = _allLoans
                    .Where(l => l.Status == selected)
                    .ToList();

            LoansGrid.ItemsSource = _filteredLoans;
        }

        private async void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            if (LoansGrid.SelectedItem is BookLoanInfo selectedLoan)
            {
                try
                {
                    await _loanService.ReturnLoanAsync(selectedLoan.LoanId);
                    System.Windows.MessageBox.Show("Книга успешно возвращена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    await LoadLoans(); // обновляем список после возврата
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Выберите запись для возврата книги.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.MainFrame.Navigate(new BooksPage(_mainWindow));
        }

        private void DocsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверяем, есть ли данные для экспорта
                if (_filteredLoans == null || _filteredLoans.Count == 0)
                {
                    System.Windows.MessageBox.Show("Нет данных для экспорта.", "Информация",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Создаем Excel приложение
                var excelApp = new Microsoft.Office.Interop.Excel.Application();
                excelApp.Visible = true;

                // Создаем новую книгу
                var workbook = excelApp.Workbooks.Add();
                var worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Sheets[1];

                // Заголовки столбцов
                string[] headers = { "ID выдачи", "Читатель", "Книга", "Дата выдачи",
                            "Дата возврата", "Статус", "Дней просрочки" };

                // Записываем заголовки
                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1] = headers[i];
                    ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[1, i + 1]).Font.Bold = true;
                    ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[1, i + 1]).Interior.Color =
                        System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                }

                // Заполняем данные
                for (int i = 0; i < _filteredLoans.Count; i++)
                {
                    var loan = _filteredLoans[i];
                    worksheet.Cells[i + 2, 1] = loan.LoanId;
                    worksheet.Cells[i + 2, 2] = loan.Login;
                    worksheet.Cells[i + 2, 3] = loan.BookTitle;
                    worksheet.Cells[i + 2, 4] = loan.LoanDate.ToShortDateString();
                    worksheet.Cells[i + 2, 5] = loan.ReturnDate?.ToShortDateString() ?? "";
                    worksheet.Cells[i + 2, 6] = loan.Status;
                    worksheet.Cells[i + 2, 7] = loan.DueDate;
                }

                // Автоподбор ширины столбцов
                worksheet.Columns.AutoFit();

                // Форматируем таблицу
                var usedRange = worksheet.UsedRange;
                usedRange.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                usedRange.Borders.Weight = Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin;

                // Фиксируем заголовки
                worksheet.Application.ActiveWindow.SplitRow = 1;
                worksheet.Application.ActiveWindow.FreezePanes = true;

                // Выбираем первую ячейку
                worksheet.Cells[1, 1].Select();

                // === Подсчет статусов ===
                var statusGroups = _filteredLoans
                    .GroupBy(l => l.Status)
                    .Select(g => new { Status = g.Key, Count = g.Count() })
                    .ToList();

                // Начальная колонка для сводки (справа от таблицы)
                int summaryColumn = headers.Length + 2;

                // Заголовки сводной таблицы
                worksheet.Cells[1, summaryColumn] = "Статус";
                worksheet.Cells[1, summaryColumn + 1] = "Количество";

                ((Excel.Range)worksheet.Cells[1, summaryColumn]).Font.Bold = true;
                ((Excel.Range)worksheet.Cells[1, summaryColumn + 1]).Font.Bold = true;

                // Заполняем сводку
                for (int i = 0; i < statusGroups.Count; i++)
                {
                    worksheet.Cells[i + 2, summaryColumn] = statusGroups[i].Status;
                    worksheet.Cells[i + 2, summaryColumn + 1] = statusGroups[i].Count;
                }

                // Диапазон данных для диаграммы
                Excel.Range chartRange = worksheet.Range[
                    worksheet.Cells[1, summaryColumn],
                    worksheet.Cells[statusGroups.Count + 1, summaryColumn + 1]
                ];

                // === Создание диаграммы ===
                Excel.ChartObjects chartObjects = (Excel.ChartObjects)worksheet.ChartObjects();
                Excel.ChartObject chartObject = chartObjects.Add(
                    Left: 50,
                    Top: 300,
                    Width: 500,
                    Height: 300
                );

                Excel.Chart chart = chartObject.Chart;
                chart.SetSourceData(chartRange);
                chart.ChartType = Excel.XlChartType.xlPie; // 🔁 Можно заменить на xlColumnClustered

                chart.HasTitle = true;
                chart.ChartTitle.Text = "Распределение выдач по статусам";

                // Подписи данных
                chart.ApplyDataLabels(Excel.XlDataLabelsType.xlDataLabelsShowPercent);


                System.Windows.MessageBox.Show($"Экспорт завершен. Экспортировано {_filteredLoans.Count} записей.",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка при экспорте в Excel: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
