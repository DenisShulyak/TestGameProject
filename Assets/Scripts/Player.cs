using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// �������� "�����".
/// </summary>
public class Player : Entity
{
    /// <summary>
    /// �������� �����������.
    /// </summary>
    public Joystick joystick;

    /// <summary>
    /// ������ ��������.
    /// </summary>
    public Button shootButton;

    /// <summary>
    /// ������ ���������.
    /// </summary>
    public Button bagButton;

    /// <summary>
    /// ���� ��������.
    /// </summary>
    [SerializeField] private int shootDamage = 50;

    /// <summary>
    /// ������ ������ ������.
    /// </summary>
    [SerializeField] private float searchRadius = 5f;

    /// <summary>
    /// ����� ���������� ��������.
    /// </summary>
    public TMP_Text textCountBullets;

    /// <summary>
    /// ����� ���-�� ���������.
    /// </summary>
    public TMP_Text textCountItems;

    /// <summary>
    /// ���-�� ��������.
    /// </summary>
    private int countBullets = 10;

    [Header("Inventory Settings")]
    public Image bag; // �������� ����������� ���������.
    public List<Image> inventorySlots = new List<Image>(); // ������ ����� ���������.
    public List<TMP_Text> inventorySlotsTexts = new List<TMP_Text>(); // ������ ����� ���������.
    public List<Button> inventorySlotsDeleteButtons = new List<Button>(); // ������ ������ ��������.
    public List<Button> inventorySlotsUseButtons = new List<Button>();
    private List<SlotInventory> slots = new List<SlotInventory>();
    private bool isBagOpen = false; // ���� ��������� ���������.

    protected override void Start()
    {
        base.Start();

        textCountBullets.text = "x" + countBullets;
        SetBagVisibility(isBagOpen);

        // ������������� ������ ���������.
        if (inventorySlots.Count == inventorySlotsTexts.Count && inventorySlotsDeleteButtons.Count == inventorySlots.Count)
        {
            for (int i = 0; i < inventorySlots.Count; i++)
            {
                slots.Add(new SlotInventory
                {
                    Image = inventorySlots[i],
                    CountSlot = inventorySlotsTexts[i],
                    DeleteButton = inventorySlotsDeleteButtons[i],
                    UseButton = inventorySlotsUseButtons[i],
                });
                slots[i].DeleteButton.gameObject.SetActive(false);
                int index = i;
                slots[i].DeleteButton.onClick.AddListener(() => DeleteItem(slots[index]));

                slots[i].UseButton.onClick.AddListener(() => UseItem(slots[index]));
            }
        }

        // ������������� �� ������� ������� ������
        shootButton.onClick.AddListener(ShootNearestEnemy);
        bagButton.onClick.AddListener(ToggleBag);
    }

    void Update()
    {
        Vector2 movement = new Vector2(joystick.Horizontal, joystick.Vertical);
        rb.velocity = movement * moveSpeed;
    }

    /// <summary>
    /// ����������� ����������� ���������
    /// </summary>
    private void ToggleBag()
    {
        isBagOpen = !isBagOpen;
        SetBagVisibility(isBagOpen);

        // ���� ��������� ������ - ��������� ����������� ���������
        if (isBagOpen)
        {
            UpdateInventoryUI();
        }
    }

    /// <summary>
    /// �������� ��������.
    /// </summary>
    /// <param name="slot"></param>
    private void DeleteItem(SlotInventory slot)
    {
        if(slot.Item.Count > 1)
        {
            slot.Item.Count--;
        }
        else
        {
            // ���� �������� ���������, ��������� ����.
            slot.Item = null;
            slot.DeleteButton.gameObject.SetActive(false);
            slot.CountSlot.text = string.Empty;
        }

        UpdateInventoryUI();
    }

    /// <summary>
    /// ������������� ��������.
    /// </summary>
    /// <param name="slot"></param>
    private void UseItem(SlotInventory slot)
    {
        if(slot.Item != null)
        {
            var name = slot.Item.Name;
            if(name == "Bullets")
            {
                countBullets += 10;
                textCountBullets.text = "x" + countBullets;
            }
            else if(name == "BulletproofCloak")
            {
                currentHealth += 30;
                UpdateHealthBar();
            }
            else if (name == "MilitaryHelmet")
            {
                currentHealth += 20;
                UpdateHealthBar();
            }
            else if(name == "SovietBag")
            {
                moveSpeed++;
            }

            if(currentHealth > MaxHealth)
            {
                currentHealth = MaxHealth;
            }

            DeleteItem(slot);
        }
    }

    /// <summary>
    /// ��������� ����������� ��������� � ���������
    /// </summary>
    private void UpdateInventoryUI()
    {
        // ������� ������� ��� ������
        foreach (var slot in slots)
        {
            slot.Image.sprite = null;
            slot.Image.color = new Color(1, 1, 1, 0.2f); // ��������������, ���� �����
        }

        foreach(var slot in slots)
        {
            if(slot.Item != null)
            {
                var itemSprite = slot.Item.GameObject.GetComponent<SpriteRenderer>()?.sprite;
                if (itemSprite != null)
                {
                    slot.Image.sprite = itemSprite;
                    slot.Image.color = Color.white;
                    slot.CountSlot.text = "x" + slot.Item.Count;
                }
            }
        }

        textCountItems.text = "x" + slots.Where(x => x.Item != null).Count();
    }

    /// <summary>
    /// ������������� ��������� ���������
    /// </summary>
    private void SetBagVisibility(bool visible)
    {
        bag.gameObject.SetActive(visible);
    }


    /// <summary>
    /// ������� � ������� ���������� �����.
    /// </summary>
    private void ShootNearestEnemy()
    {
        if (countBullets > 0)
        {
            // �������� ��� ���������� � �������
            Collider2D[] allColliders = Physics2D.OverlapCircleAll(transform.position, searchRadius);

            Entity nearestEnemy = null;
            float minDistance = Mathf.Infinity;

            foreach (Collider2D collider in allColliders)
            {
                // ���������, ��� ��� ���� (�� ���� ��� ����������)
                if (collider.CompareTag("Enemy") || collider.GetComponent<Enemy>() != null)
                {
                    Entity enemy = collider.GetComponent<Entity>();
                    if (enemy != null)
                    {
                        float distance = Vector2.Distance(transform.position, collider.transform.position);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            nearestEnemy = enemy;
                        }
                    }
                }
            }

            if (nearestEnemy != null)
            {
                nearestEnemy.TakeDamage(shootDamage);
                countBullets--;
                textCountBullets.text = "x" + countBullets;
                Debug.Log($"Hit enemy for {shootDamage} damage!");
            }
            else
            {
                Debug.Log("No enemies in range!");
            }
        }
    }

    // ��������� ���������� ������������
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
        {
            // ��������� ������� � ������

            var item = other.gameObject;

            var slot = slots.Where(x => x.Item != null).FirstOrDefault(x => item.name.Contains(x.Item.Name));

            if(slot == null)
            {
                slot = slots.Where(x => x.Item == null).First();
                slot.Item = new ItemInventory
                {
                    GameObject = item,
                    Name = item.name.Replace("(Clone)", "").Trim(),
                    Count = 1,
                };
                slot.DeleteButton.gameObject.SetActive(true);
            }
            else
            {
                slot.Item.Count++;
            }

            // ��������� ������� (����� �������� �� Destroy)
            item.SetActive(false);

            UpdateInventoryUI();
        }
    }

    // ������������ ������� ������ � ���������
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, searchRadius);
    }

    protected override void Die()
    {
        base.Die();
        Debug.Log("Player is died!");
        // ���������� ����� ��� ������ ����� ����.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}