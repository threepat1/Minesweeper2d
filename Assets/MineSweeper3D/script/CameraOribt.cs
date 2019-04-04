using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOribt : MonoBehaviour
{
    public float zoomSpeed = 10f;
    public float xSpeed = 120f;
    public float ySpeed = 120f;
    public float yMin = -80f;
    public float yMax = 80f;
    public float distanceMin = 10f;
    public float distanceMax = 30f;
    private float x = 0f;
    private float y = 0f;
    private float distance;

    // Use this for initialization
    void Start()
    {
        distance = distanceMax;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Input.GetMouseButton(1))
        {
            float mousex = Input.GetAxis("Mouse X");
            float mousey = Input.GetAxis("Mouse Y");
            x += mousex * xSpeed * Time.deltaTime;
            y -= mousey * ySpeed * Time.deltaTime;
            y = Mathf.Clamp(y, yMin, yMax);
        }
        float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
        distance -= scrollWheel * zoomSpeed;
        distance = Mathf.Clamp(distance, distanceMin, distanceMax);

        transform.rotation = Quaternion.Euler(y, x, 0);
        transform.position = -transform.forward * distance;


    }
}
