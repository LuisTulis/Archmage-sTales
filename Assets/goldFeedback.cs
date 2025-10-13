using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class goldFeedback : MonoBehaviour
{
    GameObject goldCount;
    TMP_Text text;
    private float yDifference;
    private float firstPosition;
    public int amount;
    // Start is called before the first frame update
    void Start()
    {
        goldCount = GameObject.Find("Orito");
        text = this.GetComponent<TMP_Text>();
        text.text = amount.ToString() + "$";
        StartCoroutine(changeColor());
        yDifference = goldCount.transform.position.y - this.transform.position.y;
        firstPosition = this.transform.position.y;
    }

    IEnumerator changeColor()
    {
        while (this.text.color.a < 1)
        {
            this.text.color += new Color(0, 0, 0, Time.deltaTime * 0.5f);
            this.transform.position = new Vector3(this.transform.position.x, this.firstPosition + (this.text.color.a * yDifference), this.transform.position.z);
            yield return null;
        }
        Destroy(this.gameObject);
    }
}
