using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    [SerializeField]
    private float AISight = 2.5f;
    [SerializeField]
    private float traceSpeed = 0.05f;
    [SerializeField]
    private float avoid = 0.3f;
    [SerializeField]
    private float attackDelay = 2.0f;
    [SerializeField]
    private GameObject bullet;
    [SerializeField]
    private GameObject attackPos;

    public GameObject player;
    private Animator animator;

    private float toPlayerDistance;
    protected Vector3 toPlayerDirection;

    private bool notRedundantHit = true;
    private bool onDestroy = false;
    private bool onAttack = false;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (OnHit() && notRedundantHit)
        {
            RandomHit(avoid);
        }
        else if (!onDestroy)
        {
            toPlayerDistance = DistanceToPlayer(player);
            toPlayerDirection = DirectionToPlayer(player);
            reverseToPlayer(toPlayerDirection);

            if (toPlayerDistance <= AISight)
            {
                if (!onAttack)
                {
                    Attack();
                    Wait(attackDelay);
                }
            }

            else
            {
                Trace(toPlayerDirection);
            }
        }
    }

    // Decorator [피격 인식]
    private bool OnHit()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Monster_HIT") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f && notRedundantHit)
        {
            animator.SetBool("ON_HIT", false);
        }

        if (GameObject.Find("Trail(Clone)"))
        {
            float toTrailDistance = Vector3.Distance(GameObject.Find("Trail(Clone)").transform.position, gameObject.transform.position);
            if (toTrailDistance <= gameObject.transform.lossyScale.x + 1)
            {
                return true;
            }
        }

        notRedundantHit = true;

        return false;
    }

    // Service [Player와의 거리 연산]
    private float DistanceToPlayer(GameObject player)
    {
        return Vector3.Distance(player.transform.position, gameObject.transform.position);
    }

    // Service [Player와의 방향 벡터 연산]
    private Vector3 DirectionToPlayer(GameObject player)
    {
        return (player.transform.position - gameObject.transform.position).normalized;
    }

    // Service [Player 방향으로 바라보도록 좌우 변환]
    private void reverseToPlayer(Vector3 toPlayerDirection)
    {
        if (toPlayerDirection.x < 0)
        {
            attackPos.transform.localPosition = new Vector3(-2.5f, -2.0f, 0);
            spriteRenderer.flipX = false;
        }
        else
        {
            attackPos.transform.localPosition = new Vector3(2.5f, -2.0f, 0);
            spriteRenderer.flipX = true;
        }
    }

    // Task [Player를 추격]
    private void Trace(Vector3 traceDirection)
    {
        gameObject.transform.position += traceDirection * traceSpeed * Time.deltaTime;
    }

    // Task [피격 판정]
    private void RandomHit(float avoid)
    {
        animator.SetBool("ON_HIT", true);
        notRedundantHit = false;

        float randN = Random.Range(0.0f, 1.0f);

        if (!(randN < avoid))
        {
            onDestroy = true;
            spriteRenderer.color = new Color(255f, 0f, 0f, 0.5f);
            Destroy(gameObject, 0.5f);
        }
    }

    // Task [공격 간 딜레이]
    private void Wait(float attackDelay)
    {
        onAttack = true;
        Invoke("AttackDelay", attackDelay);
    }

    private void AttackDelay()
    {
        onAttack = false;
    }

    private void Attack()
    {
        Instantiate(bullet, attackPos.transform.position, attackPos.transform.rotation);
    }
}