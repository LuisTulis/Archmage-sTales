using UnityEngine;
using UnityEngine.UI;

public class CustomerObjective : MonoBehaviour
{
    public Transform cameraTransform;
    public Sprite[] images;
    public string objective;
    public Image image;

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

            }
        }
        else
        {
            this.image.sprite = images[4];
            //this.image.enabled = false;
        }
    }
}
