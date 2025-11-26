using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PrototypeGameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int oro_inicial;
    public int horo;
    public TMP_Text horo_mostrar;
    public GameObject feedbackPrefab;
    public GameObject feedbackPlacement;
    public GameObject canvas;

    public bool isPlaying = true;
    public bool isOpen = true;
    public int dayCount;
    public float actualHour;
    public Light light;

    public bool aletargamiento = false;
    public bool costoso = false;
    public List<int> goldQueue;
    public float goldCooldown = 0;

    public int openTime = 180;
    public int closeTime = 30;

    public int gastosMesas = 0;
    public int gastosEmpleadosDiario = 0;
    public int deudaEmpleados = 0;
    public int perdidas = 0;

    public bool UIOpen = true;

    private bool bajarTexto = true;
    private float upTextHour = 0;

    public GameObject pauseMenu;
    public bool isPaused = false;

    private void Awake()
    {
        goldQueue = new List<int>();

        oro_inicial = 0;
        horo = 0;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isPaused = false;
        pauseMenu = FindObjectOfType<PauseMenu>(true).gameObject;
        if (light == null)
        {
            light = FindObjectOfType<Light>();
        }
    }

    private void Update()
    {

        if (goldCooldown > 0)
        {
            goldCooldown -= Time.deltaTime * 4f;
        }
        else
        {
            if (goldQueue.Count > 0)
            {
                AudioManager.Instance.PlaySound(goldQueue[0] > 0 ? "GanarDinero" : "GastarDinero");

                StartCoroutine(goldCoroutine(goldQueue[0]));
                goldQueue.RemoveAt(0);
                goldCooldown += 1;
            }

        }
        // PAUSE MENU
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenu)
            {
                isPaused = !isPaused;
                pauseMenu.SetActive(isPaused);
            }
        }

        // HACK TO OPEN/CLOSE THE SHOP
        if (Input.GetKeyDown(KeyCode.J) || actualHour > openTime || (!isOpen && actualHour > closeTime))
        {
            Open(!this.isOpen);
        }

        // HACK TO GET MONEY
        if (Input.GetKeyDown(KeyCode.C))
        {
            addGold(1000);
        }

        if (isPlaying)
        {
            actualHour += Time.deltaTime;
        }

        int hour = isOpen ? 6 + (int)(actualHour * 18 / openTime) : (int)(actualHour * 6 / closeTime);

        string hourString = hour < 10 ? "0" + hour.ToString() : hour.ToString();

        AudioManager.Instance.HandleAmbience(hour);
    }

    public void addGold(int amount)
    {
        goldQueue.Add(amount);
        horo += amount;
    }

    public void Open(bool open)
    {
        actualHour = 0;
        isOpen = open;
        if (isOpen)
        {
            bajarTexto = true;
            if (dayCount != 1)
            {
                float random = Random.Range(0f, 1f);
                Vector3 randomPosition = new Vector3(Random.Range(10, 20), 0, Random.Range(-30, 20));
                if (random < .1f)
                {
                    GlobalEnemiesManager.Instance.SpawnSkeleton(randomPosition);
                }
                else if (random < .2f)
                {
                    GlobalEnemiesManager.Instance.SpawnThug(randomPosition);
                }

            }
            if (dayCount % 4 == 3)
            {
                float dayRandom = Random.Range(0f, 1f);
                if (dayRandom < .33f)
                {
                    StartCoroutine(OpenCloseShop(new Color(1, 1, 0, 1)));
                    costoso = true;
                }
                else if (dayRandom < .66f)
                {
                    StartCoroutine(OpenCloseShop(new Color(1, 0.5f, 0.5f, 1)));
                    aletargamiento = true;
                }
                else
                {
                    StartCoroutine(OpenCloseShop(new Color(0.25f, 0.75f, 1f, 1)));
                    GlobalCustomerManager.Instance.maxCustomersInScene = 2;
                }
            }
            else
            {
                StartCoroutine(OpenCloseShop(new Color(1, 1, 1, 1)));
                aletargamiento = false;
                costoso = false;
                GlobalCustomerManager.Instance.maxCustomersInScene = 5;

            }
            GlobalCharactersManager.Instance.GenerateCandidates();
            dayCount += 1;
            oro_inicial = horo;
        }
        else
        {
            foreach (CustomerComponent customer in GlobalCustomerManager.Instance.customers)
            {
                if (customer.objectiveStation == null)
                {
                    customer.LeaveWithoutBuy();
                }
            }
            StartCoroutine(OpenCloseShop(new Color(0, 0, 1, 1), 5f, true));
        }
    }

    IEnumerator goldCoroutine(int amount)
    {
        Debug.Log("Entré al coroutine");
        GameObject instance = Instantiate(feedbackPrefab, feedbackPlacement.transform.position, Quaternion.identity, canvas.transform);
        instance.GetComponent<goldFeedback>().amount = amount;
        yield return new WaitForSeconds(2);
        horo_mostrar.text = horo.ToString() + "$";
        Debug.Log(horo);
    }

    public void removeGold(int amount)
    {
        if (horo - amount < 0)
        {
            amount = horo;
        }
        StartCoroutine(goldCoroutine(amount));
        horo -= amount;
    }

    private IEnumerator OpenCloseShop(Color newColor, float duration = 5, bool close = false)
    {
        Color startColor = light.color;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            light.color = Color.Lerp(startColor, newColor, elapsed / duration);
            yield return null;
        }

        light.color = newColor;
    }

}