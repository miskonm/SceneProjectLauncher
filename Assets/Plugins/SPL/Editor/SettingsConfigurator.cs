using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace SPL.Editor
{
    public static class SettingsConfigurator
    {
        public static bool Configure(Settings settings = null)
        {
            settings ??= SettingsLocator.Settings;

            List<Scene> scenes = GetActiveScenes();
            bool needUpdateSettings = NeedUpdateSettings(scenes, settings);
            if (needUpdateSettings)
            {
                Update(scenes, settings);
            }

            return needUpdateSettings;
        }

        private static bool NeedUpdateSettings(List<Scene> newScenes, Settings settings)
        {
            List<Scene> savedScenes = settings.AllScenes;

            if (savedScenes == null)
                return true;

            if (newScenes.Count != savedScenes.Count)
                return true;

            for (int i = 0; i < newScenes.Count; i++)
            {
                Scene savedScene = savedScenes[i];
                Scene newScene = newScenes[i];

                if (savedScene != newScene)
                    return true;
            }

            return false;
        }

        private static void Update(List<Scene> scenes, Settings settings)
        {
            bool isStartSceneSet = false;
            bool isStopSceneSet = false;
            GUID startSceneGuid = settings.StartScene.Guid;
            GUID stopSceneGuid = settings.StopScene.Guid;

            foreach (Scene newScene in scenes)
            {
                if (newScene.Guid == startSceneGuid)
                {
                    isStartSceneSet = true;
                    settings.StartScene = newScene;
                }

                if (newScene.Guid == stopSceneGuid)
                {
                    isStopSceneSet = true;
                    settings.StopScene = newScene;
                }
            }

            settings.AllScenes = scenes;

            if (!isStartSceneSet)
                settings.SelectStartScene(0);

            if (!isStopSceneSet)
                settings.SelectStopScene(0);
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
                    Guid = scene.guid
                });

                index++;
            }

            return activeScenes;
        }

        private static string GetSceneName(EditorBuildSettingsScene scene) =>
            scene.path.Split('/').Last().Replace(".unity", string.Empty);
    }
}