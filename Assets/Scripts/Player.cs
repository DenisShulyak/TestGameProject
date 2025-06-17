using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
    private int countBullets = 99;

    /// <summary>
    /// Предметы.
    /// </summary>
    private List<GameObject> items = new List<GameObject>();


    [Header("Inventory Settings")]
    public Image bag; // Основное изображение инвентаря
    public List<Image> inventorySlots = new List<Image>(); // Список ячеек инвентаря
    private bool isBagOpen = false; // Флаг состояния инвентаря

    protected override void Start()
    {
        base.Start();

        textCountBullets.text = "x" + countBullets;
        SetBagVisibility(isBagOpen);

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
    /// Обновляет отображение предметов в инвентаре
    /// </summary>
    private void UpdateInventoryUI()
    {
        // Сначала очищаем все ячейки
        foreach (var slot in inventorySlots)
        {
            slot.sprite = null;
            slot.color = new Color(1, 1, 1, 0.2f); // Полупрозрачный, если пусто
        }

        // Заполняем ячейки собранными предметами
        for (int i = 0; i < items.Count && i < inventorySlots.Count; i++)
        {
            var itemSprite = items[i].GetComponent<SpriteRenderer>()?.sprite;
            if (itemSprite != null)
            {
                inventorySlots[i].sprite = itemSprite;
                inventorySlots[i].color = Color.white;
            }
        }

        textCountItems.text = "x" + items.Count;
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
            items.Add(other.gameObject);

            // Отключаем предмет (можно заменить на Destroy)
            other.gameObject.SetActive(false);

            // Или полностью удаляем со сцены:
            // Destroy(other.gameObject);
            textCountItems.text = "x" + items.Count;
            Debug.Log($"Item collected! Total items: {items.Count}");
        }
    }

    // Визуализация радиуса поиска в редакторе
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, searchRadius);
    }
}