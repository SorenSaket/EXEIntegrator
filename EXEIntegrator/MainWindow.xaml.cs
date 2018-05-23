using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Forms;
using IWshRuntimeLibrary;
using System.Diagnostics;
using System.Text.RegularExpressions;
using static EXEIntegrator.Integrator;
namespace EXEIntegrator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            

            /*Console.WriteLine(StringHelper.MatchPercentage(new string[] { "ICEpower" }, "AudioWizard"));
            Console.WriteLine(StringHelper.MatchPercentage(new string[] { "Cisco" }, "CiscoEapFast"));
            Console.WriteLine(StringHelper.MatchPercentage(new string[] { "Cisco", "Cisco EAP-FAST Module" }, "CiscoEapFast"));
            Console.WriteLine(StringHelper.MatchPercentage(new string[] { "Asus", "APRP" }, "ASUSProductReg"));
            Console.WriteLine(StringHelper.MatchPercentage(new string[] { "USBChargerPlus" }, "USBChargerPlus.exe"));*/
        }


        // -------- Callers --------
        private void IntegrationPathTextbox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                IntegrationPathTextbox.Text = fbd.SelectedPath;
            }
        }
        private void IntegrateButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SelectionWindow selectionWindow = new SelectionWindow();
            selectionWindow.InitializeSelectionWindow(GetApplicationInfos(IntegrationPathTextbox.Text));
            this.Close();

            /*MessageBoxResult result = System.Windows.MessageBox.Show("Are you sure you want to integrate all applications in " + IntegrationPathTextbox.Text + "?" + Environment.NewLine + "This cannot be undone", "EXE Integrator", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
                Integrate(IntegrationPathTextbox.Text);*/
        }
        private void MinimizeButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }
        private void CloseButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
        private void IntegrationPath_textbox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }
        private void Background_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}