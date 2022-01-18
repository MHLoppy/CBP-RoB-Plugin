/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.IO;
using System.Windows;
using CBPSDK;

namespace CBP_RoB_Plugin
{
    public class RoB_Plugin : IPluginCBP
    {
        public string PluginTitle => "Rise of Babel Installer (no audio files)";

        public string PluginVersion => "0.1.3";

        public string PluginAuthor => "MHLoppy";

        public bool CBPCompatible => true;

        public bool DefaultMultiplayerCompatible => true;

        public string PluginDescription => "An installer for the Rise of Babel taunt pack. The current version installs ONLY the taunt text (in English), not the copyrighted taunt audio."
                                            + "\n\nSource code: https://github.com/MHLoppy/CBP-RoB-Plugin";

        public bool IsSimpleMod => false;

        public string LoadResult { get; set; }

        private string soundOrig;
        private string RoBFolder;
        private string loadedRoB;

        public void DoSomething(string workshopModsPath, string localModsPath)
        {
            soundOrig = Path.GetFullPath(Path.Combine(localModsPath, @"..\", "Data", "sound.xml"));
            RoBFolder = Path.GetFullPath(Path.Combine(localModsPath, @"..\", "CBP", "RoB"));
            loadedRoB = Path.Combine(RoBFolder, "robinstallerplugin.txt");

            //if folder doesn't exist, make it
            if (!Directory.Exists(RoBFolder))
            {
                try
                {
                    Directory.CreateDirectory(RoBFolder);
                    LoadResult = (PluginTitle + " detected for first time. Doing first-time setup.");
                }
                catch (Exception ex)
                {
                    LoadResult = (PluginTitle + ": error writing first-time file:\n\n" + ex);
                }
            }
            else
            {
                LoadResult = (RoBFolder + " already exists; no action taken.");
            }

            //if file doesn't exist, make one
            if (!File.Exists(loadedRoB))
            {
                try
                {
                    File.WriteAllText(loadedRoB, "0");
                    LoadResult = (PluginTitle + " completed first time setup successfully. Created file:\n" + loadedRoB);
                    //MessageBox.Show(PluginTitle + ": Created file:\n" + loadedMTP);//removed to reduce number of popups for first-time CBP users
                }
                catch (Exception ex)
                {
                    LoadResult = (PluginTitle + ": error writing first-time file:\n\n" + ex);
                }
            }
            else
            {
                LoadResult = (loadedRoB + " already exists; no action taken.");
            }

            CheckIfLoaded();//this can be important to do here, otherwise the bool might be accessed without a value depending on how other stuff is set up
        }

        public bool CheckIfLoaded()
        {
            if (File.ReadAllText(loadedRoB) != "0")
            {
                if (!LoadResult.Contains("is loaded"))
                {
                    LoadResult += "\n\n" + PluginTitle + " is loaded.";
                }
                return true;
            }
            else
            {
                if (!LoadResult.Contains("is not loaded"))
                {
                    LoadResult += "\n\n" + PluginTitle + " is not loaded.";
                }
                return false;
            }
        }

        public void LoadPlugin(string workshopModsPath, string localModsPath)
        {
            try
            {
                BackupSoundXML();
                new RoBInstallerWindow().Show();

                File.WriteAllText(loadedRoB, "1");
                CheckIfLoaded();
                LoadResult = (PluginTitle + " was loaded.");
            }
            catch (Exception ex)
            {
                LoadResult = (PluginTitle + " had an error while loading: " + ex);
                MessageBox.Show("Error while loading:\n\n" + ex);
            }
        }

        public void UnloadPlugin(string workshopModsPath, string localModsPath)
        {
            try
            {
                RestoreSoundXML();

                File.WriteAllText(loadedRoB, "0");
                CheckIfLoaded();
                LoadResult = (PluginTitle + ": Previous sound.xml file has been restored.");
                MessageBox.Show("Previous sound.xml file has been restored.");
            }
            catch (Exception ex)
            {
                LoadResult = (PluginTitle + " had an error while unloading: " + ex);
                MessageBox.Show("Error while unloading:\n\n" + ex);
            }
        }

        public void UpdatePlugin(string workshopModsPath, string localModsPath)
        {
            //not needed, so do nothing - plugin itself is kept updated by Steam
        }

        private void BackupSoundXML()
        {
            File.Copy(soundOrig, Path.Combine(RoBFolder, "sound.xml"), true);
        }

        private void RestoreSoundXML()
        {
            File.Copy(Path.Combine(RoBFolder, "sound.xml"), soundOrig, true);
        }
    }
}
