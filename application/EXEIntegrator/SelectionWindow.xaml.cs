using EXEIntegrator.Scripts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
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
using TsudaKageyu;
using static EXEIntegrator.Integrator;
namespace EXEIntegrator
{
    public partial class SelectionWindow : Window
    {
        List<ApplicationInfoContainer> applications;

        // Window Initialization
        public SelectionWindow()
        {
            InitializeComponent();
        }
        // 
        public void SetAppData(List<ApplicationInfoContainer> applicationInfoContainer)
        {
            applications = new List<ApplicationInfoContainer>();
            for (int x = 0; x < applicationInfoContainer.Count; x++)
            {
                if(applicationInfoContainer[x].IsCompany)
                {
                    for (int y = 0; y < applicationInfoContainer[x].children.Count; y++)
                    {
                        if(applicationInfoContainer[x].children[y].ApplicationExecutable != null)
                            applications.Add(applicationInfoContainer[x].children[y]);
                    }
                }
                else
                {
                    if (applicationInfoContainer[x].ApplicationExecutable != null)
                        applications.Add(applicationInfoContainer[x]);
                }
            }


            ListCollectionView collectionView = new ListCollectionView(applications);
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("ParentName"));
            ApplicationTable.ItemsSource = collectionView;

            /*applications = applicationInfoContainer;
            ApplicationTable.ItemsSource = applications;*/
            Show();
        }

        public void CreateLogFile(List<ApplicationInfoContainer> infoContainers)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\intergrationLog" + /*DateTime.Now +*/  ".txt";
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(path))
            {
                for (int i = 0; i < infoContainers.Count; i++)
                {
                    if (infoContainers[i].IsCompany)
                    {
                        sw.WriteLine(infoContainers[i].ApplicationName + " - " + infoContainers[i].ApplicationDirectory.FullName);
                        for (int x = 0; x < infoContainers[i].children.Count; x++)
                        {
                            string line = "\t" + infoContainers[i].children[x].ApplicationName + " - " + infoContainers[i].children[x].ApplicationPath + " - ";
                            for (int y = 0; y < infoContainers[i].children[x].debug.keywords.Count; y++)
                            {
                                line += infoContainers[i].children[x].debug.keywords[y] + ".";
                            }
                            line += infoContainers[i].children[x].debug.keywords.Count;

                           sw.WriteLine(line);
                        }
                    }
                    else
                    {
                        sw.WriteLine(infoContainers[i].ApplicationName + " - " + infoContainers[i].ApplicationPath);
                    }
                    
                }
            }
            Process.Start(@"notepad.exe", path);
        }

        //
        private void ApplicationGridImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is Image img)
            {
                System.Windows.Forms.OpenFileDialog fd = new System.Windows.Forms.OpenFileDialog
                {
                    Filter = "Icon (.ico)|*.ico|Image files (.png)|*.png|Application (.exe)|*.exe"
                };
                int index = applications[ApplicationTable.SelectedIndex].ApplicationPath.LastIndexOf(@"\");
                if (index > 0)
                    fd.InitialDirectory = applications[ApplicationTable.SelectedIndex].ApplicationPath.Substring(0, index + 1);

                System.Windows.Forms.DialogResult result = fd.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK || result == System.Windows.Forms.DialogResult.Yes)
                {
                    IconExtractor iconExtractor = new IconExtractor(fd.FileName);
                    // If any icon is found
                    if (iconExtractor.Count > 0)
                    {
                        // Gets the largest icon from the exe
                        img.Source = iconExtractor.GetIcon(0).ToImageSource();
                    }

                    //img.Source = ImageUtilities.ToImageSource(fd.FileName);
                } 
            }
        }
        //
        private void ApplicationPathTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(sender is TextBox tb)
            {
                System.Windows.Forms.OpenFileDialog fd = new System.Windows.Forms.OpenFileDialog
                {
                    Filter = "Application (.exe)|*.exe"
                };
                int index = tb.Text.LastIndexOf(@"\");
                if (index > 0)
                    fd.InitialDirectory = tb.Text.Substring(0, index + 1);

                System.Windows.Forms.DialogResult result = fd.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK || result == System.Windows.Forms.DialogResult.Yes)
                {
                    tb.Text = fd.FileName;
                }
            }
        }
        //
        private void IntegrateButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            CreateLogFile(applications);
            
            /*MessageBoxResult result = System.Windows.MessageBox.Show("Are you sure you want to integrate all applications in " + "the selected folder" + "?" + Environment.NewLine + "This cannot be undone", "EXE Integrator", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
                Integrate(applications.UpdateVariables());*/
        }
        //
        private void CancelButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Hide();
            WindowManager.mainWindow.Show();
        }
    }
}