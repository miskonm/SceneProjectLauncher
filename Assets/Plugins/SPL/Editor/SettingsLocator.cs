using System.IO;
using UnityEditor;
using UnityEngine;

namespace SPL.Editor
{
    public static class SettingsLocator
    {
        public const string SettingsPath = "SPLSettings";

        private static Settings _settings;

        public static Settings Settings
        {
            get
            {
                if (_settings == null)
                    _settings = AssetDatabase.LoadAssetAtPath<Settings>(GetPathToDefaultSettings());
                
                if (_settings != null)
                    return _settings;

                _settings = ScriptableObject.CreateInstance<Settings>();

                if (!IsPluginFolderExist())
                {
                    CreatePluginFolder();
                    AssetDatabase.Refresh();
                }

                SettingsConfigurator.Configure(_settings);
                AssetDatabase.CreateAsset(_settings, GetPathToDefaultSettings());
                AssetDatabase.SaveAssets();
                
                return _settings;
            }
        }

        private static string GetPathToPluginFolder() =>
            "Assets/Plugins/SPL/";

        private static bool IsPluginFolderExist() =>
            Directory.Exists(GetPathToPluginFolder());

        private static void CreatePluginFolder() =>
            Directory.CreateDirectory(GetPathToPluginFolder());

        private static string GetPathToDefaultSettings() =>
            $"{GetPathToPluginFolder()}/{SettingsPath}.asset";
    }
}