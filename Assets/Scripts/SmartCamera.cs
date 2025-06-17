using UnityEngine;

public class SmartCamera : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    private Transform target;
    [SerializeField] private float offset = 1f;
    public GameObject Player;
    void Start()
    {
        target = Player.transform;
        transform.position = new Vector3(target.transform.position.x, target.transform.position.y + offset, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = target.position;
        position.z = transform.position.z;
        position.y += offset;
        transform.position = Vector3.Lerp(transform.position, position, speed * Time.deltaTime);
    }
}
