using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraCtrl : MonoBehaviour
{
    CinemachineVirtualCamera cam;
    private bool canZoomOut = true;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<CinemachineVirtualCamera>();
    }

    public void ZoomIn(GameObject target)
    {
        cam.Follow = target.transform;
        cam.m_Lens.OrthographicSize = 3;
    }

    public void ZoooomIn(GameObject target)
    {
        canZoomOut = false;
        cam.Follow = target.transform;
        cam.m_Lens.OrthographicSize = 5;
        cam.GetCinemachineComponent<CinemachineTransposer>().m_XDamping = 5f;

        Invoke("ZoooomOut", 5.0f);
    }

    public void ZoomOut()
    {
        if(canZoomOut)
        {
            cam.Follow = GameObject.FindGameObjectWithTag("Player").transform;
            cam.m_Lens.OrthographicSize = 5;
        }
    }

    public void ZoooomOut()
    {
        cam.Follow = GameObject.FindGameObjectWithTag("Player").transform;
        cam.m_Lens.OrthographicSize = 5;
        cam.GetCinemachineComponent<CinemachineTransposer>().m_XDamping = 2f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
