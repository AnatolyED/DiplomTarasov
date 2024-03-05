using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GridSystem : MonoBehaviour
{
    public static GridSystem instance;
    [Header("Объект для работы с фоном")]
    [SerializeField] private GameObject _backGround;
    [SerializeField] public Sprite _emptyCellSprite;
    [Header("Компоненты поля")]
    [SerializeField] private RectTransform _boardRT;
    [Header("Работа с сеткой")]
    [SerializeField] private LevelDataScript levelData;
    public int _height, _width, _cellSize;
    [SerializeField] private GameObject[,] _gridCells;
    [SerializeField] List<GameObject> _gridCellsList;
    [SerializeField] private float _collapseSpeed;
    public bool IsShifting { get; set; }
    [Header("Работа с корутинами")]
    [SerializeField] private bool activeCellInspector;
    [SerializeField] private bool activeCollapseInspector = false;
    [SerializeField] private bool activeCheckingGrid = false;
    [SerializeField] public int _needShuflePoints = 0;
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
        _boardRT = GetComponent<RectTransform>();
        _backGround.GetComponent<Image>().sprite = levelData.levelBackGroundSprite;
        _gridCells = new GameObject[levelData.size.GetWidth, levelData.size.GetHeight];
        MapGenerator();
        StartCoroutine(CollapseCells());
    }
    #region Grid System methods
    private void MapGenerator()
    {
        _height = levelData.size.GetHeight;
        _width = levelData.size.GetWidth;
        _cellSize = levelData.cellData.GetSize;

        GameObject cellPrefab = levelData.cellData.GetPrefab;
        List<Vector2> emptyCells = levelData.GetEmptyCell;
        List<Vector2> obstaclesCell = levelData.GetObstacles;

        _boardRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _height * _cellSize);
        _boardRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _width * _cellSize);

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                _gridCells[x, y] = CellCreator(new Vector2(x, y), cellPrefab, _cellSize);
                _gridCells[x, y].GetComponent<Cell>().CellType = ChekingTheCellType(new Vector2(x, y),
                    obstaclesCell, emptyCells);
                if (_gridCells[x, y].GetComponent<Cell>().CellType == CellType.EmptyCell)
                    _gridCells[x, y].GetComponent<Cell>().SetColor(new Color(0, 0, 0, 0));
                _gridCellsList.Add(_gridCells[x, y]);
            }
        }
        StartCoroutine(StartCheckingTheGrid());
    }
    public GameObject CellCreator(Vector2 position, GameObject prefab, int cellSize)
    {
        GameObject Cell = Instantiate(prefab, gameObject.transform);
        Cell.GetComponent<Cell>().Position = position;
        Cell.GetComponent<RectTransform>().anchoredPosition += new Vector2(cellSize * position.x, -cellSize * position.y);
        Cell.GetComponent<RectTransform>().sizeDelta = new Vector2(cellSize, cellSize);
        Cell.GetComponent<BoxCollider2D>().size = new Vector2(cellSize, cellSize);
        Cell.GetComponent<BoxCollider2D>().offset = new Vector2((float)cellSize / 2, (float)-cellSize / 2);
        return Cell;
    }
    public CellType ChekingTheCellType(Vector2 position, List<Vector2> obstacles, List<Vector2> emptyCell)
    {
        for (int i = 0; i < obstacles.Count; i++)
        {
            if (obstacles[i] == position)
            {
                return CellType.Obstacle;
            }
        }
        for (int i = 0; i < emptyCell.Count; i++)
        {
            if (emptyCell[i] == position)
            {
                return CellType.EmptyCell;
            }
        }
        return CellType.Base;
    }
    private IEnumerator StartCheckingTheGrid()
    {
        activeCellInspector = true;
        List<Cell> cellNeedField = new List<Cell>();
        yield return new WaitForSeconds(Time.deltaTime);
        while (true)
        {
            cellNeedField.Clear();
            foreach (var elem in _gridCells)
            {
                if (elem.GetComponent<Cell>().GetSprite == _emptyCellSprite && elem.GetComponent<Cell>().CellType == CellType.Base)
                {
                    cellNeedField.Add(elem.GetComponent<Cell>());
                }
            }

            StartCoroutine(StartCellFielder(cellNeedField));
            if (!activeCellInspector)
            {
                StartCoroutine(CheckingTheGrid(true));
                break;
            }
            yield return null;
        }
    }
    public IEnumerator CheckingTheGrid(bool active)
    {
        activeCheckingGrid = active;
        while (true)
        {
            foreach (var cell in _gridCells)
            {
                cell.GetComponent<Cell>().CheckAndRemoveCellsSprite(false);
                new WaitForSeconds(0.5f);
            }
            if (!activeCheckingGrid)
            {
                break;
            }
            if(_needShuflePoints > 5)
            {

            }
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
    private IEnumerator StartCellFielder(List<Cell> cells) // Переработать или объединить в единую систему;
    {
        Sprite newSprite;
        for (int x = 0; x < cells.Count; x++)
        {
            if (cells[x].GetComponent<Cell>().CellType == CellType.Base)
            {
                newSprite = CellSpriteGenerator();
            }
            else if (cells[x].GetComponent<Cell>().CellType == CellType.Obstacle)
            {
                newSprite = ObstacleSpriteGenerator();
            }
            else
            {
                Debug.Log(cells[x].Position + " пустая клетка");
                continue;
            }
            cells[x].AddSprite(newSprite);
            if (x == cells.Count - 1)
                break;
        }
        activeCellInspector = false; //отрубает заполнитель и первичную проверку поля, запуская постоянную проверку игрового поля на поиск одинаковых клеток и их уничтожение

        yield return null;
    }
    public IEnumerator CollapseCells()
    {
        if (activeCollapseInspector) 
        {
            StopCoroutine(CollapseCells());
            //Debug.Log("Корутина удалена");
        }
        else
        {
            //Debug.Log("Корутина активна");
            activeCollapseInspector = true;
            for (int i = 0; i < _height; i++)
            {
                for (int y = 0; y < _height; y++)
                {
                    for (int x = 0; x < _width; x++)
                    {
                        if (_gridCells[x, y] != null && _gridCells[x, y].GetComponent<Cell>().CellType == CellType.Base)
                        {
                            int targetY = y + 1;
                            if (targetY < _height && _gridCells[x, targetY].GetComponent<Cell>().GetSprite == _emptyCellSprite && _gridCells[x, targetY].GetComponent<Cell>().CellType == CellType.Base)
                            {
                                _gridCells[x, y].GetComponent<Cell>().TransferSprite(_gridCells[x, targetY].GetComponent<Cell>());
                            }
                            if (_gridCells[x, 0].GetComponent<Cell>().GetSprite == _emptyCellSprite && _gridCells[x, 0].GetComponent<Cell>().CellType == CellType.Base)
                            {
                                _gridCells[x, 0].GetComponent<Cell>().AddSprite(CellSpriteGenerator());
                            }
                            if (y == _height - 1 && x == _width - 1)
                            {
                                break;
                            }
                        }
                    }
                    yield return new WaitForSeconds(Time.deltaTime);
                }
            }
            activeCollapseInspector = false;
        }
    }
    public void DeadLock()
    {
        Vector2[] _cords = { new Vector2(-1,-1), new Vector2(-1,0),new Vector2(-1,1),new Vector2(0,-1),new Vector2(0,0),new Vector2(0,1),
                new Vector2(1,-1),new Vector2(1,0),new Vector2(1,1)};
        bool needShufle = true;
        List<Cell> _cells = new List<Cell>();
        foreach (var cell in _gridCells)
        {
            _cells.Clear();
            
            _cells.Add(cell.GetComponent<Cell>());

            Vector2 left = cell.GetComponent<Cell>().Position + Vector2.left
                , right = cell.GetComponent<Cell>().Position + Vector2.right
                , down = cell.GetComponent<Cell>().Position + Vector2.down
                , up = cell.GetComponent<Cell>().Position + Vector2.up;

            foreach (var cord in _cords)
            {
                Vector2 nextCellPos = cell.GetComponent<Cell>().Position + cord;
                if (nextCellPos.x >= 0 && nextCellPos.x < _width)
                    if (nextCellPos.y >= 0 && nextCellPos.y < _height)
                    {
                        Cell nextCell = _gridCells[(int)nextCellPos.x, (int)nextCellPos.y].GetComponent<Cell>();
                        if (nextCell.GetSprite == cell.GetComponent<Cell>().GetSprite)
                            _cells.Add(nextCell);
                    }
            }
            if (_cells.Count >= 2) {
                var duplicateValuesX = _cells.GroupBy(i => i.Position.x)
                                       .Where(g => g.Count() > 1)
                                       .Select(g => g.Key)
                                       .ToList();
                var duplicateValuesY = _cells.GroupBy(i => i.Position.y)
                                       .Where(g => g.Count() > 1)
                                       .Select(g => g.Key)
                                       .ToList();
                if (duplicateValuesX.Count > 1 || duplicateValuesY.Count > 1)
                {

                    needShufle = false;
                    foreach (var elem in _cells)
                    {
                        elem.Illumination();
                        elem.matchFound = true;
                    }
                    break;
#region наработки
                    /*
                    int count = _cells.Count;
                    Debug.Log("Тест прошел успешно");
                    foreach (var elem in _cells) {
                        foreach(var cord in _cords)
                    {
                            Vector2 nextCellPos = elem.GetComponent<Cell>().Position + cord;
                            if (nextCellPos.x >= 0 && nextCellPos.x < _width)
                                if (nextCellPos.y >= 0 && nextCellPos.y < _height)
                                {
                                    Cell nextCell = _gridCells[(int)nextCellPos.x, (int)nextCellPos.y].GetComponent<Cell>();
                                    if (nextCell.GetSprite == elem.GetComponent<Cell>().GetSprite && !_cells.Contains(nextCell))
                                        _cells.Add(nextCell);
                                }
                        }
                    */
                    #endregion
                }
                else if(needShufle)
                {
                    _needShuflePoints++;
                }
            }
            
        }
    }
    public void GridShufle()
    {
        Debug.Log("Шафлим сетку");
        foreach (var cell in _gridCells)
        {
            cell.GetComponent<Cell>().SwapSprite(_gridCells[Random.Range(0, _width - 1), Random.Range(0, _height - 1)].GetComponent<Cell>().GetImage);
        }
    }
    public Sprite CellSpriteGenerator()
    {
        return levelData.cellData.GetSprites[Random.Range(0, levelData.cellData.GetSprites.Count)];
    }
    public Sprite ObstacleSpriteGenerator()
    {
        return levelData.GetObstaclesPrefab[Random.Range(0, levelData.GetObstaclesPrefab.Count)];
    }
    public void DeselectAllCells()
    {
        foreach (var cell in _gridCells)
        {
            if (cell.GetComponent<Cell>().IsSelect)
            {
                Debug.Log(cell.GetComponent<Cell>().IsSelect);
                cell.GetComponent<Cell>().Deselect();
            }
        }
    }
    public void IlluminationOffAllCells()
    {
        foreach (var cell in _gridCells)
        {
            if (cell.GetComponent<Cell>().matchFound)
            {
                cell.GetComponent<Cell>().DeIllumination();
            }
        }
    }
    #endregion
    #region Getters and Setters
    public GameObject[,] GetGridCells
    {
        get => _gridCells;
    }
    #endregion
}