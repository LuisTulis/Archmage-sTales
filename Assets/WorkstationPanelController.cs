using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WorkstationPanelController : MonoBehaviour
{
    public GameObject panel;
    [SerializeField] Camera cam;
    [SerializeField] TMP_Text title;
    [SerializeField] LayerMask interactableMask;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Hide();
        }

    }
    public void Show(WorkstationData data)
    {
        title.text = data.displayName;
        panel.SetActive(true);
    }

    public void Hide()
    {
        this.panel.SetActive(false);
    }
}
