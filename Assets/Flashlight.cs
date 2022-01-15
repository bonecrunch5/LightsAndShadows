using UnityEngine;

public class Flashlight : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private Light spotlight;
    private float defaultIntensity;
    [SerializeField]
    private float generatedEnergy = 0.4f;
    private float maxLightEnergy = 2.5f;
    private float lightEnergy;

    [SerializeField]
    private float lightOnMaxSeconds = 30f;
    [SerializeField]
    private float lightOnMinSeconds = 15f;
    private float lightOnLimitSeconds = -1;
    private float lightOnCurrentSeconds = 0;

    [SerializeField]
    private float shakeDetectionThreshold = 2.5f;
    private float lowPassFilterFactor = 1.0f / 60.0f;
    private Vector3 lowPassValue;

    private void Start()
    {
        defaultIntensity = spotlight.intensity;
        lightEnergy = maxLightEnergy;

        // Calculate square beforehand
        shakeDetectionThreshold *= shakeDetectionThreshold;
        lowPassValue = Input.acceleration;
    }

    // Update is called once per frame
    void Update()
    {
        if (lightOnLimitSeconds == -1)
        {
            lightOnLimitSeconds = Random.Range(lightOnMinSeconds, lightOnMaxSeconds);
            lightOnCurrentSeconds = 0;
        }

        lightOnCurrentSeconds += Time.deltaTime;

        if (lightOnCurrentSeconds >= lightOnLimitSeconds)
        {
            lightEnergy = Mathf.Max(lightEnergy - Time.deltaTime, 0);

            Vector3 acceleration = Input.acceleration;
            lowPassValue = Vector3.Lerp(lowPassValue, acceleration, lowPassFilterFactor);
            Vector3 deltaAcceleration = acceleration - lowPassValue;

            if (deltaAcceleration.sqrMagnitude >= shakeDetectionThreshold)
            {
                animator.SetTrigger("Shake");
                lightEnergy += generatedEnergy;
            }

            ChangeLightIntensity(maxLightEnergy - lightEnergy);

            if (lightEnergy >= maxLightEnergy)
            {
                lightEnergy = maxLightEnergy;
                spotlight.intensity = defaultIntensity;
                lightOnLimitSeconds = -1;
            }
        }
    }

    // Values from 0 to 2.5
    private void ChangeLightIntensity(float input)
    {
        float x = Mathf.Min(Mathf.Max(input, 0), 2.5f);

        float intensityValue = defaultIntensity - x / 5 + Mathf.Cos(6 * x) / (7 * x);

        Debug.Log("Light Function Values\nInput = " + input + "\nIntensity = " + intensityValue);

        spotlight.intensity = Mathf.Min(Mathf.Max(intensityValue, 0), defaultIntensity);
    }
}
