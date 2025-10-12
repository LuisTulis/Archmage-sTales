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
    private float spawnTimer;
    private float minSpawnInterval = 10f;
    private float maxSpawnInterval = 20f;

    [SerializeField] private int customerIdCounter = 1;

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
    }

    private void UpdateSpawnTimer() {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f) {
            SpawnCustomer();
            spawnTimer = Random.Range(minSpawnInterval, maxSpawnInterval);
        }
    }

    private void SpawnCustomer() {
        if (customers.Count < maxCustomersInScene && customerPrefabs.Count > 0 && GameManager.Instance.isOpen) {
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
        Destroy(customer.gameObject);
    }


    public int GetNewCustomerId() {
        customerIdCounter++;
        return customerIdCounter;
    }
}
