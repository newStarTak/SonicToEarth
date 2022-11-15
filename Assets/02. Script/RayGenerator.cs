using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayGenerator : MonoBehaviour
{
    [SerializeField]
    private Transform RayGeneratePoint;    // Ray가 Generate되는 위치
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

    // Ray 생성 함수
    public void RayGenerate()
    {
        RaycastHit2D hit;

        if (LastGenerateTime + GenerateDelay < Time.time) {
            Vector3 direction = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z)) - RayGeneratePoint.transform.position;    // Ray 발사 방향 결정
            TrailRenderer trail = Instantiate(RayTrail, RayGeneratePoint.position, Quaternion.identity);    // Trail Renderer 생성

            // 발사된 Ray가 충돌체를 감지
            hit = Physics2D.Raycast(RayGeneratePoint.position, direction, float.MaxValue);

            if (hit.collider != null) {
                StartCoroutine(GenerateTrail(trail, hit.point, hit.normal, ReflectingDistance, true, hit.collider.name));
            }

            else {
                // 발사된 Ray가 충돌하지 못하는 상황에선 일정 거리 진행 후 소멸
                StartCoroutine(GenerateTrail(trail, direction * 10, Vector3.zero, ReflectingDistance, false, hit.collider.name));
            }

            LastGenerateTime = Time.time;
        }
    }

    // Trail 생성 함수
    private IEnumerator GenerateTrail(TrailRenderer Trail, Vector3 ReflectingPoint, Vector3 ReflectingNormal, float ReflectingDistance, bool MadeImpact, string ColliderName)
    {
        float distance;    // Ray가 진행 할 남은 거리
        float startingDistance;    // Ray가 진행 할 전체 거리

        Vector3 startPosition = Trail.transform.position;
        Vector3 direction = (ReflectingPoint - Trail.transform.position).normalized;    // Ray의 반사 지점으로의 Direction Vector 연산

        // 마지막 반사 과정 (ReflectingDistance를 모두 소진한 경우)
        if (ReflectingDistance <= 0) {
            // 최대 LastReflectingDistance까지만 진행 가능
            distance = LastReflectingDistance;
            startingDistance = distance;

            ReflectingPoint = Vector3.Lerp(startPosition, ReflectingPoint, LastReflectingDistance / Vector3.Distance(startPosition, ReflectingPoint));    // LastReflectingDistance까지만 진행 할 때의 ReflectingPoint 연산
        }

        // 마지막을 제외한 반사 과정
        else {
            // Ray의 반사 지점까지의 거리 연산
            distance = Vector3.Distance(Trail.transform.position, ReflectingPoint);
            startingDistance = distance;
        }

        // 반사 지점으로 이동
        while (distance > 0) {
            Trail.transform.position = Vector3.Lerp(startPosition, ReflectingPoint, 1 - (distance / startingDistance));
            distance -= Time.deltaTime * Speed;

            yield return null;
        }

        ReflectingPoint = new Vector3(ReflectingPoint.x + (direction.x * -0.1f), ReflectingPoint.y + (direction.y * -0.1f), 0);
        Trail.transform.position = ReflectingPoint;

        // 반사
        if (MadeImpact) {
            // Ray가 진행 하면서 소모하는 ReflectingDistance가 남아 있는 경우
            if (ReflectingDistance > 0) {
                // 반사 이후의 Direction Vector 연산
                Vector3 ReflectingDirection = Vector3.Reflect(direction, ReflectingNormal);

                RaycastHit2D hitReflection = Physics2D.Raycast(ReflectingPoint, ReflectingDirection, ReflectingDistance);

                // 남아 있는 ReflectingDistance로 다음 반사 지점까지 도달 할 수 있는 경우
                if (hitReflection.collider != null) {
                    yield return StartCoroutine(GenerateTrail(
                        Trail,
                        hitReflection.point,
                        hitReflection.normal,
                        ReflectingDistance - Vector3.Distance(hitReflection.point, ReflectingPoint),
                        true,
                        hitReflection.collider.name
                    ));
                }

                else {
                    // Last ReflectingPoint를 연산하기 위한 Raycast
                    RaycastHit2D hitReflectionLast = Physics2D.Raycast(ReflectingPoint, ReflectingDirection, float.MaxValue);

                    if (hitReflectionLast.collider != null) {
                        LastReflectingDistance = ReflectingDistance;

                        yield return StartCoroutine(GenerateTrail(
                            Trail,
                            hitReflectionLast.point,
                            Vector3.zero,
                            0,    // LastReflectingDistance에 최종 잔여 ReflectingDistance를 저장하고, 마지막 반사 과정임을 알리기 위해 인자로 0을 전달
                            false,
                            hitReflectionLast.collider.name
                        ));
                    }

                    // 반사 이후 도달할 ReflectingPoint가 없는 경우 [예외 처리]
                    else
                        Destroy(Trail.gameObject, Trail.time);
                }
            }
        }

        Destroy(Trail.gameObject, Trail.time);
    }
}
