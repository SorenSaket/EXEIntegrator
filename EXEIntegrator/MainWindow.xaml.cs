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
using EXEIntegrator.Scripts;

namespace EXEIntegrator
{
    public partial class MainWindow : Window
    {
        // Window initialization
        public MainWindow()
        {
            InitializeComponent();
            WindowManager.mainWindow = this;
        }
        /// -------- Callers --------
        //
        private void IntegrationPathTextbox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog
            {
                SelectedPath = IntegrationPathTextbox.Text
            };

            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                IntegrationPathTextbox.Text = fbd.SelectedPath;
            }
        }
        //  
        private void IntegrateButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            WindowManager.loadingWindow = new LoadingWindow
            {
                Top = Top,
                Left = Left
            };
            WindowManager.loadingWindow.Show();
            WindowManager.selectionWindow = new SelectionWindow();
            Integrator.Analyze(IntegrationPathTextbox.Text);
            Close();
        }
        //
        private void MinimizeButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }
        //
        private void CloseButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
        //
        private void Background_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        //
        private void IntegrateButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            IntegrateButton.Source = new BitmapImage(new Uri(@"/Resources/button_clicked_integrate.png", UriKind.Relative));
        }
        //
        private void IntegrateButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            IntegrateButton.Source = new BitmapImage(new Uri(@"/Resources/button_integrate.png", UriKind.Relative));
        }
    }
}