using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jugador : MonoBehaviour
{
    public float fuerzaSalto;
    public float fuerzaSaltoHorizontal = 950f; 
    private Rigidbody2D rigidbody2D;
    private Animator animator;
    private bool salto = false;
    public GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        salto = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetBool("estaSaltando", true); 
            if (salto == true) 
            {
                rigidbody2D.AddForce(new Vector2(0, fuerzaSalto));
                rigidbody2D.AddForce(new Vector2(fuerzaSaltoHorizontal, 0));
                salto = false; 
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Suelo")
        {
            animator.SetBool("estaSaltando", false); 
            salto = true;
        }
        if (collision.gameObject.tag == "Obstaculo")
        {
            gameManager.gameOver = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Banana")
        {
            gameManager.RecogerBanana(other.gameObject);
            Destroy(other.gameObject);
        }
    }
}
