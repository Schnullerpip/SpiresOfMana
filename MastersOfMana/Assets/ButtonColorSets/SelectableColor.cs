using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectableColor : MonoBehaviour 
{
    public ButtonColorSet colorSet;

    private void OnValidate()
    {
        GetComponent<Selectable>().colors = colorSet.colors;
    }
}
