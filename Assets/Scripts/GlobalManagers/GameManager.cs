using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public ParticleSystem rain;

    public int oro_inicial;
    public int horo;
    public TMP_Text horo_mostrar;
    public TMP_Text dia_mostrar;
    public TMP_Text tipo_mostrar;
    public GameObject feedbackPrefab;
    public GameObject feedbackPlacement;
    public GameObject canvas;
    public GameObject dailyStatistics;

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
    private bool addDebt = false;
    public TMP_Text mostrar_ganancia;
    public TMP_Text mostrar_perdida;
    public TMP_Text mostrar_mesa;
    public TMP_Text mostrar_salario;
    public TMP_Text mostrar_deuda;
    public TMP_Text mostrar_total;
    public TMP_Text mostrar_ahorro;
    public Toggle toggle_salario;
    public Toggle toggle_deuda;
    public Toggle toggle_impuesto;
    public TMP_Text mostrar_impuesto;

    public GameObject showHour;
    public TMP_Text showCloseTime;

    public bool UIOpen = true;

    private GameObject grupoTextoDia;
    private bool bajarTexto = true;
    private float upTextHour = 0;

    public GameObject pauseMenu;
    public bool isPaused = false;

    public float realKarma = 0;
    public float reputacion = 0;
    public Dialogue tutorial;
    private bool tutorialShowed = false;

    public Dialogue escenaProgra;
    private bool escenaPrograFlag = true;

    private bool attemptClose = false;

    public int actualFloor = 0;
    public int maxFloor = 0;
    public int lastKarmaValue = 0;
    public Image karmaBarColor;
    public GameObject karmaBarMark;
    public GameObject barContainer;

    public GameObject itemEntryPrefab;
    public GameObject actualItemList;
    public List<GameObject> actualItemListObjects;

    public GameObject marketUI;
    public GameObject[] towerFloors;
    private void Awake()
    {
        goldQueue = new List<int>();
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (rain != null)
        {
            rain.Stop();
        }

        grupoTextoDia = GameObject.Find("GrupoDia");

        oro_inicial = 0;
        horo = 0;
        setFloor(0);
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
        if(attemptClose)
        {
            if(GlobalCustomerManager.Instance.customers.Count == 0)
            {
                showDailyStatistics(true);
            }
            else
            {
                actualHour -= Time.deltaTime;
            }
        }


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
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            setFloor(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            setFloor(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            setFloor(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            setFloor(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            setFloor(4);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            if (escenaPrograFlag)
            {
                escenaPrograFlag = false;
                DialogueManager.Instance.showDialoge(escenaProgra);
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            openMarket();
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

        if (isPlaying && !isPaused)
        {
            actualHour += Time.deltaTime;
        }

        int hour = isOpen ? 6 + (int)(actualHour * 18 / openTime) : (int)(actualHour * 6 / closeTime);
        if(hour == 7 && !tutorialShowed)
        {

            setFloor(0);
            DialogueManager.Instance.showDialoge(tutorial);
            tutorialShowed = true;
        }

        if (isOpen)
        {
            if (bajarTexto)
            {
                if (grupoTextoDia.transform.localPosition.y > 0)
                {
                    grupoTextoDia.transform.localPosition -= new Vector3(0, 1, 0);
                    upTextHour = actualHour + 3;
                }
                else
                {
                    bajarTexto = false;
                }
            }
            else
            {
                if (actualHour > upTextHour && grupoTextoDia.transform.localPosition.y < 150)
                {
                    grupoTextoDia.transform.localPosition += new Vector3(0, 1, 0);
                }
            }
        }

        if (isOpen && actualHour < 3 && grupoTextoDia.transform.localPosition.y > 0)
        {
            grupoTextoDia.transform.localPosition -= new Vector3(0, 1, 0);
        }
        if (isOpen && actualHour > 5 && actualHour < 7)
        {
            grupoTextoDia.transform.localPosition += new Vector3(0, 1, 0);
        }

        string hourString = hour < 10 ? "0" + hour.ToString() : hour.ToString();

        dia_mostrar.text = "Día " + dayCount.ToString() + " " + hourString + ":00";

        AudioManager.Instance.HandleAmbience(hour);
    }

    public void setFloor(int floor)
    {
        if(maxFloor >= floor)
        {
            actualFloor = floor;
            GlobalWorkstationManager.Instance.showFloorRooms(actualFloor);
            for (int i = 0; i < towerFloors.Length; i++)
            {
                if (i > actualFloor)
                {
                    towerFloors[i].gameObject.SetActive(false);
                }
                else
                {

                    towerFloors[i].gameObject.SetActive(true);
                }
            }
        }
       

    }
    public void openMarket()
    {
        UIOpen = true;
        isPlaying = false;
        marketUI.gameObject.GetComponent<MarketController>().showUnlockedMarkets();
        marketUI.SetActive(true);
    }

    public void closeMarket()
    {
        UIOpen = false;
        isPlaying = true;
        marketUI.SetActive(false);
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
            if(realKarma != lastKarmaValue)
            {
                StartCoroutine(showKarmaBar());
            }

            float minMentalDecay = realKarma < 0 ? (-2f/25f) * realKarma + 1 : (-1f/50f) * realKarma + 1;
            float maxMentalDecay = realKarma < 0 ? (-13f / 50f) * realKarma + 7 : (-2f / 25f) * realKarma + 7;

            GlobalCustomerManager.Instance.minMentalDecayRate = (int)minMentalDecay;
            GlobalCustomerManager.Instance.maxMentalDecayRate = (int)maxMentalDecay;

            float maxClients = realKarma < 0 ? (-3f / -50f) * realKarma + 5 : (1f / 10f) * realKarma + 5;

            ItemController.Instance.checkActualItems();
            bajarTexto = true;
            if (dayCount != 1)
            {
                float randomThug = Random.Range(0f, 1f);
                float randomSkeleton = Random.Range(0f, 1f);
                float randomSpawnValue = realKarma > 0 ? -9f / 50f * realKarma + 10 : -3f / 10f * realKarma + 10;
                Vector3 randomPosition = new Vector3(Random.Range(10, 20), 0, Random.Range(-30, 20));

                if (randomThug < randomSpawnValue/100)
                {
                    GlobalEnemiesManager.Instance.SpawnThug(randomPosition);
                    randomPosition = new Vector3(Random.Range(10, 20), 0, Random.Range(-30, 20));
                }
                if (randomSkeleton < 1* randomSpawnValue/100)
                {
                    GlobalEnemiesManager.Instance.SpawnSkeleton(randomPosition);
                }

            }
            if (dayCount % 4 == 3)
            {
                float dayRandom = Random.Range(0f, 1f);
                if (dayRandom < .33f)
                {
                    StartCoroutine(OpenCloseShop(new Color(1, 1, 0, 1)));
                    tipo_mostrar.text = "Trabajadores torpes";
                    costoso = true;
                }
                else if (dayRandom < .66f)
                {
                    StartCoroutine(OpenCloseShop(new Color(1, 0.5f, 0.5f, 1)));
                    aletargamiento = true;
                    tipo_mostrar.text = "Maldición de sueño";
                }
                else
                {
                    StartCoroutine(OpenCloseShop(new Color(0.25f, 0.75f, 1f, 1)));
                    tipo_mostrar.text = "Día lluvioso";
                    GlobalCustomerManager.Instance.maxCustomersInScene = (int)(maxClients * .6f);
                    rain.Play();
                }
            }
            else
            {
                StartCoroutine(OpenCloseShop(new Color(1, 1, 1, 1)));
                aletargamiento = false;
                costoso = false;
                GlobalCustomerManager.Instance.maxCustomersInScene = (int)maxClients;
                rain.Stop();

                tipo_mostrar.text = "Día normal";
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

    IEnumerator showKarmaBar()
    {
        RectTransform barTransform = barContainer.GetComponent<RectTransform>();
        while(barTransform.localPosition.y < 0)
        {
            barTransform.localPosition += new Vector3(0, 1000, 0) * Time.deltaTime;
            yield return null;
        }
        float elapsed = 0;
        float amountPerSecond = (lastKarmaValue - realKarma) / 2.5f;
        int actualKarma = lastKarmaValue;
        bool test = lastKarmaValue < realKarma;
        while(elapsed < 2.5f)
        {
            elapsed += Time.deltaTime;
            float newValue = (actualKarma - (amountPerSecond * elapsed));
            lastKarmaValue = (int)newValue;
            int red = (int)(-2.56f * lastKarmaValue + 128);
            int green = (int)(2.56f * lastKarmaValue + 128);

            karmaBarMark.GetComponent<RectTransform>().localPosition = new Vector3(lastKarmaValue * 10, 0, 0);

            karmaBarColor.color = new Color(red / 255f, green / 255f, 0);
            yield return null;
        }

        while (barTransform.localPosition.y > -1000)
        {
            barTransform.localPosition -= new Vector3(0, 1000, 0) * Time.deltaTime;
            yield return null;
        }


    }

    public void showDailyStatistics(bool showCheckboxPay)
    {
        foreach(GameObject itemEntry in actualItemListObjects)
        {
            Destroy(itemEntry);
        }
        attemptClose = false;
        addDebt = !showCheckboxPay;
        isPlaying = false;
        mostrar_ahorro.text = "Ahorro: " + oro_inicial.ToString();

        int ganancia = horo - oro_inicial + (perdidas + gastosMesas);
        mostrar_ganancia.text = "Ganancia: " + ganancia.ToString();
        mostrar_perdida.text = "Perdidas: -" + perdidas.ToString();
        mostrar_mesa.text = "Gastos en mesas: -" + gastosMesas.ToString();
        int salario_actual = GlobalCharactersManager.Instance.getAllSalary();
        mostrar_salario.text = "Salario de empleados: -" + salario_actual.ToString();
        mostrar_deuda.text = "Deudas: -" + deudaEmpleados.ToString();

        int total = horo;

        toggle_salario.gameObject.SetActive(showCheckboxPay);
        toggle_deuda.gameObject.SetActive(showCheckboxPay);

        toggle_impuesto.gameObject.SetActive(showCheckboxPay && (dayCount % 7 == 0));
        mostrar_impuesto.text = "Impuesto: " + (50 + dayCount * 100 * GlobalWorkstationManager.Instance.actualStations.Count).ToString(); 

        showHour.gameObject.SetActive(!showCheckboxPay);
        showCloseTime.text = isOpen ? "Cierre: 00:00" : "Apertura: 06:00";

        if(!showCheckboxPay)
        {
            int itemIndex = 0;
            while(itemIndex < 12)
            {
                if(ItemController.Instance.activeItemsState[itemIndex])
                {
                    GameObject go = Instantiate(itemEntryPrefab, actualItemList.transform);
                    go.GetComponent<Image>().sprite = ItemController.Instance.baseItems[itemIndex].itemImage;
                    actualItemListObjects.Add(go);

                }
                itemIndex++;
            }
        }

        if (toggle_impuesto.gameObject.activeSelf && toggle_impuesto.isOn)
        {
            if (horo > (50 + dayCount * 100 * GlobalWorkstationManager.Instance.actualStations.Count))
            {
                total -= (50 + dayCount * 100 * GlobalWorkstationManager.Instance.actualStations.Count);
            }
            else
            {
                toggle_impuesto.isOn = false;
            }

        }


        if (toggle_salario.gameObject.activeSelf && toggle_salario.isOn)
        {
            int horo_aux = toggle_impuesto.isOn ? horo - (50 + dayCount * 100 * GlobalWorkstationManager.Instance.actualStations.Count) : horo;

            if (horo_aux > salario_actual)
            {
                total -= salario_actual;
            }
            else
            {
                toggle_salario.isOn = false;
            }
        }



        if (toggle_deuda.gameObject.activeSelf && toggle_deuda.isOn)
        {
            int horo_aux = toggle_impuesto.isOn ? horo - (50 + dayCount * 100 * GlobalWorkstationManager.Instance.actualStations.Count) : horo;
            horo_aux = toggle_salario.isOn ? horo_aux - salario_actual : horo;
            if (horo_aux > deudaEmpleados)
            {
                total -= deudaEmpleados;
            }
            else
            {
                toggle_deuda.isOn = false;
            }
        }
        

        mostrar_total.text = "Subtotal: " + total.ToString();

        this.dailyStatistics.SetActive(true);

    }

    public void hideDailyStatistics()
    {
        if (!addDebt)
        {
            this.gastosMesas = 0;
            this.perdidas = 0;
            int salario = GlobalCharactersManager.Instance.getAllSalary();
            float magicCircleMultiply = ItemController.Instance.activeItemsState[5] ? .25f : 1;
            if (toggle_deuda.isOn)
            {
                addGold(-deudaEmpleados);
                deudaEmpleados = 0;
            }
            if (deudaEmpleados > 0)
            {
                GlobalCharactersManager.Instance.changeMental((int)(-5 * magicCircleMultiply));
            }

            if (toggle_salario.isOn)
            {
                addGold(-salario);
                GlobalCharactersManager.Instance.changeMental(10);
            }
            else
            {
                deudaEmpleados += salario;
                GlobalCharactersManager.Instance.changeMental((int)(-20 * magicCircleMultiply));
            }

            if(toggle_impuesto.isOn)
            {
                addGold(-(50 + dayCount * 100 * GlobalWorkstationManager.Instance.actualStations.Count));
            }
            else
            {
                if(toggle_impuesto.IsActive())
                {
                    SceneManager.LoadScene("GameOver_Scene");
                }
            }

        }
        isPlaying = true;
        this.UIOpen = false;
        this.dailyStatistics.SetActive(false);
    }

    IEnumerator goldCoroutine(int amount)
    {
        GameObject instance = Instantiate(feedbackPrefab, feedbackPlacement.transform.position, Quaternion.identity, canvas.transform);
        instance.GetComponent<goldFeedback>().amount = amount;
        yield return new WaitForSeconds(2);
        horo_mostrar.text = "¤" + horo.ToString();
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

        if (close)
        {
            attemptClose = true;
        }
    }

}