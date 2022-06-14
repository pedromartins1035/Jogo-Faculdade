using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixDepth : MonoBehaviour {

	// Variável para atualizar a profundidade em cada quadro
    public bool fixEveryFrame;
    SpriteRenderer spr;

	void Start () {
        spr = GetComponent<SpriteRenderer>();
        spr.sortingLayerName = "Player";
        spr.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
	}

	void Update () {
        if (fixEveryFrame) {
            spr.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
        }
	}
}
