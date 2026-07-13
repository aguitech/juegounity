using UnityEngine;

public class OrbFloat : MonoBehaviour
{
    public float amplitude = 0.4f;
    public float frequency = 2f;
    public float phase = 0f;

    private Vector3 startPos;
    private float timeOffset;

    void Start()
    {
        startPos = transform.position;
        timeOffset = Random.Range(0f, 10f);
    }

    void Update()
    {
        float y = startPos.y + Mathf.Sin((Time.time + timeOffset + phase) * frequency) * amplitude;
        transform.position = new Vector3(startPos.x, y, startPos.z);

        // Rotación
        transform.Rotate(Vector3.up, 90f * Time.deltaTime);
    }
}