using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeirdCreatureMovement : MonoBehaviour
{
    private Rigidbody _rb;

    public float moveSpeed;
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_rb.velocity.magnitude < 5f)
        {
            _rb.AddForce(transform.forward * Time.deltaTime * moveSpeed);
        }
    }
}
