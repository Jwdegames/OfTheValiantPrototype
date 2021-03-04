using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UnitInfoTooltip : MonoBehaviour
{
    public GameObject infoTooltipBox;
    public GameObject imageObject;
    public float imageFactor;
    public Text infoTooltipText;
    public Image infoTooltipArrow;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   public void applyImageScale()
    {
        imageObject.transform.localScale = new Vector3(imageObject.transform.localScale.x * imageFactor, imageObject.transform.localScale.y * imageFactor, 1f);
    }

    public void setSize(float x, float y)
    {
        Vector3 size = new Vector3(x, y, 1f);
        setSize(size);
    }

    public void setSize(Vector3 size)
    {
        transform.localScale = size;
        //applyImageScale();
    }

    public void showInfoTooltip()
    {
        infoTooltipBox.SetActive(true);
        infoTooltipArrow.enabled = true;
    }

    public void disableInfoTooltip()
    {
        infoTooltipBox.SetActive(false);
        infoTooltipArrow.enabled = false;
    }

    public void setInfoToolTipText(string text)
    {
        infoTooltipText.text = text;
    }
}
