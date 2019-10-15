﻿using System;
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

                TimeSpan timeSinceStart = DateTime.Now - process.StartTime;

                tb.Text = $"       Process Name: {process.ProcessName} | Process Id: {process.Id} | Started: {timeSinceStart.Hours} hours and {timeSinceStart.Minutes} minutes ago | Show Threads";
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
                label.MouseDoubleClick += ListBoxItem_MouseDoubleClick;

                stackPanel.Children.Add(label);
                item.Content = stackPanel;

                ProcessBox.Items.Add(item);
            }

        }

        private void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Label label = (Label)sender;
            Process clickedProcess = (Process)label.FindResource("process");

            Process[] processes = Process.GetProcesses();

            Process process = null;

            foreach (Process proc in processes)
            {
                if (proc.Id == clickedProcess.Id)
                {
                    process = proc;
                }
            }

            StackPanel stackPanel = (StackPanel)VisualTreeHelper.GetParent(label);

            stackPanel.Children.RemoveAt(1);

            TextBlock tb = new TextBlock();
            tb.TextWrapping = TextWrapping.Wrap;

            TimeSpan timeSinceStart = DateTime.Now - process.StartTime;

            tb.Text = $"       Process Name: {process.ProcessName} | Process Id: {process.Id} | Started: {timeSinceStart.Hours} hours and {timeSinceStart.Minutes} minutes ago | Show Threads";
            stackPanel.Children.Add(tb);
        }
    }
}
