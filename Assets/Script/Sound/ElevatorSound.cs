using System.Collections;
using UnityEngine;

public class ElevatorSound : MonoBehaviour
{
    public static ElevatorSound Instance { get; private set; }

    public AudioSource audioSource;
    public AudioClip startSound;    
    public AudioClip ongoingSound;   

    private float delayTime = 2f;

    public float startVolume = 0.5f;
    public float ongoingVolume = 0.3f;

    void Start()
    {
        StartCoroutine(PlaySoundsSequence());
    }

    private void Awake()
    {
        // กำหนดให้ตัวเองเป็น Instance ไว้เรียกใช้
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    IEnumerator PlaySoundsSequence()
    {
        audioSource.volume = startVolume;
        audioSource.clip = startSound;
        audioSource.Play();

        yield return new WaitForSeconds(delayTime);

        audioSource.loop = true;
        audioSource.volume = ongoingVolume;
        audioSource.clip = ongoingSound;
        audioSource.Play();
    }

    public void StopOngoingSound()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.loop = false; 
        }
    }
}
