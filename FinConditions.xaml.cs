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
using System.Windows.Shapes;

using Telerik.Windows.Controls;

namespace UrbanHelp
{
    /// <summary>
    /// Логика взаимодействия для FinConditions.xaml
    /// </summary>
    public partial class FinConditions : Window
    {
       
        public Person Persone { get; set; }
        public FinConditions()
        {
          
            LocalizationManager.Manager = new CustomLocalizationManager();
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
     
            rgvFinCond.ItemsSource = Persone.FinConditions.OrderBy(f=>f.Year).ToList();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Hide();
        }

        private void RadButton_Click_1(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Hide();
        }
    }
}
