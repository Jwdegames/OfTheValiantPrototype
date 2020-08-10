using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelector : MonoBehaviour
{
    [SerializeField]
    private Color color;
    private SpriteRenderer renderer;
    //Set color to color variable
    // Use this for initialization
    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        updateColor();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setColor(Color c)
    {
        color = c;
        updateColor();
    }

    public Color getColor()
    {
        return color;
    }

    public void updateColor()
    {
        if (renderer == null)
        {
            return;
        }
        renderer.color = color;
    }
}
