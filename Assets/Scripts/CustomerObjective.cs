using UnityEngine;
using UnityEngine.UI;

public class CustomerObjective : MonoBehaviour
{
    public Transform cameraTransform;
    public Sprite[] images;
    public string objective;
    public Image image;
    public bool show;
    void Awake()
    {
        this.show = true;
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
        if(show)
        {
            this.transform.localPosition = new Vector3(0, 0, 0);
        }
        else
        {
            this.transform.localPosition = new Vector3(0, -5000, 0);
        }
        lookAtCamera();
        if (objective != "")
        {
            this.image.enabled = true;
            switch (objective)
            {
                case "caldero":
                    this.image.sprite = images[0];
                    break;
                case "adivinacion":
                    this.image.sprite = images[1];
                    break;
                case "invocacion":
                    this.image.sprite = images[2];
                    break;
                case "encantamiento":
                    this.image.sprite = images[3];
                    break;
                case "exclamacion":
                    this.image.sprite = images[5];
                    break;

            }
        }
        else
        {
            this.image.sprite = images[4];
            //this.image.enabled = false;
        }
    }
}
