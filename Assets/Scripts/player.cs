using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    private CharacterController controller; //  var para declarar character controle
    private Animator anima // var da animação

    [Header("Config")]
    public int HP; // var de vida

    public float pVel = 3f; // var de velocidade

    private Vector3 direction; // var de vector de destino 
    private bool isWalking;// var para disparar animação

    //Input
    private float hor; //var para denominar eixo horizontal
    private float ver;// var para demnominar eixo vertical

    [Header("Attack Config")]
    public ParticleSystem fxAttack; // pegando efeito de particula 
    public Transform hitBox; // pegando colisor  para o ataque 
    [Range(0.2f, 2f)]
    public float hitRange = 0.5f; // tamanho do ataque 
    public LayerMask hitMask;// quais layer vai indetificar o ataque 
    private bool isAttack; // se o ataque 
    public Collider[] hitInfo; // pega os colisores que entra em colisão com hitbox
    public int amountDmg;// valor do dano 

    private void Start()
    {
        controller = GetComponent<CharacterController>();// pegando character cotroler
        anima = GetComponent<Animator>();// pegando animator
    }

    private void Update()
    {
        Inputs(); // imputs
        PlayerMoves();// moviemnto do player

        UpdateAnimator();// animações
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "TakeDamage")// se colidir a tag dispara dano 
        {
            GetHit(1);
        }
    }

    #region MEUS MÉTODOS

    void Inputs()
    {
        hor = Input.GetAxis("Horizontal"); // pegando eixo horizontal
        ver = Input.GetAxis("Vertical");// pegando eixo vertical

        if (Input.GetButtonDown("Fire1") && !isAttack)// diparando ataque se for verdadeiro
        {
            Attack(); // metodo ataque
        }
    }

    void Attack() //metodo de ataque diparado ele com a animação  e efeito 
    {
         isAttack = true; 
         anima.SetTrigger("Attack");
         fxAttack.Emit(1);

        hitInfo = Physics.OverlapSphere(hitBox.position, hitRange, hitMask); //pegando uma esfera de colisão para hitbox
             foreach(Collider c in hitInfo)
             {
                  c.gameObject.SendMessage("GetHit", amountDmg);//mando messagen de dano e o valor para objeto atigindo 
            print(c);
             }


        }

    void PlayerMoves() // movimentos
    {
        direction = new Vector3(hor, 0, ver).normalized;

        if (direction.magnitude > 0.1f) // se  o player se mover mais que valor x dispara animação e de move tendo rotção
        {
            //float pAngle = Mathf.Atan2(direction.x * 2, direction.z * 2) * Mathf.Rad2Deg;
            //transform.rotation = Quaternion.Euler(0, pAngle, 0);

            Quaternion qto = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, qto, Time.deltaTime * pVel * 4);

            isWalking = true;
        }
        else
        {
            isWalking = false;
        }

        controller.Move(direction * pVel * Time.deltaTime); // dando velocidade ao movimento 
    }

    void UpdateAnimator()
    {
        anima.SetBool("isWalking", isWalking); // diparando animação de  andar 
    }

   void AttackIsDone()
    {
        isAttack = false; 
    }
   // metodo que recebe o valor da vida 
    void GetHit(int amount)
    {
        HP -= amount;
        // receber dano e a vida inda for maior que 0 dispara animação de dano se não dipsra animação de morte 
        if (HP > 0)
        {
            anima.SetTrigger("Hit");
        }
        else
        {
            anima.SetTrigger("Die");
        }
    }

    #endregion

    private void OnDrawGizmosSelected() // mostrando  o hitbox na scene da unity 
    {
        if(hitBox != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(hitBox.position, hitRange);
        }
    }

}
