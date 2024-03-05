using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GUIManager : MonoBehaviour
{
    public static GUIManager instance;

    [SerializeField] private GameObject _optionMenu;
    [SerializeField] private GameObject _shop;
    [SerializeField] public TextMeshProUGUI _levelNumberText;
    public List<GameObject> _endGamePanels = new List<GameObject>();
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public IEnumerator EndGame(bool state)
    {
        yield return new WaitForSecondsRealtime(1f);
        if (state)
        {
            _endGamePanels[1].SetActive(true);
        }
        else
        {
            _endGamePanels[0].SetActive(true);
        }
        StopAllCoroutines();
        yield break;
    }
    public void GameOver()
    {
       StartCoroutine(EndGame(false));
    }
    public void GameWin()
    {
        if (TaskSystem.instance._receivingAnAward) 
        {
            TaskSystem.instance._receivingAnAward = false;
            DontDestroy.instance.shopData.valletCount += 1;
            DontDestroy.instance.shopData.levelNumber = SceneManager.GetActiveScene().buildIndex;
        }
        StartCoroutine(EndGame(true));
    }
    public void OptionMenu()
    {
        bool state = _optionMenu.activeSelf == true ? false : true;
        _optionMenu.SetActive(state);
        if(GameManager.instance != null)
            GameManager.instance.GameState(!state);

    }
    public void Shop()
    {
        bool state = _shop.activeSelf == true ? false : true;
        if (!state)
        {
            //_shop.GetComponent<Shop>().SaveBackGroundSelected();
            _shop.GetComponent<Shop>().LoadBackgroundSelected();
            //DontDestroy.SaveBackGroundSelected(_shop.GetComponent<Shop>().SelectedSprite
        }
        _shop.SetActive(state);
        _shop.GetComponent<Shop>().ShopUiUpdate();
    }
    public void ClearGameData()
    {
        PlayerPrefs.DeleteAll();
        DontDestroy.instance.shopData.ClearData();
        ReturnStartParameters();
        SceneManager.LoadScene(0);
    }
    public void LoadGameData()
    {
        DontDestroy.instance.LoadGameData();
        LevelNumberUpdate();
    }
    public void LevelNumberUpdate()
    {
        _levelNumberText.text = DontDestroy.instance.shopData.levelNumber.ToString();
    }
    public void ReturnStartParameters()
    {
        DontDestroy.instance.shopData.selectBacground += DontDestroy.instance._reserveStartShopData.selectBacground;
        DontDestroy.instance.shopData.selectBacground += DontDestroy.instance._reserveStartShopData.levelNumber;
        DontDestroy.instance.shopData.selectBacground += DontDestroy.instance._reserveStartShopData.valletCount;
        DontDestroy.instance.shopData._purchasedBackground.AddRange(DontDestroy.instance._reserveStartShopData._purchasedBackground);
    }
    public void Exit()
    {
        DontDestroy.instance.SaveGameData();
        Application.Quit();
    }
}
