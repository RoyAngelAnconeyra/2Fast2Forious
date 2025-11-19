using UnityEngine;

public class AudioFX : MonoBehaviour
{
    public AudioClip[] fxs;
    AudioSource audioS;

    void Start()
    {
        audioS = GetComponent<AudioSource>();
    }

    // 0 choque
    public void FXSonidoChoque()
    {
        audioS.clip = fxs[0];
        audioS.Play();
    }

    // 1 music game
    public void FXMusic()
    {
        audioS.clip = fxs[1];
        audioS.Play();
    }
    
}
