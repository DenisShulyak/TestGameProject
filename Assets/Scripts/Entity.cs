using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ������� ��������.
/// </summary>
public abstract class Entity : MonoBehaviour
{
    /// <summary>
    /// �������� ������������. 
    /// </summary>
    public float moveSpeed = 5f;

    /// <summary>
    /// ������������ ������.
    /// </summary>
    public int MaxHealth = 100;

    /// <summary>
    /// ����������� ��������.
    /// </summary>
    protected const int _minHealth = 0;

    /// <summary>
    /// ������� ��������.
    /// </summary>
    protected int currentHealth = 100;

    /// <summary>
    /// ���� ��������.
    /// </summary>
    protected Rigidbody2D rb;

    /// <summary>
    /// ��� ������ ��������.
    /// </summary>
    public Image Background;

    /// <summary>
    /// ������ ��������.
    /// </summary>
    public Image Fill;

    /// <summary>
    /// ���� �����.
    /// </summary>
    public AudioClip clipDamage;

    /// <summary>
    /// �������������.
    /// </summary>
    protected AudioSource sharedAudioSource;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (sharedAudioSource == null)
        {
            sharedAudioSource = FindObjectOfType<AudioSource>();
            if (sharedAudioSource == null)
            {
                GameObject audioObj = new GameObject("UI_AudioSource");
                sharedAudioSource = audioObj.AddComponent<AudioSource>();
            }
        }
    }

    void Update()
    {
    }

    /// <summary>
    /// ��������� �����.
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
