using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SlimeIA : MonoBehaviour
{
    private GameManager _GM;

    private Animator anima;
    public ParticleSystem fxBlood;
    public int HP;

    private bool isDie;


    public enemyState state;

    //IA
    private bool isWalk;
    private bool isAlert;
    private bool isAttack;
    private bool isPlayerVisible;
    private NavMeshAgent agent;
    private int idWaypoint;
    private Vector3 destination;

    private void Start()
    {
        _GM = FindObjectOfType(typeof(GameManager)) as GameManager;

        anima = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        changeState(state);
    }

    private void Update()
    {
        stateManager();

        if(agent.desiredVelocity.magnitude >= 0.1f)
        {
            isWalk = true;
        }
        else
        {
            isWalk = false;
        }
        anima.SetBool("isWalk", isWalk);
        anima.SetBool("isAlert", isAlert);
    }

    IEnumerator enemyDisapear()
    {
        isDie = true;
        yield return new WaitForSeconds(2.5f);
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player") //se player entrou no campo de visao
        {
            isPlayerVisible = true;
            if (state == enemyState.IDLE || state == enemyState.PATROL) //e se o state é IDLE ou PATROL fica em alerta
            {
                changeState(enemyState.ALERT);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isPlayerVisible = false;
        }
    }

    #region MEUS MÉTODOS
    void GetHit(int amount)
    {
        if (isDie)
        {
            return;
        }

        HP -= amount;
        if(HP > 0)
        {
            changeState(enemyState.FURY);
            anima.SetTrigger("GetHit");
            fxBlood.Emit(25);
        }
        else
        {
            anima.SetTrigger("Die");
            StartCoroutine("enemyDisapear");
        }
        
    }

    void stateManager()
    {
        switch (state)
        {
            case enemyState.IDLE:

                break;
            case enemyState.ALERT:

                LookAt();

                break;
            case enemyState.FOLLOW:

                LookAt();


                destination = _GM.player.position;
                agent.destination = destination;

                if(agent.remainingDistance <= agent.stoppingDistance)
                {
                    Attack();
                }

                break;
            case enemyState.FURY:

                LookAt();


                destination = _GM.player.position;
                agent.destination = destination;

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    Attack();
                }

                break;
            case enemyState.PATROL:

                break;
        }
    }

    void changeState(enemyState newState)
    {
        StopAllCoroutines();
        
        isAlert = false;

        switch (newState)
        {
            case enemyState.IDLE:
                agent.stoppingDistance = 0; //para objetos como player nao interferirem na chegada ao destino da patrulha
                destination = transform.position;
                agent.destination = destination;
                StartCoroutine("IDLE");
                break;
            case enemyState.ALERT:
                agent.stoppingDistance = 0; //para objetos como player nao interferirem na chegada ao destino da patrulha
                destination = transform.position;
                agent.destination = destination;
                isAlert = true;
                StartCoroutine("ALERT");
                break;
            case enemyState.PATROL:
                agent.stoppingDistance = 0; //para objetos como player nao interferirem na chegada ao destino da patrulha
                idWaypoint = Random.Range(0, _GM.slimeWayPoints.Length);
                destination = _GM.slimeWayPoints[idWaypoint].position;
                agent.destination = destination;

                StartCoroutine("PATROL");
                
                break;
            case enemyState.FOLLOW:

                destination = transform.position;
                agent.stoppingDistance = _GM.slimeDistanceToAttack;
                agent.destination = destination;
                StartCoroutine("FOLLOW");

                break;
            case enemyState.FURY:
                destination = transform.position;
                agent.stoppingDistance = _GM.slimeDistanceToAttack;
                agent.destination = destination;
                break;
            
        }
        state = newState;
    }

    IEnumerator IDLE()
    {
        yield return new WaitForSeconds(_GM.slimeIdleWaitTime);
        stayStill(50);
    }

    IEnumerator PATROL()
    {
        yield return new WaitUntil(() => agent.remainingDistance <= 0); //operador lambda ativa quando distancia remanescente do agent for menor ou igual a 0
        stayStill(30);
    }

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

    IEnumerator ATTACK()
    {
        yield return new WaitForSeconds(_GM.slimeAttackDelay);
        isAttack = false;
    }

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

    int Randomic()
    {
        int randomic = Random.Range(0, 100);
        return randomic;
    }

    void Attack()
    {
        if (!isAttack && isPlayerVisible == true)
        {
            isAttack = true;
            anima.SetTrigger("Attack");
        }

        StartCoroutine("ATTACK");

    }

    void LookAt()
    {
        Vector3 lookDir = (_GM.player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(lookDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, _GM.slimeLookAtSpeed * Time.deltaTime);
    }

    #endregion
}
