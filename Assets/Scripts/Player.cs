using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float velocity = 8f;
    public float rotationSpeed = 50f;
    public Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }
    private void Move()
    {
        float verticalAxis = Input.GetAxis("Vertical");
        float horizontalAxis = Input.GetAxis("Horizontal");

        transform.Rotate(0, horizontalAxis * Time.deltaTime * rotationSpeed, 0);

        transform.Translate(0, 0, verticalAxis * Time.deltaTime * velocity);

        anim.SetFloat("horizontal", horizontalAxis);
        anim.SetFloat("vertical", verticalAxis);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Attack"))
        {
            anim.SetTrigger("isHit");
        }
    }
}
