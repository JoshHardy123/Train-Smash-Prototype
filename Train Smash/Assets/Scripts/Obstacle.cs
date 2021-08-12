using PathCreation.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Obstacle : MonoBehaviour
{
    public UnityEvent OnTriggerEvents;
    public bool DieOnTriggerEnter = true;
    public float minSpeed = 2;
    public float speedReduction = 2;
    PathFollower player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PathFollower>();
    }

    public void ReducePlayerspeed(float amount)
    {
        player.currentSpeed -= amount;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(player.currentSpeed >= minSpeed)
        {
            ReducePlayerspeed(speedReduction);
            OnTriggerEvents.Invoke();
            if (DieOnTriggerEnter) Destroy(gameObject);
        }
        else
        {
            ReducePlayerspeed(minSpeed*1.2f);
        }
      //  TimeManager.timeMan.CallSlowTime(0.2f,0.5f);
    }

}
