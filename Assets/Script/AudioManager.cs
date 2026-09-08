using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource audioSource;
    private AudioSource playerStateSource;
    private AudioSource playerAdditionalSource;

    [Header("Elevator Settings")]
    public AudioClip elevatorStart;
    [Range(0f, 1f)] public float startVolume = 0.4f;
    public AudioClip elevatorOngoing;
    [Range(0f, 1f)] public float ongoingVolume = 0.2f;
    public AudioClip elevatorDing;
    [Range(0f, 1f)] public float dingVolume = 0.5f;

    [Header("State Settings")]
    public AudioClip state1Sound; // เสียงอั้นช่วงแรก (1-30)
    [Range(0f, 1f)] public float state1Volume = 0.5f;
    public AudioClip state2Sound; // เสียงอั้นช่วงแรก (1-30)
    [Range(0f, 1f)] public float state2Volume = 0.5f;
    public AudioClip state3Sound; // เสียงเข้าหน้าแดง (80+)
    [Range(0f, 1f)] public float state3Volume = 0.6f;

    [Header("Environment")]
    public AudioClip winSound;
    [Range(0f, 1f)] public float winVolume = 0.5f;
    public AudioClip loseSound;
    [Range(0f, 1f)] public float loseVolume = 0.6f;

    [Header("PlayerSFX")]
    [Tooltip("เสียงเอฟเฟกต์เล่นรอบเดียว ตอนขยับเข้า State 2")]
    public AudioClip state2TransitionSFX;
    [Range(0f, 1f)] public float state2SFXVolume = 0.6f;
    [Tooltip("เสียงเอฟเฟกต์เล่นรอบเดียว ตอนขยับเข้า State 3")]
    public AudioClip state3TransitionSFX;
    [Range(0f, 1f)] public float state3SFXVolume = 0.6f;

    private float delayTime = 2f;

    void Start()
    {
        StartCoroutine(PlaySoundsSequence());
    }

    private void Awake()
    {
        Instance = this;

        GameObject musicHolder = new GameObject("PlayerStateMusicHolder");
        musicHolder.transform.SetParent(this.transform);
        playerStateSource = musicHolder.AddComponent<AudioSource>();
        playerStateSource.playOnAwake = false;

        GameObject additionalHolder = new GameObject("PlayerAdditionalSFXHolder");
        additionalHolder.transform.SetParent(this.transform);
        playerAdditionalSource = additionalHolder.AddComponent<AudioSource>();
        playerAdditionalSource.playOnAwake = false;
    }

    IEnumerator PlaySoundsSequence()
    {
        audioSource.volume = startVolume;
        audioSource.clip = elevatorStart;
        audioSource.Play();

        yield return new WaitForSeconds(delayTime);

        audioSource.loop = true;
        audioSource.volume = ongoingVolume;
        audioSource.clip = elevatorOngoing;
        audioSource.Play();
    }
    public void elevatorStop()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.loop = false;
        }
    }

    [System.Serializable]
    public class MonsterSoundGroup
    {
        [Tooltip("ลาก Prefab ของมอนสเตอร์ตัวนั้นๆ มาใส่ช่องนี้ได้เลย")]
        public GameObject monsterPrefab;
        public AudioClip[] clips;
        public bool isLooping;

        [Range(0f, 1f)]
        public float monsterVolume = 0.5f;
    }

    [Header("Monster Settings")]
    public List<MonsterSoundGroup> monsterSoundsList;

    public void PlayElevatorDing()
    {
        if (audioSource != null && elevatorDing != null)
        {
            audioSource.PlayOneShot(elevatorDing);
        }
    }
    public void PlayWinSound()
    {
        if (audioSource != null && winSound != null)
        {
            audioSource.PlayOneShot(winSound, winVolume);
        }
    }
    public void PlayLoseSound()
    {
        if (audioSource != null && loseSound != null)
        {
            audioSource.PlayOneShot(loseSound, loseVolume);
        }
    }
    public void PlayMonsterSound(GameObject activeMonster)
    {
        foreach (var group in monsterSoundsList)
        {
            // ตรวจสอบว่ามอนสเตอร์ตัวนี้เกิดมาจาก Prefab ตัวเดียวกันไหม
            if (activeMonster.name.StartsWith(group.monsterPrefab.name))
            {
                int randomIndex = Random.Range(0, group.clips.Length);
                AudioClip selectedClip = group.clips[randomIndex];

                if (group.isLooping)
                {
                    AudioSource customSource = activeMonster.AddComponent<AudioSource>();

                    customSource.volume = group.monsterVolume;
                    customSource.clip = selectedClip;
                    customSource.loop = true;
                    customSource.playOnAwake = false;

                    customSource.Play();
                }
                else
                {
                    audioSource.PlayOneShot(selectedClip, group.monsterVolume);
                }
                return;
            }
        }
    }

    public void StopAllMonsterLoopSounds()
    {
        // ปิดเสียงหลัก (เสียงลิฟต์วิ่ง) ทันที
        if (audioSource != null)
        {
            audioSource.Stop();
        }

        AudioSource[] allAudioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);

        foreach (AudioSource source in allAudioSources)
        {
            // ตรวจเช็กว่าไม่ใช่ลำโพงหลักของ AudioManager และเป็นลำโพงที่กำลังเปิดวนลูปอยู่
            if (source != audioSource && source.loop)
            {
                source.Stop();
                Destroy(source);
            }
        }
    }
    public void PlayPlayerStateSound(int level)
    {
        if (playerStateSource == null || playerAdditionalSource == null) return;

        AudioClip nextMusicClip = null;
        float nextMusicVolume = 0.5f;

        AudioClip nextAdditionalClip = null;
        float nextAdditionalVolume = 0.5f;

        if (level == 1 || level == 0)
        {
            nextMusicClip = state1Sound; nextMusicVolume = state1Volume;
        }
        else if (level == 2)
        {
            nextMusicClip = state2Sound; nextMusicVolume = state2Volume;
            nextAdditionalClip = state2TransitionSFX; nextAdditionalVolume = state2SFXVolume;
        }
        else if (level == 3)
        {
            nextMusicClip = state3Sound; nextMusicVolume = state3Volume;
            nextAdditionalClip = state3TransitionSFX; nextAdditionalVolume = state3SFXVolume;
        }

        if (nextMusicClip != null)
        {
            if (playerStateSource.clip != nextMusicClip)
            {
                playerStateSource.Stop();
                playerStateSource.clip = nextMusicClip;
                playerStateSource.volume = nextMusicVolume;
                playerStateSource.loop = true; 
                playerStateSource.Play();
            }
        }
        else
        {
            playerStateSource.Stop();
            playerStateSource.clip = null;
        }

        if (nextAdditionalClip != null)
        {
            if (playerAdditionalSource.clip != nextAdditionalClip)
            {
                playerAdditionalSource.Stop();
                playerAdditionalSource.clip = nextAdditionalClip;
                playerAdditionalSource.volume = nextAdditionalVolume;
                playerAdditionalSource.loop = true;
                playerAdditionalSource.Play();
            }
        }
        else
        {
            playerAdditionalSource.Stop();
            playerAdditionalSource.clip = null;
        }
    }
    public void PausePlayerStateSound()
    {
        if (playerStateSource != null && playerStateSource.isPlaying)
        {
            playerStateSource.Pause();
        }
    }
    public void ResumePlayerStateSound()
    {
        if (playerStateSource != null)
        {
            playerStateSource.UnPause();
        }
    }

    public void StopAllSounds()
    {
        if (audioSource != null) audioSource.Stop();
        if (playerStateSource != null) playerStateSource.Stop();
        if (playerAdditionalSource != null) playerAdditionalSource.Stop(); // ดับเสียงอาการเสริมลูปด้วย

        AudioSource[] allAudioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (AudioSource source in allAudioSources)
        {
            if (source != audioSource && source != playerStateSource && source != playerAdditionalSource && source.loop)
            {
                source.Stop();
                Destroy(source);
            }
        }
    }
}
