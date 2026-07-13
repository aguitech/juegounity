using UnityEngine;

public class GroundScroller : MonoBehaviour
{
    public float scrollSpeed = 12f;
    public float resetDistance = 40f;
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.currentState != GameManager.GameState.Playing) return;

        transform.Translate(Vector3.back * GameManager.Instance.speed * Time.deltaTime);

        // Cuando se aleja demasiado, vuelve a la posición inicial para loop infinito
        if (transform.position.z < startPos.z - resetDistance)
        {
            transform.position = startPos;
        }
    }
}