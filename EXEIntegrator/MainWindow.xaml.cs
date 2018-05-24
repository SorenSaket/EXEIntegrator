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
using TsudaKageyu;
using System.Drawing;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using static EXEIntegrator.Integrator;
using System.Threading;
using System.Threading.Tasks;

namespace EXEIntegrator
{
    public partial class MainWindow : Window
    {
        public string SelectedPath
        {
            get
            {
                return IntegrationPathTextbox.Text;
            }
        }

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
            fbd.SelectedPath = IntegrationPathTextbox.Text;
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                IntegrationPathTextbox.Text = fbd.SelectedPath;
            }
        }
        private async void IntegrateButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            await Task.Run(() => Integrator.Analyze(IntegrationPathTextbox.Text));
            /* BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += Integrator.Analyze;
            worker.RunWorkerAsync();*/
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

        private void IntegrateButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            IntegrateButton.Source = new BitmapImage(new Uri(@"/Resources/button_clicked_integrate.png", UriKind.Relative));
        }

        private void IntegrateButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            IntegrateButton.Source = new BitmapImage(new Uri(@"/Resources/button_integrate.png", UriKind.Relative));
        }
    }
}