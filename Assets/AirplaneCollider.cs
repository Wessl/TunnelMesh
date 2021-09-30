using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirplaneCollider : MonoBehaviour
{
    public GameObject camera;
    public GameManager gameManager;

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Tunnel"))
        {
            camera.transform.SetParent(null);
            Debug.Log("Oh shit, you fucking crashed kiddo. shit fuck luck");
            gameManager.OnDeath();
            Destroy(gameObject);
        }
    }
}
