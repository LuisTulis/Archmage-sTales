using System.Collections;
using UnityEngine;


[RequireComponent(typeof(Animator))]
public class WorkstationFX : MonoBehaviour
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

    [Header("Upgrade sparkles")]
    public ParticleSystem mainUpgradeSparkles;

    [Header("Level 2 upgrade")]
    public GameObject levelTwoObject;
    public ParticleSystem levelTwoSparkles;

    [Header("Level 3 upgrade")]
    public GameObject levelThreeObject;
    public ParticleSystem levelThreeSparkles;

    [Header("Animator")]
    public Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();

        if (fireLight) fireLight.enabled = false;
        if (steam) steam.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        perlinSeed = Random.value * 10f;
    }

    public void Update()
    {
        SetOn(isWorking);

        if (isWorking && fireLight)
        {
            float n = Mathf.PerlinNoise(Time.time * flickerSpeed, perlinSeed);
            float t = (n - 0.5f) * 2f;
            fireLight.intensity = baseIntensity + t * intensityAmp;
            fireLight.range = baseRange + t * rangeAmp;
        }
    }
    public void SetWorking(bool on)
    {
        if (isWorking == on) return;
        SetOn(on);
    }

    public void SetOn(bool on)
    {
        if (anim)
        {
            isWorking = on;
            anim.SetBool("isOn", on);
        }

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

    private IEnumerator PlayOnceAndStop(ParticleSystem ps)
    {
        if (ps == null) yield break;

        bool wasActive = ps.gameObject.activeSelf;
        if (!wasActive) ps.gameObject.SetActive(true);

        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        yield return null;
        ps.Play(true);

        var main = ps.main;
        float lifetimeMax = main.startLifetime.constantMax;
        float wait = Mathf.Max(0.1f, main.duration + lifetimeMax + 0.05f);

        yield return new WaitForSeconds(wait);

        ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        if (!wasActive) ps.gameObject.SetActive(false);
    }

    public void ApplyUpgradeLevel(int level)
    {
        Debug.Log("Playando main particles: " + mainUpgradeSparkles != null);
        if (mainUpgradeSparkles != null) StartCoroutine(PlayOnceAndStop(mainUpgradeSparkles));

        if (level >= 2 && levelTwoObject != null)
        {
            if (!levelTwoObject.activeSelf) levelTwoObject.SetActive(true);
            if (levelTwoSparkles != null) StartCoroutine(PlayOnceAndStop(levelTwoSparkles));
        }

        if (level >= 3 && levelThreeObject != null)
        {
            if (!levelThreeObject.activeSelf) levelThreeObject.SetActive(true);
            if (levelThreeSparkles != null) StartCoroutine(PlayOnceAndStop(levelThreeSparkles));
        }
    }

}
