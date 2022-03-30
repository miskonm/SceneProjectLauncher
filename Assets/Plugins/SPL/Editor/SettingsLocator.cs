using System.IO;
using UnityEditor;
using UnityEngine;

namespace SPL.Editor
{
    public static class SettingsLocator
    {
        public const string SettingsPath = "SPLSettings";
        
        private static Settings _settings;
        private static string _pathToFolder;

        public static Settings Settings
        {
            get
            {
                if (_settings == null)
                    _settings = Resources.Load<Settings>(SettingsPath);

                if (_settings != null)
                    return _settings;

                _settings = ScriptableObject.CreateInstance<Settings>();

                if (!IsResourceFolderExist())
                    CreateResourceFolder();

                AssetDatabase.CreateAsset(_settings, GetPathToDefaultSettings());
                AssetDatabase.SaveAssets();

                return _settings;
            }
        }

        public static string GetPathToScriptsFolder() =>
            $"{GetPathToFolder()}Editor/";

        private static string GetPathToDefaultSettings() =>
            $"{GetPathToResourcesFolder()}/{SettingsPath}.asset";

        private static string GetPathToResourcesFolder() =>
            $"{GetPathToFolder()}Resources";

        private static bool IsResourceFolderExist() =>
            Directory.Exists(GetPathToResourcesFolder());

        private static void CreateResourceFolder() =>
            Directory.CreateDirectory(GetPathToResourcesFolder());

        private static string GetPathToFolder()
        {
            if (!string.IsNullOrEmpty(_pathToFolder))
                return _pathToFolder;

            string[] guids = AssetDatabase.FindAssets(nameof(SettingsLocator));

            if (guids.Length == 0)
            {
                Debug.LogError($"Could not locate the Start Project Launcher folder because " +
                    $"the {nameof(SettingsLocator)} script has been moved or removed.");
            }

            if (guids.Length > 1)
            {
                Debug.LogError($"Could not locate the Start Project Launcher folder because " +
                    $"more than one {nameof(SettingsLocator)} script exists in the project, " +
                    $"but this needs to be unique to locate the folder.");
            }

            _pathToFolder = AssetDatabase.GUIDToAssetPath(guids[0]).Split(new[] { "Editor" },
                System.StringSplitOptions.RemoveEmptyEntries)[0];

            return _pathToFolder;
        }
    }
}
