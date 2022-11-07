using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayGenerator : MonoBehaviour
{
    [SerializeField]
    private Transform RayGeneratePoint;    // Ray�� Generate�Ǵ� ��ġ
    [SerializeField]
    private TrailRenderer RayTrail;
    [SerializeField]
    private float GenerateDelay = 0.5f;
    [SerializeField]
    private float Speed = 10.0f;
    [SerializeField]
    private float ReflectingDistance = 100f;

    private float LastGenerateTime;
    private float LastReflectingDistance = 0;

    // Ray ���� �Լ�
    public void RayGenerate()
    {
        RaycastHit2D hit;

        if (LastGenerateTime + GenerateDelay < Time.time) {
            Vector3 direction = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z)) - RayGeneratePoint.transform.position;    // Ray �߻� ���� ����
            TrailRenderer trail = Instantiate(RayTrail, RayGeneratePoint.position, Quaternion.identity);    // Trail Renderer ����

            // �߻�� Ray�� �浹ü�� ����
            hit = Physics2D.Raycast(RayGeneratePoint.position, direction);

            if (hit) {
                StartCoroutine(GenerateTrail(trail, hit.point, hit.normal, ReflectingDistance, true));
            }

            /*
            if (Physics2D.Raycast(RayGeneratePoint.position, direction, out RaycastHit hit, float.MaxValue)) {
                StartCoroutine(GenerateTrail(trail, hit.point, hit.normal, ReflectingDistance, true));    // Trail Generate
            }
            */

            else {
                // �߻�� Ray�� �浹���� ���ϴ� ��Ȳ���� ���� �Ÿ� ���� �� �Ҹ�
                StartCoroutine(GenerateTrail(trail, direction * 10, Vector3.zero, ReflectingDistance, false));
            }

            LastGenerateTime = Time.time;
        }
    }

    // Trail ���� �Լ�
    private IEnumerator GenerateTrail(TrailRenderer Trail, Vector3 ReflectingPoint, Vector3 ReflectingNormal, float ReflectingDistance, bool MadeImpact)
    {
        float distance;    // Ray�� ���� �� ���� �Ÿ�
        float startingDistance;    // Ray�� ���� �� ��ü �Ÿ�

        Vector3 startPosition = Trail.transform.position;
        Vector3 direction = (ReflectingPoint - Trail.transform.position).normalized;    // Ray�� �ݻ� ���������� Direction Vector ����

        // ������ �ݻ� ���� (ReflectingDistance�� ��� ������ ���)
        if (ReflectingDistance <= 0) {
            // �ִ� LastReflectingDistance������ ���� ����
            distance = LastReflectingDistance;
            startingDistance = distance;

            ReflectingPoint = Vector3.Lerp(startPosition, ReflectingPoint, LastReflectingDistance / Vector3.Distance(startPosition, ReflectingPoint));    // LastReflectingDistance������ ���� �� ���� ReflectingPoint ����
        }

        // �������� ������ �ݻ� ����
        else {
            // Ray�� �ݻ� ���������� �Ÿ� ����
            distance = Vector3.Distance(Trail.transform.position, ReflectingPoint);
            startingDistance = distance;
        }

        // �ݻ� �������� �̵�
        while (distance > 0) {
            Trail.transform.position = Vector3.Lerp(startPosition, ReflectingPoint, 1 - (distance / startingDistance));
            distance -= Time.deltaTime * Speed;

            yield return null;
        }

        Trail.transform.position = ReflectingPoint;

        // �ݻ�
        if (MadeImpact) {
            // Ray�� ���� �ϸ鼭 �Ҹ��ϴ� ReflectingDistance�� ���� �ִ� ���
            if (ReflectingDistance > 0) {
                // �ݻ� ������ Direction Vector ����
                Vector3 ReflectingDirection = Vector3.Reflect(direction, ReflectingNormal);

                RaycastHit2D nextHit;
                nextHit = Physics2D.Raycast(ReflectingPoint, ReflectingDirection, ReflectingDistance);

                if (nextHit) {
                    yield return StartCoroutine(GenerateTrail(
                        Trail,
                        nextHit.point,
                        nextHit.normal,
                        ReflectingDistance - Vector3.Distance(nextHit.point, ReflectingPoint),
                        true
                    ));
                }

                else {
                    Debug.Log("LastHit");
                    //LastReflectingPoint�� �����ϱ� ���� Raycast
                    RaycastHit2D lastHit;
                    lastHit = Physics2D.Raycast(ReflectingPoint, ReflectingDirection, float.MaxValue);

                    if (lastHit) {
                        LastReflectingDistance = ReflectingDistance;

                        yield return StartCoroutine(GenerateTrail(
                            Trail,
                            lastHit.point,
                            Vector3.zero,
                            0,    // LastReflectingDistance�� ���� �ܿ� ReflectingDistance�� �����ϰ�, ������ �ݻ� �������� �˸��� ���� ���ڷ� 0�� ����
                            false
                        ));
                    }

                    else
                        Destroy(Trail.gameObject, Trail.time);
                }

                /*
                // ���� �ִ� ReflectingDistance�� ���� �ݻ� �������� ���� �� �� �ִ� ���
                if (Physics.Raycast(ReflectingPoint, ReflectingDirection, out RaycastHit hit, ReflectingDistance)) {
                    yield return StartCoroutine(GenerateTrail(
                        Trail,
                        hit.point,
                        hit.normal,
                        ReflectingDistance - Vector3.Distance(hit.point, ReflectingPoint),
                        true
                    ));
                }

                else {
                    // Last ReflectingPoint�� �����ϱ� ���� Raycast
                    if (Physics.Raycast(ReflectingPoint, ReflectingDirection, out RaycastHit lastHit, float.MaxValue)) {
                        LastReflectingDistance = ReflectingDistance;

                        yield return StartCoroutine(GenerateTrail(
                            Trail,
                            lastHit.point,
                            Vector3.zero,
                            0,    // LastReflectingDistance�� ���� �ܿ� ReflectingDistance�� �����ϰ�, ������ �ݻ� �������� �˸��� ���� ���ڷ� 0�� ����
                            false
                        ));
                    }

                    // �ݻ� ���� ������ ReflectingPoint�� ���� ��� [���� ó��]
                    else
                        Destroy(Trail.gameObject, Trail.time);
                }
                */
            }
        }

        Destroy(Trail.gameObject, Trail.time);
    }
}
