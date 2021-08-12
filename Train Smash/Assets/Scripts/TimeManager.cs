using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager timeMan;

    private void Awake()
    {
        if (timeMan == null) timeMan = this;

        else Destroy(this);
    }

    private void Update()
    {
        Time.timeScale = Mathf.Clamp(Time.timeScale,0.1f,1);
    }
    // Start is called before the first frame update
    public void CallSlowTime(float newTimeScale, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(SlowTime(newTimeScale, duration));
    }
    public IEnumerator SlowTime(float timeScale, float duration)
    {
        float target = timeScale;
        for (int i = 0; i < 20; i++)
        { 
            Time.timeScale -= 0.8f / 20f;
            yield return new WaitForSecondsRealtime((duration/2)/20);
        }
        for (int i = 0; i < 20; i++)
        {
            Time.timeScale += 0.8f / 20f;
            yield return new WaitForSecondsRealtime((duration / 2) / 20);
        }

        Time.timeScale = 1;
    }
}
