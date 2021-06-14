using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grass : MonoBehaviour
{
    public ParticleSystem fxHit;
    private bool isCutted;
    void GetHit(int amount)
    {
        if (!isCutted)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
            fxHit.Emit(10);
            isCutted = true;
        }
    }
}
