using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class Boss : MonoBehaviour {

	// Variáveis ​​para gerenciar o raio de visão, atacar e acelerar
	public float visionRadius;
	public float attackRadius;
	public float speed;

	// Variáveis ​​relacionadas ao ataque
	[Tooltip("Prefab da pedra que ira disparar")]
	public GameObject Pedrada;
	[Tooltip("Velocidade de ataque (segundos entre ataques)")]
	public float attackSpeed = 2f;
	bool attacking;

	///--- Variáveis ​​relacionadas à vida
	[Tooltip("Pontos de vida")]
	public int maxHp = 3;
	[Tooltip("Vida atual")]
	public int hp;

	// Variável para salvar o jogador
	GameObject player;

	// Salva Posição inicial
	Vector3 initialPosition, target;

	// Animador e corpo cinematográfico com rotação em Z congelado
	Animator anim;
	Rigidbody2D rb2d;

	void Start () {

		// Nós recuperamos o jogador usando à tag
		player = GameObject.FindGameObjectWithTag("Player");

		// Guardamos nossa posicão inicial
		initialPosition = transform.position;

		anim = GetComponent<Animator>();
		rb2d = GetComponent<Rigidbody2D>();

		///--- Iniciamos a vida
		hp = maxHp;
	}

	void Update () {

		// Por padrão, nossa meta sempre será nossa posição inicial
		target = initialPosition;

		// Nós checamos um Raycast do inimigo até que o jogador
		RaycastHit2D hit = Physics2D.Raycast(
			transform.position, 
			player.transform.position - transform.position, 
			visionRadius, 
			1 << LayerMask.NameToLayer("Default") 

			// Coloque o inimigo em uma camada diferente do padrão para evitar o raycast
			// Coloque também o objeto Attack e o Prefab Slash em um Layer Attack
			// Se não detectá-los como um ambiente, retroceda ao fazer ataques
		);

		// Aqui podemos depurar o Raycast
		Vector3 forward = transform.TransformDirection(player.transform.position - transform.position);
		Debug.DrawRay(transform.position, forward, Color.red);

		// Se o Raycast encontrar o jogador, nós o colocaremos no alvo
		if (hit.collider != null) {
			if (hit.collider.tag == "Player"){
				target = player.transform.position;
			}
		}

		// Calculamos a distância e direção atual para o alvo
		float distance = Vector3.Distance(target, transform.position);
		Vector3 dir = (target - transform.position).normalized;

		// Se é o inimigo e está no alcance do ataque nós paramos e atacamos
		if (target != initialPosition && distance < attackRadius){
			// Aqui nós o atacávamos, mas por enquanto nós apenas mudamos a animação
			anim.SetFloat("movX", dir.x);
			anim.SetFloat("movY", dir.y);
			anim.Play("Enemy_Walk", -1, 0);  // Congela a animacão de andar

			///-- Começamos atacando (importante uma camada no ataque para evitar Raycast)
			if (!attacking) StartCoroutine(Attack(attackSpeed));
		}
		// Caso contrário, nos movemos em direção a ele
		else {
			rb2d.MovePosition(transform.position + dir * speed * Time.deltaTime);

			// Quando nos movemos, estabelecemos a animação do movimento
			anim.speed = 1;
			anim.SetFloat("movX", dir.x);
			anim.SetFloat("movY", dir.y);
			anim.SetBool("walking", true);
		}

		// Uma última verificação para evitar erros, forçando a posição inicial
		if (target == initialPosition && distance < 0.05f){
			transform.position = initialPosition; 
			// Y cambiamos la animación de nuevo a Idle
			anim.SetBool("walking", false);
		}

		// E uma depuração opcional com uma linha até o destino
		Debug.DrawLine(transform.position, target, Color.green);
	}

	// Podemos desenhar o raio de visão e atacar a cena desenhando uma esfera
	void OnDrawGizmosSelected() {

		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, visionRadius);
		Gizmos.DrawWireSphere(transform.position, attackRadius);

	}

	IEnumerator Attack(float seconds){
		attacking = true;  // Ativamos a bandeira
		// Se temos objetivo e a prefab está correta, criamos a rocha
		if (target != initialPosition && Pedrada != null) {
			Instantiate(Pedrada, transform.position, transform.rotation);
			// esperamos os segundos do turno antes de fazer outro ataque
			yield return new WaitForSeconds(seconds);
		}
		attacking = false; // Desativamos a bandeira
	}

	///--- Gestão do ataque, subtraímos uma vida
	public void Attacked(){
		if (--hp <= 0) {
			Destroy (gameObject);
			//Restart();		
		}
	}

	/*public void Restart(){

		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		//Application.LoadLevel(SceneManager.GetActiveScene().name);
	}*/

	///---  Nós chamamos a vida do inimigo em uma barra
	void OnGUI() {
		// Nós mantemos a posição do inimigo no mundo com relação à câmera
		Vector2 pos = Camera.main.WorldToScreenPoint (transform.position);

		// Nós desenhamos o quadrado abaixo do inimigo com o texto
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