using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cactusCut : MonoBehaviour
{
    public GameObject cutted;
    public ParticleSystem fxHit;
    private bool isCutted;
    void GetHit(int amount)
    {
        if (!isCutted)
        {
            //transform.localScale = new Vector3(1f, 1f, 1f);
            this.gameObject.GetComponent<MeshRenderer>().enabled = false;
            this.gameObject.GetComponent<Collider>().enabled = false;
            cutted.SetActive(true);
            fxHit.Emit(10);
            isCutted = true;
        }
    }
}
