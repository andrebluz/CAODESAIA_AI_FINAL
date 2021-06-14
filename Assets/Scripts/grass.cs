using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grass : MonoBehaviour
{
    public ParticleSystem fxHit;
    private bool isCutted;
    void GetHit(int amount) 
    {
        if (!isCutted)// se for verdadeiro o ataque do player no obejto, ele diminui  scale do objeto  e dispare um efeito
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
            fxHit.Emit(10);
            isCutted = true;
        }
    }
}
