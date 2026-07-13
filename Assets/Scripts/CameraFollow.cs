using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 5, -8);
    public float smoothSpeed = 8f;
    public float lookAheadFactor = 0.2f;

    public float shakeAmount = 0f;
    private Vector3 shakeOffset;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition + shakeOffset;

        // Look at target con offset hacia adelante
        Vector3 lookTarget = target.position + Vector3.forward * lookAheadFactor * (GameManager.Instance != null ? GameManager.Instance.speed : 12f);
        transform.LookAt(lookTarget);

        // Shake
        if (shakeAmount > 0)
        {
            shakeOffset = Random.insideUnitSphere * shakeAmount;
            shakeAmount -= Time.deltaTime * 2f;
        }
        else
        {
            shakeOffset = Vector3.zero;
        }

        // Game over: cámara lenta + shake fuerte
        if (GameManager.Instance != null && GameManager.Instance.currentState == GameManager.GameState.GameOver)
        {
            shakeOffset += Random.insideUnitSphere * 0.5f;
        }
    }

    public void Shake(float amount)
    {
        shakeAmount = amount;
    }
}