using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
    private int countBullets = 99;

    /// <summary>
    /// ��������.
    /// </summary>
    private List<GameObject> items = new List<GameObject>();


    [Header("Inventory Settings")]
    public Image bag; // �������� ����������� ���������
    public List<Image> inventorySlots = new List<Image>(); // ������ ����� ���������
    private bool isBagOpen = false; // ���� ��������� ���������

    protected override void Start()
    {
        base.Start();

        textCountBullets.text = "x" + countBullets;
        SetBagVisibility(isBagOpen);

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
    /// ��������� ����������� ��������� � ���������
    /// </summary>
    private void UpdateInventoryUI()
    {
        // ������� ������� ��� ������
        foreach (var slot in inventorySlots)
        {
            slot.sprite = null;
            slot.color = new Color(1, 1, 1, 0.2f); // ��������������, ���� �����
        }

        // ��������� ������ ���������� ����������
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
            items.Add(other.gameObject);

            // ��������� ������� (����� �������� �� Destroy)
            other.gameObject.SetActive(false);

            // ��� ��������� ������� �� �����:
            // Destroy(other.gameObject);
            textCountItems.text = "x" + items.Count;
            Debug.Log($"Item collected! Total items: {items.Count}");
        }
    }

    // ������������ ������� ������ � ���������
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, searchRadius);
    }
}