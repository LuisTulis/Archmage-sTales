using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class goldFeedback2 : MonoBehaviour
{
    public Transform cameraTransform;
    private TextMesh text;
    // Start is called before the first frame update
    void Awake()
    {
        text = this.GetComponent<TextMesh>();

        if(cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
    }
    public void changeText(string profit)
    {
        this.text.text = profit + "$";
    }
    void lookAtCamera()
    {
        transform.rotation = Quaternion.LookRotation(cameraTransform.forward, cameraTransform.up);
    }
    // Update is called once per frame
    void Update()
    {
        lookAtCamera();
        text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - (1f * Time.deltaTime));
        this.transform.position += new Vector3(0, Time.deltaTime, 0);
        if(text.color.a <= 0)
        {
            Destroy(this.gameObject);
        }
        
    }
}
