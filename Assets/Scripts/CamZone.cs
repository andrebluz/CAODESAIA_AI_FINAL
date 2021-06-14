using Cinemachine;
using UnityEngine;

public class CamZone : MonoBehaviour
{
  #region Inspector

  [SerializeField]
  private CinemachineVirtualCamera cam = default;

  #endregion


  #region MonoBehaviour

  private void Start ()
  {
    cam.gameObject.SetActive(false); // setando a camera como falsa
  }

  private void OnTriggerEnter (Collider other) 
  {
    if ( other.CompareTag("Player") )// se entra em colisão com player ativa o objeto camera
      cam.gameObject.SetActive(true);
  }

  private void OnTriggerExit (Collider other)// se sair da colisão com player desativa o objeto camera
  {
    if ( other.CompareTag("Player") )
      cam.gameObject.SetActive(false);
  }

  #endregion
}
