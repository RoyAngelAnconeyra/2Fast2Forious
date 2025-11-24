using UnityEngine;
using UnityEngine.SceneManagement;

public class Botones : MonoBehaviour
{

    public void CambiarEscena(int indiceEscena)
    {
        SceneManager.LoadScene(indiceEscena);

    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
