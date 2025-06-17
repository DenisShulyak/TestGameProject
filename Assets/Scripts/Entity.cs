using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Игровая сущность.
/// </summary>
public abstract class Entity : MonoBehaviour
{
    /// <summary>
    /// Скорость передвижения. 
    /// </summary>
    public float moveSpeed = 5f;

    /// <summary>
    /// Максимальное здовье.
    /// </summary>
    public int MaxHealth = 100;

    /// <summary>
    /// Минимальное здоровье.
    /// </summary>
    protected const int _minHealth = 0;

    /// <summary>
    /// Текущее здоровье.
    /// </summary>
    protected int currentHealth = 100;

    /// <summary>
    /// Тело сущности.
    /// </summary>
    protected Rigidbody2D rb;

    /// <summary>
    /// Фон полосы здоровья.
    /// </summary>
    public Image Background;

    /// <summary>
    /// Полоса здоровья.
    /// </summary>
    public Image Fill;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
    }

    /// <summary>
    /// Получение урона.
    /// </summary>
    /// <param name="damage"></param>
    public virtual void TakeDamage(int damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, _minHealth, MaxHealth);
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void UpdateHealthBar()
    {
        if (Fill != null)
        {
            Fill.fillAmount = (float)currentHealth / MaxHealth;
        }
    }

    protected virtual void Die()
    {
        Debug.Log("Entity died!");
    }
}
