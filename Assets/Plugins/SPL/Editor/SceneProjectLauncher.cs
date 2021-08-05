using UnityEditor;
using UnityEditor.SceneManagement;

namespace SPL
{
    [InitializeOnLoad]
    public class SceneProjectLauncher
    {
        static SceneProjectLauncher()
        {
            EditorApplication.playModeStateChanged += EditorApplicationOnPlayModeStateChanged;
        }

        private static void EditorApplicationOnPlayModeStateChanged(PlayModeStateChange playModeStateChange)
        {
            var settings = SettingsLocator.Settings;

            switch (playModeStateChange)
            {
                case PlayModeStateChange.EnteredEditMode:
                {
                    if (settings.NeedOpenSceneOnStop)
                    {
                        EditorSceneManager.OpenScene(settings.StopScene.Path); 
                    }

                    break;
                }
                case PlayModeStateChange.ExitingEditMode:
                {
                    if (settings.NeedOpenSceneOnStart)
                    {
                        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                        EditorSceneManager.OpenScene(settings.StartScene.Path);
                    }

                    break;
                }
            }
        }
    }
}
