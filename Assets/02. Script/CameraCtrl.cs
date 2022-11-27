using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraCtrl : MonoBehaviour
{
    CinemachineVirtualCamera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<CinemachineVirtualCamera>();
    }

    public void ZoomIn(GameObject target)
    {
        cam.Follow = target.transform;
        cam.m_Lens.OrthographicSize = 5;
    }

    public void ZoomOut()
    {
        cam.Follow = GameObject.FindGameObjectWithTag("Player").transform;
        cam.m_Lens.OrthographicSize = 10;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
