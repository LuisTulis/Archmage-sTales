using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkerStatus : MonoBehaviour
{
    public Transform cameraTransform;
    public Sprite[] images;
    public Image image;
    public float time;

    void Awake()
    {
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void lookAtCamera()
    {
        transform.rotation = Quaternion.LookRotation(cameraTransform.forward, cameraTransform.up);
    }

    void Update()
    {
        if(time < 0)
        {
            this.image.enabled = false;

        }
        else
        {
            time -= Time.deltaTime;
            lookAtCamera();
        }       
    }

    public void setStatus(int type)
    {
        time = 5f;
        this.image.enabled = true;
        this.image.sprite = images[type];
    }
}
