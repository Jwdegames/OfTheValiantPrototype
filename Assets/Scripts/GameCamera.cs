using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    public bool changeable = false;
    public string changeType = "Level";
    //
    public float moveDistance = 0.1f;
    public float leftBound = 0;
    public float rightBound = 0;
    public float upperBound = 0;
    public float lowerBound = 0;
    public float minOrtho = 10f;
    public float maxOrtho = 40f;

    private Camera camera;
    // Use this for initialization
    void Start()
    {
        camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (changeable)
        {
            if (changeType == "Level")
            {
                if (Input.GetKey(KeyCode.W) == true && gameObject.transform.position.y + moveDistance < upperBound)
                {
                    transform.Translate(0, moveDistance, 0);
                }
                if (Input.GetKey(KeyCode.A) == true && gameObject.transform.position.x - moveDistance > leftBound)
                {
                    transform.Translate(-moveDistance, 0, 0);
                }
                if (Input.GetKey(KeyCode.S) == true && gameObject.transform.position.y - moveDistance > lowerBound)
                {
                    transform.Translate(0, -moveDistance, 0);
                }
                if (Input.GetKey(KeyCode.D) == true && gameObject.transform.position.x + moveDistance < rightBound)
                {
                    transform.Translate(moveDistance, 0, 0);
                }
            }
        }
        float d = Input.GetAxis("Mouse ScrollWheel")* 10;
        if (camera.orthographicSize - d <= maxOrtho && camera.orthographicSize - d >= minOrtho)
        {
            if (d < 0f)
            {
                camera.orthographicSize -= d;
            }
            else if (d > 0f)
            {
                camera.orthographicSize -= d;
            }
        }
    }

    public void setBounds(float left, float right, float up, float down)
    {
        leftBound = left;
        rightBound = right;
        upperBound = up;
        lowerBound = down;
    }

    public void setScroll(float scroll)
    {
        camera.orthographicSize = scroll;
    }

    public void moveCamera(int x, int y)
    {
        transform.position = new Vector3(x, y, transform.position.z);
    }

    public void centerMap()
    {
        transform.position = new Vector3((leftBound + rightBound) / 2, (upperBound + lowerBound) / 2, transform.position.z);
    }
}
