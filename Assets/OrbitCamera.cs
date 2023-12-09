using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    public Transform target; // Target object to orbit around
    public float rotationSpeed = 5.0f;

    private Vector3 lastMousePosition;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("Target not assigned to CameraOrbit script! Assign a target object.");
            enabled = false;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            transform.RotateAround(target.position, Vector3.up, delta.x * rotationSpeed * Time.deltaTime);
            lastMousePosition = Input.mousePosition;
        }
    }
}
