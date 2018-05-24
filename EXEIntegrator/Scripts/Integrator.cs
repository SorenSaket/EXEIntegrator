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
        public static void Analyze(string path)
        {
            
            SelectionWindow selectionWindow = new SelectionWindow();
            selectionWindow.InitializeSelectionWindow(GetApplicationInfos(path));
            /*this.Close();*/

            /*MessageBoxResult result = System.Windows.MessageBox.Show("Are you sure you want to integrate all applications in " + IntegrationPathTextbox.Text + "?" + Environment.NewLine + "This cannot be undone", "EXE Integrator", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
                Integrate(IntegrationPathTextbox.Text);*/
        }


        public static ApplicationInfoContainer[] GetApplicationInfos(string path)
        {
            LoadingWindow loadingWindow = new LoadingWindow();
            loadingWindow.Show();
            loadingWindow.Focus();

            // The directory to look for applications
            DirectoryInfo d = new DirectoryInfo(path);
            // All sub folders
            DirectoryInfo[] dirs = d.GetDirectories();

            List<ApplicationInfoContainer> infoContainers = new List<ApplicationInfoContainer>();

            // For each subfolder
            for (int i = 0; i < dirs.Length; i++)
            {
                ApplicationInfoContainer temp = new ApplicationInfoContainer(dirs[i]);
                loadingWindow.SetLoad(i / dirs.Length * 100, "Analyzing " + dirs[i].Name + "...");
                // Create application refrence
                infoContainers.Add(temp);
                if(temp.ApplicationEXE != null)
                    Console.WriteLine("Integrating " + temp.ApplicationEXE.Name + " with " + temp.ApplicationDirectory.FullName);
                /*CreateShortcut(app.applicationName, app.applicationEXE);
                SetFolderIcon(dirs[i].FullName, app.applicationEXE.FullName);*/

            }
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

            List<ApplicationMatch> executableContenders = new List<ApplicationMatch>();

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
                executableContenders.Add(new ApplicationMatch( file, StringMatcher.MatchPercentage(keywords.ToArray(), file.Name)));
            }

            if (Directory.Exists(applicationFolder + @"/bin"))
            {
                foreach (var file in new DirectoryInfo(applicationFolder + @"/bin").GetFiles("*.exe"))
                {
                    executableContenders.Add(new ApplicationMatch(file, StringMatcher.MatchPercentage(keywords.ToArray(), file.Name)));
                }
            }

            if(HasFoundMatch(executableContenders))
                return CheckForBestMatch(executableContenders);

            executableContenders = new List<ApplicationMatch>();
            foreach (var file in applicationFolder.GetFiles("*.exe", SearchOption.AllDirectories))
            {
                executableContenders.Add(new ApplicationMatch(file, StringMatcher.MatchPercentage(keywords.ToArray(), file.Name)));
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

            List<ApplicationMatch> executableContenders = new List<ApplicationMatch>();

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
                executableContenders.Add(new ApplicationMatch(file, StringMatcher.MatchPercentage(keywords.ToArray(), file.Name)));
            }

            if (HasFoundMatch(executableContenders))
            {
                temp.ApplicationEXE = CheckForBestMatch(executableContenders);
                
                return temp;
            }

            if (Directory.Exists(applicationFolder + @"/bin"))
            {
                foreach (var file in new DirectoryInfo(applicationFolder + @"/bin").GetFiles("*.exe"))
                {
                    executableContenders.Add(new ApplicationMatch(file, StringMatcher.MatchPercentage(keywords.ToArray(), file.Name)));
                }
            }

            if (HasFoundMatch(executableContenders))
            {
                temp.ApplicationEXE = CheckForBestMatch(executableContenders);
                return temp;
            }

            executableContenders = new List<ApplicationMatch>();
            foreach (var file in applicationFolder.GetFiles("*.exe", SearchOption.AllDirectories))
            {
                executableContenders.Add(new ApplicationMatch(file, StringMatcher.MatchPercentage(keywords.ToArray(), file.Name)));
            }


            if (executableContenders.Count > 0)
            {
                temp.ApplicationEXE = CheckForBestMatch(executableContenders);
                return temp;
            }

            return null;
        }
        //
        private static FileInfo CheckForMatch(List<ApplicationMatch> executableContenders, float threshold)
        {
            executableContenders = executableContenders.OrderBy(o => o.matchPercentage).ToList();

            for (int i = 0; i < executableContenders.Count; i++)
            {
                if (executableContenders[i].matchPercentage >= threshold)
                    return executableContenders[i].executable;
            }
            return null;
        }
        private static FileInfo CheckForBestMatch(List<ApplicationMatch> executableContenders)
        {
            List<ApplicationMatch> temp = executableContenders.OrderByDescending(o => o.matchPercentage).ThenBy(x => x.executable.Name.Length).ToList();
            for (int i = 0; i < temp.Count; i++)
            {
                Console.WriteLine("Application Match : " + temp[i].executable.Name + " with " + temp[i].matchPercentage*100 + "%");
            }
            
            return temp[0].executable;
        }
        private static float CheckForBestMatchPercentage(List<ApplicationMatch> executableContenders)
        {
            List<ApplicationMatch> temp = executableContenders.OrderByDescending(o => o.matchPercentage).ThenBy(x => x.executable.Name.Length).ToList();
            return temp[0].matchPercentage;
        }
        private static bool HasFoundMatch(List<ApplicationMatch> executableContenders)
        {
            if(executableContenders.Count > 0)
            {
                List<ApplicationMatch> temp = executableContenders.OrderByDescending(o => o.matchPercentage).ThenBy(x => x.executable.Name.Length).ToList();
                float scoreToBeat = 0;
                int contenders = 3.Clamp(0, temp.Count - 1);

                for (int i = 1; i < contenders; i++)
                {
                    scoreToBeat += temp[i].matchPercentage;
                }

                if (temp[0].matchPercentage > scoreToBeat / contenders * 1.1f)
                    return true;
            }
            return false;
        }

        public class ApplicationMatch
        {
            public float matchPercentage;
            public FileInfo executable;

            public ApplicationMatch( FileInfo _executable, float _matchPercentage)
            {
                matchPercentage = _matchPercentage;
                executable = _executable;
            }
        }

        public class ApplicationInfoContainer
        {
            public ImageSource ApplicationIcon { get; set; }

            public string ApplicationName { get; set; }
            public string ApplicationPath { get; set; }

            public FileInfo ApplicationEXE { get; set; }
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
                    if (temp.ApplicationEXE != null)
                    {
                        StartMenu = true;
                        ApplicationPath = temp.ApplicationEXE.FullName;
                        ApplicationEXE = temp.ApplicationEXE;
                        IconExtractor iconExtractor = new IconExtractor(temp.ApplicationEXE.FullName);
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

            public ApplicationInfoContainer()
            {
            }
        }
    }
}