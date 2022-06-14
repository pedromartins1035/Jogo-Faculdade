using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

	public Joystick joystick;
	public Joybutton joybutton;
	private int cont = 0;

    public float speed = 4f;
    public GameObject initialMap; 
    public GameObject slashPrefab;

    Animator anim;
    Rigidbody2D rb2d;
	Vector2 mov;  // Agora é visível entre os métodos

    CircleCollider2D attackCollider;

    Aura aura;

    bool movePrevent;

	///--- Variáveis ​​relacionadas à vida
	[Tooltip("Pontos de vida")]
	public int maxHp = 3;
	[Tooltip("Vida atual")]
	public int hp;

	void Awake() {
        Assert.IsNotNull(initialMap);
        Assert.IsNotNull(slashPrefab);
    }

    void Start () {
        anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();

		joybutton = FindObjectOfType<Joybutton>();

		//* Recuperamos o colisor de ataque e o desativamos
        attackCollider = transform.GetChild(0).GetComponent<CircleCollider2D>();
        attackCollider.enabled = false;  

        Camera.main.GetComponent<MainCamera>().SetBound(initialMap);

        aura = transform.GetChild(1).GetComponent<Aura>();

    }

    void Update () {
		if (joybutton.pressed) {
			cont++;
		} 

		// detectamos o movimento
        Movements ();

		//  processamos as animações
        Animations ();

		// Atacar com espada
        SwordAttack (); 

        // Ataque com Slash
        SlashAttack ();

		// Impedir o movimento
        PreventMovement ();


    }

	///--- Gestão do ataque, subtraímos uma vida
	//public float restartDelay = 2f;
	public void Attacked(){
		if (--hp <= 0) {
			Destroy (gameObject);
			Restart();		
		}
	}

	public void Restart(){

		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		//Application.LoadLevel(SceneManager.GetActiveScene().name);
	}
		

	void FixedUpdate () {
		// nos movemos no fixo pelo físico
        rb2d.MovePosition(rb2d.position + mov * speed * Time.deltaTime);
    }

    void Movements () {
		// Detectamos movimento em um vetor 2D
		mov = new Vector2(
			joystick.Horizontal * 1f,
			joystick.Vertical * 1f
		);
    }

    void Animations () {

        if (mov != Vector2.zero) {
            anim.SetFloat("movX", mov.x);
            anim.SetFloat("movY", mov.y);
            anim.SetBool("walking", true);
        } else {
            anim.SetBool("walking", false);
        }
    }

    void SwordAttack () {

		// Estamos atualizando a posição da colisão de ataque
        if (mov != Vector2.zero) {
            attackCollider.offset = new Vector2(mov.x/2, mov.y/2);
        }

		// Procuramos o estado atual olhando as informações do animador
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        bool attacking = stateInfo.IsName("Player_Attack");

		// Nós detectamos o ataque, ele tem prioridade, então ele cai completamente			//Comando para o pc
        /*if ((Input.GetKeyDown("space") || Input.GetKeyDown(KeyCode.X)) && !attacking ){  
			anim.SetTrigger("attacking");
        }
		*/

		if (((joybutton.pressed && cont < 5 ) || Input.GetKeyDown(KeyCode.X)) && !attacking ){  
			anim.SetTrigger("attacking");
			cont = 0;
		}

		// Ative o colisor no meio da animação de ataque
		if(attacking) { // O normalizado sempre acaba por ser um ciclo entre 0 e 1
            float playbackTime = stateInfo.normalizedTime;

            if (playbackTime > 0.33 && playbackTime < 0.66) attackCollider.enabled = true;
            else attackCollider.enabled = false;
        }

    }

    void SlashAttack () {
		

		// Procuramos o estado atual olhando as informações do animador
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        bool loading = stateInfo.IsName("Player_Slash");

        // Ataque a distancia
		if ((joybutton.pressed  && cont > 10 && cont <=15 )||Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.Z)){ 
            anim.SetTrigger("loading");
            aura.AuraStart();
		} else if ((!joybutton.pressed && cont > 5 )||Input.GetKeyUp(KeyCode.LeftAlt) || Input.GetKeyUp(KeyCode.Z)){ 
			cont = 0;
            anim.SetTrigger("attacking");
            if (aura.IsLoaded()) {
				// Para mover desde o início, temos que atribuir um
				// valor inicial para movX ou movY no editor diferente de zero
                float angle = Mathf.Atan2(
                    anim.GetFloat("movY"), 
                    anim.GetFloat("movX")
                ) * Mathf.Rad2Deg;

                GameObject slashObj = Instantiate(
                    slashPrefab, transform.position, 
                    Quaternion.AngleAxis(angle, Vector3.forward)
                );

                Slash slash = slashObj.GetComponent<Slash>();
                slash.mov.x = anim.GetFloat("movX");
                slash.mov.y = anim.GetFloat("movY");
            }
            aura.AuraStop();
            StartCoroutine(EnableMovementAfter(0.4f));
        } 

		// Eu previno movimento ao carregar
        if (loading) { 
            movePrevent = true;
        }

    }

    void PreventMovement () {
        if (movePrevent) { 
            mov = Vector2.zero;
        }
    }

    IEnumerator EnableMovementAfter(float seconds){
        yield return new WaitForSeconds(seconds);
        movePrevent = false;
    }

	void OnGUI() {
		// posição do inimigo no mundo com relação à câmera
		Vector2 pos = Camera.main.WorldToScreenPoint (transform.position);

		// desenhamos o quadrado abaixo do inimigo com o texto
		GUI.Box(
			new Rect(
				pos.x - 20,                   // posição x da barra
				Screen.height - pos.y + 60,   // posição y da barra
				40,                           // largura da barra    
				24                            // altura da barra  
			), hp + "/" + maxHp               // texto da barra
		);
	}
		
}
