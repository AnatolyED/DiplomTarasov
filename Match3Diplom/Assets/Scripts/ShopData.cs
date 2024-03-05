using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShopData
{
    public static ShopData instance;

    public int levelNumber;
    public int valletCount;
    public int selectBacground;
    
    public List<int> _purchasedBackground = new List<int>() {};
    public string ConvertPurchasedBackgroundToString()
    {
        string result = string.Empty;
        foreach (var element in _purchasedBackground )
        {
            result += element.ToString();
        }
        return result;
    }
    public void ConvertPurchasedBackgroundToList()
    {
        string values = PlayerPrefs.GetString("PurchasedBackground");
        int valueLength = values.Length;
        _purchasedBackground.Clear();
        for (int i = 0; i < valueLength; i++)
        {
            int number = int.Parse(values[i].ToString());
            if (!_purchasedBackground.Contains(number))
            {
                _purchasedBackground.Add(number);

            }
        }
    }
    public void ClearData()
    {
        levelNumber = 0;
        valletCount = 0;
        selectBacground = 0;
        _purchasedBackground.Clear();
    }
}
