using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
public class GameManager : MonoBehaviour
{
    public PlayerStateOverlay playerOverlay;
    public SpaceLevelAnim spaceLevelAnim;

    [Header("Game Settings")]
    [Tooltip("ค่าอั้นตดสูงสุด")]
    public float maxGauge = 10;
    public float gaugeNow = 0;
    public float startGaugeIncrease = 1;
    public float gaugeIncreaseAdjust = 0.2f;
    public float maxGaugeIncrease = 3;
    public float gaugeDecreaseAmount = 1;

    [Header("Win Time Setting")]
    public float minWinTime = 20f;
    public float maxWinTime = 40f;
    [SerializeField] private float winTime = 30;

    [Header("Threshold Settings")]
    [Tooltip("ใส่เปอร์เซ็นต์ Trigger Effect เรียงจากน้อยไปมาก")]
    public List<float> effectPercentages = new List<float>() { 20f, 40f, 60f, 80f };

    [Header("Debug / Monitoring")]
    [SerializeField] private float timer = 0f;
    [SerializeField] private bool gameActive = true;
    [SerializeField] private float gaugeIncreaseNow;
    [SerializeField] private int currentLevel = 0;

    [Header("Encounter State")]
    [SerializeField] private bool isEncounterActive = false;

    public TMP_Text liftFloor;

    public float CurrentTimer => timer;
    public bool IsGameActive => gameActive;
    [SerializeField]GameObject gaugeUI;

    private List<float> calculatedThresholds = new List<float>();

    private void Start()
    {
        winTime = Random.Range(minWinTime, maxWinTime);
        gaugeIncreaseNow = startGaugeIncrease;
        CalculateThresholds();
        StartCoroutine(GaugeIncrease());
        StartCoroutine(GaugeIncreaseAdjust());
        spaceLevelAnim.SetSpaceLevel(currentLevel);
        gaugeUI.SetActive(true);
    }

    [Header("Player Sound Control")]
    private int currentSoundLevel = -1;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ReleaseGauge();
        }
        if (!gameActive || isEncounterActive ) 
        {
            return;
        }

        timer += Time.deltaTime;
        if (timer >= winTime)
        {
            PlayerWin();
            return;
        }

        if (gaugeNow > maxGauge)
        {
            PlayerLose();
            return;
        }
        //show text on lift panel
        liftFloor.text = $"{(int)timer}";

        if (gameActive && maxGauge > 0)
        {
            float currentPercentage = (gaugeNow / maxGauge) * 100f;
            int targetLevel = 0;

            // ตรวจจับ 3 ช่วงตัวเลขแบบเด็ดขาด 
            if (currentPercentage >= 1f && currentPercentage < 30f) targetLevel = 1;
            else if (currentPercentage >= 31f && currentPercentage < 70f) targetLevel = 2;
            else if (currentPercentage >= 71f) targetLevel = 3;
            else targetLevel = 0; 

            if (targetLevel != currentSoundLevel)
            {
                currentSoundLevel = targetLevel;
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayPlayerStateSound(currentSoundLevel);
                }
            }
        }
    }

    private void ReleaseGauge()
    {
        gaugeNow = Mathf.Max(0f, gaugeNow - gaugeDecreaseAmount);
        CheckGaugeLevel();
        Debug.Log($"[Release] Gauge Now: {gaugeNow}");
    }
    private void CheckGaugeLevel()
    {
        int newLevel = 0;

        // วนลูปเทียบค่าจากระดับสูงสุดลงมาต่ำสุด
        for (int i = calculatedThresholds.Count - 1; i >= 0; i--)
        {
            if (gaugeNow >= calculatedThresholds[i])
            {
                newLevel = i + 1;
                break;
            }
        }

        if (newLevel != currentLevel)
        {
            currentLevel = newLevel;
            TriggerGaugeEffect(currentLevel);
        }
    }
    private void TriggerGaugeEffect(int level)
    {
        spaceLevelAnim.SetSpaceLevel(level);

        switch (level)
        {
            //ใส่ effect หน้าแดงตาม Percent ของ Gauge ตรงนี้ 
            case 1: 
                Debug.Log($"[Level 1: {effectPercentages[0]}%] หน้าเริ่มแดง"); 
                playerOverlay.BackToBlinkAnimation();
                break;
            case 2: 
                Debug.Log($"[Level 2: {effectPercentages[1]}%] ตัวเริ่มสั่น"); 
                playerOverlay.BackToBlinkAnimation();
                break;
            case 3: 
                Debug.Log($"[Level 3: {effectPercentages[2]}%] เสียงหัวใจเต้นเร็ว"); 
                playerOverlay.BackToBlinkAnimation();
                break;
            case 4:
                playerOverlay.SetRed(true);
                Debug.Log($"[Level 4: {effectPercentages[3]}%] จอกะพริบแดงวิกฤต!"); break;
            case 0: 
                Debug.Log("[Normal] สภาวะปกติ"); 
                playerOverlay.BackToBlinkAnimation();
                break;
            default: Debug.Log($"[Level {level}] ทำงาน!");
                break;
        }
    }
    IEnumerator GaugeIncrease()
    {
        print("start GaugeIncrease");
        while (gameActive)
        {
            
            yield return new WaitForSeconds(1f);
            
            if (!gameActive) yield break;
            gaugeNow += gaugeIncreaseNow;
            CheckGaugeLevel();
            Debug.Log($"Gauge Now: {gaugeNow} | Time: {timer}s");
            
        }
    }
    IEnumerator GaugeIncreaseAdjust()
    {
        while (gameActive && gaugeIncreaseNow < maxGaugeIncrease)
        {
            yield return new WaitWhile(() => isEncounterActive);
            yield return new WaitForSeconds(1f);
            yield return new WaitWhile(() => isEncounterActive);
            if (!gameActive) yield break;
            gaugeIncreaseNow = Mathf.Min(gaugeIncreaseNow + gaugeIncreaseAdjust, maxGaugeIncrease);
            Debug.Log($"Gauge Increase Rate: {gaugeIncreaseNow} | Time: {timer}s");
        }
    }
    public void PlayerWin()
    {
        gameActive = false;
        StopAllCoroutines();
        // ให้ใส่ effect เวลาที่ player ชนะทั้งหมดตรงนี้

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopAllSounds();
            AudioManager.Instance.PlayWinSound();
        }
        gaugeUI.SetActive(false);
        playerOverlay.TriggerWin();
        spaceLevelAnim.SetSpaceLevel(-1);
        AnimationController.Instance.PlayWinAnimation();
        Debug.Log("Player Win!");
    }
    public void PlayerLose()
    {
        gameActive = false;
        StopAllCoroutines();
        // ให้ใส่ effect เวลาที่ player แพ้ทั้งหมดตรงนี้

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopAllSounds();
            AudioManager.Instance.PlayLoseSound();
        }
        gaugeUI.SetActive(false);
        AnimationController.Instance.PlayCloseAnimation();
        playerOverlay.TriggerLose();
        spaceLevelAnim.SetSpaceLevel(-1);
        Debug.Log("Player Lose! ตดแตกเรียบร้อย");
    }
    private void CalculateThresholds()
    {
        calculatedThresholds.Clear();
        for (int i = 0; i < effectPercentages.Count; i++)
        {
            calculatedThresholds.Add(maxGauge * (effectPercentages[i] / 100f));
        }
    }
    public void AddGaugeInstantly(float amount)
    {
        gaugeNow = Mathf.Min(maxGauge, gaugeNow + amount);
        CheckGaugeLevel();
        Debug.Log($"Gauge instant increase +{amount} | Gauge now: {gaugeNow}");
    }
    public void ModifyIncreaseRate(float changeAmount)
    {
        gaugeIncreaseNow = Mathf.Max(0f, gaugeIncreaseNow + changeAmount);
        Debug.Log($"Gauge Increase Now: {gaugeIncreaseNow}");
    }
    public void StartEncounter()
    {
        isEncounterActive = true;
        Debug.Log($" isEncounterActive = {isEncounterActive}");
    }
    public void EndEncounter()
    {
        isEncounterActive = false;
        Debug.Log($" isEncounterActive = {isEncounterActive}");
    }
}