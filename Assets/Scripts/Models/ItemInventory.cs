using UnityEngine;

/// <summary>
/// Предмет в инвенторе.
/// </summary>
public class ItemInventory
{
    public GameObject GameObject { get; set; }

    public string Name { get; set; }

    public int Count { get; set; } = 0;
}