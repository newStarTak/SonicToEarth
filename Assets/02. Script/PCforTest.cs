using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCforTest : MonoBehaviour
{
    [SerializeField]
    private float speed = 0.1f;

    // Update is called once per frame
    void Update()
    {
        float hAxis = Input.GetAxisRaw("Horizontal");
        float vAxis = Input.GetAxisRaw("Vertical");

        transform.position += new Vector3(hAxis, vAxis, 0) * speed * Time.deltaTime;
    }
}
