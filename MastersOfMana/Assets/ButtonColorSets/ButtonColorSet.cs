using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Button Color Set", menuName = "Button Color Set", order = 2)]
public class ButtonColorSet : ScriptableObject 
{
    public ColorBlock colors;
}