using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;          // O Player

    [Header("Offsets")]
    public Vector3 offset = new Vector3(0, 0, -10);

    [Header("Smoothness")]
    public float smoothSpeedX = 8f;   // Quanto a câmera acompanha no X
    public float smoothSpeedY = 2f;   // Quanto a câmera acompanha no Y

    [Header("Pixel Perfect (opcional)")]
    public bool pixelSnap = false;    // Ativar arredondamento de pixels
    public float snapValue = 0.01f;   // Valor de arredondamento

    void LateUpdate()
    {
        if (target == null) return;

        // Posição desejada da câmera (segue o player + offset)
        Vector3 desiredPosition = target.position + offset;

        // Interpolação separada em X e Y
        float newX = Mathf.Lerp(transform.position.x, desiredPosition.x, smoothSpeedX * Time.deltaTime);
        float newY = Mathf.Lerp(transform.position.y, desiredPosition.y, smoothSpeedY * Time.deltaTime);

        Vector3 smoothedPosition = new Vector3(newX, newY, desiredPosition.z);

        // Se pixelSnap estiver ativo, arredonda para evitar jitter
        if (pixelSnap)
        {
            smoothedPosition.x = Mathf.Round(smoothedPosition.x / snapValue) * snapValue;
            smoothedPosition.y = Mathf.Round(smoothedPosition.y / snapValue) * snapValue;
        }

        transform.position = smoothedPosition;
    }
}