using UnityEngine;

[CreateAssetMenu(fileName = "New Selectable SFX Set", menuName = "UI FX/Selectable SFX Set", order = 1)]
public class SelectableSFXSet : ScriptableObject
{
    public AudioClip enterSFX, exitSFX, clickSFX;
}