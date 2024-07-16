using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject col;
    public Renderer fondo;
    public float velocidad = 2;
    public float velocidadBananas = 1;
    public GameObject perro;
    public GameObject madera;
    public GameObject banana;
    public GameObject palmera;
    public bool gameOver = false;
    public bool gameStart = false;
    public GameObject menuStart;
    public GameObject menuOver;
    public TMP_Text textoGanaste; // Agregar referencia al texto que muestra "Ganaste"

    public List<GameObject> cols;
    public List<GameObject> obstaculos;
    public List<GameObject> bananas;

    private int contadorBananas = 0;
    public TMP_Text textoContadorBananas;
    private float margenMinimo = 3f; // Margen mínimo entre los obstáculos

    void Start()
    {
        cols = new List<GameObject>();
        obstaculos = new List<GameObject>();
        bananas = new List<GameObject>();

        // Crear mapa
        for (int i = 0; i < 40; i++)
        {
            cols.Add(Instantiate(col, new Vector2(-20 + i, -3), Quaternion.identity));
        }
        // Crear obstaculos
        obstaculos.Add(Instantiate(perro, new Vector2(6, -1.5f), Quaternion.identity));
        obstaculos.Add(Instantiate(madera, new Vector2(18, -1.5f), Quaternion.identity));
        obstaculos.Add(Instantiate(palmera, new Vector2(29, -1.5f), Quaternion.identity));

        // Generar bananas continuamente
        StartCoroutine(GenerarBananas());

        ActualizarContadorBananas();
    }

    IEnumerator GenerarBananas()
    {
        while (true)
        {
            float x = Random.Range(7f, 10f);
            float y = Random.Range(-3f, 3f);
            GameObject nuevaBanana = Instantiate(banana, new Vector2(x, y), Quaternion.identity);
            bananas.Add(nuevaBanana);
            yield return new WaitForSeconds(1f);
        }
    }

    void Update()
    {
        if (gameStart == false)
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                gameStart = true;
            }
        }
        if (gameStart == true && gameOver == true)
        {
            menuOver.SetActive(true);
            if (Input.GetKeyDown(KeyCode.X))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
        if (gameStart == true && gameOver == false)
        {
            menuStart.SetActive(false);
            fondo.material.mainTextureOffset = fondo.material.mainTextureOffset + new Vector2(0.015f, 0) * Time.deltaTime;

            // Mover mapa
            for (int i = 0; i < cols.Count; i++)
            {
                if (cols[i].transform.position.x <= -20)
                {
                    cols[i].transform.position = new Vector3(20, -3, 0);
                }

                cols[i].transform.position = cols[i].transform.position + new Vector3(-1, 0, 0) * Time.deltaTime * velocidad;
            }
            // Mover obstaculos
            for (int i = 0; i < obstaculos.Count; i++)
            {
                if (obstaculos[i].transform.position.x <= -10)
                {
                    ReubicarObstaculo(obstaculos[i]);
                }

                obstaculos[i].transform.position = obstaculos[i].transform.position + new Vector3(-1, 0, 0) * Time.deltaTime * velocidad;
            }
            // Mover bananas
            for (int i = 0; i < bananas.Count; i++)
            {
                if (bananas[i].transform.position.x <= -10)
                {
                    float ran = Random.Range(10, 18);
                    bananas[i].transform.position = new Vector3(ran, Random.Range(-3f, 3f), 0);
                }

                bananas[i].transform.position = bananas[i].transform.position + new Vector3(-1, 0, 0) * Time.deltaTime * velocidadBananas;
            }
        }

        // Verificar si se alcanzó la cantidad de bananas para ganar
        if (contadorBananas >= 15)
        {
            Ganaste();
        }
    }

    private void ReubicarObstaculo(GameObject obstaculo)
    {
        bool posicionValida = false;
        float ranX = 0;
        while (!posicionValida)
        {
            ranX = Random.Range(10, 23);
            posicionValida = true;
            foreach (var obs in obstaculos)
            {
                if (obs != obstaculo && Mathf.Abs(obs.transform.position.x - ranX) < margenMinimo)
                {
                    posicionValida = false;
                    break;
                }
            }
        }
        obstaculo.transform.position = new Vector3(ranX, -1.5f, 0);
    }

    public void RecogerBanana(GameObject bananaRecogida)
    {
        contadorBananas++;
        ActualizarContadorBananas();

        bananas.Remove(bananaRecogida);
        Destroy(bananaRecogida);
    }

    private void ActualizarContadorBananas()
    {
        if (textoContadorBananas != null)
        {
            textoContadorBananas.text = "Bananas: " + contadorBananas;
        }
    }

    // Método para mostrar "Ganaste" y reiniciar el juego
    void Ganaste()
    {
        Debug.Log("¡Ganaste!");
        // Mostrar el mensaje de "Ganaste"
        textoGanaste.gameObject.SetActive(true);
        // Desactivar el juego
        gameStart = false;
        gameOver = true;
        // Mostrar el menú de inicio
        menuStart.SetActive(true);
    }

    // Método para reiniciar el juego
    void ReiniciarJuego()
    {
        // Reiniciar el contador de bananas
        contadorBananas = 0;
        ActualizarContadorBananas();
        // Ocultar el menú de inicio
        menuStart.SetActive(false);
        // Reiniciar el juego
        gameStart = true;
        gameOver = false;
    }
}
