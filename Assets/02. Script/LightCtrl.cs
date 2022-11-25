using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightCtrl : MonoBehaviour
{
    // 빛이 커지거나 작아져야 할 때, 혹은 완전히 사라져야 할 때를 구분하는 bool 변수
    public bool isUp = false;
    public bool isDown = false;
    public bool isDestroy = false;
    
    // Light의 Radius는 외부 값이 내부 값의 딱 2배가 되는 것이 이상적, 따라서 하나의 변수로 둘을 효과적으로 관리
    public float maxRadius;

    Light2D light;

    // Start is called before the first frame update
    void Start()
    {
        light = GetComponent<Light2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // 빛이 커져야 할 때 최대값까지만 일정한 비율을 통해 커지도록 함
        if(isUp && light.pointLightOuterRadius <= maxRadius)
        {
            light.pointLightInnerRadius += ((maxRadius / 2) - light.pointLightInnerRadius) / 100f;
            light.pointLightOuterRadius += (maxRadius - light.pointLightOuterRadius) / 100f;
        }

        // 빛이 작아져야 할 때 최소값까지만 일정한 비율을 통해 작아지도록 함
        else if (isDown && light.pointLightOuterRadius >= maxRadius / 2)
        {
            light.pointLightInnerRadius -= (light.pointLightInnerRadius - (maxRadius / 4)) / 100f;
            light.pointLightOuterRadius -= (light.pointLightOuterRadius - (maxRadius / 2)) / 100f;
        }

        // 빛이 사라져야 할 때 일정한 비율을 통해 작아지도록 함
        else if (isDestroy)
        {
            light.pointLightInnerRadius -= light.pointLightInnerRadius / 100f;
            light.pointLightOuterRadius -= light.pointLightOuterRadius / 100f;
        }
    }
}
