using BinCompletionAlgorithm.Algorithm;
using BinCompletionAlgorithm.Models;
using BinCompletionAlgorithm.Services;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BinCompletionAlgorithm
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Bin> WorkingBins { get; set; } = new();
        public ObservableCollection<Item> WorkingElts { get; set; } = new();
        public bool CanExport { get; set; }
        public List<Bin> res { get; private set; } = new();
        CSVUtility CSVUtil = new CSVUtility();
        public MainWindow()
        {
            InitializeComponent();
        }

        void ShowBins()
        {
            WorkingBins = new(WorkingBins.OrderByDescending(b => b.Capacity));
            BinList.ItemsSource = WorkingBins;
        }

        void ShowItems()
        {
            WorkingElts = new(WorkingElts.OrderByDescending(b => b.Value));
            ItemList.ItemsSource = WorkingElts;
        }

        bool CheckForReadiness()
        {
            return WorkingBins.Any() && WorkingElts.Any();
        }

        private void BinAddButtonClicked(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(BinSize.Text))
            {
                var NewBin = new Bin()
                {
                    Capacity = int.Parse(BinSize.Text),
                    Label = string.IsNullOrEmpty(BinLabel.Text) ? $"b{BinSize.Text}" : BinLabel.Text
                };
                WorkingBins.Add(NewBin);
                ShowBins();
                RunProgramButton.IsEnabled = CheckForReadiness();
            }
        }

        private void ItemAddButtonClicked(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(ItemSize.Text))
            {
                foreach (var item in ItemSize.Text.Split(",", StringSplitOptions.RemoveEmptyEntries))
                {
                    var NewItem = new Item()
                    {
                        Value = int.Parse(item),
                        Label = "item-" + item
                    };
                    WorkingElts.Add(NewItem);
                }
                ShowItems();
                RunProgramButton.IsEnabled = CheckForReadiness();
            }
        }

        private void CheckForNumericInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ResetAllValues(object sender, RoutedEventArgs e)
        {
            WorkingElts.Clear();
            WorkingBins.Clear();
            res.Clear();
            ResultList.ItemsSource = null;
            SaveToFileButton.IsEnabled = false;
            ShowBins();
            ShowItems();
            RunProgramButton.IsEnabled = CheckForReadiness();
        }

        private async void RunProgramButton_Click(object sender, RoutedEventArgs e)
        {
            AlgorithmClass algorithm = new()
            {
                InitialBins = WorkingBins.ToList(),
                InitialElements = WorkingElts.ToList()
            };
            var watch = new System.Diagnostics.Stopwatch();
            
                watch.Start();

                res = await algorithm.Execute()  ?? new();

                watch.Stop();
                SaveToFileButton.IsEnabled = true;
                RunTimeValue.Content = $"Execution Time: {watch.ElapsedMilliseconds} ms";
                ResultList.ItemsSource = res;
                MinBinsValue.Content = res.Count();
            
        }

        private async void SaveToFileButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog savefileDlg = new Microsoft.Win32.SaveFileDialog();
            savefileDlg.DefaultExt = ".csv";
            savefileDlg.Filter = "CSV documents (.csv)|*.csv";
            var result = savefileDlg.ShowDialog();
            if (savefileDlg.FileName != "")
            {
                await CSVUtil.WriteCSVResult(savefileDlg.FileName, res);
            }
        }

        private void SelectItemFilesClick(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
            openFileDlg.Title = "Open File Containing Bin Items";
            openFileDlg.DefaultExt = ".csv";
            openFileDlg.Filter = "CSV documents (.csv)|*.csv";
            // Launch OpenFileDialog by calling ShowDialog method
            var result = openFileDlg.ShowDialog();
            // Get the selected file name and display in a TextBox.
            // Load content of file in a TextBlock
            if (result == true)
            {
                BinsCSVName.Content = openFileDlg.SafeFileName;
                var items = CSVUtil.ReadCSVItems(openFileDlg.FileName);
                if (items is not null)
                    WorkingElts = new(items);
            }
            ShowItems();
            RunProgramButton.IsEnabled = CheckForReadiness();
        }

        private void SelectBinFilesClick(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();

            openFileDlg.DefaultExt = ".csv";
            openFileDlg.Filter = "CSV documents (.csv)|*.csv";
            // Launch OpenFileDialog by calling ShowDialog method
            var result = openFileDlg.ShowDialog();
            // Get the selected file name and display in a TextBox.
            // Load content of file in a TextBlock
            if (result == true)
            {
                BinsCSVName.Content = openFileDlg.SafeFileName;
                var items = CSVUtil.ReadCSVBins(openFileDlg.FileName);
                if (items is not null)
                    WorkingBins = new(items);
            }
            ShowBins();
            RunProgramButton.IsEnabled = CheckForReadiness();
        }
    }
}
