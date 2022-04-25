using System.Collections.Generic;
using UnityEngine;

namespace SPL.Editor
{
    [CreateAssetMenu(fileName = SettingsLocator.SettingsPath, menuName = "SPL/Settings")]
    public class Settings : ScriptableObject
    {
        public bool NeedOpenSceneOnStart;
        public Scene StartScene;

        public bool NeedOpenSceneOnStop;
        public Scene StopScene;

        public List<Scene> AllScenes;

        public void SelectStartScene(int newIndex) =>
            StartScene = Scene(newIndex);

        public void SelectStopScene(int newIndex) =>
            StopScene = Scene(newIndex);

        private Scene Scene(int newIndex)
        {
            if (AllScenes.Count <= newIndex)
                return newIndex == 0 ? default : AllScenes[0];

            return AllScenes[newIndex];
        }
    }
}