using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SPL.Editor
{
    public class SettingsWindow : EditorWindow
    {
        [MenuItem("Tools/SPL/Settings", false, 1000)]
        public static void Init()
        {
            SettingsWindow window = GetWindow<SettingsWindow>("Scene Project Launcher");
            window.Show();
        }

        private void OnGUI()
        {
            EditorStyle style = EditorStyle.Get;

            EditorGUIUtility.labelWidth = 240;
            GUILayout.Label("Base Setting", style.Heading);

            bool isUpdating = EditorApplication.isUpdating;
            bool isCompiling = EditorApplication.isCompiling;

            if (isCompiling)
            {
                GUILayout.Label("Wait a minutes. Project is compiling...", style.Subheading);
            }
            else if (isUpdating)
            {
                GUILayout.Label("Wait a minutes. Project is updating...", style.Subheading);
            }

            if (isCompiling || isUpdating)
                return;

            bool isConfigured = SettingsConfigurator.Configure();
            Settings settings = SettingsLocator.Settings;

            EditorGUI.BeginChangeCheck();

            DrawToggles(style, settings);

            if (settings.NeedOpenSceneOnStart || settings.NeedOpenSceneOnStop)
            {
                GUILayout.Label("Scenes", style.Heading);

                using (new EditorGUILayout.VerticalScope(style.Area))
                {
                    DrawStartScene(settings);
                    DrawStopScene(settings);
                }
            }

            bool isChanged = EditorGUI.EndChangeCheck();
            if (isChanged)
                EditorUtility.SetDirty(settings);

            if (isChanged || isConfigured)
                AssetDatabase.SaveAssets();
        }

        private static void DrawToggles(EditorStyle style, Settings settings)
        {
            using (new EditorGUILayout.VerticalScope(style.Area))
            {
                settings.NeedOpenSceneOnStart = EditorGUILayout.Toggle("Need Open Concrete Scene On Play",
                    settings.NeedOpenSceneOnStart, style.Toggle);

                settings.NeedOpenSceneOnStop = EditorGUILayout.Toggle("Need Open Concrete Scene On Stop",
                    settings.NeedOpenSceneOnStop, style.Toggle);
            }
        }

        private void DrawStartScene(Settings settings)
        {
            if (!settings.NeedOpenSceneOnStart)
                return;

            if (DrawScene(settings, settings.StartScene, "Start Scene", out int newIndex))
                settings.SelectStartScene(newIndex);
        }

        private void DrawStopScene(Settings settings)
        {
            if (!settings.NeedOpenSceneOnStop)
                return;

            if (DrawScene(settings, settings.StopScene, "Stop Scene", out int newIndex))
                settings.SelectStopScene(newIndex);
        }

        private bool DrawScene(Settings settings, Scene scene, string label, out int newIndex)
        {
            List<Scene> allScenes = settings.AllScenes;
            int oldIndex = scene.Index;

            string[] displayedOptions = SceneNames(allScenes);
            newIndex = EditorGUILayout.Popup(label, oldIndex, displayedOptions);

            return newIndex != oldIndex;
        }

        private string[] SceneNames(List<Scene> scenes)
        {
            if (scenes == null)
                return Array.Empty<string>();

            string[] strings = new string[scenes.Count];

            for (int i = 0; i < scenes.Count; i++)
            {
                strings[i] = scenes[i].Name;
            }

            return strings;
        }
    }
}