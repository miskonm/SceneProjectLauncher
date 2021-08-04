using System;
using UnityEditor;
using UnityEngine;

namespace SPL.Editor
{
    public class SettingsWindow : EditorWindow
    {
        [MenuItem("Window/Scene Project Launcher", false, 1000)]
        [MenuItem("Scene Project Launcher/Settings", false, 1000)]
        public static void Init()
        {
            SettingsWindow window = GetWindow<SettingsWindow>("Scene Project Launcher");
            window.Show();
        }

        private void OnGUI()
        {
            var style = EditorStyle.Get;
            var settings = SettingsLocator.Settings;

            EditorGUIUtility.labelWidth = 240;
            GUILayout.Label("Base Setting", style.Heading);

            EditorGUI.BeginChangeCheck();

            using (new EditorGUILayout.VerticalScope(style.Area))
            {
                settings.NeedOpenSceneOnStart = EditorGUILayout.Toggle("Need Open Concrete Scene On Play",
                    settings.NeedOpenSceneOnStart, style.Toggle);

                settings.NeedOpenSceneOnStop = EditorGUILayout.Toggle("Need Open Concrete Scene On Stop",
                    settings.NeedOpenSceneOnStop, style.Toggle);
            }

            if (settings.NeedOpenSceneOnStart || settings.NeedOpenSceneOnStop)
            {
                GUILayout.Label("Scenes", style.Heading);

                using (new EditorGUILayout.VerticalScope(style.Area))
                {
                    var sceneValueArray = Enum.GetValues(typeof(SceneName));

                    if (sceneValueArray.Length == 0)
                    {
                        GUILayout.Label("Need to configure Scene Enum", style.Subheading2);
                    }
                    else
                    {
                        DrawStartScene(settings);
                        DrawStopScene(settings);
                    }

                    GUILayout.Space(10);

                    if (GUILayout.Button("Recreate Scene Enum", style.MenuButton))
                    {
                        if (SettingsConfigurator.Configure()) settings.ResetSceneNames();
                    }
                }
            }

            if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(settings);
        }

        private void DrawStartScene(Settings settings)
        {
            if (!settings.NeedOpenSceneOnStart)
            {
                return;
            }

            var previousSceneName = settings.StartSceneName;
            settings.StartSceneName =
                    (SceneName)EditorGUILayout.EnumPopup("Start Scene", settings.StartSceneName);

            if (previousSceneName != settings.StartSceneName)
            {
                settings.UpdateStartScene(settings.StartSceneName);
            }
        }

        private void DrawStopScene(Settings settings)
        {
            if (!settings.NeedOpenSceneOnStop)
            {
                return;
            }

            var previousSceneName = settings.StopSceneName;
            settings.StopSceneName =
                    (SceneName)EditorGUILayout.EnumPopup("Stop Scene", settings.StopSceneName);

            if (previousSceneName != settings.StopSceneName)
            {
                settings.UpdateStopScene(settings.StopSceneName);
            }
        }
    }
}
