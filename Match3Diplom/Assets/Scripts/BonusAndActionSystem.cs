using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BonusAndActionSystem : MonoBehaviour
{
    public static BonusAndActionSystem instance;
    [SerializeField] public BonusSystem _bonusSystemData;
    [SerializeField] public ActionSystem _actionSystemData;
    [SerializeField] public TextMeshProUGUI _firstBonusText, _secondBonusText, _thirdBonusText;
    [SerializeField] public bool _actionSetActive = true;
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
    private void Start()
    {
        TaskSystem.instance._theMovePanel.text = _bonusSystemData.MovePoints.ToString();
        UpdateAllBonuses();

    }
    public void FirstBonus()
    {
        if (_actionSystemData.FirstBonusCount > 0)
        {
            Debug.Log("Активирован первый бонус");
            _actionSystemData._firstBonusEnabled = true;
            _actionSystemData._secondBonusEnabled = false;
            _actionSystemData._thirdBonusEnabled = false;
            UpdateFirstBonusText();
        }
    }
    public void SecondBonus()
    {
        if (_actionSystemData.SecondBonusCount > 0) 
        {
            Debug.Log("Активирован второй бонус");
            _actionSystemData._secondBonusEnabled = true;
            _actionSystemData._firstBonusEnabled = false;
            _actionSystemData._thirdBonusEnabled = false;
            UpdateSecondBonusText();
        }
    }
    public void ThirdBonus()
    {
        if (_actionSystemData.ThirdBonusCount > 0)
        {
            Debug.Log("Активирован третий бонус");
            _actionSystemData._thirdBonusEnabled = true;
            _actionSystemData._secondBonusEnabled = false;
            _actionSystemData._firstBonusEnabled = false;
            UpdateThirdBonusText();
        }
    }
    public void FindMove()
    {

        GridSystem.instance.DeadLock();

    }
    public void UpdateFirstBonusText()
    {
        _firstBonusText.text = _actionSystemData.FirstBonusCount.ToString();
    }
    public void UpdateSecondBonusText()
    {
        _secondBonusText.text = _actionSystemData.SecondBonusCount.ToString();
    }
    public void UpdateThirdBonusText()
    {
        _thirdBonusText.text = _actionSystemData.ThirdBonusCount.ToString();
    }
    public void UpdateAllBonuses()
    {
        UpdateFirstBonusText();
        UpdateSecondBonusText();
        UpdateThirdBonusText();
    }
}
[System.Serializable]
public struct BonusSystem
{
    [SerializeField] private int _movePoints;
    public int MovePoints
    {
        get { return _movePoints; }
        set { _movePoints = value; }
    }
    public void RemoveMovePoint()
    {
        MovePoints--;
        TaskSystem.instance._theMovePanel.text = MovePoints.ToString();
    }
    public void AddMovePoint()
    {
        MovePoints++;
        TaskSystem.instance._theMovePanel.text = MovePoints.ToString();
    }
}
[System.Serializable]
public struct ActionSystem
{
    [SerializeField] private int _firstBonusCount, _secondBonusCount, _thirdBonusCount;
    [SerializeField] public bool _firstBonusEnabled, _secondBonusEnabled, _thirdBonusEnabled;
    public int FirstBonusCount
    {
        get { return _firstBonusCount; }
        set { _firstBonusCount = value; }
    }
    public void AddFirstBonusPoint()
    {
        _firstBonusCount++;
    }
    public void RemoveFirstBonusPoint()
    {
        _firstBonusCount--;
    }
    public int SecondBonusCount
    {
        get { return _secondBonusCount; }
        set { _secondBonusCount = value; }
    }
    public void AddSecondBonusPoint()
    {
        _secondBonusCount++;
    }
    public void RemoveSecondBonusPoint()
    {
        _secondBonusCount--;
    }
    public int ThirdBonusCount
    {
        get { return _thirdBonusCount; }
        set { _thirdBonusCount = value; }
    }
    public void AddThirdBonusPoint()
    {
        _thirdBonusCount++;
    }
    public void RemoveThirdBonusPoint()
    {
        _thirdBonusCount--;
    }
}