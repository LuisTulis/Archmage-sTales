using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalNpcManager : MonoBehaviour {
    public static GlobalNpcManager Instance { get; private set; }

    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform talkPoint;
    public Transform taxCollectorPoint;
    [SerializeField] private Transform despawnPoint;
    public bool ShopIsOpen = true;

    [SerializeField] private TaxCollectorComponent TaxCollectorPrefab;
    [SerializeField] private SellerComponent LordEnzoPrefab;
    [SerializeField] private SellerComponent PaolaSanToroPrefab;
    [SerializeField] private SellerComponent EhrLichPrefab;
    [SerializeField] private SellerComponent AlejandroPrefab;
    [SerializeField] private SellerComponent FelixPrefab;
    [SerializeField] private SellerComponent FacundoPrefab;

    [SerializeField] private int taxCollectorIntervalDays = 7;
    private int lastTaxSpawnDay = -999;

    [SerializeField] private int sellerIntervalDaysAfterFirstRound = 2;
    private int lastSellerSpawnDay = -999;

    private TaxCollectorComponent currentTaxCollector;
    private SellerComponent currentSeller;
    private SellerComponent[] sellerPrefabs;
    private bool[] sellerHasSpawned;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
            return;
        }

        sellerPrefabs = new SellerComponent[] { LordEnzoPrefab, PaolaSanToroPrefab, EhrLichPrefab, AlejandroPrefab, FelixPrefab, FacundoPrefab };
        sellerHasSpawned = new bool[sellerPrefabs.Length];
    }

    void Update() {
        if (!ShopIsOpen) return;
        if (GameManager.Instance == null) return;

        int day = GameManager.Instance.dayCount;

        if (TaxCollectorPrefab != null) {
            if (day % taxCollectorIntervalDays == 0 && day != lastTaxSpawnDay) {
                if (currentTaxCollector == null) {
                    SpawnTaxCollector();
                } else {
                    lastTaxSpawnDay = day;
                }
            }
        }

        if (day >= 1 && sellerPrefabs.Length > 0) {
            if (currentSeller == null) {
                bool allSpawnedOnce = true;
                foreach (var b in sellerHasSpawned) if (!b) { allSpawnedOnce = false; break; }
                if (!allSpawnedOnce) {
                    if (day != lastSellerSpawnDay) {
                        int idx = -1;
                        for (int i = 0; i < sellerHasSpawned.Length; i++) {
                            if (!sellerHasSpawned[i]) { idx = i; break; }
                        }
                        if (idx >= 0) {
                            SpawnSeller(idx);
                        }
                    }
                } else {
                    if (day - lastSellerSpawnDay >= sellerIntervalDaysAfterFirstRound) {
                        int rand = Random.Range(0, sellerPrefabs.Length);
                        SpawnSeller(rand);
                    }
                }
            }
        }
    }

    private void SpawnTaxCollector() {
        if (TaxCollectorPrefab == null || spawnPoint == null || taxCollectorPoint == null || despawnPoint == null) return;
        if (currentTaxCollector != null) return;

        var taxCollector = Instantiate(TaxCollectorPrefab, spawnPoint.position, spawnPoint.rotation);


        currentTaxCollector = taxCollector;
        lastTaxSpawnDay = GameManager.Instance.dayCount;
    }

    private void SpawnSeller(int index) {
        if (index < 0 || index >= sellerPrefabs.Length) return;
        var prefab = sellerPrefabs[index];
        if (prefab == null || spawnPoint == null || talkPoint == null || despawnPoint == null) return;
        if (currentSeller != null) return;

        var seller = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);

        currentSeller = seller;
        sellerHasSpawned[index] = true;
        lastSellerSpawnDay = GameManager.Instance.dayCount;
    }

    public void NpcLeft(NpcComponent npc) {
        if (npc == null) return;

        if (currentTaxCollector != null && npc == currentTaxCollector) {
            currentTaxCollector = null;
        }

        if (currentSeller != null && npc == currentSeller) {
            currentSeller = null;
        }
    }

}