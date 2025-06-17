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
        // Находим Canvas с именем "SpawnZone"
        GameObject spawnZoneObject = GameObject.Find("SpawnZone");
        if (spawnZoneObject != null)
        {
            spawnZone = spawnZoneObject.GetComponent<RectTransform>();
        }
        else
        {
            Debug.LogError("Не найден объект Canvas с именем 'SpawnZone'");
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

        // Получаем случайную позицию в пределах SpawnZone
        Vector2 spawnPosition = GetRandomPositionInSpawnZone();

        // Создаем объект
        Instantiate(toSpawn, spawnPosition, Quaternion.identity, spawnZone);
    }

    Vector2 GetRandomPositionInSpawnZone()
    {
        // Получаем размеры SpawnZone
        float width = spawnZone.rect.width;
        float height = spawnZone.rect.height;

        // Получаем случайные координаты внутри SpawnZone
        float randomX = Random.Range(-width / 2, width / 2);
        float randomY = Random.Range(-height / 2, height / 2);

        // Возвращаем позицию относительно центра SpawnZone
        return spawnZone.TransformPoint(new Vector2(randomX, randomY));
    }
}