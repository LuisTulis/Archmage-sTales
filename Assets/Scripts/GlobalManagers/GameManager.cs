using System.Collections;
using System.Xml.Linq;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour 
{
    public static GameManager Instance { get; private set; }

    public int horo;
    public TMP_Text horo_mostrar;
    public GameObject feedbackPrefab;
    public GameObject feedbackPlacement;
    public GameObject canvas;

    public bool isOpen = true;
    public int dayCount;
    public float actualHour;
    public Light light;

    public bool aletargamiento = false;
    public bool costoso = false;
    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }


    public void addGold(int amount)
    {
        StartCoroutine(goldCoroutine(amount));
        horo += amount;
    }

    public void Open(bool open)
    {
        isOpen = open;
        if(isOpen)
        {
            if(dayCount != 1)
            {
                float random = Random.Range(0f, 1f);
                Vector3 randomPosition = new Vector3(Random.Range(10, 20), 0, Random.Range(-30, 20));
                if (random < .1f)
                {
                    GlobalEnemiesManager.Instance.SpawnSkeleton(randomPosition);
                }
                else if(random < .2f)
                {
                    GlobalEnemiesManager.Instance.SpawnThug(randomPosition);
                }

            }
            if(dayCount % 4 == 3)
            {
                float dayRandom = Random.Range(0f, 1f);
                if (dayRandom < .33f)
                {
                    light.color = new Color(1, 1, 0, 1);
                    costoso = true;
                }
                else if(dayRandom < .66f)
                {
                    light.color = new Color(1, 0.5f, 0.5f, 1);
                    aletargamiento = true;
                }
                else
                {
                    light.color = new Color(0.25f, 0.75f, 1f, 1);
                    GlobalCustomerManager.Instance.maxCustomersInScene = 2;
                }
            }
            else
            {
                light.color = new Color(1, 1, 1, 1);
                aletargamiento = false;
                costoso = false;
                GlobalCustomerManager.Instance.maxCustomersInScene = 5;
            }
            GlobalCharactersManager.Instance.GenerateCandidates();
            dayCount += 1;
        }
        else
        {
            foreach(Customer customer in GlobalCustomerManager.Instance.customers)
            {
                customer.LeaveWithoutBuy();
            }
            light.color = new Color(0, 0, 1, 1);
        }
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

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.J))
        {
            Open(!this.isOpen);
        }
        if(isOpen)
        {
            actualHour += Time.deltaTime;
        }
        
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