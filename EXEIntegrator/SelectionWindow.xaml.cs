using EXEIntegrator.Scripts;
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
        ApplicationInfoContainer[] applications;

        // Window Initialization
        public SelectionWindow()
        {
            InitializeComponent();
        }
        // 
        public void SetAppData(ApplicationInfoContainer[] applicationInfoContainer)
        {
            applications = applicationInfoContainer;
            ApplicationTable.ItemsSource = applications;
            Show();
        }
        // 
        private void ApplicationGridImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine(sender.GetType().ToString());
            Console.WriteLine(sender.ToString());
        }

        private void ApplicationGridImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is Image img)
            {
                System.Windows.Forms.OpenFileDialog fd = new System.Windows.Forms.OpenFileDialog
                {
                    Filter = "Icon (.ico)|*.ico" //+ "Image file|*.png|" + "Application|*.exe|"
                };
                System.Windows.Forms.DialogResult result = fd.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK || result == System.Windows.Forms.DialogResult.Yes)
                {
                    img.Source = ImageUtilities.ToImageSource(fd.FileName);
                } 
            }
        }
    }
}