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
using static EXEIntegrator.Scripts.MathAddons;
using System.Threading;
using System.Threading.Tasks;

namespace EXEIntegrator
{
    public static class Integrator
    {
        public static LoadingWindow loadingWindow;
        public static SelectionWindow selectionWindow;
        public static void Analyze(string path, LoadingWindow _loadingWindow, SelectionWindow _selectionWindow)
        {
            loadingWindow = _loadingWindow;
            selectionWindow = _selectionWindow;

            BackgroundWorker worker = new BackgroundWorker
            {
                WorkerReportsProgress = true
            };
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync(path);

            loadingWindow.Show();
            loadingWindow.Focus();

            
            /*this.Close();*/

            /*MessageBoxResult result = System.Windows.MessageBox.Show("Are you sure you want to integrate all applications in " + IntegrationPathTextbox.Text + "?" + Environment.NewLine + "This cannot be undone", "EXE Integrator", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
                Integrate(IntegrationPathTextbox.Text);*/
        }


        static void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            // The directory to look for applications
            DirectoryInfo d = new DirectoryInfo(e.Argument.ToString());
            // All sub folders
            DirectoryInfo[] dirs = d.GetDirectories();

            List<ApplicationInfoContainer> infoContainers = new List<ApplicationInfoContainer>();

            // For each subfolder
            for (int i = 0; i < dirs.Length; i++)
            {
                (sender as BackgroundWorker).ReportProgress(Convert.ToInt32((i * 100 / dirs.Length)), "Analyzing " + dirs[i].Name + "...");
                ApplicationInfoContainer temp = new ApplicationInfoContainer(dirs[i]);
                // Create application refrence
                infoContainers.Add(temp);
                if (temp.ApplicationExecutable != null)
                    Console.WriteLine("Integrating " + temp.ApplicationExecutable.Name + " with " + temp.ApplicationDirectory.FullName);
                System.Threading.Thread.Sleep(1);


            }
            e.Result = infoContainers.ToArray();
        }

        static void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Console.WriteLine(e.ProgressPercentage + e.UserState.ToString());
            loadingWindow.SetLoad(e.ProgressPercentage, e.UserState.ToString());
        }

        static void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            selectionWindow.InitializeSelectionWindow((ApplicationInfoContainer[]) e.Result);
            loadingWindow.Close();
            selectionWindow.Show();
            //System.Windows.MessageBox.Show("Analyzed Applications", "EXE Integrator");
        }


        public static ApplicationInfoContainer[] GetApplicationInfos(string path)
        {
            // The directory to look for applications
            DirectoryInfo d = new DirectoryInfo(path);
            // All sub folders
            DirectoryInfo[] dirs = d.GetDirectories();

            List<ApplicationInfoContainer> infoContainers = new List<ApplicationInfoContainer>();

            // For each subfolder
            for (int i = 0; i < dirs.Length; i++)
            {
                //loadingWindow.SetLoad(i / dirs.Length * 100, "Analyzing " + dirs[i].Name + "...");
                ApplicationInfoContainer temp = new ApplicationInfoContainer(dirs[i]);
                // Create application refrence
                infoContainers.Add(temp);
                if(temp.ApplicationExecutable != null)
                    Console.WriteLine("Integrating " + temp.ApplicationExecutable.Name + " with " + temp.ApplicationDirectory.FullName);
                /*CreateShortcut(app.applicationName, app.applicationEXE);
                SetFolderIcon(dirs[i].FullName, app.applicationEXE.FullName);*/

            }
            //loadingWindow.Close();
            // System.Windows.MessageBox.Show("Applications intergrated", "EXE Integrator");
            return infoContainers.ToArray();
        }
        /*
        // -------- Functions --------
        static void Integrate(string path)
        {
            // The directory to look for applications
            DirectoryInfo d = new DirectoryInfo(path);
            // All sub folders
            DirectoryInfo[] dirs = d.GetDirectories();

            // For each subfolder
            for (int i = 0; i < dirs.Length; i++)
            {
                // Create application refrence
                ApplicationInfoContainer app = new ApplicationInfoContainer(dirs[i].Name, GetExecutable(dirs[i]), dirs[i]);


                if (app.ApplicationEXE != null)
                {
                    Console.WriteLine("Integrating " + app.ApplicationEXE.Name + " with " + app.ApplicationDirectory.FullName);
                    CreateShortcut(app.applicationName, app.applicationEXE);
                    SetFolderIcon(dirs[i].FullName, app.applicationEXE.FullName);
                }
            }
            System.Windows.MessageBox.Show("Applications intergrated", "EXE Integrator");
        }*/

        // -------- Statics --------
        // Creates a shortcut in start menu folder
        private static void CreateShortcut(string shortcutName, FileInfo targetFile)
        {
            string shortcutLocation = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\", shortcutName + ".lnk");
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);

            shortcut.Description = FileVersionInfo.GetVersionInfo(targetFile.FullName).FileDescription;   // The description of the shortcut
            shortcut.IconLocation = targetFile.FullName;        // The icon of the shortcut
            shortcut.TargetPath = targetFile.FullName;          // The path of the file that will launch when the shortcut is run
            shortcut.Save();                                    // Save the shortcut
        }
        private static void SetFolderIcon(string path, string iconPath)
        {
            /* Remove any existing desktop.ini */
            if (System.IO.File.Exists(path + @"\desktop.ini")) System.IO.File.Delete(path + @"\desktop.ini");

            /* Write the desktop.ini */
            StreamWriter sw = System.IO.File.CreateText(path + @"\desktop.ini");
            sw.WriteLine("[.ShellClassInfo]");
            sw.WriteLine("IconResource=" + iconPath + ",0");
            sw.Close();
            sw.Dispose();

            /* Set the desktop.ini to be hidden */
            System.IO.File.SetAttributes(path + @"\desktop.ini", System.IO.File.GetAttributes(path + @"\desktop.ini") | FileAttributes.Hidden);

            /* Set the path to system */
            System.IO.File.SetAttributes(path, System.IO.File.GetAttributes(path) | FileAttributes.System);
        }

        // Get the matching Executable for the application
        private static FileInfo GetExecutable(DirectoryInfo applicationFolder)
        {
            ApplicationInfoContainer temp = new ApplicationInfoContainer();

            List<ApplicationInfoContainer> executableContenders = new List<ApplicationInfoContainer>();

            List<string> keywords = new List<string>() { applicationFolder.Name };

            // Check dir
            //
            if (applicationFolder.GetFiles().Count() == 0)
            {
                switch (applicationFolder.GetDirectories().Count())
                {
                    case 0:
                        return null;
                    case 1:
                        applicationFolder = applicationFolder.GetDirectories()[0];
                        keywords.Add(applicationFolder.Name);
                        break;
                    default:
                        break;
                }
            }

            foreach (var file in applicationFolder.GetFiles("*.exe"))
            {
                executableContenders.Add(new ApplicationInfoContainer( file, StringMatcher.MatchPercentage(keywords.ToArray(), file.Name)));
            }

            if (Directory.Exists(applicationFolder + @"/bin"))
            {
                foreach (var file in new DirectoryInfo(applicationFolder + @"/bin").GetFiles("*.exe"))
                {
                    executableContenders.Add(new ApplicationInfoContainer(file, StringMatcher.MatchPercentage(keywords.ToArray(), file.Name)));
                }
            }

            if(HasFoundMatch(executableContenders))
                return CheckForBestMatch(executableContenders);

            executableContenders = new List<ApplicationInfoContainer>();
            foreach (var file in applicationFolder.GetFiles("*.exe", SearchOption.AllDirectories))
            {
                executableContenders.Add(new ApplicationInfoContainer(file, StringMatcher.MatchPercentage(keywords.ToArray(), file.Name)));
            }
            if (executableContenders.Count > 0)
                return CheckForBestMatch(executableContenders);

            return null;
        }
        private static ApplicationInfoContainer GetApplicationExecutable(DirectoryInfo applicationFolder)
        {
            ApplicationInfoContainer temp = new ApplicationInfoContainer
            {
                ApplicationDirectory = applicationFolder,
                ApplicationName = applicationFolder.Name
            };

            List<ApplicationInfoContainer> executableContenders = new List<ApplicationInfoContainer>();

            List<string> keywords = new List<string>() { applicationFolder.Name };

            // Check dir
            if (applicationFolder.GetFiles().Count() == 0)
            {
                switch (applicationFolder.GetDirectories().Count())
                {
                    case 0:
                        return null;
                    case 1:
                        applicationFolder = temp.ApplicationDirectory = applicationFolder.GetDirectories()[0];
                        keywords.Add(applicationFolder.Name);
                        temp.ApplicationName = applicationFolder.Name;
                        break;
                    default:
                        break;
                }
            }

            foreach (var file in applicationFolder.GetFiles("*.exe"))
            {
                executableContenders.Add(new ApplicationInfoContainer(file, StringMatcher.MatchPercentage(keywords.ToArray(), file.Name)));
            }

            if (HasFoundMatch(executableContenders))
            {
                temp.ApplicationExecutable = CheckForBestMatch(executableContenders);
                
                return temp;
            }

            if (Directory.Exists(applicationFolder + @"/bin"))
            {
                foreach (var file in new DirectoryInfo(applicationFolder + @"/bin").GetFiles("*.exe"))
                {
                    executableContenders.Add(new ApplicationInfoContainer(file, StringMatcher.MatchPercentage(keywords.ToArray(), file.Name)));
                }
            }

            if (HasFoundMatch(executableContenders))
            {
                temp.ApplicationExecutable = CheckForBestMatch(executableContenders);
                return temp;
            }

            executableContenders = new List<ApplicationInfoContainer>();
            foreach (var file in applicationFolder.GetFiles("*.exe", SearchOption.AllDirectories))
            {
                executableContenders.Add(new ApplicationInfoContainer(file, StringMatcher.MatchPercentage(keywords.ToArray(), file.Name)));
            }


            if (executableContenders.Count > 0)
            {
                temp.ApplicationExecutable = CheckForBestMatch(executableContenders);
                return temp;
            }

            return null;
        }
        //
        private static FileInfo CheckForMatch(List<ApplicationInfoContainer> executableContenders, float threshold)
        {
            executableContenders = executableContenders.OrderBy(o => o.MatchPercentage).ToList();

            for (int i = 0; i < executableContenders.Count; i++)
            {
                if (executableContenders[i].MatchPercentage >= threshold)
                    return executableContenders[i].ApplicationExecutable;
            }
            return null;
        }
        private static FileInfo CheckForBestMatch(List<ApplicationInfoContainer> executableContenders)
        {
            List<ApplicationInfoContainer> temp = executableContenders.OrderByDescending(o => o.MatchPercentage).ThenBy(x => x.ApplicationExecutable.Name.Length).ToList();
            for (int i = 0; i < temp.Count; i++)
            {
                Console.WriteLine("Application Match : " + temp[i].ApplicationExecutable.Name + " with " + temp[i].MatchPercentage*100 + "%");
            }
            
            return temp[0].ApplicationExecutable;
        }
        private static float CheckForBestMatchPercentage(List<ApplicationInfoContainer> executableContenders)
        {
            List<ApplicationInfoContainer> temp = executableContenders.OrderByDescending(o => o.MatchPercentage).ThenBy(x => x.ApplicationExecutable.Name.Length).ToList();
            return temp[0].MatchPercentage;
        }
        private static bool HasFoundMatch(List<ApplicationInfoContainer> executableContenders)
        {
            if(executableContenders.Count > 0)
            {
                List<ApplicationInfoContainer> temp = executableContenders.OrderByDescending(o => o.MatchPercentage).ThenBy(x => x.ApplicationExecutable.Name.Length).ToList();
                float scoreToBeat = 0;
                int contenders = 3.Clamp(0, temp.Count - 1);

                for (int i = 1; i < contenders; i++)
                {
                    scoreToBeat += temp[i].MatchPercentage;
                }

                if (temp[0].MatchPercentage > scoreToBeat / contenders * 1.1f)
                    return true;
            }
            return false;
        }
        
        /*
        public class ApplicationMatch
        {
            public float matchPercentage;
            public FileInfo executable;

            public ApplicationMatch( FileInfo _executable, float _matchPercentage)
            {
                matchPercentage = _matchPercentage;
                executable = _executable;
            }
        }*/


        private static ApplicationInfoContainer FillInfo(this ApplicationInfoContainer aIC)
        {
            if(aIC.ApplicationDirectory == null)
            {
                if(aIC.ApplicationExecutable == null)
                {
                    
                    return aIC;
                }


            }else if (aIC.ApplicationExecutable == null)
            {


            }




            if (string.IsNullOrWhiteSpace( aIC.ApplicationName))
            {

            }
            return aIC;
        }
        

        public class ApplicationInfoContainer
        {
            public ImageSource ApplicationIcon { get; set; }

            public string ApplicationName { get; set; }
            public string ApplicationPath { get; set; }
            public float MatchPercentage { get; set; }

            public FileInfo ApplicationExecutable { get; set; }
            public DirectoryInfo ApplicationDirectory { get; set; }
            public bool Autorun { get; set; }
            public bool StartMenu { get; set; } 
            /*
            public ApplicationInfoContainer(string name, FileInfo executable, DirectoryInfo directory)
            {
                ApplicationName = name;
                
                ApplicationDirectory = directory;
                
                Autorun = false;
                if (executable != null)
                {
                    StartMenu = true;
                    ApplicationPath = executable.FullName;
                    ApplicationEXE = executable;
                    IconExtractor iconExtractor = new IconExtractor(executable.FullName);
                    if (iconExtractor.Count > 0)
                    {
                        ImageSourceConverter converter = new ImageSourceConverter();
                        Icon icon = iconExtractor.GetIcon(0);
                        ApplicationIcon = icon.ToImageSource();
                    }
                } 
                else
                    StartMenu = false;
            }*/

            public ApplicationInfoContainer(DirectoryInfo directory)
            {
                ApplicationInfoContainer temp = GetApplicationExecutable(directory);
                ApplicationDirectory = directory;
                if (temp != null)
                {
                    ApplicationName = temp.ApplicationName;
                    Autorun = false;
                    if (temp.ApplicationExecutable != null)
                    {
                        StartMenu = true;
                        ApplicationPath = temp.ApplicationExecutable.FullName;
                        ApplicationExecutable = temp.ApplicationExecutable;
                        IconExtractor iconExtractor = new IconExtractor(temp.ApplicationExecutable.FullName);
                        if (iconExtractor.Count > 0)
                        {
                            ImageSourceConverter converter = new ImageSourceConverter();
                            Icon icon = iconExtractor.GetIcon(0);
                            ApplicationIcon = icon.ToImageSource();
                        }
                    }
                }
                else
                {
                    ApplicationName = directory.Name;
                    StartMenu = false;
                }
              
            }
            public ApplicationInfoContainer(FileInfo executable, float matchPercentage)
            {
                ApplicationExecutable = executable;
                MatchPercentage = matchPercentage;
            }
            public ApplicationInfoContainer()
            {
            }
        }
    }
}