using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Selectable))]
public class SelectableSFX : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public SelectableSFXSet sfxs;

    public void OnPointerClick(PointerEventData eventData)
    {
        Play(sfxs.clickSFX);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Play(sfxs.enterSFX);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Play(sfxs.exitSFX);
    }

    private void Play(AudioClip clip)
    {
        if(clip == null)
        {
            return;
        }
        GameManager.instance.audioSource2D.PlayOneShot(clip);
    }
}