using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStatsPanel : MonoBehaviour
{
    [SerializeField] private GameObject statPrefab;
    [SerializeField] private Transform statsGrid;
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TMP_Text characterNameText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Button followButton;
    [SerializeField] private Button dismissButton;

    [SerializeField] private GameObject mentalSection;
    [SerializeField] private TMP_Text mentalText;

    [SerializeField] private GameObject statusSection;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private Image statusImg;

    private void Start()
    {
        panelRoot.SetActive(false);
        GlobalCharactersManager.Instance.OnCharacterSelected += ShowCharacterStats;
        GlobalCharactersManager.Instance.OnCharacterDeselected += HidePanel;

        if (followButton != null)
            followButton.onClick.AddListener(OnFollowButtonClicked);

        if (dismissButton != null)
        {
            dismissButton.gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (GlobalCharactersManager.Instance == null) return;
        GlobalCharactersManager.Instance.OnCharacterSelected -= ShowCharacterStats;
        GlobalCharactersManager.Instance.OnCharacterDeselected -= HidePanel;


        if (followButton != null)
            followButton.onClick.RemoveListener(OnFollowButtonClicked);


        if (dismissButton != null)
            dismissButton.onClick.RemoveAllListeners();
    }

    private void ShowCharacterStats(CharacterComponent character)
    {
        ClearStats();
        panelRoot.SetActive(true);

        Dictionary<string, object> stats = character.GetStats();

        if (stats.ContainsKey("Name"))
        {
            characterNameText.text = stats["Name"].ToString();
            stats.Remove("Name");
        }
        else
        {
            characterNameText.text = "Unknown";
        }

        if (character is not StaffAdorComponent)
        {
            if (character is WorkerComponent worker)
            {
                mentalSection.SetActive(true);
                mentalText.text = "Mental: " + worker.GetComponent<WorkerModel>().mental.ToString();

                // TODO: get image from workstation type

                //statusSection.SetActive(true);
                //var stationType = character.GetComponent<BaseWorkerModel>().AsignatedStation?.type;
                //statusImg.sprite = stationType.image;
            }

            if (character is CustomerComponent customer)
            {
                mentalSection.SetActive(true);
                mentalText.text = "Mental: " + customer.GetComponent<CustomerModel>().mental.ToString();

                statusSection.SetActive(true);
                statusImg.sprite = customer.GetComponentInChildren<CustomerObjective>().image.sprite;
            }

        }
        else
        {
            mentalSection.SetActive(false);
            statusSection.SetActive(false);
        }


        foreach (var kvp in stats) {
            if (kvp.Key == "Icon" && kvp.Value is Sprite icon) {
                iconImage.sprite = icon;
                continue;
            }

            GameObject statGO = Instantiate(statPrefab, statsGrid);
            statGO.transform.Find("StatName").GetComponent<TMP_Text>().text = kvp.Key;
            statGO.transform.Find("StatValue").GetComponent<TMP_Text>().text = kvp.Value.ToString();
        }

        HandleDismissButton(character);
    }

    private void HidePanel(CharacterComponent character)
    {
        panelRoot.SetActive(false);
        ClearStats();
    }

    public void auxHidePanel()
    {
        GlobalCharactersManager.Instance.SelectedCharacter = null;
        panelRoot.SetActive(false);
        ClearStats();
    }

    private void ClearStats()
    {
        foreach (Transform child in statsGrid)
            Destroy(child.gameObject);

        if (characterNameText != null)
            characterNameText.text = string.Empty;
    }

    private void OnFollowButtonClicked()
    {
        var selected = GlobalCharactersManager.Instance.SelectedCharacter;
        if (selected == null) return;

        CameraControl.Instance.cameraTarget = selected.transform;
    }

    private void HandleDismissButton(CharacterComponent character)
    {
        if (dismissButton == null) return;

        dismissButton.gameObject.SetActive(false);
        dismissButton.onClick.RemoveAllListeners();

        if (character is WorkerComponent worker && character is not StaffAdorComponent)
        {
            dismissButton.gameObject.SetActive(true);
            dismissButton.onClick.AddListener(() => OnDismissButtonClicked(worker));
        }
        if (character is CustomerComponent customer)
        {
            dismissButton.gameObject.SetActive(true);
            dismissButton.onClick.AddListener(() => OnDismissCustomerButtonClicked(customer));
        }
    }

    private void OnDismissButtonClicked(WorkerComponent worker)
    {
        Debug.Log($"Despedido el trabajador: {worker.name}");

        worker.BeFired();
        auxHidePanel();
    }
    private void OnDismissCustomerButtonClicked(CustomerComponent customer)
    {
        Debug.Log($"Despedido el trabajador: {customer.name}");

        customer.LeaveWithoutBuy();
        auxHidePanel();
    }
}
