using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Diagnostics;
using System.Linq;

namespace ProcWatcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            GenerateListBoxItems();
        }


        private void ListBoxItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Label label = (Label)sender;
            Process process = (Process)label.FindResource("process");

            StackPanel stackPanel = (StackPanel)VisualTreeHelper.GetParent(label);
            if ( stackPanel.Children.Count >= 2)
            {
                stackPanel.Children.RemoveAt(1);
            }
            else
            {
                TextBlock tb = new TextBlock();
                tb.TextWrapping = TextWrapping.Wrap;

                DateTime procStart = process.StartTime;
                TimeSpan timeSinceStart = DateTime.Now - process.StartTime;

                PerformanceCounter cpu = new PerformanceCounter("Process", "% Processor Time", process.ProcessName);
                PerformanceCounter ram = new PerformanceCounter("Process", "Working Set", process.ProcessName);

                double cpuUsage = cpu.NextValue();

                tb.Text = $"       CPU Usage: {cpuUsage} % \n" +
                    $"       Memory: {ConvertToMegaBytes(ram.NextValue())} megabytes \n" +
                    $"       Running Time: {timeSinceStart.Hours} hours and {timeSinceStart.Minutes} minutes \n" +
                    $"       Start Time: {procStart.Month}.{procStart.Day} - {procStart.Hour}:{procStart.Minute}";
                stackPanel.Children.Add(tb);
            }
        }

        private void GenerateListBoxItems() 
        {
            Process[] processes = Process.GetProcesses();


            foreach (Process process in processes) 
            {
                ListBoxItem item = new ListBoxItem();       

                StackPanel stackPanel = new StackPanel();

                Label label = new Label();

                label.Content = $"[{process.Id}] {process.ProcessName}";

                ResourceDictionary rd = new ResourceDictionary();
                rd.Add("process", process);
                label.Resources = rd;

                label.PreviewMouseLeftButtonDown += ListBoxItem_PreviewMouseLeftButtonDown;

                stackPanel.Children.Add(label);
                item.Content = stackPanel;

                ProcessBox.Items.Add(item);
            }

        }
        private int ConvertToMegaBytes(double number)
        {
            double dividiedNum = number / 1024 / 1024;
            return Convert.ToInt32(Math.Round(dividiedNum));
        }
    }
}
