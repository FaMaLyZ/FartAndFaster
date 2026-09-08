using UnityEngine;

public class PressAnyKey : MonoBehaviour
{
    public SceneLoader sceneLoader;

    private bool hasPressed = false;

    void Update()
    {
        if (hasPressed) return;

        if (Input.anyKeyDown)
        {
            hasPressed = true;
            sceneLoader.LoadMainGameplay();
        }
    }
}