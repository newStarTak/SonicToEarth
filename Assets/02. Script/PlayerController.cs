using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    RayGenerator rayGenerator;

    void Start()
    {
        rayGenerator = GameObject.Find("RayGenerator").GetComponent<RayGenerator>();
    }

    void Update()
    {
        if (Input.GetMouseButton(0)) {
            rayGenerator.RayGenerate();
        }
    }
}
