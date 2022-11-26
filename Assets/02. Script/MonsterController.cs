using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
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
}