using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BixuIA : MonoBehaviour
{
    //busca gamemanager da scene
    private GameManager _GM;
    //declarando o animator
    private Animator anima;
    //particulas que saem do enemy ao receber dano
    public ParticleSystem fxBlood;
    //hp do enemy
    public int HP;
    // condição de morte
    private bool isDie;
    //confirmação de morte para funções secubndárias
    private bool die_confirm = false;


    public enemyState state;

    //IA
    private bool isWalk;
    private bool isAlert; 
    private bool isAttack;
    private bool isPlayerVisible;
    private NavMeshAgent agent;
    private int idWaypoint;
    private Vector3 destination; 
    // objetos que iram aparecer de mensagens nos estados
    public GameObject alert;
    public GameObject attack;
    public GameObject deadsymbol;

    private void Start()
    {
        _GM = FindObjectOfType(typeof(GameManager)) as GameManager;//pegando script do game manager, para usar seus metodos e variaveis 

        anima = GetComponent<Animator>();// pegando  o animator
        agent = GetComponent<NavMeshAgent>();// pegando navmesh

        changeState(state);
    }

    private void Update()
    {
    
        stateManager();
        // dispara a animação de alerta
        if (isAlert)
        {
            alert.SetActive(true);
        }
        else
        {
            alert.SetActive(false);
        }
        //dispara animação de ataque
        if (isAttack || state == enemyState.FOLLOW || state == enemyState.FURY)
        {
            attack.SetActive(true);
        }
        else
        {
            attack.SetActive(false);
        }
        //dispara animação walk
        if (agent.desiredVelocity.magnitude >= 0.1f)
        {
            isWalk = true;
        }
        else
        {
            isWalk = false;
        }
        anima.SetBool("isWalk", isWalk);// dispara a animação de walk 
        anima.SetBool("isAlert", isAlert);
         // se o hp estiver em 0 e a morte for verdadeira dispara as  animações e aparece a sprite de dead 
        if (HP <= 0 && !die_confirm)
        {
            //die_confirm = true;
            anima.SetTrigger("Die");
            attack.SetActive(false);
            deadsymbol.SetActive(true);
        }
    }
    //rotina de morte do IA
    IEnumerator enemyDisapear()
    {
        isDie = true;
        yield return new WaitForSeconds(2.5f);
        Destroy(this.gameObject, 5);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player") //se player entrou no campo de visao
        {
            isPlayerVisible = true;
            if (state == enemyState.IDLE || state == enemyState.PATROL) // se o state é IDLE ou PATROL fica em alerta
            {
                changeState(enemyState.ALERT);
            }
        }
    }
    // se o player sair do campo de visão do inimigo
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player") 
        {
            isPlayerVisible = false;
        }
    }

    #region MEUS MÉTODOS
    // metodo que dispara um hit
    void GetHit(int amount)
    {
        if (isDie)// morreu retorna verdadeiro
        {
            return;
        }

        HP -= amount; // vida menos valor da variavel amount
        if(HP > 0) // se a vida for maior que 0 
        {
            changeState(enemyState.FURY);// fica no estado de furia 
            anima.SetTrigger("GetHit");// dispara animação de hit 
            fxBlood.Emit(25);//dispara o efeito de sangue 
        }
        else //se for hp 0 || <
        {
            anima.SetTrigger("Die");// dispara animacão de morte
            StartCoroutine("enemyDisapear");
        }
        
    }
// maquina de estados
    void stateManager()
    {
        if (HP > 0)
        {
        
            switch (state)
            {
                case enemyState.IDLE:// estado parado

                    break;
                case enemyState.ALERT://estado em alerta

                    LookAt();

                    break;
                case enemyState.FOLLOW:// estado de perseguir

                    LookAt();


                    destination = _GM.player.position; //passando a posição do player
                    agent.destination = destination; // passando a posição x pro agente de destino  do navemesh

                    if (agent.remainingDistance <= agent.stoppingDistance) // para  se a distancia  for menor que x distacia
                    {
                        Attack();
                    }

                    break;
                case enemyState.FURY:// estado de furia 

                    LookAt();


                    destination = _GM.player.position;//passando a posição do player
                    agent.destination = destination;// passando a posição x pro agente de destino  do navmesh

                    if (agent.remainingDistance <= agent.stoppingDistance)// Attack() se proximo suficiente do alvo
                    {
                        Attack();
                    }

                    break;
                case enemyState.PATROL:// estado de patrulha

                    break;
            }
        }
    }

    void changeState(enemyState newState)
    {
        StopAllCoroutines(); // parando todas as rotinas paralelas
        
        isAlert = false; //alerta falso

        switch (newState)
        {
               // estado parado, pega o destino e se move com navmesh, dispara coroutine
            case enemyState.IDLE:
                agent.stoppingDistance = 0; //para objetos como player nao interferirem na chegada ao destino da patrulha
                destination = transform.position; 
                agent.destination = destination; 
                StartCoroutine("IDLE");
                break;
                // estado alert, pega o destino e se move com navemesh, se torna verdadeiro e dispara corountine
            case enemyState.ALERT:
                agent.stoppingDistance = 0; //para objetos como player nao interferirem na chegada ao destino da patrulha
                destination = transform.position;
                agent.destination = destination;
                isAlert = true;
                StartCoroutine("ALERT");
                break;
                // estado patrol, pega o destino e se move com navemesh entre pontos definidos, dispara corountine
            case enemyState.PATROL:
                agent.stoppingDistance = 0; //para objetos como player nao interferirem na chegada ao destino da patrulha
                idWaypoint = Random.Range(0, _GM.slimeWayPoints.Length);
                destination = _GM.slimeWayPoints[idWaypoint].position;
                agent.destination = destination;

                StartCoroutine("PATROL");
                
                break;
                // estado Fallow, pega o destino e se move com navemesh,mas para em uma distacia pre determinada para dispara o ataque  ,dispara corountine
            case enemyState.FOLLOW:

                destination = transform.position;
                agent.stoppingDistance = _GM.slimeDistanceToAttack;
                agent.destination = destination;
                StartCoroutine("FOLLOW");

                break;
                 // estado Fury, pega o destino e se move com navemesh,mas para em uma distacia pre determinada para dispara o ataque
            case enemyState.FURY:
                destination = transform.position;
                agent.stoppingDistance = _GM.slimeDistanceToAttack;
                agent.destination = destination;
                break;
            
        }
        state = newState;
    }
// tempo de idle quanto ele fica parado
    IEnumerator IDLE()
    {
        yield return new WaitForSeconds(_GM.slimeIdleWaitTime);
        stayStill(50);
    }
// tempo da patrulha quando chega no destino ou perto dele
    IEnumerator PATROL()
    {
        yield return new WaitUntil(() => agent.remainingDistance <= 0); //operador lambda ativa quando distancia remanescente do agent for menor ou igual a 0
        stayStill(30);
    }
// tempo para que se não ver o player pare de persegir 
    IEnumerator FOLLOW()
    {
        yield return new WaitUntil(() => !isPlayerVisible);
        print("perdi você");

        yield return new WaitForSeconds(_GM.slimeAlertTime);
        if (!isPlayerVisible)
        {
            stayStill(50);
        }
        else
        {
            changeState(enemyState.FOLLOW);
        }
        
    }
// tempo do alerta para que ele siga o player
    IEnumerator ALERT()
    {
        yield return new WaitForSeconds(_GM.slimeAlertTime);

        if (isPlayerVisible == true)
        {
            changeState(enemyState.FOLLOW);
        }
        else
        {
            stayStill(10);
        }
    }
// para ter um tempo de ataque pra ataque 
    IEnumerator ATTACK()
    {
        yield return new WaitForSeconds(_GM.slimeAttackDelay);
        isAttack = false;
    }
 // estado randomico em  idle e patrol
    void stayStill(int yes)
    {
        if (Randomic() < yes)
        {
            changeState(enemyState.IDLE);
        }
        else
        {
            changeState(enemyState.PATROL);
        }
    }
  // radomico
    int Randomic()
    {
        int randomic = Random.Range(0, 100);
        return randomic;
    }
// metodo de atack
    void Attack()
    {
        if (!isAttack && isPlayerVisible == true) // se ataque for verdadeiro e se player estiver na visao  dispara ataque e a animação
        {
            isAttack = true;
            anima.SetTrigger("Attack");
        }

        StartCoroutine("ATTACK");

    }
// metodo para que o npc olhe para o player e o persige
    void LookAt()
    {
        Vector3 lookDir = (_GM.player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(lookDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, _GM.slimeLookAtSpeed * Time.deltaTime);
    }

    #endregion
}
