using Assets.Scripts.Helpers;
using System.Collections.Generic;
using UnityEngine;

public class GlobalCustomerManager : MonoBehaviour

{
    public static GlobalCustomerManager Instance { get; private set; }

    public Transform spawnPoint;
    public Transform despawnPoint;
    public bool ShopIsOpen = true;

    [SerializeField]
    private List<CustomerComponent> customerPrefabs;

    public List<CustomerComponent> customers;
    public int maxCustomersInScene = 5;
    public int skeletonAmount = 0;
    private float spawnTimer;
    private float minSpawnInterval = 10f;
    private float maxSpawnInterval = 20f;
    public int minMentalDecayRate = 1;
    public int maxMentalDecayRate = 7;

    private bool firstCustomerLeft = true;

    [SerializeField] private int customerIdCounter = 1;

    public Dialogue workerTutorial;
    private bool lastPowerUpState = false;
    private bool lastSkarmyState = false;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        customers = new List<CustomerComponent>();
        spawnTimer = Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    void Update() {
        if (ShopIsOpen) {
            UpdateSpawnTimer();
        }
        if(lastPowerUpState != ItemController.Instance.activeItemsState[7])
        {
            lastPowerUpState = ItemController.Instance.activeItemsState[7];
            foreach (var CustomerComponent in customers)
            {
                CustomerComponent.GetComponent<CharacterLocomotion>().checkSpeedUpgrade(lastPowerUpState);
            }
        }
        if (ItemController.Instance.activeItemsState[4])
        {
            ItemController.Instance.activeItemsState[4] = false;
            summonSkarmy();
        }
    }

    private void summonSkarmy()
    {
        for(int i = 0; i < 5; i++)
        {
            SpawnSkeletonCustomer();
        }
        skeletonAmount += 5;
    }

    private void UpdateSpawnTimer() {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f) {
            SpawnCustomer();
            spawnTimer = Random.Range(minSpawnInterval, maxSpawnInterval);
        }
    }

    private void SpawnSkeletonCustomer()
    {
        int randomIndex = Random.Range(0, customerPrefabs.Count);
        CustomerComponent randomCustomer = customerPrefabs[randomIndex];
        CustomerComponent newCustomer = Instantiate(randomCustomer, spawnPoint.position, Quaternion.identity);

        newCustomer.model.Id = GetNewCustomerId();
        newCustomer.model.CharacterName = CharacterNameHelper.GetRandomSkeletonName();
        newCustomer.model.Speed = 2;
        newCustomer.model.mental = 0;
        newCustomer.model.waitingTime = 0f;
        newCustomer.model.skeleton = true;
        Debug.Log("aca estoy generando un esqueletito");



        customers.Add(newCustomer);
        
    }

    private void SpawnCustomer() {
        if (customers.Count < (maxCustomersInScene + skeletonAmount) &&
            customerPrefabs.Count > 0 && GameManager.Instance.isOpen &&
            GlobalWorkstationManager.Instance.activeStations.Count > 0 && GameManager.Instance.isPlaying) {
            int randomIndex = Random.Range(0, customerPrefabs.Count);
            CustomerComponent randomCustomer = customerPrefabs[randomIndex];
            Debug.Log("Spawning customer: " + randomCustomer);
            CustomerComponent newCustomer = Instantiate(randomCustomer, spawnPoint.position, Quaternion.identity);

            newCustomer.model.Id = GetNewCustomerId();
            newCustomer.model.CharacterName = CharacterNameHelper.GetRandomName();
            newCustomer.model.Speed = 2;
            newCustomer.model.mental = 50;
            newCustomer.model.waitingTime = 0f;


            var randomNumber = Random.Range(0f, 1f);
            newCustomer.model.thief = randomNumber < 0.05f;



            customers.Add(newCustomer);
        }
    }

    public void CustomerLeft(CustomerComponent customer) {
        customers.Remove(customer);
        if(customer.model.skeleton)
        {
            skeletonAmount--;
        }
        Destroy(customer.gameObject);
        if(firstCustomerLeft)
        {
            firstCustomerLeft = false;
            DialogueManager.Instance.workerDialogue = true;
            DialogueManager.Instance.showDialoge(workerTutorial);
        }
    }


    public int GetNewCustomerId() {
        customerIdCounter++;
        return customerIdCounter;
    }
}
