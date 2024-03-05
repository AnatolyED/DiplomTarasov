using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    [Header("Компоненты клетки")]
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private BoxCollider2D _boxColider;
    [SerializeField] private Image _image;
    [Header("Данные клетки")]
    [SerializeField] private CellType _cellType;
    [SerializeField] private Vector2 _position;
    [Header("Работа с клеткой")]
    private static Color _selectedColor = new Color(.5f, .5f, .5f, 1.0f);
    public static Cell _previousSelected = null;
    [SerializeField] private bool isSelected = false;
    public bool IsSelect
    {
        get { return isSelected; }
    }
    private Vector2[] _cellDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
    public bool matchFound = false;
    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _boxColider = GetComponent<BoxCollider2D>();
        _image = GetComponent<Image>();
    }
    #region работа с Image
    public void RemoveSprite()
    {
        _image.sprite = GridSystem.instance._emptyCellSprite;
        StartCoroutine(GridSystem.instance.CollapseCells());
    }
    public void RemoveCell(bool earnPoints)
    {
        RemoveSprite();
        if (earnPoints)
        {
            TaskSystem.instance._currentNumberOfPoints += 10;
        }
    }
    public void AddSprite(Sprite sprite)
    {
        _image.sprite = sprite;
    }
    public void TransferSprite(Cell target)
    {
        Sprite _sprite = GetSprite;
        AddSprite(target.GetSprite);
        target.AddSprite(_sprite);
    }
    public Sprite GetSprite
    {
        get => _image.sprite;
    }
    public Image GetImage
    {
        get => _image;
    }
    public void SetColor(Color newColor)
    {
        _image.color = newColor;
    }
    #endregion
    #region работа с клеткой
    public void SetSize(int size)
    {
        _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
        _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
    }
    public Vector2 Position
    {
        get => _position;
        set
        {
            if (value.x >= Vector2.zero.y || value.y >= Vector2.zero.y)
            {
                _position = value;
            }
            else
            {
                Debug.Log("Выход за границы!");
            }
        }
    }
    public CellType CellType
    {
        get => _cellType;
        set => _cellType = value;
    }
    private void OnMouseDown()
    {
        if (!BonusAndActionSystem.instance._actionSetActive) return;// проверка на выполненые задачи и вывод иконки - победа или поражение
        if (_cellType != CellType.Base || GridSystem.instance.IsShifting) return;
        if (BonusAndActionSystem.instance._actionSystemData._firstBonusEnabled || BonusAndActionSystem.instance._actionSystemData._secondBonusEnabled || BonusAndActionSystem.instance._actionSystemData._thirdBonusEnabled)
        {
            Bonuses();
            GridSystem.instance.DeselectAllCells();
        }
        else if (isSelected) Deselect();
        else
        {
            if (_previousSelected == null)
            {
                Select();
            }
            else
            {
                if (CheckingTheCells())
                {
                    SwapSprite(_previousSelected._image);
                    _previousSelected.Deselect();
                }
                else
                {
                    _previousSelected.GetComponent<Cell>().Deselect();
                    Select();
                }
            }
        }
    }
    private void Select()
    {
        isSelected = true;
        _image.color = _selectedColor;
        _previousSelected = gameObject.GetComponent<Cell>();
    }
    public void Illumination()
    {
        _image.color = Color.red;
    }
    public void DeIllumination()
    {
        _image.color = Color.white;
    }
    public void Deselect()
    {
        isSelected = false;
        _image.color = Color.white;
        _previousSelected = null;
    }
    public void SwapSprite(Image _image2)
    {
        if (_image.sprite == _image2.sprite)
        {
            return;
        }

        Sprite tempSprite = _image2.sprite;
        _image2.sprite = _image.sprite;
        _image.sprite = tempSprite;
        BonusAndActionSystem.instance._bonusSystemData.RemoveMovePoint();
        if(BonusAndActionSystem.instance._bonusSystemData.MovePoints <= 0)
        {
            BonusAndActionSystem.instance._actionSetActive = false;
        }
        GridSystem.instance.IlluminationOffAllCells();
        CheckAndRemoveCellsSprite(true);
        // проигрование музыки
    }
    private bool CheckingTheCells()
    {
        for (int i = 0; i < _cellDirections.Length; i++)
        {
            Vector2 cords = _previousSelected._position + _cellDirections[i];
            if (cords == GetComponent<Cell>()._position)
            {
                return true;
            }
        }
        return false;
    }   
    private void Bonuses()
    {
        List<GameObject> _needRemove = new List<GameObject>();
        if (BonusAndActionSystem.instance._actionSystemData._firstBonusEnabled) // удаление крестом
        {
            for (int x = 0; x < GridSystem.instance._width; x++)
            {
                for (int y = 0; y < GridSystem.instance._height; y++)
                {
                    if (x == _position.x)
                        _needRemove.Add(GridSystem.instance.GetGridCells[x, y]);
                    else if (y == _position.y)
                        _needRemove.Add(GridSystem.instance.GetGridCells[x, y]);
                }
            }
            BonusAndActionSystem.instance._actionSystemData._firstBonusEnabled = false;
            BonusAndActionSystem.instance._actionSystemData.FirstBonusCount -= 1;
        }
        else if (BonusAndActionSystem.instance._actionSystemData._secondBonusEnabled)
        {
            Vector2[] _cords = { new Vector2(-1,-1), new Vector2(-1,0),new Vector2(-1,1),new Vector2(0,-1),new Vector2(0,0),new Vector2(0,1),
                new Vector2(1,-1),new Vector2(1,0),new Vector2(1,1)};
            
            foreach(var cord in _cords)
            {
                Vector2 _cellCord = _position - cord;
                if(_cellCord.x >= 0 && _cellCord.x < GridSystem.instance._width)
                {
                    if(_cellCord.y >= 0 && _cellCord.y < GridSystem.instance._height)
                    {
                        _needRemove.Add(GridSystem.instance.GetGridCells[(int)_cellCord.x, (int)_cellCord.y]);
                    }
                }
            }
            BonusAndActionSystem.instance._actionSystemData._secondBonusEnabled = false;
            BonusAndActionSystem.instance._actionSystemData.SecondBonusCount -= 1;
        }
        else
        {
            foreach (var cell in GridSystem.instance.GetGridCells)
            {
                if(cell.GetComponent<Cell>().GetSprite == _image.sprite)
                {
                    _needRemove.Add(cell);
                }
            }
            BonusAndActionSystem.instance._actionSystemData._thirdBonusEnabled = false;
            BonusAndActionSystem.instance._actionSystemData.ThirdBonusCount -= 1;
        }
        TaskSystem.instance._currentNumberOfPoints += _needRemove.Count * 6;
        TaskSystem.instance.PointsUpdate();
        foreach (var cell in _needRemove)
            cell.GetComponent<Cell>().RemoveSprite();
    }
    public void CheckAndRemoveCellsSprite(bool bonus)
    {
        List<Cell> vertical = CheckVertical();
        List<Cell> horizontal = CheckHorizontal();
        List<Cell> cells = new List<Cell>();

        if (vertical.Count > 2 && horizontal.Count > 2)
        {
            for (int i = 0; i < vertical.Count; i++)
            {
                if (!cells.Contains(vertical[i]))
                {
                    cells.Add(vertical[i]);
                }
            }
            for (int i = 0; i < horizontal.Count; i++)
            {
                if (!cells.Contains(horizontal[i]))
                {
                    cells.Add(horizontal[i]);
                }
            }
        }
        else if (vertical.Count > 2)
        {
            for (int i = 0; i < vertical.Count; i++)
            {
                if (!cells.Contains(vertical[i]))
                {
                    cells.Add(vertical[i]);
                }
            }
        }
        else if (horizontal.Count > 2)
        {
            for (int i = 0; i < horizontal.Count; i++)
            {
                if (!cells.Contains(horizontal[i]))
                {
                    cells.Add(horizontal[i]);
                }
            }
        }

        //Debug.Log("Кол-во клеток: " + cells.Count);
        //Система начисление очков бонусов + подключить систему очков

        if (cells.Count > 0) 
        {
            Debug.Log(cells.Count);
            if (cells.Count > 2 && cells.Count < 4)
            {
                TaskSystem.instance._currentNumberOfPoints += 5 * cells.Count;
            }
            else if (cells.Count > 5)
            {
                if(bonus)
                    BonusAndActionSystem.instance._actionSystemData.AddThirdBonusPoint();
                BonusAndActionSystem.instance.UpdateThirdBonusText();
                TaskSystem.instance._currentNumberOfPoints += 8 * cells.Count;
            }
            else if (cells.Count == 5)
            {
                if(bonus)
                    BonusAndActionSystem.instance._actionSystemData.AddSecondBonusPoint();
                BonusAndActionSystem.instance.UpdateSecondBonusText();
                TaskSystem.instance._currentNumberOfPoints += 7 * cells.Count;
            }
            else if (cells.Count == 4)
            {
                if(bonus)
                    BonusAndActionSystem.instance._actionSystemData.AddFirstBonusPoint();
                BonusAndActionSystem.instance.UpdateFirstBonusText();
                TaskSystem.instance._currentNumberOfPoints += 6 * cells.Count;
            }
            TaskSystem.instance.CheckingCondition(cells[0].GetSprite, cells.Count);
            foreach (var element in cells)
            {
                element.RemoveSprite();
                if(element.matchFound == true)
                {
                    DeIllumination();
                }
            }
            GridSystem.instance.CollapseCells();
            new WaitForSeconds(3f);
        }

    }
    private List<Cell> CheckVertical()
    {
        //Debug.Log("Проверка вертикали");
        List<Cell> cells = new List<Cell>();
        cells.Add(GetComponent<Cell>());

        int number = 1;
        bool firstSide = false, secondSide = false;

        Vector2 vectorVariable = new Vector2(Position.x, Position.y);

        for (int y = 0; y <= GridSystem.instance._height - 1; y++)
        {
            vectorVariable.y += number;

            if (vectorVariable.y <= 0 || vectorVariable.y >= GridSystem.instance._height - 1 
                || GridSystem.instance.GetGridCells[(int)vectorVariable.x, (int)vectorVariable.y].GetComponent<Cell>().GetSprite != _image.sprite)
            {
                number = number == 1 ? -1 : 1;
                vectorVariable = new Vector2(Position.x, Position.y);

                if (vectorVariable.y < 0 || vectorVariable.y == GridSystem.instance._height)
                {
                    break;
                }

                if (number == 1)
                    secondSide = true;
                else if (number == -1)
                    firstSide = true;

                if (secondSide && firstSide)
                {
                    break;
                }
            }
            else if (GridSystem.instance.GetGridCells[(int)vectorVariable.x, (int)vectorVariable.y].GetComponent<Cell>().GetSprite == _image.sprite)
            {
                cells.Add(GridSystem.instance.GetGridCells[(int)vectorVariable.x, (int)vectorVariable.y].GetComponent<Cell>());
            }
        }
        if (cells.Count < 2)
            cells.Clear();
        return cells;
    }
    private List<Cell> CheckHorizontal()
    {
        //Debug.Log("Проверка горизонтали");
        List<Cell> cells = new List<Cell>();
        cells.Add(GetComponent<Cell>());

        int number = 1;
        bool firstSide = false, secondSide = false;

        Vector2 vectorVariable = new Vector2(Position.x, Position.y);

        for (int x = 0; x <= GridSystem.instance._width - 1; x++)
        {
            vectorVariable.x += number;

            if (vectorVariable.x <= 0 || vectorVariable.x >= GridSystem.instance._width - 1 
                || GridSystem.instance.GetGridCells[(int)vectorVariable.x, (int)vectorVariable.y].GetComponent<Cell>().GetSprite != _image.sprite)
            {
                number = number == 1 ? -1 : 1;
                vectorVariable = new Vector2(Position.x, Position.y);

                if (vectorVariable.x < 0 || vectorVariable.x == GridSystem.instance._width)
                {
                    break;
                }

                if (number == 1)
                    secondSide = true;
                else if (number == -1)
                    firstSide = true;

                if (secondSide && firstSide)
                {
                    break;
                }
            }
            else if (GridSystem.instance.GetGridCells[(int)vectorVariable.x, (int)vectorVariable.y].GetComponent<Cell>().GetSprite == _image.sprite)
            {
                cells.Add(GridSystem.instance.GetGridCells[(int)vectorVariable.x, (int)vectorVariable.y].GetComponent<Cell>());
            }
        }
        if (cells.Count < 2)
            cells.Clear();
        return cells;
    }
    #endregion
}
