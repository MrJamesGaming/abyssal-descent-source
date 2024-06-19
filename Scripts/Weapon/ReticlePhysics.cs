using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class ReticlePhysics : MonoBehaviour
{

    CapsuleCollider2D capsule;
    // Start is called before the first frame update
    void Start()
    {
        capsule = GetComponent<CapsuleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collider){
        capsule.enabled = true;
    }

}
