using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Fundido : MonoBehaviour
{
    public Image fundido;
    public string[] escenas;

    void Start()
    {
        fundido.CrossFadeAlpha(0, 1, false);
    }

    public void FadeOut(int s)
    {
        fundido.CrossFadeAlpha(1, 1, false);
        StartCoroutine(CambioEscena(escenas[s]));
    }

    IEnumerator CambioEscena(string escena)
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(escena);
    }
}
