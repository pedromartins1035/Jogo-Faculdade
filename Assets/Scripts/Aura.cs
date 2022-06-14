using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aura : MonoBehaviour {

	// Tempo de pré-carga
    public float waitBeforePlay;

    Animator anim;
    Coroutine manager;
    bool loaded;

    void Start () {
        anim = GetComponent<Animator>();
    }

    public void AuraStart() {
        manager = StartCoroutine(Manager());
        anim.Play("Aura_Idle");
    }

    public void AuraStop() {
        StopCoroutine(manager);
        anim.Play("Aura_Idle");
        loaded = false;
    }

	// Método para verificar se já carregamos o suficiente
    public IEnumerator Manager() {
        yield return new WaitForSeconds(waitBeforePlay);
        anim.Play("Aura_Play");
        loaded = true;
    }

	// Método para verificar se já carregamos o suficiente
    public bool IsLoaded() {
        return loaded;
    }
}