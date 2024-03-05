using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InfoUpdateMenu : MonoBehaviour
{
    public static InfoUpdateMenu instance;
    [SerializeField] private TextMeshProUGUI _levelNumber;
    [SerializeField] private Image _menuBackGround;
    [SerializeField] private Shop _shopScript;
    private void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        //_shopScript.LoadBackgroundSelected();
        //_levelNumber.text = DontDestroy.instance.shopData.levelNumber.ToString();
    }
}
