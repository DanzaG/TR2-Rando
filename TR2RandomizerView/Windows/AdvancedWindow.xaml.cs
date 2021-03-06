﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using TR2RandomizerView.Model;
using TR2RandomizerView.Utilities;

namespace TR2RandomizerView.Windows
{
    /// <summary>
    /// Interaction logic for AdvancedWindow.xaml
    /// </summary>
    public partial class AdvancedWindow : Window
    {
        public static readonly DependencyProperty MainDescriptionProperty = DependencyProperty.Register
        (
            nameof(MainDescription), typeof(string), typeof(AdvancedWindow)
        );

        public static readonly DependencyProperty BoolItemsSourceProperty = DependencyProperty.Register
        (
            nameof(BoolItemsSource), typeof(List<BoolItemControlClass>), typeof(AdvancedWindow)
        );

        public string MainDescription
        {
            get => (string)GetValue(MainDescriptionProperty);
            set => SetValue(MainDescriptionProperty, value);
        }

        public List<BoolItemControlClass> BoolItemsSource
        {
            get => (List<BoolItemControlClass>)GetValue(BoolItemsSourceProperty);
            set => SetValue(BoolItemsSourceProperty, value);
        }

        public AdvancedWindow()
        {
            InitializeComponent();
            _content.DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowUtils.TidyMenu(this);
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Visibility = Visibility.Hidden;
        }
    }
}
