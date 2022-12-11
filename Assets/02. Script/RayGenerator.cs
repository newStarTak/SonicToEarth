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

    public Collider2D prevColl; 

    public GameObject hitLight;
    public GameObject bigLight;
    public GameObject hitParticle;
    public float lifeTime;

    // Ray ���� �Լ�
    public void RayGenerate()
    {
        RaycastHit2D hit;

        if (LastGenerateTime + GenerateDelay < Time.time) {
            Vector3 direction = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z)) - RayGeneratePoint.transform.position;    // Ray �߻� ���� ����
            TrailRenderer trail = Instantiate(RayTrail, RayGeneratePoint.position, Quaternion.identity);    // Trail Renderer ����

            // �߻�� Ray�� �浹ü�� ����
            hit = Physics2D.Raycast(RayGeneratePoint.position, direction, float.MaxValue);

            if (hit.collider != null) {
                StartCoroutine(GenerateTrail(trail, hit.point, hit.normal, ReflectingDistance, true, hit.collider.name));

                /* ���̸� �̸� ������ �� ���� ��ȣ�ۿ� ������Ʈ���
                 * isNextRayHitSpecial ������ true�� ������ ���� �浹�� ����ص�
                 * ���� �浹 ó���� �ݻ� ���� �ȿ��� �����ص� */
                if (hit.collider.tag == "LANTERN" || hit.collider.tag == "DOOR")
                {
                    prevColl = hit.collider;
                }
            }

            else {
                // �߻�� Ray�� �浹���� ���ϴ� ��Ȳ���� ���� �Ÿ� ���� �� �Ҹ�
                StartCoroutine(GenerateTrail(trail, direction * 10, Vector3.zero, ReflectingDistance, false, hit.collider.name));
            }

            LastGenerateTime = Time.time;
        }
    }

    // Trail ���� �Լ�
    private IEnumerator GenerateTrail(TrailRenderer Trail, Vector3 ReflectingPoint, Vector3 ReflectingNormal, float ReflectingDistance, bool MadeImpact, string ColliderName)
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

        ReflectingPoint = new Vector3(ReflectingPoint.x + (direction.x * -0.1f), ReflectingPoint.y + (direction.y * -0.1f), 0);
        Trail.transform.position = ReflectingPoint;

        // �ݻ�
        if (MadeImpact) {
            // Ray�� ���� �ϸ鼭 �Ҹ��ϴ� ReflectingDistance�� ���� �ִ� ���
            if (ReflectingDistance > 0) {
                // �ݻ� ������ Direction Vector ����
                Vector3 ReflectingDirection = Vector3.Reflect(direction, ReflectingNormal);

                RaycastHit2D hitReflection = Physics2D.Raycast(ReflectingPoint, ReflectingDirection, ReflectingDistance);

                // ���� �ִ� ReflectingDistance�� ���� �ݻ� �������� ���� �� �� �ִ� ���
                if (hitReflection.collider != null) {

                    /* ������ ���� ��ȣ�ۿ� ������Ʈ�� �浹�� ��ȣ�ۿ��� �ϰ� ��
                     * ���� �˻縦 ���� �ؾ� ���� �浹 �� ���������� ���� ��ȣ�ۿ��� �̷���� */
                    if (prevColl)
                    {
                        if (prevColl.tag == "DOOR")
                        {
                            Instantiate(bigLight, Trail.transform.position, Quaternion.identity);

                            GameObject.FindGameObjectWithTag("FOLLOWCAM").GetComponent<CameraCtrl>().
                                ZoooomIn(GameObject.FindGameObjectWithTag("Respawn"));

                            foreach (GameObject door in GameObject.FindGameObjectsWithTag("DOOR"))
                            {
                                Destroy(door);
                            }
                            foreach (GameObject door in GameObject.FindGameObjectsWithTag("FAKEDOOR"))
                            {
                                Destroy(door);
                            }

                            prevColl = null;
                        }
                        else if (!prevColl.GetComponent<LightCtrl>().isUp)
                        {
                            prevColl.GetComponent<LightCtrl>().isUp = true;
                            GameObject.FindGameObjectWithTag("FOLLOWCAM").GetComponent<CameraCtrl>().
                                ZoomIn(prevColl.gameObject);
                            prevColl = null;
                            Debug.Log("- ! = = = = = < L A N T E R N > = = = = = ! -");
                        }
                    }
                    else
                    {
                        // �� �ݻ� �� Point Light�� Particle ����
                        GameObject newHitLight = Instantiate(hitLight, Trail.transform.position, Quaternion.identity);
                        Destroy(newHitLight, lifeTime);
                        GameObject newHitParticle = Instantiate(hitParticle, Trail.transform.position, Quaternion.identity);
                        Destroy(newHitParticle, lifeTime);
                    }

                    /* ���̸� �̸� ������ �� ���� ��ȣ�ۿ� ������Ʈ���
                     * isNextRayHitSpecial ������ true�� ������ ���� �浹�� ����ص� */
                    if (hitReflection.collider.tag == "LANTERN" || hitReflection.collider.tag == "DOOR")
                    {
                        prevColl = hitReflection.collider;
                    }

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
                    // Last ReflectingPoint�� �����ϱ� ���� Raycast
                    RaycastHit2D hitReflectionLast = Physics2D.Raycast(ReflectingPoint, ReflectingDirection, float.MaxValue);

                    /* ������ ���� ��ȣ�ۿ� ������Ʈ�� �浹�� ��ȣ�ۿ��� �ϰ� ��
                     * ���� �˻縦 ���� �ؾ� ���� �浹 �� ���������� ���� ��ȣ�ۿ��� �̷���� */
                    if (prevColl)
                    {
                        if (prevColl.tag == "DOOR")
                        {
                            Instantiate(bigLight, Trail.transform.position, Quaternion.identity);

                            GameObject.FindGameObjectWithTag("FOLLOWCAM").GetComponent<CameraCtrl>().
                                ZoooomIn(GameObject.FindGameObjectWithTag("Respawn"));

                            foreach (GameObject door in GameObject.FindGameObjectsWithTag("DOOR"))
                            {
                                Destroy(door);
                            }
                            foreach (GameObject door in GameObject.FindGameObjectsWithTag("FAKEDOOR"))
                            {
                                Destroy(door);
                            }

                            prevColl = null;
                        }
                        else if (!prevColl.GetComponent<LightCtrl>().isUp)
                        {
                            prevColl.GetComponent<LightCtrl>().isUp = true;
                            GameObject.FindGameObjectWithTag("FOLLOWCAM").GetComponent<CameraCtrl>().
                                ZoomIn(prevColl.gameObject);
                            prevColl = null;
                            Debug.Log("- ! = = = = = < L A N T E R N > = = = = = ! - Last Hit");
                        }
                    }
                    else
                    {
                        // �� �ݻ� �� Point Light�� Particle ����
                        GameObject newHitLight = Instantiate(hitLight, Trail.transform.position, Quaternion.identity);
                        Destroy(newHitLight, lifeTime);
                        GameObject newHitParticle = Instantiate(hitParticle, Trail.transform.position, Quaternion.identity);
                        Destroy(newHitParticle, lifeTime);
                    }

                    /* ���̸� �̸� ������ �� ���� ��ȣ�ۿ� ������Ʈ���
                     * isNextRayHitSpecial ������ true�� ������ ���� �浹�� ����ص� */
                    if (hitReflectionLast.collider.tag == "LANTERN" || hitReflectionLast.collider.tag == "DOOR")
                    {
                        prevColl = hitReflectionLast.collider;
                    }

                    if (hitReflectionLast.collider != null) {
                        LastReflectingDistance = ReflectingDistance;

                        //Debug.Log("Last Hit Obj Name: " + hitReflectionLast.collider.name);   // hitReflectionLast�� ���� ������Ʈ�� �Ÿ��� ���� �ʾ� ������ Ʈ������ ���� ����

                        yield return StartCoroutine(GenerateTrail(
                            Trail,
                            hitReflectionLast.point,
                            Vector3.zero,
                            0,    // LastReflectingDistance�� ���� �ܿ� ReflectingDistance�� �����ϰ�, ������ �ݻ� �������� �˸��� ���� ���ڷ� 0�� ����
                            false,
                            hitReflectionLast.collider.name
                        ));
                    }

                    // �ݻ� ���� ������ ReflectingPoint�� ���� ��� [���� ó��]
                    else
                        Destroy(Trail.gameObject, Trail.time);
                }
            }
        }

        Destroy(Trail.gameObject, Trail.time);
    }
}
