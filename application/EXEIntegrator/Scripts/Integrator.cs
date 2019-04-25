using EXEIntegrator.Scripts;
using IWshRuntimeLibrary;
using Microsoft.Win32;
using static EXEIntegrator.Scripts.MathAddons;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows;
using System;
using TsudaKageyu;

namespace EXEIntegrator
{
    public static class Integrator
    {
        // -------- Analyzation --------
        public static void Analyze(string path){
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
        private static void Analyzer_DoWork(object sender, DoWorkEventArgs e){
            // The directory to look for applications
            DirectoryInfo d = new DirectoryInfo(e.Argument.ToString());
            // All sub folders
            DirectoryInfo[] dirs = d.GetDirectories();

            List<ApplicationInfoContainer> infoContainers = new List<ApplicationInfoContainer>();

            // For each subfolder
            for (int i = 0; i < dirs.Length; i++)
            {
                (sender as BackgroundWorker).ReportProgress(Convert.ToInt32((i * 100 / dirs.Length)), "Analyzing " + dirs[i].Name + "...");

                infoContainers.Add(new ApplicationInfoContainer(dirs[i]));
            }

            e.Result = infoContainers;
        }
        private static void Analyzer_ProgressChanged(object sender, ProgressChangedEventArgs e){
            WindowManager.loadingWindow.SetLoad(e.ProgressPercentage, e.UserState.ToString());
        }
        private static void Analyzer_Completed(object sender, RunWorkerCompletedEventArgs e){
            if (e.Cancelled)
                System.Windows.MessageBox.Show("Operation was canceled");
            else if (e.Error != null)
                System.Windows.MessageBox.Show(e.Error.Message);
            else
            {
                List<ApplicationInfoContainer> infoContainers = ((List<ApplicationInfoContainer>)e.Result);
                WindowManager.selectionWindow.SetAppData(infoContainers);
                WindowManager.loadingWindow.Hide();
            }
        }


        public class ApplicationInfoContainer : ObservableCollection<ApplicationInfoContainer>
        {
            public string ApplicationName { get; set; }
            public string ApplicationPath { get; set; }
            public string ApplicationDescription { get; set; }

            public float MatchPercentage { get; set; }

            public bool IsCompany { get; set; }
            public string ParentName { get; set; }

            public ImageSource ApplicationIcon { get; set; }
            public FileInfo ApplicationExecutable { get; set; }
            public DirectoryInfo ApplicationDirectory { get; set; }

            public bool Autorun { get; set; }
            public bool StartMenu { get; set; }
            public bool Desktop { get; set; }

            public List<ApplicationInfoContainer> children;

            //Debug
            public ApplicationExecutableInfoDebug debug;

            public ApplicationInfoContainer(DirectoryInfo directory)
            {
                ApplicationDirectory = directory;
                ApplicationName = GetApplicationName(directory.Name);

                ApplicationExecutableInfo tempExeInfo = GetApplicationExecutable(directory);
                debug = tempExeInfo.debug;

                switch (tempExeInfo.directoryType)
                {
                    case ApplicationDirectoryType.Empty:

                        break;
                    case ApplicationDirectoryType.Application:
                        ApplicationExecutable = tempExeInfo.applicationExecutable;

                        break;
                    case ApplicationDirectoryType.Company:
                        IsCompany = true;
                        children = new List<ApplicationInfoContainer>();
                        DirectoryInfo[] subdirectories;

                        if (tempExeInfo.subdirectory != null)
                            subdirectories = tempExeInfo.subdirectory.GetDirectories();
                        else
                            subdirectories = directory.GetDirectories();

                        for (int i = 0; i < subdirectories.Length; i++)
                        {
                            children.Add(new ApplicationInfoContainer(subdirectories[i]));
                            children[children.Count - 1].ParentName = ApplicationName;
                        }
                        break;
                    default:
                        break;
                }


                if (ApplicationExecutable != null)
                {
                    ApplicationPath = ApplicationExecutable.FullName;
                    ApplicationDescription = FileVersionInfo.GetVersionInfo(ApplicationExecutable.FullName).FileDescription;
                    ApplicationIcon = GetApplicationIcon(ApplicationExecutable.FullName);
                }else
                {
                    ApplicationIcon = GetApplicationIcon("");
                }

                
                if (ApplicationIcon.CanFreeze)
                    ApplicationIcon.Freeze();
            }
        }


        private static string GetApplicationName(string name)
        {
            switch (Settings.nameFormatting)
            {
                case NameFormatting.Capitalize:
                    return new CultureInfo("en-US", false).TextInfo.ToTitleCase(name);
                case NameFormatting.lowercase:
                    return name.ToLower();
                default:
                    return name;
            }
        }
        private static ApplicationExecutableInfo GetApplicationExecutable(DirectoryInfo directory)
        {
            DirectoryInfo currentlyTargetedFolder = directory;

            // Keyword formatting and splitting
            List<string> keywords = StringMatcher.FormatKeyword(directory.Name);

            List<string> executableKeywords = new List<string>(keywords);
            executableKeywords.AddRange(priorityExecutableNames);

            List<string> folderKeywords = new List<string>(keywords);
            folderKeywords.AddRange(priorityFolderNames);

            // The return var
            ApplicationExecutableInfo temp = new ApplicationExecutableInfo
            {
                debug = new ApplicationExecutableInfoDebug
                {
                    keywords = keywords
                }
            };

            // Get executables in root
            FileInfo[] subExecutables = directory.GetFiles("*.exe");

            FileInfo tempFileInfo = SearchFolderForExecutable(directory, executableKeywords);
            if(tempFileInfo != null)
            {
                temp.directoryType = ApplicationDirectoryType.Application;
                temp.applicationExecutable = tempFileInfo;
                return temp;
            }

            // Get sub directories in root
            DirectoryInfo[] subDirectories = directory.GetDirectories();
           
            // If there's no files or folders return empty folder
            if (subDirectories.Length == 0 && subExecutables.Length == 0)
            {
                temp.directoryType = ApplicationDirectoryType.Empty;
                return temp;
            }

            // Check if the intallation is in a single subfolder
            if(subDirectories.Length == 1)
            {
                // Check if the intallation is in a single subfolder by matching keywords or checking if has a version number
                if (StringMatcher.IsMatch(subDirectories[0].Name, folderKeywords) || Regex.IsMatch(subDirectories[0].Name, @"\d"))
                {
                    currentlyTargetedFolder = temp.subdirectory = subDirectories[0];

                    FileInfo tempFileInfo2 = SearchFolderForExecutable(currentlyTargetedFolder, executableKeywords);
                    if (tempFileInfo2 != null)
                    {
                        temp.directoryType = ApplicationDirectoryType.Application;
                        temp.applicationExecutable = tempFileInfo2;
                        return temp;
                    }

                    subDirectories = currentlyTargetedFolder.GetDirectories();
                }
                if (subExecutables.Length == 0)
                {
                    temp.directoryType = ApplicationDirectoryType.Company;
                    return temp;
                }
            }

            // Check if multiple subdirectories match with directory name then it's probably a company folder
            if(subExecutables.Length == 0)
            {
                
                int numberOfMatchingSubdirectories = 0;
                for (int i = 0; i < subDirectories.Length; i++)
                {
                    if (subDirectories[i].Name.ToLower().Contains(directory.Name.ToLower()))
                        numberOfMatchingSubdirectories++;
                }
                Console.WriteLine(directory.Name + " empty as shit but it has " + subDirectories.Length + " subfolders and " + numberOfMatchingSubdirectories + " subdirs match parent");
                if ((numberOfMatchingSubdirectories / subDirectories.Length) > 0.4f)
                {
                    Console.WriteLine(directory.Name + "is a company");
                    temp.directoryType = ApplicationDirectoryType.Company;
                    return temp;
                }
            }

            // Check sub directories with prioritized names
            for (int x = 0; x < subDirectories.Length; x++)
            {
                for (int y = 0; y < folderKeywords.Count; y++)
                {
                    if (subDirectories[x].Name.ToLower().Contains(folderKeywords[y].ToLower()))
                    {
                        FileInfo tempfileinto = SearchFolderForExecutable(subDirectories[x], executableKeywords);
                        if (tempfileinto != null)
                        {
                            temp.directoryType = ApplicationDirectoryType.Application;
                            temp.applicationExecutable = tempfileinto;
                            return temp;
                        }
                    }
                }
            }

            // Do full check
            if(Settings.searchMethod == SearchMethod.Full)
            {
                // Search all directories for executable 
                for (int i = 0; i < subDirectories.Length; i++)
                {
                    FileInfo tempfileinto = SearchFolderForExecutable(subDirectories[i], executableKeywords);
                    if (tempfileinto != null)
                    {
                        temp.directoryType = ApplicationDirectoryType.Application;
                        temp.applicationExecutable = tempfileinto;
                        return temp;
                    }
                }
                //Get the executable with shortest name in root directory
                if (subExecutables.Length > 0)
                {
                    if (subExecutables.Length == 1)
                    {
                        temp.directoryType = ApplicationDirectoryType.Application;
                        temp.applicationExecutable = subExecutables[0];
                        return temp;
                    }
                    else
                    {
                        FileInfo executableWithShortestName = null;

                        for (int i = 0; i < subExecutables.Length; i++)
                        {
                            if(!StringMatcher.IsMatch(subExecutables[i].Name, excludedExecutableNames.ToList()))
                            {
                                if (executableWithShortestName == null)
                                    executableWithShortestName = subExecutables[i];
                                else if (executableWithShortestName.Name.Length > subExecutables[i].Name.Length)
                                    executableWithShortestName = subExecutables[i];
                            }
                        }
                        if(executableWithShortestName != null)
                        {
                            temp.directoryType = ApplicationDirectoryType.Application;
                            temp.applicationExecutable = executableWithShortestName;
                            return temp;
                        }
                    }
                }
            }


            // Company folder
            temp.directoryType = ApplicationDirectoryType.Company;
            return temp;
        }
        private static ImageSource GetApplicationIcon(string path) { 
            if(!string.IsNullOrWhiteSpace(path))
            {
                IconExtractor iconExtractor = new IconExtractor(path);
                // If any icon is found
                if (iconExtractor.Count > 0)
                {
                    // Gets the largest icon from the exe
                    return iconExtractor.GetIcon(0).ToImageSource();
                }
            }
            return IconUtil.ToImageSource(SystemIcons.Application);
        }



        private static FileInfo SearchFolderForExecutable(DirectoryInfo directory, List<string> keywords)
        {
            FileInfo[] subExecutables = directory.GetFiles("*.exe");
            // Checks for match in root
            for (int i = 0; i < subExecutables.Length; i++)
            {
                if (StringMatcher.IsMatch(subExecutables[i].Name, keywords) && !StringMatcher.IsMatch(subExecutables[i].Name, excludedExecutableNames.ToList()))
                {
                    return subExecutables[i];
                }
            }
            // Get sub directories in root
            DirectoryInfo[] subDirectories = directory.GetDirectories();

            // If there's no files or folders
            if (subDirectories.Length == 0 && subExecutables.Length == 0)
            {
                return null;
            }

            // Check sub directories with prioritized names
            for (int x = 0; x < subDirectories.Length; x++)
            {
                for (int y = 0; y < priorityFolderNames.Length; y++)
                {
                    if (subDirectories[x].Name.ToLower().Contains(priorityFolderNames[y].ToLower()))
                    {
                        FileInfo tempfileinto = SearchFolderForExecutable(subDirectories[x], keywords);
                        if (tempfileinto != null)
                        {
                            return tempfileinto;
                        }
                    }
                }
            }

            return null;
        }

        public class ApplicationExecutableInfo
        {
            public ApplicationDirectoryType directoryType;
            public FileInfo applicationExecutable;
            public DirectoryInfo subdirectory;
            public ApplicationExecutableInfoDebug debug;
        }

        public class ApplicationExecutableInfoDebug
        {
            public List<string> keywords;
        }


        // 1.   Just an empty folder. Ignore
        // 2.   The folder contains a single application
        // 3.   Company is a category that is applied to a folder with multiple programs in it. "Adobe" "Autodesk"
        public enum ApplicationDirectoryType { Empty = 0, Application = 1, Company = 2 }
        public enum NameFormatting { Capitalize = 0, lowercase = 1 }
        public enum SearchMethod { Fast = 0, Full = 1 }
        public static string[] priorityFolderNames = new string[] { "bin", "win", "binaries", "dist", "64", "32", "installation", "release", "application"};
        public static string[] priorityExecutableNames = new string[] { "launcher" };
        public static string[] excludedExecutableNames = new string[] { "update", "install", "setup", "uninstall", "unins"};
    }
}