using UnityEngine;
using System.Collections;

public class ChangeColor : MonoBehaviour {
    public Color defaultColor = Color.white;
    public Color highlightColor = Color.green;
    public Color selectColor = Color.blue;


	// Use this for initialization
	void Start () {
        StaticMenu menu = GetComponent<StaticMenu>();
        if (menu) {
            foreach (Transform item in menu.items) {
                item.GetComponent<Renderer>().material.color = defaultColor;
            }
        }
	}
	
    void Menu_Highlight(Transform item)
    {
        item.GetComponent<Renderer>().material.color = highlightColor;
    }

    void Menu_Unhighlight(Transform item)
    {
        item.GetComponent<Renderer>().material.color = defaultColor;
    }

    void Menu_Select(Transform item)
    {
        item.GetComponent<Renderer>().material.color = selectColor;
    }
}
