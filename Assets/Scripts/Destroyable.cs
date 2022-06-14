using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Script para criar um objeto destrutível
public class Destroyable : MonoBehaviour {

	// Variável para salvar o nome do estado de destruição
    public string destroyState;
	// Variável com os segundos a aguardar antes de desativar a colisão
    public float timeForDisable;

	// Animador para controlar a animação
    Animator anim;

    void Start () {
        anim = GetComponent<Animator>();
    }

	// Detectamos a colisão com uma co-rotina
    IEnumerator OnTriggerEnter2D (Collider2D col) {

        // Se é um ataque
        if (col.tag == "Attack") {

			// reproduzimos a animação de destruição e esperamos
            anim.Play(destroyState);
            yield return new WaitForSeconds(timeForDisable);

			// Após os segundos de espera, desativamos os colisores 2D
            foreach(Collider2D c in GetComponents<Collider2D>()){
                c.enabled = false;
            }

        }

    }

    void Update () {
        
		// "Destrua" o objeto no final da animação de destruição
		// O estado deve ter o atributo 'loop' como false para não se repetir
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName(destroyState) && stateInfo.normalizedTime >= 1) {
            Destroy(gameObject);
			// No futuro, poderíamos armazenar a instância e sua transformação
			// para criá-los novamente depois de um tempo
        }

    }

}