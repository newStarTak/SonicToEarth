using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightCtrl : MonoBehaviour
{
    // �� ���� �ӵ� ��� ����
    public float modulate;

    // ���� Ŀ���ų� �۾����� �� ��, ������ ������� �� ��, Ȥ�� Ư�� ������Ʈ ���� �������� �����ϴ� bool ����
    public bool isUp = false;
    public bool isDown = false;
    public bool isDestroy = false;
    public bool isTrail = false;
    public bool isLantern = false;
    
    // Light�� Radius�� �ܺ� ���� ���� ���� �� 2�谡 �Ǵ� ���� �̻���, ���� �ϳ��� ������ ���� ȿ�������� ����
    public float maxRadius;

    Light2D light;

    // Start is called before the first frame update
    void Start()
    {
        light = GetComponent<Light2D>();
        
        if(!(isTrail || isLantern))
        {
            Invoke("ItsTimeToOff", 2.0f);
        }
    }

    void ItsTimeToOff()
    {
        isUp = false;
        isDown = false;
        isDestroy = true;
    }

    // Update is called once per frame
    void Update()
    {
        // ���� Ŀ���� �� �� �ִ밪������ ������ ������ ���� Ŀ������ ��
        if(isUp && light.pointLightOuterRadius <= maxRadius)
        {
            light.pointLightInnerRadius += (((maxRadius / 2) - light.pointLightInnerRadius) / 100f) * modulate;
            light.pointLightOuterRadius += ((maxRadius - light.pointLightOuterRadius) / 100f) * modulate;
        }

        // ���� �۾����� �� �� �ּҰ������� ������ ������ ���� �۾������� ��
        else if (isDown && light.pointLightOuterRadius >= maxRadius / 2)
        {
            light.pointLightInnerRadius -= ((light.pointLightInnerRadius - (maxRadius / 4)) / 100f) * modulate;
            light.pointLightOuterRadius -= ((light.pointLightOuterRadius - (maxRadius / 2)) / 100f) * modulate;
        }

        // ���� ������� �� �� ������ ������ ���� �۾������� ��
        else if (isDestroy)
        {
            light.pointLightInnerRadius -= (light.pointLightInnerRadius / 100f) * modulate;
            light.pointLightOuterRadius -= (light.pointLightOuterRadius / 100f) * modulate;
        }
    }

    void OnDestroy()
    {
        if(isTrail)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().canShoot = true;
            // GameObject.FindGameObjectWithTag("FOLLOWCAM").GetComponent<CameraCtrl>().ZoomOut();
        }
    }
}
