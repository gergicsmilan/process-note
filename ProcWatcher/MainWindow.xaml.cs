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
                tb.Text = $"       Process Name: {process.ProcessName} | Process Id: {process.Id} | Started At: {process.StartTime.Hour} hours ago | Show Threads";
                stackPanel.Children.Add(tb);
            }
        }

        private void GenerateListBoxItems() 
        {
            Process[] processes = Process.GetProcesses();
            HashSet<Process> processSet = processes.ToHashSet();


            foreach (Process process in processSet) 
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

    }
}
