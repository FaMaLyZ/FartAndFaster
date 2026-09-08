using UnityEngine;

public class PressAnyKey : MonoBehaviour
{
    public SceneLoader sceneLoader;

    private bool hasPressed = false;

    void Update()
    {
        if (hasPressed) return;

        // กดคีย์บอร์ดเท่านั้น (ไม่รวม Mouse)
        if (Input.anyKeyDown &&
            !Input.GetMouseButtonDown(0) &&
            !Input.GetMouseButtonDown(1) &&
            !Input.GetMouseButtonDown(2))
        {
            hasPressed = true;
            sceneLoader.LoadMainGameplay();
        }
    }
}