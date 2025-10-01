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

    public void removeGold(int amount) {
        if(horo - amount < 0) {
            amount = horo;
        }
        StartCoroutine(goldCoroutine(amount));
        horo -= amount;
    }


    IEnumerator goldCoroutine(int amount)
        {
            Debug.Log("Entré al coroutine");
            GameObject instance = Instantiate(feedbackPrefab, feedbackPlacement.transform.position, Quaternion.identity, canvas.transform);
            Debug.Log(instance.ToString());
            //instance.GetComponent<goldFeedback>().amount = amount;
            yield return new WaitForSeconds(1);
            horo_mostrar.text = horo.ToString() + "$";
            Debug.Log(horo);
        }

    }