using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HeartTransformController : MonoBehaviour
{
    public float rotationSpeed = 0.1f;
    public GameObject GO;

    private Vector3 lastMousePosition;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Check if left mouse button is clicked
        {
            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0)) // Check if left mouse button is held down
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            GO.transform.Rotate(Vector3.up, -delta.x * rotationSpeed, Space.World);
            GO.transform.Rotate(Vector3.right, delta.y * rotationSpeed, Space.World);

            lastMousePosition = Input.mousePosition;
        }
    }
}
