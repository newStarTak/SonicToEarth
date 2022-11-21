using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerKGT : MonoBehaviour
{
    RayGeneratorKGT rayGenerator;

    void Start()
    {
        rayGenerator = GameObject.Find("RayGenerator").GetComponent<RayGeneratorKGT>();
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            rayGenerator.RayGenerate();
        }
    }
}
