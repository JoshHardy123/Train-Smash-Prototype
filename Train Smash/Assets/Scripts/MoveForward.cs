using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForward : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody rb;
    public float speed = 5;
    public bool stop { get; set; }
    bool stopping;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKey(KeyCode.Space) && !stop)
        //{
        if (!stop)
        {
            rb.AddForce(-Vector3.forward * speed * Time.deltaTime);
            rb.AddForce(-Vector3.up * speed/4 * Time.deltaTime);
        }
        else
        {
            rb.drag = 2;
        }
        //}
        //else if (stop && !stopping)
        //{
        //    //StartCoroutine(SlowDown());
        //    Slow();
        //}
    }

    public void Slow()
    {
        rb.velocity.Set(0,0, rb.velocity.z / 2);
    }

    IEnumerator SlowDown()
    {
        stopping = true;
        for (int i = 0; i < 100; i++)
        {
            rb.velocity.Set(0,0,rb.velocity.z * (1- i/100));
            yield return new WaitForSeconds(0.1f);
        }
        stopping = false;   
    }
}
