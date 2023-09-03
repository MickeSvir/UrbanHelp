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
using Telerik.Windows.Documents.Spreadsheet.Expressions.Functions;

namespace UrbanHelp
{
    /// <summary>
    /// Логика взаимодействия для frmDegree.xaml
    /// </summary>
    public partial class frmDegree : Window
    {
        public List<Values> Degree 
        { 
            set => rgvSelect.ItemsSource=value; 
        }
        public string Result { get; private set; }
        public frmDegree()
        {
            InitializeComponent();
          }

        private void rgvSelect_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var sel = (rgvSelect.SelectedItem as Values).Value;
            this.DialogResult = true;
            Result= sel;
            this.Hide();
        }

        private void cmbDegreeCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Hide();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            rgvSelect.Select(rgvSelect.Items[0].ToEnumerable());
        }
    }
}
