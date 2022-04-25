using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Callbacks;

namespace SPL.Editor
{
    public static class SettingsConfigurator
    {
        public static bool Configure()
        {
            Type type = typeof(SceneName);

            if (!TryGetPath(type, out string path))
                path = CreateFile(type);

            List<Scene> scenes = GetActiveScenes();
            string[] newKeys = GenerateKeys(scenes);

            string inputKeyData = ReadFile(path);
            string outputKeyData = ReplaceKeys(inputKeyData, newKeys, type, out bool isChanged);

            WriteFile(path, outputKeyData);

            SettingsLocator.Settings.NeedRefresh = true;

            if (!isChanged)
                UpdateScenesAndSet();

            AssetDatabase.Refresh();

            return isChanged;
        }

        private static List<Scene> GetActiveScenes()
        {
            List<Scene> activeScenes = new List<Scene>();
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            int index = 0;

            foreach (EditorBuildSettingsScene scene in scenes)
            {
                if (!scene.enabled)
                    continue;

                activeScenes.Add(new Scene
                {
                    Index = index,
                    Name = GetSceneName(scene),
                    Path = scene.path,
                });

                index++;
            }

            return activeScenes;
        }

        private static string GetSceneName(EditorBuildSettingsScene scene) =>
            scene.path.Split('/').Last().Replace(".unity", string.Empty);

        private static bool TryGetPath(Type type, out string path)
        {
            path = null;

            string[] assets = AssetDatabase.FindAssets($"{type.Name} t:script");

            if (assets.Length <= 0)
                return false;

            path = AssetDatabase.GUIDToAssetPath(assets.First());

            return true;

        }

        private static string CreateFile(Type type)
        {
            string pathToFolder = SettingsLocator.GetPathToScriptsFolder();
            string enumPath = GetPathToEnumScript(pathToFolder);
            string enumData = GenerateEnumData();

            WriteFile(enumPath, enumData);

            return enumPath;
        }

        private static string GetPathToEnumScript(string pathToFolder) =>
            $"{pathToFolder}{nameof(SceneName)}.cs";

        private static string GenerateEnumData()
        {
            string tab = new string(' ', 4);
            string newData = $"namespace {nameof(SPL)}" + "\n{\n" + $"{tab}public enum " + $"{nameof(SceneName)}" +
                    $"\n{tab}" + "{" + $"\n{tab}" + "}\n}";

            return newData;
        }

        private static string[] GenerateKeys(List<Scene> scenes)
        {
            int keysCount = scenes.Count;
            string[] keys = new string[keysCount];

            for (var i = 0; i < keysCount; i++)
            {
                Scene scene = scenes[i];
                keys[i] = $"{scene.Name} = {scene.Index}";
            }

            return keys;
        }

        private static string ReplaceKeys(string input, string[] keys, Type keyType, out bool isChanged)
        {
            string tab = new string(' ', 4);
            string doubleTab = $"{tab}{tab}";
            string newKeys = string.Join($",\n{doubleTab}", keys);
            string newData = $"enum {keyType.Name}" + $"\n{tab}" + "{" + $"\n{doubleTab}{newKeys}" + $"\n{tab}" + "}";

            Regex regex = new Regex(@"enum\s*" + keyType.Name + @"\s*{(.+?)}", RegexOptions.Singleline);
            string replaced = regex.Replace(input, newData);

            isChanged = !string.Equals(input, replaced);

            return replaced;
        }

        private static string ReadFile(string path)
        {
            using StreamReader streamReader = new StreamReader(path);
            string data = streamReader.ReadToEnd();

            return data;
        }

        private static void WriteFile(string path, string data)
        {
            using StreamWriter streamWriter = new StreamWriter(path);
            streamWriter.Write(data);
        }

        [DidReloadScripts]
        private static void UpdateScenesAndSet()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
                return;
            
            Settings settings = SettingsLocator.Settings;

            if (!settings.NeedRefresh)
                return;

            settings.NeedRefresh = false;

            List<Scene> scenes = GetActiveScenes();
            Array enumValues = Enum.GetValues(typeof(SceneName));

            for (int i = 0; i < scenes.Count; i++)
            {
                if (enumValues.Length <= i)
                    break;

                Scene scene = scenes[i];
                scene.SceneName = (SceneName) enumValues.GetValue(i);
                scenes[i] = scene;
            }

            settings.AllScenes = scenes;
            settings.UpdateStartScene(settings.StartSceneName);
            settings.UpdateStopScene(settings.StopSceneName);
        }
    }
}
