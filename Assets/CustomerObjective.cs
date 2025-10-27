using UnityEngine;
using UnityEngine.UI;

public class CustomerObjective : MonoBehaviour
{
    public Transform cameraTransform;
    public Sprite[] images;
    public string objective;
    public Image image;
    // Start is called before the first frame update
    void Awake()
    {
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
        //this.image = GetComponent<Image>();
    }

    void lookAtCamera()
    {
        transform.rotation = Quaternion.LookRotation(cameraTransform.forward, cameraTransform.up);
    }
    // Update is called once per frame
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
            this.image.enabled = false;
        }
    }
}
