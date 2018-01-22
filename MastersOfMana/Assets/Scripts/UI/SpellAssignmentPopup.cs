using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellAssignmentPopup : PopupMenu {

    public GameObject normalPanel, ultiPanel;

    public void Open(bool isUlti)
    {
        normalPanel.SetActive(!isUlti);
        ultiPanel.SetActive(isUlti);
        base.Open();
    }

}
