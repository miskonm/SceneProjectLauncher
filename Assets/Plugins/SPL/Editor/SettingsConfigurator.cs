using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Callbacks;

namespace SPL
{
    public static class SettingsConfigurator
    {
        public static bool Configure()
        {
            var type = typeof(SceneName);

            if (!TryGetPath(type, out var path))
            {
                path = CreateFile(type);
            }

            var scenes = GetActiveScenes();
            var newKeys = GenerateKeys(scenes);

            var inputKeyData = ReadFile(path);
            var outputKeyData = ReplaceKeys(inputKeyData, newKeys, type, out var isChanged);

            WriteFile(path, outputKeyData);

            SettingsLocator.Settings.NeedRefresh = true;

            if (!isChanged)
            {
                UpdateScenesAndSet();
            }

            AssetDatabase.Refresh();

            return isChanged;
        }

        private static List<Scene> GetActiveScenes()
        {
            var activeScenes = new List<Scene>();
            var scenes = EditorBuildSettings.scenes;
            var index = 0;

            foreach (var scene in scenes)
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

        private static string GetSceneName(EditorBuildSettingsScene scene)
        {
            return scene.path.Split('/').Last().Replace(".unity", string.Empty);
        }

        private static bool TryGetPath(Type type, out string path)
        {
            path = null;

            var assets = AssetDatabase.FindAssets($"{type.Name} t:script");

            if (assets.Length <= 0)
                return false;

            path = AssetDatabase.GUIDToAssetPath(assets.First());

            return true;

        }

        private static string CreateFile(Type type)
        {
            var pathToFolder = SettingsLocator.GetPathToScriptsFolder();
            var enumPath = GetPathToEnumScript(pathToFolder);
            var enumData = GenerateEnumData();

            WriteFile(enumPath, enumData);

            return enumPath;
        }

        private static string GetPathToEnumScript(string pathToFolder)
        {
            return $"{pathToFolder}{nameof(SceneName)}.cs";
        }

        private static string GenerateEnumData()
        {
            var tab = new string(' ', 4);
            var newData = $"namespace {nameof(SPL)}" + "\n{\n" + $"{tab}public enum " + $"{nameof(SceneName)}" +
                    $"\n{tab}" + "{" + $"\n{tab}" + "}\n}";

            var regex = new Regex(newData, RegexOptions.Singleline);

            return newData;
        }

        private static string[] GenerateKeys(List<Scene> scenes)
        {
            var keysCount = scenes.Count;
            var keys = new string[keysCount];

            for (var i = 0; i < keysCount; i++)
            {
                var scene = scenes[i];
                keys[i] = $"{scene.Name} = {scene.Index}";
            }

            return keys;
        }

        private static string ReplaceKeys(string input, string[] keys, Type keyType, out bool isChanged)
        {
            var tab = new string(' ', 4);
            var doubleTab = $"{tab}{tab}";
            var newKeys = string.Join($",\n{doubleTab}", keys);
            var newData = $"enum {keyType.Name}" + $"\n{tab}" + "{" + $"\n{doubleTab}{newKeys}" + $"\n{tab}" + "}";

            var regex = new Regex(@"enum\s*" + keyType.Name + @"\s*{(.+?)}", RegexOptions.Singleline);
            var replaced = regex.Replace(input, newData);

            isChanged = !string.Equals(input, replaced);

            return replaced;
        }

        private static string ReadFile(string path)
        {
            string data;

            using var streamReader = new StreamReader(path);
            data = streamReader.ReadToEnd();

            return data;
        }

        private static void WriteFile(string path, string data)
        {
            using var streamWriter = new StreamWriter(path);
            streamWriter.Write(data);
        }

        [DidReloadScripts]
        private static void UpdateScenesAndSet()
        {
            var settings = SettingsLocator.Settings;

            if (!settings.NeedRefresh)
            {
                return;
            }

            settings.NeedRefresh = false;

            var scenes = GetActiveScenes();
            var enumValues = Enum.GetValues(typeof(SceneName));

            for (int i = 0; i < scenes.Count; i++)
            {
                if (enumValues.Length <= i)
                {
                    break;
                }

                var scene = scenes[i];
                scene.SceneName = (SceneName) enumValues.GetValue(i);
                scenes[i] = scene;
            }

            settings.AllScenes = scenes;
            settings.UpdateStartScene(settings.StartSceneName);
            settings.UpdateStopScene(settings.StopSceneName);
        }
    }
}
