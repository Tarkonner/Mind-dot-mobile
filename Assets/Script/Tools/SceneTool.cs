using UnityEditor;
using UnityEngine.SceneManagement;

#if(UNITY_EDITOR)
[InitializeOnLoad]
public static class SceneTool
{
    static SceneTool()
    {
        EditorApplication.playModeStateChanged += LoadManagerScene;
    }


    static void LoadManagerScene(PlayModeStateChange state)
    {
        if (state != PlayModeStateChange.EnteredPlayMode)
            return;

        if (SceneManager.GetActiveScene().buildIndex != 0)
            SceneManager.LoadScene(0, LoadSceneMode.Additive);
    }

}
#endif
