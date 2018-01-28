using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Button Color Set", menuName = "UI FX/Button Color Set", order = 0)]
public class ButtonColorSet : ScriptableObject 
{
    public ColorBlock colors;
}