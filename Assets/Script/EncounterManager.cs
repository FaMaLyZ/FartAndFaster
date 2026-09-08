using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public enum EncounterType
{
    //ถ้าอยากได้ encounter เพิ่มก็ง่ายๆละเพิ่มประเภทตรงนี้และไปปรับที่ encounter event แล้วก็เพิ่มโค้ดว่าตอนเกิดกับตอนออกทำอะไร 
    //แต่ตอนนี้ยัง hard code อยู่ 
    LightOff,      
    SomeoneComeIn, 
    Distraction,      // เสียงหรือสิ่งรบกวน
}

[System.Serializable]
public class EncounterEvent
{
    public string eventName = "New Encounter";
    [Tooltip("เกิดที่วิที่เท่าไหร่")]
    public float triggerTime = 5f;
    public EncounterType encounterType;
    [Tooltip("ค่าที่ใช้ปรับ AddGaugeInstantly / ModifyIncreaseRate")]
    public float value = 2f;
    [Tooltip("เกิดกี่วิ")]
    public float duration = 3f;
    public bool isTriggered = false;
}

public class EncounterManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    [Header("Encounter Settings")]
    public List<EncounterEvent> encounters = new List<EncounterEvent>();
    public List<GameObject> someOne = new List<GameObject>();
    [SerializeField]private GameObject currentSomeOne;

    private void Awake()
    {
        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<GameManager>();
        }
    }

    private void Start()
    {
        // รีเซ็ตสถานะทุก Event ให้พร้อมทำงาน
        for (int i = 0; i < encounters.Count; i++)
        {
            encounters[i].isTriggered = false;
        }
    }

    private void Update()
    {
        if (gameManager == null || !gameManager.IsGameActive)
        {
            return;
        }
        CheckEncounters(gameManager.CurrentTimer);
    }

    private void CheckEncounters(float currentTime)
    {
        for (int i = 0; i < encounters.Count; i++)
        {
            EncounterEvent e = encounters[i];

            if (!e.isTriggered && currentTime >= e.triggerTime)
            {
                e.isTriggered = true;
                ExecuteEncounter(e);
            }
        }
    }
    private void ExecuteEncounter(EncounterEvent encounter)
    {
        Debug.Log($"<color=orange>[Encounter Triggered!]</color> เวลา {gameManager.CurrentTimer:F1}s: {encounter.eventName}");

        switch (encounter.encounterType)
        {
            case EncounterType.SomeoneComeIn:
                Debug.Log($"Someone come in Encounter");
                // มีคนเข้ามาในลิฟต์
                SelectSomeOneActive();

                if (currentSomeOne != null)
                {
                    AudioManager.Instance.PlayMonsterSound(currentSomeOne);
                }

                StartCoroutine(AddGaugeInstantRoutine(encounter.value, encounter.duration));
                break;

            case EncounterType.LightOff:
                Debug.Log($"LightOff Encounter");
                // ไฟดับทำอะไรบ้าง
                StartCoroutine(TemporaryBoostRoutine(encounter.value, encounter.duration));
                break;

            case EncounterType.Distraction:
                Debug.Log($"Distraction Encounter");
                // อยากใส่ encounter อะไรปั่นที่ไม่มีผลต่อ gauge ก็ตรงนี้
                break;
        }
    }

    private IEnumerator TemporaryBoostRoutine(float extraIncrease, float duration)
    {
        gameManager.ModifyIncreaseRate(extraIncrease);
        Debug.Log($"Temporary Boost Routine Start");
        yield return new WaitForSeconds(duration);
        gameManager.ModifyIncreaseRate(-extraIncrease);
        // เวลาออกทำอะไรบ้างตรงนี้ 
        Debug.Log($"Temporary Boost Routine End");
    }
    private IEnumerator AddGaugeInstantRoutine(float crampAmount, float duration)
    {
        gameManager.StartEncounter();
        AnimationController.Instance.PlayOpenAnimation();

        AudioManager.Instance.PlayElevatorDing();

        if (currentSomeOne != null && currentSomeOne.name.StartsWith("TopHatHolder"))
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PausePlayerStateSound();
            }
        }

        Debug.Log($"Add Gauge Instant Routine Start");
        gameManager.AddGaugeInstantly(crampAmount);
        yield return new WaitForSeconds(duration);
        AnimationController.Instance.PlayCloseAnimation();

        yield return new WaitForSeconds(0.5f);
        gameManager.EndEncounter();

        if (currentSomeOne != null && currentSomeOne.name.StartsWith("TopHatHolder"))
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.ResumePlayerStateSound();
            }
        }

        if (!gameManager.IsGameActive)
        {
            yield break;
        }
        // เวลาออกทำอะไรบ้างตรงนี้ 
        Debug.Log($"Add Gauge Instant Routine End");
    }
    void SelectSomeOneActive()
    {
        if (someOne == null)
        {
            return;
        }
        else
        {
            int someOneSelected = Random.Range(0,someOne.Count);
            currentSomeOne = someOne[someOneSelected];
            currentSomeOne.SetActive(true);
            someOne.RemoveAt(someOneSelected);
        }
    }
}