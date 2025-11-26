using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalCustomerManager_Prototipo : MonoBehaviour

{
    public static GlobalCustomerManager_Prototipo Instance { get; private set; }

    public Transform spawnPoint;
    public Transform despawnPoint;
    public bool ShopIsOpen = true;

    [SerializeField]
    private List<Customer_Prototipo> customerPrefabs;

    public List<Customer_Prototipo> customers;
    public int maxCustomersInScene = 5;
    private float spawnTimer;
    private float minSpawnInterval = 10f;
    private float maxSpawnInterval = 20f;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        customers = new List<Customer_Prototipo>();
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
        if (customers.Count < maxCustomersInScene && customerPrefabs.Count > 0 && GameManager_Prototipo.Instance.isOpen) {
            int randomIndex = Random.Range(0, customerPrefabs.Count);
            Customer_Prototipo randomCustomer = customerPrefabs[randomIndex];
            Customer_Prototipo newCustomer = Instantiate(randomCustomer, spawnPoint.position, Quaternion.identity);
            customers.Add(newCustomer);
        }
    }

    public void CustomerLeft(Customer_Prototipo customer) {
        customers.Remove(customer);
        Destroy(customer.gameObject);
    }
}
