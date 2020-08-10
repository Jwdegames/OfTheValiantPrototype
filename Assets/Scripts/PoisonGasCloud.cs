using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonGasCloud : MonoBehaviour
{
    public Vector3 recordedPos, upPos, downPos;
    public float minFloat = 1f, maxFloat = 1f, animTime = 1f, elaspedTime = 0f;
    public bool ascending = false;
    public bool animating = false;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    public void recordPos(Vector3 vector)
    {
        recordedPos = vector;
        upPos = new Vector3(recordedPos.x, recordedPos.y + maxFloat, recordedPos.z);
        downPos = new Vector3(recordedPos.x, recordedPos.y - minFloat, recordedPos.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (animating)
        {
            if (!ascending)
            {
                transform.position = Vector3.Lerp(recordedPos, downPos, elaspedTime / animTime);
            }
            else
            {
                transform.position = Vector3.Lerp(recordedPos, upPos, elaspedTime / animTime);
            }
            elaspedTime += Time.deltaTime;
            if (elaspedTime >= animTime)
            {
                elaspedTime = 0;
                ascending = !ascending;
                recordedPos = transform.position;
            }
        }
    }
}
