using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pedra : MonoBehaviour {

	[Tooltip("Velocidade de movimento em unidades do mundo")]
    public float speed;

	GameObject player;   // Nós recuperamos o objeto jogador
	Rigidbody2D rb2d;    // Nós recuperamos o componente do corpo rígido
	Vector3 target, dir; // Vetores para armazenar o objetivo e seu endereço

    void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        rb2d = GetComponent<Rigidbody2D>();

		// Recuperamos a posição do jogador e o endereço normalizado
        if (player != null){
            target = player.transform.position;
            dir = (target - transform.position).normalized;
        }
	}

    void FixedUpdate () {
		// Se houver um alvo, movemos a rocha para a sua posição
        if (target != Vector3.zero) {
            rb2d.MovePosition(transform.position + (dir * speed) * Time.deltaTime);
        }
	}

    void OnTriggerEnter2D(Collider2D col){
		// Se colidirmos com o jogador ou um ataque, nós o apagamos
        if (col.transform.tag == "Player" || col.transform.tag == "Attack"){
            Destroy(gameObject);
			if (col.tag == "Player") col.SendMessage("Attacked");
			Destroy(gameObject);
        }
			
    }

    void OnBecameInvisible() {
		// Se você sair da tela, nós apagamos a rocha
        Destroy(gameObject);
    }
}
