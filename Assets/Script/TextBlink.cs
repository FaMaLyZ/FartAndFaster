using UnityEngine;
using TMPro;

public class TextBlink : MonoBehaviour
{
    public float speed = 2f;
    public float minOpacity = 0.2f;
    public float maxOpacity = 1f;

    private TMP_Text text;
    private Color originalColor;

    void Start()
    {
        text = GetComponent<TMP_Text>();
        originalColor = text.color;
    }

    void Update()
    {
        float t = (Mathf.Sin(Time.time * speed) + 1f) / 2f;

        float alpha = Mathf.Lerp(minOpacity, maxOpacity, t);

        Color color = originalColor;
        color.a = alpha;

        text.color = color;
    }
}