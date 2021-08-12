using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class FindPlayerScript : MonoBehaviour
{
    Transform target;
    CinemachineVirtualCamera vCam;
    // Start is called before the first frame update
    void Start()
    {
        vCam = GetComponent<CinemachineVirtualCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!vCam.m_LookAt)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
            vCam.m_Follow = target;
            vCam.LookAt = target;

        }
    }
}
