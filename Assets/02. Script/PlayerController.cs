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

    public bool canShoot;

    private float h;
    private Rigidbody2D rb;

    public float maxSpeed;
    public float jumpForce;
    private bool isSpeedDown;

    void Start()
    {
        rayGenerator = GameObject.Find("RayGenerator").GetComponent<RayGenerator>();

        // samples �迭�� ���� ��(44100��) �����صΰ� ����ũ �۵� ����
        samples = new float[sampleRate];
        auc = Microphone.Start(Microphone.devices[0].ToString(), true, 1, sampleRate);

        canShoot = true;

        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        h = Input.GetAxisRaw("Horizontal");
        rb.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        if(Mathf.Abs(rb.velocity.x) > maxSpeed)
        {
            rb.velocity = new Vector2(maxSpeed * h, rb.velocity.y);
        }
        else if(isSpeedDown)
        {
            rb.velocity *= new Vector2(0.96f, 1f);
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        if(Input.GetKeyDown(KeyCode.A)) {
            rayGenerator.RayGenerate();
        }

        // Translate�� �̿��� �̵��ϸ� ���� �ε��� ��� ���� ���� �߻�, rigidBody ����� ���� �ذ�
        //transform.Translate(new Vector2(walkSpeed * h * Time.deltaTime, 0));

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

        if ((resultValue > rayShootValue || Input.GetMouseButton(0)) && canShoot)
        {
            canShoot = false;
            rayGenerator.RayGenerate();
        }
    }
}
