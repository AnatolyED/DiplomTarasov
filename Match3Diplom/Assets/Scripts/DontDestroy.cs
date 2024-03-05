using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    public static DontDestroy instance;

    public ShopData shopData;
    public ShopData _reserveStartShopData;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this);
        GUIManager.instance.LevelNumberUpdate();
        SaveStartParameters();
    }

    public void SaveGameData()
    {
        PlayerPrefs.SetInt("Vallet",shopData.valletCount);
        PlayerPrefs.SetInt("SelectBackgroud",shopData.selectBacground);
        PlayerPrefs.SetInt("CurrentLevel",shopData.levelNumber);
        PlayerPrefs.SetString("PurchasedBackground", shopData.ConvertPurchasedBackgroundToString());
    }
    public void SaveStartParameters()
    {
        _reserveStartShopData.selectBacground += shopData.selectBacground;
        _reserveStartShopData.levelNumber += shopData.levelNumber;
        _reserveStartShopData.valletCount += shopData.valletCount;
        _reserveStartShopData._purchasedBackground.AddRange(shopData._purchasedBackground);
    }
    public void LoadGameData()
    {
        shopData.valletCount = PlayerPrefs.GetInt("Vallet");
        shopData.selectBacground = PlayerPrefs.GetInt("SelectBackgroud");
        shopData.levelNumber = PlayerPrefs.GetInt("CurrentLevel");
        shopData.ConvertPurchasedBackgroundToList();
    }

}
