using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static EXEIntegrator.Integrator;

namespace EXEIntegrator.Scripts
{
    public static class WindowManager
    {
        public static LoadingWindow loadingWindow;
        public static MainWindow mainWindow;
        public static SelectionWindow selectionWindow;

        public static ApplicationInfoContainer[] globalApplicationInfoContainers;

        public static void InitializeSelectionWindow()
        {
            selectionWindow.SetAppData(globalApplicationInfoContainers);
            loadingWindow.Close();

            /*selectionWindow = new SelectionWindow();
            selectionWindow.SetAppData(globalApplicationInfoContainers);
            selectionWindow.Show();
            loadingWindow.Close();*/
            /*
            ApplicationInfoContainer[] temp = applicationInfoContainers;
            selectionWindow.SetAppData(temp);*/
        }
    }
}
