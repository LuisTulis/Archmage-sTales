using System.Collections;
using TMPro;
using UnityEngine;
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
    public Toggle toggle_salario;
    public Toggle toggle_deuda;
    private void Awake()
    {
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
        oro_inicial = 0;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.J) || actualHour > openTime || (!isOpen && actualHour > closeTime))
        {
            Open(!this.isOpen);
            actualHour = 0;
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            addGold(1000);
        }
        if(isPlaying)
        {
            actualHour += Time.deltaTime;
        }

        int hour = isOpen ? 6 + (int)(actualHour * 18 / openTime) : (int)(actualHour * 6 / closeTime);

        string hourString = hour < 10 ? "0" + hour.ToString() : hour.ToString();

        dia_mostrar.text = "Día " + dayCount.ToString() + " " + hourString + ":00";

    }

    public void addGold(int amount)
    {
        StartCoroutine(goldCoroutine(amount));
        horo += amount;
    }

    public void Open(bool open)
    {
        isOpen = open;
        if (isOpen)
        {
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
                    light.color = new Color(1, 1, 0, 1);
                    tipo_mostrar.text = "Trabajadores torpes";
                    costoso = true;
                }
                else if (dayRandom < .66f)
                {
                    light.color = new Color(1, 0.5f, 0.5f, 1);
                    aletargamiento = true;
                    tipo_mostrar.text = "Maldición de sueño";
                }
                else
                {
                    light.color = new Color(0.25f, 0.75f, 1f, 1);
                    tipo_mostrar.text = "Día lluvioso";
                    GlobalCustomerManager.Instance.maxCustomersInScene = 2;
                    rain.Play();
                }
            }
            else
            {
                light.color = new Color(1, 1, 1, 1);
                aletargamiento = false;
                costoso = false;
                GlobalCustomerManager.Instance.maxCustomersInScene = 5;
                rain.Stop();

                tipo_mostrar.text = "Día normal";
            }
            GlobalCharactersManager.Instance.GenerateCandidates();
            dayCount += 1;
            oro_inicial = horo;
        }
        else
        {
            showDailyStatistics(true);
            foreach (CustomerComponent customer in GlobalCustomerManager.Instance.customers)
            {
                customer.LeaveWithoutBuy();
            }
            light.color = new Color(0, 0, 1, 1);
        }
    }

    public void showDailyStatistics(bool showCheckboxPay)
    {
        addDebt = !showCheckboxPay;
        isPlaying = false;
        int ganancia = horo - oro_inicial + (perdidas + gastosMesas);
        mostrar_ganancia.text = "Ganancia: " + ganancia.ToString();
        mostrar_perdida.text = "Perdidas: -" + perdidas.ToString();
        mostrar_mesa.text = "Gastos en mesas: -" + gastosMesas.ToString();
        int salario_actual = GlobalCharactersManager.Instance.getAllSalary();
        mostrar_salario.text = "Salario de empleados: -" + salario_actual.ToString();
        mostrar_deuda.text = "Deudas: -" + deudaEmpleados.ToString();

        int total = ganancia - perdidas - gastosMesas;

        toggle_salario.gameObject.SetActive(showCheckboxPay);
        toggle_deuda.gameObject.SetActive(showCheckboxPay);

        if (toggle_salario.gameObject.activeSelf && toggle_salario.isOn)
        {
            if(horo > salario_actual)
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
            int horo_aux = toggle_salario.isOn ? horo - salario_actual : horo;
            if (horo_aux > deudaEmpleados)
            {
                total -= deudaEmpleados;
            }
            else
            {
                toggle_deuda.isOn = false;
            }
        }

        mostrar_total.text = "Total: " + total.ToString();

        this.dailyStatistics.SetActive(true);

    }

    public void hideDailyStatistics()
    {
        if(!addDebt) {
            this.gastosMesas = 0;
            this.perdidas = 0;
            int salario = GlobalCharactersManager.Instance.getAllSalary();
            if (toggle_deuda.isOn)
            {
                addGold(-deudaEmpleados);
            }
            if(deudaEmpleados > 0)
            {
                GlobalCharactersManager.Instance.changeMental(-5);
            }

            if(toggle_salario.isOn)
            {
                addGold(-salario);
                GlobalCharactersManager.Instance.changeMental(10);
            }
            else
            {
                deudaEmpleados += salario;
                GlobalCharactersManager.Instance.changeMental(-20);
            }
        }
        isPlaying = true;
        this.dailyStatistics.SetActive(false);
    }

    IEnumerator goldCoroutine(int amount)
    {
        Debug.Log("Entré al coroutine");
        GameObject instance = Instantiate(feedbackPrefab, feedbackPlacement.transform.position, Quaternion.identity, canvas.transform);
        instance.GetComponent<goldFeedback>().amount = amount;
        yield return new WaitForSeconds(1);
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

}