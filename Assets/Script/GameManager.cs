using UnityEngine;
using TMPro; // ลบทิ้งได้ถ้าไม่ได้ใช้ TextMeshPro

/// <summary>
/// Gameplay: ผู้เล่นต้องกด Spacebar ซ้ำๆ ให้ทันภายในช่วงเวลาที่กำหนด
/// ยิ่งเวลาผ่านไป ช่วงเวลาที่อนุญาตให้กดจะสั้นลงเรื่อยๆ (ยากขึ้น)
/// - ถ้ากดไม่ทัน (เว้นช่วงนานเกินไป) -> แพ้
/// - ถ้าเล่นรอดจนถึงเวลาที่กำหนด (winTime) -> ชนะ
/// </summary>
public class SpacebarSurvivalGame : MonoBehaviour
{
    [Header("Game Settings")]
    [Tooltip("เวลาทั้งหมดที่ต้องเล่นให้รอดถึงจะชนะ (วินาที)")]
    public float winTime = 60f;

    [Tooltip("ช่วงเวลาสูงสุดระหว่างการกดตอนเริ่มเกม (วินาที) - ค่ายิ่งมาก ยิ่งง่าย")]
    public float startAllowedInterval = 1.0f;

    [Tooltip("ช่วงเวลาสูงสุดที่กดได้ตอนยากที่สุด (วินาที) - ค่ายิ่งน้อย ยิ่งยาก")]
    public float minAllowedInterval = 0.15f;

    [Tooltip("ใช้เวลากี่วินาทีในการไต่ความยากจากง่ายสุดไปยากสุด")]
    public float difficultyRampDuration = 60f;

    [Header("Difficulty Curve (Optional)")]
    [Tooltip("ปรับเส้นโค้งความยาก ถ้าไม่ต้องการปรับ ปล่อย Linear ไว้ได้เลย")]
    public AnimationCurve difficultyCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [Header("UI (Optional - ลากมาใส่ใน Inspector หรือปล่อยว่างได้)")]
    public TMP_Text timerText;
    public TMP_Text statusText;
    public UnityEngine.UI.Slider difficultySlider; // แสดงเวลาที่เหลือก่อนตาย (ถ้ามี)

    [Header("Events (Optional)")]
    public UnityEngine.Events.UnityEvent onWin;
    public UnityEngine.Events.UnityEvent onLose;

    private float elapsedTime = 0f;
    private float timeSinceLastPress = 0f;
    private bool gameActive = true;

    void Start() 
    {
        Debug.Log($"start time : {timeSinceLastPress}");   
    }
    void Update()
    {
        if (!gameActive) return;

        elapsedTime += Time.deltaTime;
        timeSinceLastPress += Time.deltaTime;

        // คำนวณช่วงเวลาที่อนุญาตในปัจจุบัน (ลดลงเรื่อยๆ ตามเวลาที่ผ่านไป)
        float t = Mathf.Clamp01(elapsedTime / difficultyRampDuration);
        float curvedT = difficultyCurve.Evaluate(t);
        float currentAllowedInterval = Mathf.Lerp(startAllowedInterval, minAllowedInterval, curvedT);

        // รับ input
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log($"space press || time: {Time.time} || current allowed interval: {currentAllowedInterval}");
            timeSinceLastPress = 0f;
        }

        // เช็คแพ้: เว้นช่วงนานเกินไปโดยไม่ได้กด
        if (timeSinceLastPress > currentAllowedInterval)
        {  
            Debug.Log($"lose time: {Time.time}");
            Debug.Log($"time since last press: {timeSinceLastPress} current allowed interval :{currentAllowedInterval}");
            LoseGame();
            return;
        }

        // เช็คชนะ: เล่นรอดจนถึงเวลาที่กำหนด
        if (elapsedTime >= winTime)
        {
            WinGame();
            return;
        }

        UpdateUI(currentAllowedInterval);
    }

    void UpdateUI(float currentAllowedInterval)
    {
        if (timerText != null)
        {
            timerText.text = $"เวลา: {elapsedTime:F1} / {winTime:F1}\nต้องกดภายใน: {currentAllowedInterval:F2} วิ";
        }

        if (difficultySlider != null)
        {
            // แสดงว่าผู้เล่นเหลือเวลาก่อนตายกี่ % ของ interval ปัจจุบัน
            float remainingRatio = 1f - Mathf.Clamp01(timeSinceLastPress / currentAllowedInterval);
            difficultySlider.value = remainingRatio;
        }
    }

    void LoseGame()
    {
        gameActive = false;
        if (statusText != null) statusText.text = "YOU LOSE!";
        Debug.Log("Game Over - Player Lost");
        onLose?.Invoke();
        // TODO: แสดง UI แพ้ / เล่นเสียง / restart scene ตรงนี้
    }

    void WinGame()
    {
        gameActive = false;
        if (statusText != null) statusText.text = "YOU WIN!";
        Debug.Log("Game Over - Player Won");
        onWin?.Invoke();
        // TODO: แสดง UI ชนะ / เล่นเสียง / ไปด่านถัดไป ตรงนี้
    }

    /// <summary>
    /// เรียกฟังก์ชันนี้เพื่อเริ่มเกมใหม่ (เช่นผูกกับปุ่ม Restart)
    /// </summary>
    public void RestartGame()
    {
        elapsedTime = 0f;
        timeSinceLastPress = 0f;
        gameActive = true;
        if (statusText != null) statusText.text = "";
    }
}