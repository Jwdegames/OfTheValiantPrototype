using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class UIObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public UIManager ui;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*private void OnMouseOver()
    {
        ui.selectingAction = true;
    }

    private void OnMouseExit()
    {
        ui.selectingAction = false;
    }*/

    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.selectingAction = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.selectingAction = false;
    }
}
