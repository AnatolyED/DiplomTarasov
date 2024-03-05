using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    [SerializeField] private Image _menuBackground;
    [SerializeField] private List<Sprite> _shopBackgroundItems = new List<Sprite>();
    [SerializeField] private int _numberSelectSprite = 0;
    [SerializeField] private List<GameObject> _itemBuyingButton = new List<GameObject>();
    [SerializeField] private List<Text> _itemBuyingText = new List<Text>();
    [SerializeField] private Sprite _takedItem, _untakedItem;
    [SerializeField] public Text _valletText;
    private void Start()
    {
        ShopUiUpdate();
    }
    /*
    public void SaveBackGroundSelected()
    {
        PlayerPrefs.SetInt("SelectedBackground", DontDestroy.instance.shopData.selectBacground);
        Debug.Log(DontDestroy.instance.shopData.selectBacground);
    }
    */

    public void ShopUiUpdate()
    {
        ValletUpdate();
        for (int i = 0; i < _itemBuyingButton.Count; i++)
        {
            if (_itemBuyingButton[i].GetComponent<Image>().sprite == _takedItem && i != DontDestroy.instance.shopData.selectBacground)
            {
                _itemBuyingButton[i].GetComponent<Image>().sprite = _untakedItem;
            }
            else if (_itemBuyingButton[i].GetComponent<Image>().sprite != _takedItem && i == DontDestroy.instance.shopData.selectBacground)
            {
                _itemBuyingButton[i].GetComponent<Image>().sprite = _takedItem;
                if (i > 0)
                {
                    _itemBuyingText[i - 1].gameObject.SetActive(false);
                }
            }
            if (DontDestroy.instance.shopData._purchasedBackground.Contains(i))
            {
                _itemBuyingButton[i].GetComponent<Image>().sprite = _untakedItem;
                if (i > 0)
                {
                    _itemBuyingText[i - 1].gameObject.SetActive(false);
                }
                if (i == _numberSelectSprite)
                {
                    _itemBuyingButton[i].GetComponent<Image>().sprite = _takedItem;
                }
            }

        }
    }
    public void BaseBackground()
    {
        BuyingShopItem(0);
    }
    public void BuyingFirstShopItem()
    {
        BuyingShopItem(1);
    }
    public void BuyingSecondShopItem()
    {
        BuyingShopItem(2);
    }
    public void BuyingThirdShopItem()
    {
        BuyingShopItem(3);
    }
    public void BuyingFourShopItem()
    {
        BuyingShopItem(4);
    }
    private void BuyingShopItem(int number)
    {
        if (DontDestroy.instance.shopData._purchasedBackground.Contains(number))
        {
            if(_itemBuyingButton[number].GetComponent<Image>().sprite != _takedItem)
            {
                _itemBuyingButton[_numberSelectSprite].GetComponent<Image>().sprite = _untakedItem;
                _itemBuyingButton[number].GetComponent<Image>().sprite = _takedItem;
                _numberSelectSprite = number;
                DontDestroy.instance.shopData.selectBacground = number;
            }
        }
        else
        {
            if (DontDestroy.instance.shopData.valletCount > 0)
            {
                DontDestroy.instance.shopData.valletCount--;
                DontDestroy.instance.shopData._purchasedBackground.Add(number);
                ValletUpdate();
                _itemBuyingButton[number].GetComponent<Image>().sprite = _untakedItem;
                _itemBuyingButton[number].transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }
    public void ValletUpdate()
    {
        _valletText.text = DontDestroy.instance.shopData.valletCount.ToString();
    }
    public void LoadBackgroundSelected()
    {
        if (DontDestroy.instance.shopData._purchasedBackground.Contains(DontDestroy.instance.shopData.selectBacground))
        {
            _menuBackground.sprite = _shopBackgroundItems[DontDestroy.instance.shopData.selectBacground];
            _numberSelectSprite = DontDestroy.instance.shopData.selectBacground;
        }
        else
        {
            _menuBackground.sprite = _shopBackgroundItems[0];
            DontDestroy.instance.shopData.selectBacground = 0;
            _numberSelectSprite = DontDestroy.instance.shopData.selectBacground;
        }
    }
}
