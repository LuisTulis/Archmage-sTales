using UnityEngine;


[RequireComponent(typeof(Animator))]
public class AlchemyTableFX : MonoBehaviour
{
    [Header("Control")]
    public bool isWorking;

    [Header("FX")]
    public Light fireLight;
    public ParticleSystem steam;

    [Header("Fire flicker")]
    public float baseIntensity = 3f;
    public float intensityAmp = 0.8f;
    public float baseRange = 1.4f;
    public float rangeAmp = 0.2f;
    public float flickerSpeed = 3f;
    float perlinSeed;

    [Header("Animator")]
    public Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();

        if (!fireLight) fireLight = GetComponentInChildren<Light>(true);
        if (!steam) steam = GetComponentInChildren<ParticleSystem>(true);

        // Initial state: off
        if (fireLight) fireLight.enabled = false;
        if (steam) steam.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        perlinSeed = Random.value * 10f;
    }

    public void Update()
    {
        SetOn(isWorking);

        if (isWorking && fireLight)
        {
            float n = Mathf.PerlinNoise(Time.time * flickerSpeed, perlinSeed); // 0..1
            float t = (n - 0.5f) * 2f;                                         // -1..1
            fireLight.intensity = baseIntensity + t * intensityAmp;
            fireLight.range = baseRange + t * rangeAmp;
        }
    }

    public void SetOn(bool on)
    {
        anim.SetBool("isOn", on);

        if (fireLight)
        {
            fireLight.enabled = on;
        }

        if (steam)
        {
            if (on) steam.Play(true);
            else steam.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }

}
