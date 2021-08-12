using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnTriggerEvent : MonoBehaviour
{
    public UnityEvent OnTriggerEvents;
    public UnityEvent DelayedEvents;
    public float delayTimer = 1f;
    public bool DieAfterEvents = true;
    public bool deactivateCollider = true;
    private void OnTriggerEnter(Collider other)
    {
        OnTriggerEvents.Invoke();
        if(DieAfterEvents && DelayedEvents.GetPersistentEventCount() <= 0)Destroy(gameObject);
        StartCoroutine(DelayedEventsCoroutine());
        if(deactivateCollider)GetComponent<Collider>().enabled = false;
        
    }

    IEnumerator DelayedEventsCoroutine()
    {
        yield return new WaitForSeconds(delayTimer);
        DelayedEvents.Invoke();
        if (DieAfterEvents) Destroy(gameObject);
    }
}
