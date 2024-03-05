using UnityEngine;

[System.Serializable]
public class Task
{
    [SerializeField] private Sprite item;
    [SerializeField] private int count;
    [SerializeField] private bool taskHasBeenCompleted;
    public Task()
    {
        count = 0;
        item = null;
    }
    public Sprite Item
    {
        get => item;
        set => item = value;
    }
    public int Count
    {
        get => count;
        set => count = value;
    }
    public bool TaskHasBeenCompleted
    {
        get => taskHasBeenCompleted;
        set => taskHasBeenCompleted = value;
    }
}
