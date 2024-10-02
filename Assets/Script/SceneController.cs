using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance;

    [SerializeField] int gameMenuIndex = 1;
    public bool showLevels { get; private set; } = false;



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
            LoadScene(1);
    }


    public void LoadScene(int levelIndex)
    {
        SceneManager.LoadScene(levelIndex);
    }

    public void LoadMenu(bool showLevelMenu = false)
    {
        if (showLevelMenu)
            showLevels = true;

        LoadScene(gameMenuIndex);
    }

    public void HasLoadedLevel() => showLevels = false;
}
