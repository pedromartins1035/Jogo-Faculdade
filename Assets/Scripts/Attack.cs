using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour {
    void OnTriggerEnter2D (Collider2D col) {
		///--- Nós subtraímos um da vida se é um inimigo
        if (col.tag == "Enemy") col.SendMessage("Attacked");
		if (col.tag == "Player") col.SendMessage ("Attacked");
		if (col.tag == "Boss") col.SendMessage("Attacked");
    }

}
