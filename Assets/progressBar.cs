using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class progressBar : MonoBehaviour
{
    public float progress;
    public Image image;
    private bool destroying = false;
    // Update is called once per frame
    void Update()
    {
        this.image.fillAmount = progress / 100f;
        if(progress > 99 && !destroying)
        {
            destroying = true;
            StartCoroutine(deleteProgressBar());
        }
    }

    IEnumerator deleteProgressBar()
    {
        while (this.transform.localScale.x > 0)
        {
            this.transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);

            yield return null;
        }
        Destroy(this.gameObject);
    }
}
