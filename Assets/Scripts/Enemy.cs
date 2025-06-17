using UnityEngine;

/// <summary>
/// Сущность - "Враг".
/// </summary>
public class Enemy : Entity
{

    /// <summary>
    /// Дистанция обнаружения игрока.
    /// Урон при столкновении.
    /// Задержка между атаками.
    /// Цвет области видимости.
    /// Включить/выключить визуализацию.
    /// </summary>
    [Header("Enemy Settings")]
    [SerializeField] private float detectionRange = 20f;
    [SerializeField] private int damageAmount = 10;
    [SerializeField] private float damageCooldown = 1f;
    [SerializeField] private Color detectionColor = Color.red;
    [SerializeField] private bool showDetectionRange = true;

    /// <summary>
    /// Расположение игрока.
    /// </summary>
    private Transform playerTransform;

    /// <summary>
    /// Время нанаесения урона.
    /// </summary>
    private float lastDamageTime;

    /// <summary>
    /// Вектор движения.
    /// </summary>
    private Vector2 movement;

    /// <summary>
    /// Обноружение игрока.
    /// </summary>
    private bool playerDetected = false;

    /// <summary>
    /// Назначенные предметы выпадающии при смерти.
    /// </summary>
    [SerializeField] private GameObject[] itemPrefabs;

    /// <summary>
    /// Инициализация игрока.
    /// </summary>
    protected override void Start()
    {
        base.Start();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        // Проверяем дистанцию до игрока
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        playerDetected = distanceToPlayer <= detectionRange;

        if (playerDetected)
        {
            Vector3 direction = playerTransform.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            direction.Normalize();
            movement = direction;
        }
        else
        {
            movement = Vector2.zero;
        }
    }

    private void FixedUpdate()
    {
        if (playerDetected)
        {
            MoveChar(movement);
        }
    }

    private void MoveChar(Vector2 direction)
    {
        rb.MovePosition((Vector2)transform.position + (direction * moveSpeed * Time.deltaTime));
    }

    // Визуализация в редакторе
    private void OnDrawGizmosSelected()
    {
        if (showDetectionRange)
        {
            Gizmos.color = detectionColor;
            Gizmos.DrawWireSphere(transform.position, detectionRange);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Time.time - lastDamageTime >= damageCooldown)
            {
                Player player = collision.gameObject.GetComponent<Player>();
                if (player != null)
                {
                    player.TakeDamage(damageAmount);
                    lastDamageTime = Time.time;
                }
            }
        }
    }

    protected override void Die()
    {
        base.Die();

        if (itemPrefabs.Length > 0)
        {
            var randomPrefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];
            Instantiate(randomPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}