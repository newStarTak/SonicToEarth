using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    RayGenerator rayGenerator;

    // ����ũ �Է� �ޱ� ���� ����� Ŭ��
    public AudioClip auc;

    // ���ø� (44100���� ���� ����)
    private int sampleRate = 44100;
    private float[] samples;
    
    // ���õ��� ��հ��� ������ ���� �� 0 �̻��� ������ ����� ���� �� ����
    private float rmsValue;
    private float modulate = 10000f;

    // ������� �ִ� ���Ѱ�, ���� ������ ���� �ν��� �����ϵ��� �ּ� ���Ѱ�, �����, ���� �߻縦 ���� �ּ� �Ҹ� ũ�Ⱚ
    public int maxValue;
    public int cutValue;
    public int resultValue;
    public int rayShootValue;

    void Start()
    {
        rayGenerator = GameObject.Find("RayGenerator").GetComponent<RayGenerator>();

        // samples �迭�� ���� ��(44100��) �����صΰ� ����ũ �۵� ����
        samples = new float[sampleRate];
        auc = Microphone.Start(Microphone.devices[0].ToString(), true, 1, sampleRate);
    }

    void Update()
    {
        auc.GetData(samples, 0);

        // ���ð��� ���밪���� ����� ����
        float sum = 0;
        for(int i = 0; i < samples.Length; i++)
        {
            sum += samples[i] * samples[i];
        }
        rmsValue = Mathf.Sqrt(sum / samples.Length);

        // ���ð��� �ʹ� �۰� ���� ���� ���� ���� �־� 0 �̻� �ִ� ���Ѱ� ������ ������ ��ȯ
        rmsValue *= modulate;
        rmsValue = Mathf.Clamp(rmsValue, 0, maxValue);
        resultValue = Mathf.RoundToInt(rmsValue);

        if(resultValue < cutValue)
        {
            resultValue = 0;
        }

        if (resultValue > rayShootValue)
        {
            rayGenerator.RayGenerate();
        }
    }
}
