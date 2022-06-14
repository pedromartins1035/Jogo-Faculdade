using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Joybutton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler {

	[HideInInspector]
	public bool pressed;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnPointerUp(PointerEventData eventData){

		pressed = false;
	}

	public void OnPointerDown(PointerEventData eventData){

		pressed = true;
	}
}
