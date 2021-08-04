using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SPL
{
    [CreateAssetMenu(fileName = SettingsLocator.SettingsPath, menuName = "SPL/Settings")]
    public class Settings : ScriptableObject
    {
        public bool NeedOpenSceneOnStart;
        public SceneName StartSceneName;

        public bool NeedOpenSceneOnStop;
        public SceneName StopSceneName;

        public Scene StartScene;
        public Scene StopScene;
        public List<Scene> AllScenes;
        public bool NeedRefresh;

        public void ResetSceneNames()
        {
            StartSceneName = 0;
            StopSceneName = 0;
        }

        public void UpdateStartScene(SceneName sceneName)
        {
            StartScene = GetScene(sceneName);
        }

        public void UpdateStopScene(SceneName sceneName)
        {
            StopScene = GetScene(sceneName);
        }

        private Scene GetScene(SceneName sceneName)
        {
            if (AllScenes == null)
            {
                return default;
            }

            foreach (var scene in AllScenes.Where(scene => scene.SceneName == sceneName))
                return scene;

            return default;
        }
    }
}
