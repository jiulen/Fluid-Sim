using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EKGRenderer : MonoBehaviour
{
    public Transform[] waypoints; // Array to hold your list of waypoints
    public float speed = 5.0f; // Speed at which the GameObject moves
    public int currentWaypoint = 0; // Index of the current waypoint
    public GameObject LineRenderer;
    public GameObject LineRendererPrefab;
    public Transform LineRendererParent;

    public bool isBeating = false; // Flag to indicate if the heartbeat function is active
    private float beatDuration = 0.08f;
    private float beatTimer = 0.0f; // Timer for the beat function

    public bool fibrillation = false;
    float fibrillationSeed = 1.0f;
    public float timer = 1.5f;
    float maxTimer;
    public void Beat()
    {
        isBeating = true;
        beatTimer = 0;


    }
    private void Start()
    {
        GameObject GO = Instantiate(LineRendererPrefab, waypoints[0].position, Quaternion.identity);
        LineRenderer = GO;
        LineRenderer.transform.SetParent(LineRendererParent, true);
        maxTimer = timer;
        //LineRenderer.transform.position = Vector3.zero;

    }
    void Update()
    {
        timer -= Time.deltaTime;
        if(timer < 0)
        {
            MoveTowardsWaypoints();
        }
        else if (fibrillation)
        {
            fibrillationSeed = Random.Range(-1f, 1f); // Adjust the range for less erratic movement
            float targetY = fibrillationSeed; // Reduce the magnitude for a slower movement

            float newY = Mathf.Lerp(LineRenderer.transform.position.y - 5.03f, targetY, Time.deltaTime * 3.0f); // Adjust the speed of interpolation

            LineRenderer.transform.position = new Vector3(LineRenderer.transform.position.x + speed * Time.deltaTime, newY + 5.03f, LineRenderer.transform.position.z);
        }
        else if (isBeating)
        {
            // Implement heartbeat function
            beatTimer += Time.deltaTime;

            float newY = Mathf.Sin(beatTimer / beatDuration * Mathf.PI * 2) / 2.5f; // Calculate the new Y position for the heartbeat
           // Debug.Log(newY);
            LineRenderer.transform.position = new Vector3(LineRenderer.transform.position.x + speed * Time.deltaTime, newY + 5.03f, LineRenderer.transform.position.z);

            if (beatTimer >= beatDuration)
            {
                isBeating = false;
                beatTimer = 0.0f;
                LineRenderer.transform.position = new Vector3(LineRenderer.transform.position.x, 5.03f, LineRenderer.transform.position.z); // Return to the middle
            }
        }
        else
        {
            MoveTowardsWaypoints();
        }
    }
    void MoveTowardsWaypoints()
    {// Check if there are waypoints
        if (waypoints.Length == 0)
            return;

        // Move towards the current waypoint
        LineRenderer.transform.position = Vector3.MoveTowards(LineRenderer.transform.position, waypoints[1].position, speed * Time.deltaTime);

        // If the GameObject reaches the current waypoint, move to the next one
        if (LineRenderer.transform.position == waypoints[1].position)
        {
            currentWaypoint++;
            Debug.Log("TP");
            // Loop back to the first waypoint if we reach the end
            if (currentWaypoint >= waypoints.Length)
            {
                currentWaypoint = 0;
                // LineRenderer.transform.position = waypoints[currentWaypoint].position;

                Destroy(LineRenderer);
                GameObject GO = Instantiate(LineRendererPrefab, waypoints[0].position, Quaternion.identity);
                LineRenderer = GO;
                LineRenderer.transform.SetParent(LineRendererParent, true);
                timer = maxTimer;
            }
        }
    }
}
