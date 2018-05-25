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
using static EXEIntegrator.Integrator;
namespace EXEIntegrator
{
    
    public partial class SelectionWindow : Window
    {
        //
        public SelectionWindow()
        {
            InitializeComponent();
        }
        //
        public void InitializeSelectionWindow(ApplicationInfoContainer[] applications)
        {
            ApplicationTable.ItemsSource = applications;
        }
        //
        private void ApplicationGridImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine(sender.GetType().ToString());
            Console.WriteLine(sender.ToString());
        }
    }
}