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
                CreateTextBlock(stackPanel, process);
            }
        }

        private void GenerateListBoxItems() 
        {
            Process[] processes = Process.GetProcesses();


            foreach (Process process in processes) 
            {
                ListBoxItem item = new ListBoxItem();       

                StackPanel stackPanel = new StackPanel();

                CreateLabel(stackPanel, process);
                item.Content = stackPanel;

                ProcessBox.Items.Add(item);
            }

        }

        private void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Label label = (Label)sender;
            Process clickedProcess = (Process)label.FindResource("process");

            Process process = GetUpdatedProcess(clickedProcess);

            StackPanel stackPanel = (StackPanel)VisualTreeHelper.GetParent(label);

            if (GetUpdatedProcess(clickedProcess) == null)
            {
                MessageBox.Show("Process isn't running anymore.");
            }
            else
            {
                stackPanel.Children.Clear();

                CreateLabel(stackPanel, process);

                CreateTextBlock(stackPanel, process);
            }
        }

        private void CreateTextBlock(StackPanel stackPanel, Process process)
        {
            TextBlock tb = new TextBlock();
            tb.TextWrapping = TextWrapping.Wrap;

            try
            {
                TimeSpan timeSinceStart = DateTime.Now - process.StartTime;
                tb.Text = $"       Process Name: {process.ProcessName} | Process Id: {process.Id} | Started: {timeSinceStart.Hours} hours and {timeSinceStart.Minutes} minutes ago | Show Threads";
            }
            catch
            {
                return;
            }
            stackPanel.Children.Add(tb);
        }

        private void CreateLabel(StackPanel stackPanel, Process process)
        {
            Label label = new Label();

            label.Content = $"[{process.Id}] {process.ProcessName}";

            ResourceDictionary rd = new ResourceDictionary();
            rd.Add("process", process);
            label.Resources = rd;

            label.PreviewMouseLeftButtonDown += ListBoxItem_PreviewMouseLeftButtonDown;
            label.MouseDoubleClick += ListBoxItem_MouseDoubleClick;

            stackPanel.Children.Add(label);
        }

        private Process GetUpdatedProcess(Process clickedProcess)
        {
            Process[] processes = Process.GetProcesses();

            foreach (Process process in processes)
            {
                if (process.Id == clickedProcess.Id)
                {
                    return process;
                }
            }

            return null;
        }
    }
}
