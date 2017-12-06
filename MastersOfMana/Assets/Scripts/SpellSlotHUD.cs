using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellSlotHUD : MonoBehaviour {

    public Image cooldownImage;
    public Image highlight;
    public Image icon;

    public void setActive(bool active)
    {
        gameObject.SetActive(active);
    }
}
