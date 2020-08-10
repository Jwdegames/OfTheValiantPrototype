using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttributeIcon : MonoBehaviour
{
    public UIManager ui;
    //Image renderer
    public Image img;
    public Image toolTipArrow;
    public GameObject toolTip;
    public Text toolTipTXT;
    public Text toolTipTitle;
    public Text toolTipDuration;
    public Text txt;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateSprite(Sprite sprite)
    {
        img.sprite = sprite;
    }

    public void updateText(string text)
    {
        txt.text = text;
    }

    public void updateToolTipTXT(string txt)
    {
        toolTipTXT.text = txt;
    }

    public void updateToolTipTitle(string txt)
    {
        toolTipTitle.text = txt;
    }

    public void updateToolTipDuration (string txt)
    {
        toolTipDuration.text = txt;
    }
    //Make the tooltip when we mouse over the icon
    private void OnMouseOver()
    {
        toolTip.SetActive(true);
        toolTipArrow.enabled = true;

    }

    public void offsetToolTip(Vector3 offset)
    {
        toolTip.transform.position += offset;
    }

    private void OnMouseExit()
    {
        toolTip.SetActive(false);
        toolTipArrow.enabled = false;
    }


}
