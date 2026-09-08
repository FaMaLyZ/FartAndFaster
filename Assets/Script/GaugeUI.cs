using UnityEngine;
using UnityEngine.UI;

public class GaugeUI : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Image gaugeFillImage; // ลาก Image ที่ตั้งค่าเป็น Filled มาใส่ตรงนี้
    [SerializeField] private bool smoothTransition = true;
    [SerializeField] private float smoothSpeed = 10f;

    private void Update()
    {
        if (gameManager == null || gaugeFillImage == null) return;

        // คำนวณค่า Fill Rate ระหว่าง 0.0 ถึง 1.0
        float targetFill = Mathf.Clamp01(gameManager.gaugeNow / gameManager.maxGauge);

        if (smoothTransition)
        {
            // ทำให้อนิเมชันการขึ้นลงของหลอดนุ่มนวลขึ้น
            gaugeFillImage.fillAmount = Mathf.Lerp(gaugeFillImage.fillAmount, targetFill, Time.deltaTime * smoothSpeed);
        }
        else
        {
            // อัปเดตทันทีแบบไม่หน่วง
            gaugeFillImage.fillAmount = targetFill;
        }
    }
}
