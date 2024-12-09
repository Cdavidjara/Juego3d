using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Jugador : MonoBehaviour
{
    public float fuerzaSalto;
    public float fuerzaSaltoHorizontal = 950f;
    private Rigidbody2D rigidbody2D;
    private Animator animator;
    private bool salto = false;
    private GameManager gameManager; // Referencia al GameManager

    void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        salto = true;

        // Obtener referencia al GameManager en la escena
        gameManager = GameObject.FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("No se encontró el GameManager en la escena.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetBool("estaSaltando", true);
            if (salto)
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
            if (gameManager != null)
            {
                gameManager.GameOver();
            }
            else
            {
                Debug.LogError("GameManager no está inicializado.");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Banana")
        {
            if (gameManager != null)
            {
                gameManager.RecogerBanana(other.gameObject);
                Destroy(other.gameObject);

                // Verificar si el juego está activo antes de enviar los datos
                if (gameManager.gameStart && !gameManager.gameOver)
                {
                    StartCoroutine(EnviarBananasAlServidor(gameManager.ObtenerContadorBananas()));
                }
            }
            else
            {
                Debug.LogError("GameManager no está inicializado.");
            }
        }
    }

    IEnumerator EnviarBananasAlServidor(int cantidadBananas)
    {
        // URL del servidor PHP
        string url = "http://localhost/juego/guardar_bananas.php";

        // Datos a enviar al servidor (en este caso, la cantidad de bananas recolectadas)
        WWWForm form = new WWWForm();
        form.AddField("cantidadBananas", cantidadBananas);

        // Enviar la solicitud al servidor
        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error al enviar datos al servidor: " + www.error);
            }
            else
            {
                Debug.Log("Datos enviados correctamente al servidor.");
            }
        }
    }
}
