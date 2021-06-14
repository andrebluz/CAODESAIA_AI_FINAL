using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class RELOAD : MonoBehaviour
{

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // carregando cena destinta 
    }

}
