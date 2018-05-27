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
            WindowManager.mainWindow = this;
            //Example.test();
            /*Console.WriteLine(StringHelper.MatchPercentage(new string[] { "ICEpower" }, "AudioWizard"));
            Console.WriteLine(StringHelper.MatchPercentage(new string[] { "Cisco" }, "CiscoEapFast"));
            Console.WriteLine(StringHelper.MatchPercentage(new string[] { "Cisco", "Cisco EAP-FAST Module" }, "CiscoEapFast"));
            Console.WriteLine(StringHelper.MatchPercentage(new string[] { "Asus", "APRP" }, "ASUSProductReg"));
            Console.WriteLine(StringHelper.MatchPercentage(new string[] { "USBChargerPlus" }, "USBChargerPlus.exe"));*/
        }
    
        // -------- Callers --------
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

        private void IntegrateButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            WindowManager.loadingWindow = new LoadingWindow
            {
                Top = Top,
                Left = Left
            };
            WindowManager.selectionWindow = new SelectionWindow();
            Analyze(IntegrationPathTextbox.Text);
            WindowManager.mainWindow.Close();

            //selectionWindow.Show();

            //
            /*  Dispatcher.Invoke(new Action(() =>
            {
                 SelectionWindow selectionWindow = new SelectionWindow();
                 selectionWindow.InitializeSelectionWindow(GetApplicationInfos(IntegrationPathTextbox.Text, loadingWindow));
                 //Call method or controls of window here
             }));*/
            //selectionWindow.InitializeSelectionWindow(GetApplicationInfos(IntegrationPathTextbox.Text, loadingWindow));
            /* Thread thread = new Thread(() => 
             {



             });
             thread.Start();*/

            /*Thread thread = new Thread(() => {
                SelectionWindow selectionWindow = new SelectionWindow();
                selectionWindow.InitializeSelectionWindow(GetApplicationInfos(IntegrationPathTextbox.Text));

            });
            thread.Start();*/
            //Task.Run(() => Integrator.Analyze(IntegrationPathTextbox.Text));
            //Dispatcher.BeginInvoke(new ThreadStart((Integrator.Analyze(IntegrationPathTextbox.Text))));
            //await Task.Run(() => Integrator.Analyze(IntegrationPathTextbox.Text));
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


    public class Example
    {
        public static void test()
        {
            ShowThreadInfo("Application");

            var t = Task.Run(() => ShowThreadInfo("Task"));
            t.Wait();
        }

        static void ShowThreadInfo(String s)
        {
            Console.WriteLine("{0} Thread ID: {1}",
                              s, Thread.CurrentThread.ManagedThreadId);
        }
    }
}