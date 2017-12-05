using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnTriggerEvent : MonoBehaviour 
{
    public delegate void TriggerEvent(Collider col);
    public TriggerEvent onTriggerEnter;
    public TriggerEvent onTriggerStay;
    public TriggerEvent onTriggerExit;

    private void OnTriggerEnter(Collider other)
    {
        if(onTriggerEnter != null)
        {
            onTriggerEnter.Invoke(other);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (onTriggerStay != null)
        {
            onTriggerStay.Invoke(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(onTriggerExit != null)
        {
            onTriggerExit.Invoke(other);
        }
    }
}
