using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WorkstationPanelController : MonoBehaviour
{
    public GameObject panel;
    [SerializeField] Camera cam;
    [SerializeField] TMP_Text title;
    [SerializeField] TMP_Text desc;
    [SerializeField] TMP_Text profit;
    [SerializeField] TMP_Text status;
    [SerializeField] TMP_Text speed;
    [SerializeField] TMP_Text karma;
    [SerializeField] TMP_Text worker;
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
        desc.text = data.description;
        profit.text = data.profit + "$";
        status.text = data.status;
        speed.text = data.Speed.ToString() + "s";
        karma.text = data.karma.ToString() + " karma";
        worker.text = data.assignedWorker;
        panel.SetActive(true);
    }

    public void Hide()
    {
        this.panel.SetActive(false);
    }
}
