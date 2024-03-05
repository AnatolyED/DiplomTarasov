using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level Data", menuName = "Level/LevelDataScriptableObject", order = 1)]
public class LevelDataScript : ScriptableObject
{
    [Header("������ ��� ������")]
    public Sprite levelBackGroundSprite;
    [Header("������� ����")]
    public PlayingFieldSize size;
    [Header("������� ������ ������")]
    [SerializeField] private List<Vector2> emptyCells;
    public List<Vector2> GetEmptyCell { get => emptyCells; }
    [Header("������� ������ - �����������")]
    [SerializeField] private List<Sprite> obstaclesSprite;
    public List<Sprite> GetObstaclesPrefab { get => obstaclesSprite; }
    [Header("������� ������ - �����������")]
    [SerializeField] private List<Vector2> obstacles;
    public List<Vector2> GetObstacles { get => obstacles; }
    [Header("������ �������� ������")]
    public CellData cellData;
}
[System.Serializable]
public struct CellData
{
    [SerializeField]private int CellSize;
    [SerializeField]private GameObject CellPrefab;
    [SerializeField]private List<Sprite> CellSprites;
    public int GetSize
    {
        get => CellSize;
    }
    public GameObject GetPrefab
    {
        get => CellPrefab;
    }
    public List<Sprite> GetSprites
    {
        get => CellSprites;
    }
}
[System.Serializable]
public struct PlayingFieldSize
{
    [Tooltip("���-�� ������ � ������"), SerializeField]private int BoardHeight;
    [Tooltip("���-�� ������ � ������"), SerializeField]private int BoardWidth;
    public int GetHeight
    {
        get => BoardHeight;
    }
    public int GetWidth
    {
        get => BoardWidth;
    }
}
