using TMPro;
using UnityEngine.UI;

/// <summary>
/// Слот инвенторя.
/// </summary>
public class SlotInventory
{
    public Image Image { get; set; }

    public TMP_Text CountSlot { get; set; }

    public ItemInventory Item;

    public Button DeleteButton { get; set; }

    public Button UseButton { get; set; }
}