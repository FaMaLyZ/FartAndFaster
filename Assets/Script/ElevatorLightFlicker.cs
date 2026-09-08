using UnityEngine;
using System.Collections;

public class ElevatorLightFlicker : MonoBehaviour
{
    public Light[] lights;

    [Header("Flicker Settings")]
    public float minDelay = 0.05f;
    public float maxDelay = 0.2f;

    public float normalIntensity = 1f;
    public float flickerIntensity = 0.1f;

    private void Start()
    {
        StartCoroutine(Flicker());
    }

    IEnumerator Flicker()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));

            foreach (Light light in lights)
            {
                light.intensity = flickerIntensity;
            }

            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));

            foreach (Light light in lights)
            {
                light.intensity = normalIntensity;
            }
        }
    }
}