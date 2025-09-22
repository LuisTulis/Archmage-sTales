using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalCustomerManager : MonoBehaviour

{
    public static GlobalCustomerManager Instance { get; private set; }

    public Transform spawnPoint;
    public Transform despawnPoint;
    public bool ShopIsOpen = true;

    [SerializeField]
    private List<Customer> customerPrefabs;

    private List<Customer> customers;
    private int maxCustomersInScene = 5;
    private float spawnTimer;
    private float minSpawnInterval = 10f;
    private float maxSpawnInterval = 20f;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        customers = new List<Customer>();
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
        if (customers.Count < maxCustomersInScene && customerPrefabs.Count > 0) {
            int randomIndex = Random.Range(0, customerPrefabs.Count);
            Customer randomCustomer = customerPrefabs[randomIndex];
            Customer newCustomer = Instantiate(randomCustomer, spawnPoint.position, Quaternion.identity);
            customers.Add(newCustomer);
        }
    }

    public void CustomerLeft(Customer customer) {
        customers.Remove(customer);
        Destroy(customer.gameObject);
    }
}
