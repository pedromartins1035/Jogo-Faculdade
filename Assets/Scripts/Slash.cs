using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : MonoBehaviour {

    [Tooltip("Esperar X segundos antes de destruir o objeto")]
    public float waitBeforeDestroy;

    [HideInInspector]
    public Vector2 mov;

    public float speed;

    void Update () {
        transform.position += new Vector3(mov.x,mov.y,0) * speed * Time.deltaTime;
    }

    IEnumerator OnTriggerEnter2D (Collider2D col) {
        if (col.tag == "Object"){ 
            yield return new WaitForSeconds(waitBeforeDestroy);
            Destroy(gameObject);
        } else if (col.tag != "Player" && col.tag != "Attack"){ 
			///--- Nós subtraímos um da vida se é um inimigo
            if (col.tag == "Enemy") col.SendMessage("Attacked");
            Destroy(gameObject);
        }
    }

}