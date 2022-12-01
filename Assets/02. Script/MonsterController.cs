using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    [SerializeField]
    private float AISight = 2.5f;
    [SerializeField]
    private float traceSpeed = 0.05f;

    public GameObject player;

    private bool sequence = true;
    private bool taskAction = false;

    private void FixedUpdate()
    {
        taskAction = onSight(player);

        if (!taskAction) {
            Vector3 traceDirection = (player.transform.position - gameObject.transform.position).normalized;
            gameObject.transform.position += traceDirection * traceSpeed;
        }

        else
            Debug.Log("Attcak!");
    }

    bool onSight(GameObject player)
    {
        float toPlayerDistance = Vector3.Distance(player.transform.position, gameObject.transform.position);

        if (toPlayerDistance <= AISight)
            return true;

        else
            return false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
            Debug.Log("Hit");
    }

    /*
    [SerializeField]
    private float traceSpeed = 0.05f;
    [SerializeField]
    private float delayTime = 3.0f;

    public GameObject player;

    private Vector3 traceDirection;
    private float toPlayerDistance;
    private bool onAttack;
    
    void FixedUpdate()
    {
        toPlayerDistance = Vector3.Distance(player.transform.position, gameObject.transform.position);

        if(toPlayerDistance > 2.5f) {
            traceDirection = (player.transform.position - gameObject.transform.position).normalized;
            gameObject.transform.position += traceDirection * traceSpeed;
        }

        else {
            if(!onAttack) {
                Debug.Log("Attack");
                onAttack = true;
                StartCoroutine(ActionDelay(delayTime));
            }
        }
    }

    private IEnumerator ActionDelay(float delayTime)
    {
        while(true) {
            yield return new WaitForSeconds(delayTime);
            onAttack = false;
        }
    }
    */
}