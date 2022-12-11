using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnlyForFinishLine : MonoBehaviour
{
    [SerializeField]
    private string scName;

    private void OnCollisionEnter2D(Collision2D coll)
    {
        if(coll.collider.tag == "Player")
        {
            SceneManager.LoadScene(scName);
        }
    }
}
