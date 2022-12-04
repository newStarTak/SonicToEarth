using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    RayGenerator rayGenerator;
    [Header("Mic Input")]
    // 결과값의 최대 제한값, 작은 소음에 의한 인식은 무시하도록 최소 제한값, 결과값, 음파 발사를 위한 최소 소리 크기값
    public int maxValue;
    public int cutValue;
    [SerializeField]
    private int rayShootValue = 100;
    public int rayAttackValue;
    public int resultValue;
    public bool canShoot = false;

    // 마이크 입력 받기 위한 오디오 클립
    private AudioClip auc;

    // 샘플링 (44100개의 음원 샘플)
    private int sampleRate = 44100;
    private float[] samples;

    // 샘플들의 평균값을 저장할 변수 및 0 이상의 값으로 만들기 위한 곱 변수
    private float rmsValue;
    private float modulate = 10000f;

    [Header ("Character 2D rigidBody movement")]
    public float maxSpeed;
    public float jumpForce;
    private bool isSpeedDown;
    public bool canJump = true;

    private float h;
    private Rigidbody2D rb;

    void Start()
    {
        rayGenerator = GameObject.Find("RayGenerator").GetComponent<RayGenerator>();

        // samples 배열에 샘플 값(44100개) 저장해두고 마이크 작동 시작
        samples = new float[sampleRate];
        auc = Microphone.Start(Microphone.devices[0].ToString(), true, 1, sampleRate);

        rb = GetComponent<Rigidbody2D>();

        Invoke("MicEnable", 1.0f);
    }

    void MicEnable()
    {
        canShoot = true;
    }

    void Update()
    {
        // 캐릭터 발 아래로 작은 레이 생성해 점프 상태 체크
        RaycastHit2D rayHit = Physics2D.Raycast(rb.position, Vector2.down, 1.1f);
        Debug.DrawRay(rb.position, Vector2.down * 1.1f, Color.green);

        if(rayHit.collider.tag == "PLATFORM")
        {
            canJump = true;
            Debug.Log("ray hit platform");
        }

        h = Input.GetAxisRaw("Horizontal");
        rb.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        if(Mathf.Abs(rb.velocity.x) > maxSpeed)
        {
            rb.velocity = new Vector2(maxSpeed * h, rb.velocity.y);
        }
        else if(isSpeedDown)
        {
            rb.velocity *= new Vector2(0.9f, 1f);
            if(Mathf.Abs(rb.velocity.x) < 1f)
            {
                rb.velocity = new Vector2(0f, rb.velocity.y);
            }
        }

        if(Input.GetButtonDown("Horizontal"))
        {
            isSpeedDown = false;
        }
        else if(Input.GetButtonUp("Horizontal") && !Input.GetButton("Horizontal"))
        {
            isSpeedDown = true;
        }

        if (Input.GetButtonDown("Jump") && canJump)
        {
            canJump = false;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        if(Input.GetKeyDown(KeyCode.A)) {
            rayGenerator.RayGenerate();
        }

        // Translate를 이용해 이동하면 벽에 부딪힐 경우 떨림 현상 발생, rigidBody 사용해 문제 해결
        //transform.Translate(new Vector2(walkSpeed * h * Time.deltaTime, 0));

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

        if (resultValue > rayShootValue && canShoot)
        {
            canShoot = false;
            rayGenerator.RayGenerate();
        }

        // 디버깅용 마우스 클릭으로 음파 발사
        if (Input.GetMouseButtonDown(0))
        {
            canShoot = false;
            rayGenerator.RayGenerate();
        }
    }
}
