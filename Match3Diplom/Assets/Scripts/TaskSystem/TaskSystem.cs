using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskSystem : MonoBehaviour
{
    public static TaskSystem instance;
    [SerializeField,Tooltip("Тип уровня")]private LevelType _levelType;
    [Header("Объекты UI")]
    [SerializeField] public TextMeshProUGUI _theMovePanel;
    [SerializeField] public TextMeshProUGUI _theScorePanel;
    [SerializeField] public TextMeshProUGUI _targetScorePanel;
    [SerializeField] private List<Image> _theTaskUIPanelSprite;
    [SerializeField] private List<TextMeshProUGUI> _theTaskUIPanelText;
    [SerializeField] private TextMeshProUGUI _theQuestUiPanelText;
    [Header("If level type Farm conditions")]
    [SerializeField] private int _theRequiredNumberOfPoint = 0;
    public int _currentNumberOfPoints = 0;
    [Header("If level type Quest conditions")]
    [SerializeField] private List<Task> _levelTask;
    [Header("Переменные для расчетов")]
    [SerializeField]private List<int> _taskTotalNumber;
    [SerializeField]private List<int> _taskCurrentNumber;
    // костыль, для убирания лишних заходов корутин
    public bool _receivingAnAward = true;
    private bool _stopCheckQuest = false;
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
        LevelTaskLoad();
    }
    private void LevelTaskLoad()
    {
        if (_levelType == LevelType.Quest)
        {
            for (int i = 0; i < _levelTask.Count; i++)
            {
                _theTaskUIPanelSprite[i].sprite = _levelTask[i].Item;
                _taskTotalNumber[i] = _levelTask[i].Count;
                _theTaskUIPanelText[i].text = _taskCurrentNumber[i].ToString() + "/" + _levelTask[i].Count.ToString();
            }
        }
        else if (_levelType == LevelType.Farm)
        {
            _targetScorePanel.text = _theRequiredNumberOfPoint.ToString();
            _theScorePanel.text = _currentNumberOfPoints.ToString() + "/";//+ _theRequiredNumberOfPoint.ToString();
        }
    }
    #region Quest-type level methods
    private void QuestUpdateMethod()
    {

        for (int i = 0; i < _theTaskUIPanelText.Count; i++)
        {
            _theTaskUIPanelText[i].text = _taskCurrentNumber[i].ToString() + "/" + _levelTask[i].Count.ToString();
        }
        if (TaskCompletedCheck() && BonusAndActionSystem.instance._bonusSystemData.MovePoints >= 0 && _levelType == LevelType.Quest)
        {
            _stopCheckQuest = true;
            GUIManager.instance.GameWin();
        }
        else if (!TaskCompletedCheck() && BonusAndActionSystem.instance._bonusSystemData.MovePoints <= 0 && _levelType == LevelType.Quest)
        {
            _stopCheckQuest = true;
            GUIManager.instance.GameOver();
        }else if (_currentNumberOfPoints >= _theRequiredNumberOfPoint && BonusAndActionSystem.instance._bonusSystemData.MovePoints >= 0 && _levelType == LevelType.Farm)
        {
            GUIManager.instance.GameWin();
        }else if(_currentNumberOfPoints < _theRequiredNumberOfPoint && BonusAndActionSystem.instance._bonusSystemData.MovePoints <= 0 && _levelType == LevelType.Farm)
        {
            GUIManager.instance.GameOver();
        }

    }
    private bool TaskCompletedCheck()
    {
        bool result = true;
        foreach (var element in _levelTask)
        {
            if (element.TaskHasBeenCompleted != true)
            {
                result = false;
            }
        }
        return result;
    }
    #endregion
    #region Farm-type level methods
    public void PointsUpdate()
    {
        if (_currentNumberOfPoints >= _theRequiredNumberOfPoint && _levelType == LevelType.Farm)
            _theScorePanel.text = _currentNumberOfPoints.ToString() + "/" + _theRequiredNumberOfPoint.ToString();
        if (_levelType == LevelType.Quest)
            QuestUpdateMethod();
            _theScorePanel.text = _currentNumberOfPoints.ToString();
        if (_currentNumberOfPoints >= _theRequiredNumberOfPoint && _levelType == LevelType.Farm && BonusAndActionSystem.instance._bonusSystemData.MovePoints <= 0) { }
    }
    public void CheckingCondition(Sprite cell, int count)
    {
        if (_levelType == LevelType.Quest)
        {
            for (int i = 0; i < _levelTask.Count; i++)
            {
                if(cell == _levelTask[i].Item && _levelTask[i].TaskHasBeenCompleted == false)
                {
                    _taskCurrentNumber[i] += count;
                    if (_levelTask[i].Count <= _taskCurrentNumber[i])
                    {
                        _taskCurrentNumber[i] = _taskTotalNumber[i];
                        _levelTask[i].TaskHasBeenCompleted = true;
                    }
                }
            }
        }else if(_levelType == LevelType.Farm)
        {
            if (_currentNumberOfPoints >= _theRequiredNumberOfPoint && BonusAndActionSystem.instance._bonusSystemData.MovePoints >= 0)
            {
                GUIManager.instance.GameWin();
            }
            else if (_currentNumberOfPoints <= _theRequiredNumberOfPoint && BonusAndActionSystem.instance._bonusSystemData.MovePoints <= 0)
            {
                GUIManager.instance.GameOver();
            }
        }
        PointsUpdate();
    }

    /*private void FinishTheLevel()
    {
        GameManager.instance.ExitLevel();
    }*/
    #endregion
}
