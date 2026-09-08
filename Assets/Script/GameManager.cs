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

    private List<float> calculatedThresholds = new List<float>();

    private void Start()
    {
        winTime = Random.Range(minWinTime, maxWinTime);
        gaugeIncreaseNow = startGaugeIncrease;
        CalculateThresholds();
        StartCoroutine(GaugeIncrease());
        StartCoroutine(GaugeIncreaseAdjust());
    }

    private void Update()
    {
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ReleaseGauge();
        }

        if (gaugeNow > maxGauge)
        {
            PlayerLose();
            return;
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
            case 1: Debug.Log($"[Level 1: {effectPercentages[0]}%] หน้าเริ่มแดง"); break;
            case 2: Debug.Log($"[Level 2: {effectPercentages[1]}%] ตัวเริ่มสั่น"); break;
            case 3: Debug.Log($"[Level 3: {effectPercentages[2]}%] เสียงหัวใจเต้นเร็ว"); break;
            case 4:
                playerOverlay.SetRed(true);
                Debug.Log($"[Level 4: {effectPercentages[3]}%] จอกะพริบแดงวิกฤต!"); break;
            case 0: Debug.Log("[Normal] สภาวะปกติ"); break;
            default: Debug.Log($"[Level {level}] ทำงาน!"); break;
        }
    }
    IEnumerator GaugeIncrease()
    {
        print("start GaugeIncrease");
        while (gameActive)
        {
            liftFloor.text = $"{(int)timer}";
            yield return new WaitWhile(() => isEncounterActive);
            yield return new WaitForSeconds(1f);
            yield return new WaitWhile(() => isEncounterActive);
            if (!gameActive) yield break;
            gaugeNow += gaugeIncreaseNow;
            CheckGaugeLevel();
            Debug.Log($"Gauge Now: {gaugeNow} | Time: {Time.time:F1}s");
            
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
            Debug.Log($"Gauge Increase Rate: {gaugeIncreaseNow} | Time: {Time.time:F1}s");
        }
    }
    public void PlayerWin()
    {
        gameActive = false;
        StopAllCoroutines();
        // ให้ใส่ effect เวลาที่ player ชนะทั้งหมดตรงนี้

        ElevatorSound.Instance.StopOngoingSound(); //ฟิลเพิ่ม

        playerOverlay.TriggerWin();
        spaceLevelAnim.SetSpaceLevel(0);
        AnimationController.Instance.PlayWinAnimation();
        Debug.Log("Player Win!");
    }
    public void PlayerLose()
    {
        gameActive = false;
        StopAllCoroutines();
        // ให้ใส่ effect เวลาที่ player แพ้ทั้งหมดตรงนี้

        ElevatorSound.Instance.StopOngoingSound(); 

        AnimationController.Instance.PlayCloseAnimation();
        playerOverlay.TriggerLose();
        spaceLevelAnim.SetSpaceLevel(0);
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