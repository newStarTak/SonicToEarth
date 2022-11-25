using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightCtrl : MonoBehaviour
{
    // ���� Ŀ���ų� �۾����� �� ��, Ȥ�� ������ ������� �� ���� �����ϴ� bool ����
    public bool isUp = false;
    public bool isDown = false;
    public bool isDestroy = false;
    
    // Light�� Radius�� �ܺ� ���� ���� ���� �� 2�谡 �Ǵ� ���� �̻���, ���� �ϳ��� ������ ���� ȿ�������� ����
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
        // ���� Ŀ���� �� �� �ִ밪������ ������ ������ ���� Ŀ������ ��
        if(isUp && light.pointLightOuterRadius <= maxRadius)
        {
            light.pointLightInnerRadius += ((maxRadius / 2) - light.pointLightInnerRadius) / 100f;
            light.pointLightOuterRadius += (maxRadius - light.pointLightOuterRadius) / 100f;
        }

        // ���� �۾����� �� �� �ּҰ������� ������ ������ ���� �۾������� ��
        else if (isDown && light.pointLightOuterRadius >= maxRadius / 2)
        {
            light.pointLightInnerRadius -= (light.pointLightInnerRadius - (maxRadius / 4)) / 100f;
            light.pointLightOuterRadius -= (light.pointLightOuterRadius - (maxRadius / 2)) / 100f;
        }

        // ���� ������� �� �� ������ ������ ���� �۾������� ��
        else if (isDestroy)
        {
            light.pointLightInnerRadius -= light.pointLightInnerRadius / 100f;
            light.pointLightOuterRadius -= light.pointLightOuterRadius / 100f;
        }
    }
}
