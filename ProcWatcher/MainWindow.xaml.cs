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
using System.Drawing;
using System.IO;

namespace ProcWatcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            GenerateListBoxItems();
        }


        private void ListBoxItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StackPanel senderStack = (StackPanel)sender;
            Process process = (Process)senderStack.FindResource("process");

            StackPanel stackPanel = (StackPanel)VisualTreeHelper.GetParent(senderStack);
            if ( stackPanel.Children.Count >= 2)
            {
                stackPanel.Children.RemoveAt(1);
            }
            else
            {
            CreateTextBlock(stackPanel, process);

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

            StackPanel processStack = new StackPanel();
            processStack.Orientation = Orientation.Horizontal;

            TextBlock text = new TextBlock();
            text.Text = $" [{process.Id}] {process.ProcessName}";

            System.Windows.Controls.Image img = new System.Windows.Controls.Image();
            img.Source = GetImageSourceForIcon(GetProcessIconImage(process));
            img.Width = 15;
            img.Height = 15;
            img.Stretch = Stretch.Fill;

            processStack.Children.Add(img);
            processStack.Children.Add(text);


            ResourceDictionary rd = new ResourceDictionary();
            rd.Add("process", process);
            processStack.Resources = rd;

            processStack.PreviewMouseLeftButtonDown += ListBoxItem_PreviewMouseLeftButtonDown;

            stackPanel.Children.Add(processStack);
            item.Content = stackPanel;

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
        private int ConvertToMegaBytes(double number)
        {
            double dividiedNum = number / 1024 / 1024;
            return Convert.ToInt32(Math.Round(dividiedNum));
        }

        private Bitmap GetProcessIconImage(Process process) 
        {
            try
            {
                Icon ico = System.Drawing.Icon.ExtractAssociatedIcon(process.MainModule.FileName);
                return ico.ToBitmap();
            } catch (System.ComponentModel.Win32Exception e)
            {
                return new Bitmap(1, 1);
            }
            
        }

        private ImageSource GetImageSourceForIcon(Bitmap bmp) 
        {
            System.Windows.Controls.Image image = new System.Windows.Controls.Image();
            using (var ms = new MemoryStream())
            {
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Position = 0;

                var bi = new BitmapImage();
                bi.BeginInit();
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.StreamSource = ms;
                bi.EndInit();
                return image.Source = bi;
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
