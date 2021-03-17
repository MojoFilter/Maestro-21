﻿using System.Windows;

namespace MaestroCommander.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(ICommanderViewModel vm)
        {
            InitializeComponent();
            this.Content = vm;
        }
    }
}
