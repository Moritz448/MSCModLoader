﻿using Microsoft.SqlServer.Server;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace MSCLInstaller
{
    /// <summary>
    /// Interaction logic for SelectGameFolder.xaml
    /// </summary>
    public partial class SelectGameFolder : Page
    {
        MainWindow main;
        public SelectGameFolder()
        {
            InitializeComponent();
        }
        public void Init(MainWindow m)
        {
            main = m;
            FindMSCOnSteam();
            if (string.IsNullOrEmpty(Storage.mscPath))
            {
                if (File.Exists("MSCFolder.txt"))
                {
                    string mscPath = File.ReadAllText("MSCFolder.txt");
                    if (Directory.Exists(mscPath))
                    {
                        Storage.mscPath = mscPath;
                        MSCFolder.Text = Storage.mscPath;
                        Dbg.Log($"Loaded saved MSC Folder: {mscPath}");
                        GameInfo();
                    }
                    else
                    {
                        Dbg.Log($"Saved MSC Folder, doesn't exists: {mscPath}");
                        try
                        {
                            File.Delete("MSCFolder.txt");
                        }
                        catch (Exception e)
                        {
                            Dbg.Log("Error deleting MSCFolder.txt", true, true);
                            Dbg.Log(e.ToString());
                        }
                    }
                }
            }

        }

        private void FindMSCOnSteam()
        {
            Dbg.Log("Trying get MSC path from steam...");
            string steamPath = null;
            RegistryKey steam = Registry.LocalMachine.OpenSubKey("SOFTWARE\\WOW6432Node\\Valve\\Steam");
            if (steam != null)
            {
                steamPath = steam.GetValue("InstallPath").ToString();
            }
            string msc = Path.Combine(steamPath, "steamapps", "common", "My Summer Car");
            if (File.Exists(Path.Combine(msc, "mysummercar.exe")))
            {
                Storage.mscPath = msc;
            }
            else
            {
                string steamLib = Path.Combine(steamPath, "steamapps", "libraryfolders.vdf");
                if (File.Exists(Path.Combine(steamLib)))
                {
                    string[] lib = File.ReadAllLines(steamLib);
                    foreach (string s in lib)
                    {
                        if (s.Trim().StartsWith("\"path\""))
                        {
                            string msclib = Path.Combine(s.Trim().Split('"')[3], "steamapps", "common", "My Summer Car");
                            if (File.Exists(Path.Combine(msclib, "mysummercar.exe")))
                            {
                                Storage.mscPath = msclib;
                            }
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(Storage.mscPath))
            {
                MSCFolder.Text = Storage.mscPath;
                Dbg.Log($"Got MSC path from steam: {Storage.mscPath}");
                GameInfo();
            }
        }
        private void GameInfo()
        {
            string game = Path.Combine(Storage.mscPath, "mysummercar.exe");
            switch (MD5FileHashes.MD5HashFile(game))
            {
                case MD5FileHashes.msc64:
                case MD5FileHashes.msc64d9:
                    DetectedGameText.Inlines.Add(new Run("Detected: ") { FontWeight = FontWeights.Bold });
                    DetectedGameText.Inlines.Add(new Bold(new Run("My Summer Car (64-bit)") { Foreground = Brushes.Green }));
                    break;
                case MD5FileHashes.msc32:
                    DetectedGameText.Inlines.Add(new Run("Detected: ") { FontWeight = FontWeights.Bold });
                    DetectedGameText.Inlines.Add(new Bold(new Run("My Summer Car (32-bit)") { Foreground = Brushes.Green }));
                    break;
                default:
                    DetectedGameText.Inlines.Add(new Run("Detected: ") { FontWeight = FontWeights.Bold });
                    DetectedGameText.Inlines.Add(new Bold(new Run("Unknown") { Foreground = Brushes.Red }));
                    break;
            }
        }
        private void MSCFBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "mysummercar.exe|mysummercar.exe"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                Storage.mscPath = Path.GetDirectoryName(openFileDialog.FileName);
                MSCFolder.Text = Storage.mscPath;
                Dbg.Log($"MSC path set manually: {Storage.mscPath}");
                try
                {
                    File.WriteAllText("MSCFolder.txt", Storage.mscPath);
                }
                catch(Exception ex)
                {
                    Dbg.Log("Error creating MSCFolder.txt", true, true);
                    Dbg.Log(ex.ToString());
                }
                GameInfo();
            }
        }
    }
}
