using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Сущность "Игрок".
/// </summary>
public class Player : Entity
{
    /// <summary>
    /// Джойстик перемещения.
    /// </summary>
    public Joystick joystick;

    /// <summary>
    /// Кнопка выстрела.
    /// </summary>
    public Button shootButton;

    /// <summary>
    /// Кнопка инвенторя.
    /// </summary>
    public Button bagButton;

    /// <summary>
    /// Урон выстрела.
    /// </summary>
    [SerializeField] private int shootDamage = 50;

    /// <summary>
    /// Радиус поиска врагов.
    /// </summary>
    [SerializeField] private float searchRadius = 5f;

    /// <summary>
    /// Текст количества паторнов.
    /// </summary>
    public TMP_Text textCountBullets;

    /// <summary>
    /// Текст кол-ва предметов.
    /// </summary>
    public TMP_Text textCountItems;

    /// <summary>
    /// Кол-во патронов.
    /// </summary>
    private int countBullets = 10;

    [Header("Inventory Settings")]
    public Image bag; // Основное изображение инвентаря.
    public List<Image> inventorySlots = new List<Image>(); // Список ячеек инвентаря.
    public List<TMP_Text> inventorySlotsTexts = new List<TMP_Text>(); // Список ячеек инвентаря.
    public List<Button> inventorySlotsDeleteButtons = new List<Button>(); // Список кнопок удаления.
    public List<Button> inventorySlotsUseButtons = new List<Button>();
    private List<SlotInventory> slots = new List<SlotInventory>();
    private bool isBagOpen = false; // Флаг состояния инвентаря.

    protected override void Start()
    {
        base.Start();

        textCountBullets.text = "x" + countBullets;
        SetBagVisibility(isBagOpen);

        // Инициализация слотов инвентаря.
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

        // Подписываемся на событие нажатия кнопки
        shootButton.onClick.AddListener(ShootNearestEnemy);
        bagButton.onClick.AddListener(ToggleBag);
    }

    void Update()
    {
        Vector2 movement = new Vector2(joystick.Horizontal, joystick.Vertical);
        rb.velocity = movement * moveSpeed;
    }

    /// <summary>
    /// Переключает отображение инвентаря
    /// </summary>
    private void ToggleBag()
    {
        isBagOpen = !isBagOpen;
        SetBagVisibility(isBagOpen);

        // Если инвентарь открыт - обновляем отображение предметов
        if (isBagOpen)
        {
            UpdateInventoryUI();
        }
    }

    /// <summary>
    /// Удаление предмета.
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
            // Если предметы кончились, подчищаем слот.
            slot.Item = null;
            slot.DeleteButton.gameObject.SetActive(false);
            slot.CountSlot.text = string.Empty;
        }

        UpdateInventoryUI();
    }

    /// <summary>
    /// Использование предмета.
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
    /// Обновляет отображение предметов в инвентаре
    /// </summary>
    private void UpdateInventoryUI()
    {
        // Сначала очищаем все ячейки
        foreach (var slot in slots)
        {
            slot.Image.sprite = null;
            slot.Image.color = new Color(1, 1, 1, 0.2f); // Полупрозрачный, если пусто
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
    /// Устанавливает видимость инвентаря
    /// </summary>
    private void SetBagVisibility(bool visible)
    {
        bag.gameObject.SetActive(visible);
    }


    /// <summary>
    /// Находит и атакует ближайшего врага.
    /// </summary>
    private void ShootNearestEnemy()
    {
        if (countBullets > 0)
        {
            // Получаем все коллайдеры в радиусе
            Collider2D[] allColliders = Physics2D.OverlapCircleAll(transform.position, searchRadius);

            Entity nearestEnemy = null;
            float minDistance = Mathf.Infinity;

            foreach (Collider2D collider in allColliders)
            {
                // Проверяем, что это враг (по тегу или компоненту)
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

    // Добавляем обработчик столкновений
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
        {
            // Добавляем предмет в список

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

            // Отключаем предмет (можно заменить на Destroy)
            item.SetActive(false);

            UpdateInventoryUI();
        }
    }

    // Визуализация радиуса поиска в редакторе
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, searchRadius);
    }

    protected override void Die()
    {
        base.Die();
        Debug.Log("Player is died!");
        // Перезапуск сцены для начала новой игры.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}