using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    private CharacterController controller;
    private Animator anima;

    [Header("Config")]
    public int HP;

    public float pVel = 3f;

    private Vector3 direction;
    private bool isWalking;

    //Input
    private float hor;
    private float ver;

    [Header("Attack Config")]
    public ParticleSystem fxAttack;
    public Transform hitBox;
    [Range(0.2f, 2f)]
    public float hitRange = 0.5f;
    public LayerMask hitMask;
    private bool isAttack;
    public Collider[] hitInfo;
    public int amountDmg;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        anima = GetComponent<Animator>();
    }

    private void Update()
    {
        Inputs();
        PlayerMoves();

        UpdateAnimator();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "TakeDamage")
        {
            GetHit(1);
        }
    }

    #region MEUS MÉTODOS

    void Inputs()
    {
        hor = Input.GetAxis("Horizontal");
        ver = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Fire1") && !isAttack)
        {
            Attack();
        }
    }

    void Attack()
    {
         isAttack = true;
         anima.SetTrigger("Attack");
         fxAttack.Emit(1);

        hitInfo = Physics.OverlapSphere(hitBox.position, hitRange, hitMask);
             foreach(Collider c in hitInfo)
             {
                  c.gameObject.SendMessage("GetHit", amountDmg);
            print(c);
             }


        }

    void PlayerMoves()
    {
        direction = new Vector3(hor, 0, ver).normalized;

        if (direction.magnitude > 0.1f)
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

        controller.Move(direction * pVel * Time.deltaTime);
    }

    void UpdateAnimator()
    {
        anima.SetBool("isWalking", isWalking);
    }

   void AttackIsDone()
    {
        isAttack = false;
    }

    void GetHit(int amount)
    {
        HP -= amount;
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

    private void OnDrawGizmosSelected()
    {
        if(hitBox != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(hitBox.position, hitRange);
        }
    }

}
