using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCamera : MonoBehaviour
{

    private float x, y, xDelta, yDelta;
    public float distance = 6f;
    public Transform lookAt;
    private Vector3 smoothLookAt;
    public float smoothSpeed = 0.4f;
    public float lookSpeed = 5f;

    public float rotationSpeed = 1.0f;

    private Vector3 velocityCamSmooth;
    // Start is called before the first frame update
    void Start()
    {
        smoothLookAt = lookAt.position;
    }

    // Update is called once per frame
    void Update()
    {
        xDelta = Input.GetAxis("Mouse X") + Input.GetAxis("RightStickX");
        yDelta = Input.GetAxis("Mouse Y") + Input.GetAxis("RightStickY");

        x += xDelta * rotationSpeed * distance * Time.deltaTime;
        y -= yDelta * rotationSpeed * distance * Time.deltaTime;

        y = Mathf.Clamp(y, -10.0f, 60.0f);
    }

    void LateUpdate()
    {

        Quaternion rotation = Quaternion.Euler(y, x, 0);

        Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);

        //rotate negdistance by rot around 000 and then translate in look target direction
        Vector3 position = rotation * negDistance + lookAt.position; 

        Vector3 targetPosition = position;

        Vector3 newPosition = Vector3.SmoothDamp(this.transform.position, targetPosition, ref velocityCamSmooth, smoothSpeed);

        this.transform.position = newPosition;

        //smoothLookAt = Vector3.Lerp(smoothLookAt, lookAt.position, lookSpeed * Time.deltaTime);
        transform.LookAt(lookAt.position);
    }
}
