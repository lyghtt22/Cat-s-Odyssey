using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FloorLimit : MonoBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        SceneManager.LoadScene(0);
    }

    private void OnTriggerEnter(Collider other)
    {
        SceneManager.LoadScene(0);
    }

}
