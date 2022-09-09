using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestHurtbox : MonoBehaviour
{
    public void OnTriggerStay(Collider other)
    {
        //Debug.Log(other.gameObject.name);
        if (other.gameObject.tag == "Enemy")
        {
            TestCharacterController tcc = transform.parent.gameObject.GetComponent<TestCharacterController>();
            tcc.TakeDamage(other.gameObject);
        }
    }
}
