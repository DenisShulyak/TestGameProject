using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    [SerializeField] GameObject[] spawnObj;
    [SerializeField] float spawnDelay;
    [SerializeField] int countSpawn = 3;

    float timer = 0;
    RectTransform spawnZone;

    void Start()
    {
        // ������� Canvas � ������ "SpawnZone"
        GameObject spawnZoneObject = GameObject.Find("SpawnZone");
        if (spawnZoneObject != null)
        {
            spawnZone = spawnZoneObject.GetComponent<RectTransform>();
        }
        else
        {
            Debug.LogError("�� ������ ������ Canvas � ������ 'SpawnZone'");
        }
    }

    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else if (spawnZone != null)
        {
            timer = spawnDelay;
            for (int i = 0; i < countSpawn; i++)
            {
                SpawnObject();
            }
        }
    }

    void SpawnObject()
    {
        GameObject toSpawn = spawnObj[Random.Range(0, spawnObj.Length)];

        // �������� ��������� ������� � �������� SpawnZone
        Vector2 spawnPosition = GetRandomPositionInSpawnZone();

        // ������� ������
        Instantiate(toSpawn, spawnPosition, Quaternion.identity, spawnZone);
    }

    Vector2 GetRandomPositionInSpawnZone()
    {
        // �������� ������� SpawnZone
        float width = spawnZone.rect.width;
        float height = spawnZone.rect.height;

        // �������� ��������� ���������� ������ SpawnZone
        float randomX = Random.Range(-width / 2, width / 2);
        float randomY = Random.Range(-height / 2, height / 2);

        // ���������� ������� ������������ ������ SpawnZone
        return spawnZone.TransformPoint(new Vector2(randomX, randomY));
    }
}