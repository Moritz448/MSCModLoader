﻿using System;
using System.IO;
using System.Reflection;
using System.Windows;

namespace MSCLInstaller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SelectGameFolder selectGameFolder;
        MSCLoaderInstaller mscloaderInstaller;
        SelectModsFolder selectModsFolder;
        InstallProgress installProgress;
        Version MSCLInstallerVer;
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                Initialize();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}{Environment.NewLine}{Environment.NewLine}Failed to initialize installer.{Environment.NewLine}Make sure to:{Environment.NewLine}- Unpack ALL files before launching{Environment.NewLine}- Files aren't unpacked in protected folders (like Program Files or root of C drive). {Environment.NewLine}{Environment.NewLine}Installer will now close. {Environment.NewLine}You can try running it as admin.", "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Dbg.Log(ex.Message, true, true);
                Environment.Exit(0);
            }

        }
        private void Initialize()
        {
            MSCLInstallerVer = Assembly.GetExecutingAssembly().GetName().Version;
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            Storage.currentPath = Path.GetFullPath(".");
            if (File.Exists("Log.txt")) File.Delete("Log.txt");
            if (Directory.Exists(Path.Combine(Storage.currentPath, "temp"))) Directory.Delete(Path.Combine(Storage.currentPath, "temp"), true);
            Dbg.Init();
            Dbg.Log($"Current folder: {Storage.currentPath}");
            selectGameFolder = new SelectGameFolder();
            mscloaderInstaller = new MSCLoaderInstaller();
            selectModsFolder = new SelectModsFolder();
            Storage.packFiles = Directory.GetFiles(Storage.currentPath, "*.pack");
            if (Storage.packFiles.Length == 0)
            {
                Dbg.MissingFilesError();
            }
            if (!File.Exists("Ionic.Zip.Reduced.dll") || !File.Exists("INIFileParser.dll"))
            {
                Dbg.MissingFilesError();
            }

            SelectGameFolderPage(Game.MSC);
            StatusBarText.Text = $"MSCLInstaller (ver. {MSCLInstallerVer.Major}.{MSCLInstallerVer.Minor}.{MSCLInstallerVer.Build})";
        }
        internal void SetMSCLoaderVer(string ver)
        {
            StatusBarText.Text = $"MSCLInstaller (ver. {MSCLInstallerVer.Major}.{MSCLInstallerVer.Minor}.{MSCLInstallerVer.Build}) | MSCLoader (ver. {ver})";
        }
        internal void ReInitInstallerPage()
        {
            mscloaderInstaller = new MSCLoaderInstaller();
        }

        internal InstallProgress InstallProgressPage()
        {
            installProgress = new InstallProgress(this);
            MainFrame.Content = installProgress;
            return installProgress;
        }

        public void SelectGameFolderPage(Game game)
        {
            Dbg.Log("Select Game Folder", true, true);
            MSCIPageTitle.Text = "Select Game Folder";
            Storage.selectedGame = game;
            MainFrame.Content = selectGameFolder;
            selectGameFolder.Init(this);
        }

        public async void MSCLoaderInstallerPage()
        {
            Dbg.Log("MSCLoader Installer", true, true);
            MainFrame.Content = mscloaderInstaller;
            MSCIPageTitle.Text = "Please wait.... Checking current files";
            await InstallerHelpers.DelayedWork();
            MSCIPageTitle.Text = "Select Option Below";
            mscloaderInstaller.Init(this);
        }

        public void SelectModsFolderPage(bool change)
        {
            Dbg.Log("Select Mods Folder", true, true);
            MSCIPageTitle.Text = "Select Mods Folder";
            MainFrame.Content = selectModsFolder;
            selectModsFolder.Init(this, change);
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            About a = new About
            {
                Owner = this
            };
            a.ShowDialog();
        }
    }
}
