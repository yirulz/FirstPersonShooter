using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLook : MonoBehaviour
{

    public bool showCursor = false;
    public Vector2 speed = new Vector2(120f, 120f);
    public float yMinLimit = -80f, yMaxLimit = 80f;

    //current x and y degrees of rotation
    private float x, y;

    // Use this for initialization
    void Start()
    {
        //Hide the cursor
        Cursor.visible = !showCursor;
        //Lock the cursor ternary operator
        Cursor.lockState = showCursor ? CursorLockMode.None : CursorLockMode.Locked;
        //Get current camera euler rotation
        Vector3 angles = transform.eulerAngles;
        //Set X and Y degrees to current camera rotation
        x = angles.y;
        y = angles.x;

    }

    // Update is called once per frame
    void Update()
    {
        //rotate camera based on mouse x and y
        x += Input.GetAxis("Mouse X") * speed.x * Time.deltaTime;
        y -= Input.GetAxis("Mouse Y") * speed.y * Time.deltaTime;

        //Clamp the angle of the pitch rotation
        y = Mathf.Clamp(y, yMinLimit, yMaxLimit);
        //Rotate parent on y axis (yaw)
        transform.parent.rotation = Quaternion.Euler(0, x, 0);
        //Rotate local on X axis (pitch)
        transform.localRotation = Quaternion.Euler(y, 0, 0);
    }
}
