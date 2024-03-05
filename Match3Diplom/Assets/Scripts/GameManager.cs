using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        DontDestroy.instance.shopData.levelNumber = SceneManager.GetActiveScene().buildIndex;
    }
    public void GameState(bool state)
    {
        if (state)
        {
            Time.timeScale = 1;
            Debug.Log("Возобновление игры");
        }
        else
        {
            Debug.Log("Игра на паузе");
            Time.timeScale = 0;
        }
    }
    public void NextLevel()
    {
        if (SceneManager.GetActiveScene().buildIndex < 3) { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); }
        else { SceneManager.LoadScene(0); }
    }
    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void ExitLevel()
    {
        SceneManager.LoadScene(0);
    }
}
