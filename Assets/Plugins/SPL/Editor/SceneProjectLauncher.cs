using UnityEditor;
using UnityEditor.SceneManagement;

namespace SPL.Editor
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
            SettingsConfigurator.Configure();
            Settings settings = SettingsLocator.Settings;

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
