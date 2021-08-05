using System.IO;
using UnityEditor;
using UnityEngine;

namespace SPL
{
    public class SettingsLocator
    {
        public const string SettingsPath = "SPLSettings";
        private static Settings settings;
        private static string pathToFolder;

        public static Settings Settings
        {
            get
            {
                if (settings == null)
                    settings = Resources.Load<Settings>(SettingsPath);

                if (settings != null)
                    return settings;

                settings = ScriptableObject.CreateInstance<Settings>();

                if (!IsResourceFolderExist())
                    CreateResourceFolder();

                AssetDatabase.CreateAsset(settings, GetPathToDefaultSettings());
                AssetDatabase.SaveAssets();

                return settings;
            }
        }

        public static string GetPathToScriptsFolder()
        {
            return $"{GetPathToFolder()}Editor/";
        }

        private static string GetPathToDefaultSettings()
        {
            return $"{GetPathToResourcesFolder()}/{SettingsPath}.asset";
        }

        private static string GetPathToResourcesFolder()
        {
            return $"{GetPathToFolder()}Resources";
        }

        private static bool IsResourceFolderExist()
        {
            return Directory.Exists(GetPathToResourcesFolder());
        }

        private static void CreateResourceFolder()
        {
            Directory.CreateDirectory(GetPathToResourcesFolder());
        }

        private static string GetPathToFolder()
        {
            if (!string.IsNullOrEmpty(pathToFolder))
                return pathToFolder;

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

            pathToFolder = AssetDatabase.GUIDToAssetPath(guids[0]).Split(new[] { "Editor" },
                System.StringSplitOptions.RemoveEmptyEntries)[0];

            return pathToFolder;
        }
    }
}
