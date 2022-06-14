using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Warp : MonoBehaviour
{
	// Para armazenar o ponto de destino
    public GameObject target;

	// Para armazenar o mapa de destino
    public GameObject targetMap;


	// Para controlar se a transição começa ou não
    bool start = false;
	// Para controlar se a transição é de entrada ou saída
    bool isFadeIn = false;
	// Opacidade inicial do quadrado de transição
    float alpha = 0;
	// 1 segunda transição
    float fadeTime = 1f;


    GameObject area;


    void Awake ()
    {
		// Nós vamos ter certeza de que o alvo foi estabelecido ou vamos lançar
        Assert.IsNotNull(target);

		// Se quisermos, podemos esconder a depuração dos Warps
        GetComponent<SpriteRenderer> ().enabled = false;
        transform.GetChild (0).GetComponent<SpriteRenderer> ().enabled = false;

        Assert.IsNotNull(targetMap);

		// Procuramos a área para mostrar o texto
        area = GameObject.FindGameObjectWithTag("Area");

    }

    IEnumerator OnTriggerEnter2D (Collider2D col) {

        if (col.tag == "Player"){

            col.GetComponent<Animator> ().enabled = false;
            col.GetComponent<Player> ().enabled = false;
            FadeIn();

            yield return new WaitForSeconds(fadeTime);

			// atualizamos a posição
            col.transform.position = target.transform.GetChild (0).transform.position;

            FadeOut();
            col.GetComponent<Animator> ().enabled = true;
            col.GetComponent<Player> ().enabled = true;

            StartCoroutine(area.GetComponent<Area>().ShowArea(targetMap.name));

			// atualizamos a câmera
            Camera.main.GetComponent<MainCamera>().SetBound(targetMap);

        }

    }

	// Vamos desenhar um quadrado com opacidade no topo da tela simulando uma transição
    void OnGUI () {

		// Se a transição não começar, deixamos o evento diretamente
        if (!start)
            return;

		// Se começou, criamos uma cor com uma opacidade inicial em 0
        GUI.color = new Color (GUI.color.r, GUI.color.g, GUI.color.b, alpha);

		// Criamos uma textura temporária para preencher a tela
        Texture2D tex;
        tex = new Texture2D (1, 1);
        tex.SetPixel (0, 0, Color.black);
        tex.Apply ();

		// desenhamos a textura em toda a tela
        GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), tex);

		// controlamos a transparência
        if (isFadeIn) {
			// Se é para aparecer, adicionamos opacidade
            alpha = Mathf.Lerp (alpha, 1.1f, fadeTime * Time.deltaTime);
        } else {
			// Se é para desaparecer nós subtrair a opacidade
            alpha = Mathf.Lerp (alpha, -0.1f, fadeTime * Time.deltaTime);
			// Se a opacidade atingir 0, desativamos a transição
            if (alpha < 0) start = false;
        }

    }

	// Método para ativar a transição de entrada
    void FadeIn () {
        start = true;
        isFadeIn = true;
    }

	// Método para ativar a transição de saída
    void FadeOut () {
        isFadeIn = false;
    }

}
