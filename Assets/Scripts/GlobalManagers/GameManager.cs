using System.Collections;
using System.Xml.Linq;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour 
{
  
    public int horo;
    public TMP_Text horo_mostrar;
    public GameObject feedbackPrefab;
    public GameObject feedbackPlacement;
    public GameObject canvas;

    public int dayCount;
    public float actualHour;
    public bool isOpen;

    public Light light;

    public bool aletargamiento = false;
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
            Debug.Log(dayCount % 3);
            if(dayCount % 4 == 3)
            {

                light.color = new Color(1, 0.5f, 0.5f, 1);
                aletargamiento = true;
            }
            else
            {
                light.color = new Color(1, 1, 1, 1);
                aletargamiento = false;
            }
            dayCount += 1;
        }
        else
        {
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

}