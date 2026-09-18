using UnityEngine;

public class MoveDown : MonoBehaviour
{
    public float speed = 80.0f;
    private float zDestroy = -14.0f;

    private Rigidbody objectRb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        objectRb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // objectRb.AddForce(Vector3.back * speed, ForceMode.Impulse);
        objectRb.AddForce(Vector3.back * speed);

        if (transform.position.z < zDestroy)
        {
            Destroy(gameObject);
        }
    }
}
