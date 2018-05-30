﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Shapes;
using System.IO;
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
using System.Windows.Threading;
using EXEIntegrator.Scripts;
using Microsoft.Win32;

namespace EXEIntegrator
{
    public static class Integrator
    {
        // -------- Analyzation --------
        public  static void Analyze(string path)
        {
            BackgroundWorker worker = new BackgroundWorker
            {
                WorkerReportsProgress = true
            };
            worker.DoWork += Analyzer_DoWork;
            worker.ProgressChanged += Analyzer_ProgressChanged;
            worker.RunWorkerCompleted += Analyzer_Completed;
            worker.RunWorkerAsync(path);

            WindowManager.loadingWindow.Show();
            WindowManager.loadingWindow.Focus();
        }
        private static void Analyzer_DoWork(object sender, DoWorkEventArgs e)
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

                ApplicationDirectoryType foldertype = DetermineDirectoryType(dirs[i]);

                switch (foldertype)
                {
                    case ApplicationDirectoryType.Empty:
                        continue;
                    case ApplicationDirectoryType.Application:
                        ApplicationInfoContainer temp = new ApplicationInfoContainer(dirs[i]);
                        if(temp.ApplicationExecutable != null)
                            infoContainers.Add(temp);
                        break;
                    case ApplicationDirectoryType.Company:
                        DirectoryInfo[] subdirs = dirs[i].GetDirectories();
                        for (int y = 0; y < subdirs.Length; y++)
                        {
                            ApplicationInfoContainer subdirApp = new ApplicationInfoContainer(subdirs[y]);
                            if (subdirApp.ApplicationExecutable != null)
                                infoContainers.Add(subdirApp);
                        }
                        break;
                    default:
                        break;
                }
            }
            
            e.Result = infoContainers;
        }
        private static void Analyzer_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            WindowManager.loadingWindow.SetLoad(e.ProgressPercentage, e.UserState.ToString());
        }
        private static void Analyzer_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled) System.Windows.MessageBox.Show("Operation was canceled");
            else if (e.Error != null) System.Windows.MessageBox.Show(e.Error.Message);
            else
            {
                List<ApplicationInfoContainer> infoContainers = ((List<ApplicationInfoContainer>)e.Result);
                WindowManager.selectionWindow.SetAppData(infoContainers.ToArray());
                WindowManager.loadingWindow.Hide();
            }
        }

        // -------- Integration --------
        public  static void Integrate(ApplicationInfoContainer[] applicationInfos)
        {
            BackgroundWorker worker = new BackgroundWorker
            {
                WorkerReportsProgress = true
            };
            worker.DoWork += Integrator_DoWork;
            worker.ProgressChanged += Integrator_ProgressChanged;
            worker.RunWorkerCompleted += Integrator_Completed;
            worker.RunWorkerAsync(applicationInfos);

            WindowManager.loadingWindow.Show();
            WindowManager.loadingWindow.Focus();
        }
        private static void Integrator_DoWork(object sender, DoWorkEventArgs e)
        {
            ApplicationInfoContainer[] applicationInfos = e.Argument as ApplicationInfoContainer[];
            for (int i = 0; i < applicationInfos.Length; i++)
            {
                (sender as BackgroundWorker).ReportProgress(Convert.ToInt32((i * 100 / applicationInfos.Length)), "Interating " + applicationInfos[i].ApplicationName + "...");
                // Sets folder icon
                if (applicationInfos[i].ApplicationIcon != null)
                    SetFolderIcon(applicationInfos[i]);
                // Run application on startup
                if (applicationInfos[i].Autorun)
                    AddToAutoRun(applicationInfos[i]);
                // Add shortcut to startmenu
                if (applicationInfos[i].StartMenu)
                    CreateShortcut(applicationInfos[i], Environment.SpecialFolder.UserProfile, @"\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\");
                // Add shortcut to Desktop
                if (applicationInfos[i].Desktop)
                    CreateShortcut(applicationInfos[i], Environment.SpecialFolder.Desktop);
            }
        }
        private static void Integrator_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Console.WriteLine(e.ProgressPercentage + e.UserState.ToString());
            WindowManager.loadingWindow.SetLoad(e.ProgressPercentage, e.UserState.ToString());
        }
        private static void Integrator_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            if(System.Windows.MessageBox.Show("Applications intergrated", "EXE Integrator") == MessageBoxResult.OK)
            {
                WindowManager.selectionWindow.Hide();
                WindowManager.mainWindow.Show();
            }
        }

        // -------- Interation Functions --------
        #region Create Shortcut
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
        private static void CreateShortcut(ApplicationInfoContainer app, string location)
        {
            if (app.ApplicationExecutable != null)
            {
                string shortcutLocation = System.IO.Path.Combine(location, app.ApplicationName);
                WshShell shell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);

                if (!string.IsNullOrEmpty(app.ApplicationDescription))
                    shortcut.Description = app.ApplicationDescription;      // The description of the shortcut
                shortcut.IconLocation = app.ApplicationExecutable.FullName; // The icon of the shortcut
                shortcut.TargetPath = app.ApplicationExecutable.FullName;   // The path of the file that will launch when the shortcut is run
                shortcut.Save();                                            // Save the shortcut
            }
        }
        private static void CreateShortcut(ApplicationInfoContainer app, Environment.SpecialFolder specialFolder)
        {
            if (app.ApplicationExecutable != null)
            {
                string shortcutLocation = System.IO.Path.Combine(Environment.GetFolderPath(specialFolder), app.ApplicationName + ".lnk");
                WshShell shell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);

                if (!string.IsNullOrEmpty(app.ApplicationDescription))
                    shortcut.Description = app.ApplicationDescription;      // The description of the shortcut
                shortcut.IconLocation = app.ApplicationExecutable.FullName; // The icon of the shortcut
                shortcut.TargetPath = app.ApplicationExecutable.FullName;   // The path of the file that will launch when the shortcut is run
                shortcut.Save();                                            // Save the shortcut
            }
        }
        private static void CreateShortcut(ApplicationInfoContainer app, Environment.SpecialFolder specialFolder, string location)
        {
            if (app.ApplicationExecutable != null)
            {
                string shortcutLocation = System.IO.Path.Combine(Environment.GetFolderPath(specialFolder) + location, app.ApplicationName + ".lnk");
                WshShell shell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);

                if (!string.IsNullOrEmpty(app.ApplicationDescription))
                    shortcut.Description = app.ApplicationDescription;      // The description of the shortcut
                shortcut.IconLocation = app.ApplicationExecutable.FullName; // The icon of the shortcut
                shortcut.TargetPath = app.ApplicationExecutable.FullName;   // The path of the file that will launch when the shortcut is run
                shortcut.Save();                                            // Save the shortcut
            }
        }
        private static void CreateShortcutStartMenu(ApplicationInfoContainer app)
        {
            if (app.ApplicationExecutable != null)
            {
                string shortcutLocation = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\", app.ApplicationName + ".lnk");
                WshShell shell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);

                if (!string.IsNullOrEmpty(app.ApplicationDescription))
                    shortcut.Description = app.ApplicationDescription;      // The description of the shortcut
                shortcut.IconLocation = app.ApplicationExecutable.FullName; // The icon of the shortcut
                shortcut.TargetPath = app.ApplicationExecutable.FullName;   // The path of the file that will launch when the shortcut is run
                shortcut.Save();                                            // Save the shortcut
            }
        }
        private static void CreateShortcutDesktop(ApplicationInfoContainer app)
        {
            if (app.ApplicationExecutable != null)
            {
                string shortcutLocation = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), app.ApplicationName + ".lnk");
                WshShell shell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);

                if (!string.IsNullOrEmpty(app.ApplicationDescription))
                    shortcut.Description = app.ApplicationDescription;      // The description of the shortcut
                shortcut.IconLocation = app.ApplicationExecutable.FullName; // The icon of the shortcut
                shortcut.TargetPath = app.ApplicationExecutable.FullName;   // The path of the file that will launch when the shortcut is run
                shortcut.Save();                                            // Save the shortcut
            }
        }
        #endregion
        // Add to autorun
        private static void AddToAutoRun(ApplicationInfoContainer app)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
            if (key != null)
            {
                key.SetValue(app.ApplicationName, '"' + app.ApplicationExecutable.FullName + '"');
                key.Close();
            }
        }
        // Sets folder icon
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
        private static void SetFolderIcon(ApplicationInfoContainer application)
        {
            /* Remove any existing desktop.ini */
            if (System.IO.File.Exists(application.ApplicationDirectory.FullName + @"\desktop.ini")) System.IO.File.Delete(application.ApplicationDirectory.FullName + @"\desktop.ini");

            /* Write the desktop.ini */
            StreamWriter sw = System.IO.File.CreateText(application.ApplicationDirectory.FullName + @"\desktop.ini");
            sw.WriteLine("[.ShellClassInfo]");
            sw.WriteLine("IconResource=" + application.ApplicationExecutable.FullName + ",0");
            sw.Close();
            sw.Dispose();

            /* Set the desktop.ini to be hidden */
            System.IO.File.SetAttributes(application.ApplicationDirectory.FullName + @"\desktop.ini", System.IO.File.GetAttributes(application.ApplicationDirectory.FullName + @"\desktop.ini") | FileAttributes.Hidden);
            /* Set the path to system */
            System.IO.File.SetAttributes(application.ApplicationDirectory.FullName, System.IO.File.GetAttributes(application.ApplicationDirectory.FullName) | FileAttributes.System);
        }

        // -------- Helper Functions --------
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
        private static ApplicationDirectoryType DetermineDirectoryType(DirectoryInfo directory)
        {
            // If there's no files or folders
            if (directory.GetFiles("*.exe").Length == 0 && directory.GetDirectories().Length == 0)
                return ApplicationDirectoryType.Empty;

            // If there's multiple folders, there's no matching exes in root and there's no bin folder
            if (directory.GetDirectories().Length > 1 && directory.GetFiles("*.exe").Length == 0 && directory.GetDirectories("bin").Length == 0)
                return ApplicationDirectoryType.Company;


            return ApplicationDirectoryType.Application;
        }

        // -------- Data structures --------
        private static ApplicationInfoContainer FillInfo(ApplicationInfoContainer aIC)
        {
            // Gets the nesserary variables to extract the others from
            if (aIC.ApplicationDirectory == null && aIC.ApplicationExecutable == null)
                return aIC;
            else if (aIC.ApplicationExecutable == null)
                aIC.ApplicationExecutable = GetExecutable(aIC.ApplicationDirectory);
            else if (aIC.ApplicationDirectory == null)
                throw new NotImplementedException();
            // Gets Application Name
            if (string.IsNullOrWhiteSpace(aIC.ApplicationName))
                aIC.ApplicationName = aIC.ApplicationDirectory.Name;
            // Get Application dependent variables
            if(aIC.ApplicationExecutable != null)
            {
                // Gets Application Description
                if (string.IsNullOrWhiteSpace(aIC.ApplicationDescription))
                    aIC.ApplicationDescription = FileVersionInfo.GetVersionInfo(aIC.ApplicationExecutable.FullName).FileDescription;
                // Gets Application Icon
                if (aIC.ApplicationIcon == null)
                {
                    IconExtractor iconExtractor = new IconExtractor(aIC.ApplicationExecutable.FullName);
                    if (iconExtractor.Count > 0)
                    {
                        ImageSourceConverter converter = new ImageSourceConverter();
                        Icon icon = iconExtractor.GetIcon(0);
                        aIC.ApplicationIcon = icon.ToImageSource();
                        if(aIC.ApplicationIcon.CanFreeze)
                            aIC.ApplicationIcon.Freeze();
                    }
                }
                // Gets application Path
                if (string.IsNullOrWhiteSpace(aIC.ApplicationPath))
                    aIC.ApplicationPath = aIC.ApplicationExecutable.FullName;
            }
            
            return aIC;
        }

        public class ApplicationInfoContainer
        {
            public ImageSource ApplicationIcon { get; set; }

            public string ApplicationName { get; set; }
            public string ApplicationPath { get; set; }
            public string ApplicationDescription { get; set; }
            public float MatchPercentage { get; set; }

            public FileInfo ApplicationExecutable { get; set; }
            public DirectoryInfo ApplicationDirectory { get; set; }
            public bool Autorun { get; set; }
            public bool StartMenu { get; set; }
            public bool Desktop { get; set; }
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
                ApplicationDirectory = directory;

                ApplicationInfoContainer temp = FillInfo(this);

                ApplicationName = temp.ApplicationName;
                ApplicationPath = temp.ApplicationPath;
                ApplicationDescription = temp.ApplicationDescription;
                ApplicationExecutable = temp.ApplicationExecutable;
                ApplicationDirectory = temp.ApplicationDirectory;

                /*if (ApplicationExecutable != null)
                    StartMenu = true;*/
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

        public enum ApplicationDirectoryType {Empty, Application, Company }
    }
}
/*MessageBoxResult result = System.Windows.MessageBox.Show("Are you sure you want to integrate all applications in " + IntegrationPathTextbox.Text + "?" + Environment.NewLine + "This cannot be undone", "EXE Integrator", MessageBoxButton.YesNo);
if (result == MessageBoxResult.Yes)
    Integrate(IntegrationPathTextbox.Text);*/
                       /*     if (temp.ApplicationExecutable != null)
                            Console.WriteLine("Integrating " + temp.ApplicationExecutable.Name + " with " + temp.ApplicationDirectory.FullName);
                        else
                            Console.WriteLine("Could not find executale for: " + temp.ApplicationDirectory.FullName);*/