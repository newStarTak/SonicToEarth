using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    RayGenerator rayGenerator;

    // 마이크 입력 받기 위한 오디오 클립
    public AudioClip auc;

    // 샘플링 (44100개의 음원 샘플)
    private int sampleRate = 44100;
    private float[] samples;
    
    // 샘플들의 평균값을 저장할 변수 및 0 이상의 값으로 만들기 위한 곱 변수
    private float rmsValue;
    private float modulate = 10000f;

    // 결과값의 최대 제한값, 작은 소음에 의한 인식은 무시하도록 최소 제한값, 결과값, 음파 발사를 위한 최소 소리 크기값
    public int maxValue;
    public int cutValue;
    public int resultValue;
    public int rayShootValue;

    void Start()
    {
        rayGenerator = GameObject.Find("RayGenerator").GetComponent<RayGenerator>();

        // samples 배열에 샘플 값(44100개) 저장해두고 마이크 작동 시작
        samples = new float[sampleRate];
        auc = Microphone.Start(Microphone.devices[0].ToString(), true, 1, sampleRate);
    }

    void Update()
    {
        auc.GetData(samples, 0);

        // 샘플값을 절대값으로 만드는 과정
        float sum = 0;
        for(int i = 0; i < samples.Length; i++)
        {
            sum += samples[i] * samples[i];
        }
        rmsValue = Mathf.Sqrt(sum / samples.Length);

        // 샘플값이 너무 작게 나와 일정 수를 곱해 주어 0 이상 최대 제한값 사이의 정수로 변환
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
