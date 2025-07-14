using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5f;
    public Vector3 offset;

    public float minX = -20.4f;     
    public float maxX = 20.4f;

    private float halfCamWidth;

    void Start()
    {
        // Get half camera width in world units
        float camHeight = Camera.main.orthographicSize * 2;
        float camWidth = camHeight * Camera.main.aspect;
        halfCamWidth = camWidth / 2f;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPos = new Vector3(target.position.x, transform.position.y, transform.position.z) + offset;

        // Clamp the desired X position
        float clampedX = Mathf.Clamp(desiredPos.x, minX + halfCamWidth, maxX - halfCamWidth);
        Vector3 clampedPosition = new Vector3(clampedX, desiredPos.y, desiredPos.z);

        // Smooth follow
        transform.position = Vector3.Lerp(transform.position, clampedPosition, smoothSpeed * Time.deltaTime);
    }
}
