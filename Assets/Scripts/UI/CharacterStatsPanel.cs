using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class CharacterStatsPanel : MonoBehaviour {
    [SerializeField] private GameObject statPrefab;
    [SerializeField] private Transform statsGrid;
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TMP_Text characterNameText;
    [SerializeField] private Button followButton;

    private void Start() {
        panelRoot.SetActive(false);
        GlobalCharactersManager.Instance.OnCharacterSelected += ShowCharacterStats;
        GlobalCharactersManager.Instance.OnCharacterDeselected += HidePanel;

        if (followButton != null)
            followButton.onClick.AddListener(OnFollowButtonClicked);
    }

    private void OnDestroy() {
        if (GlobalCharactersManager.Instance == null) return;
        GlobalCharactersManager.Instance.OnCharacterSelected -= ShowCharacterStats;
        GlobalCharactersManager.Instance.OnCharacterDeselected -= HidePanel;


        if (followButton != null)
            followButton.onClick.RemoveListener(OnFollowButtonClicked);
    }

    private void ShowCharacterStats(CharacterComponent character) {
        ClearStats();
        panelRoot.SetActive(true);

        Dictionary<string, string> stats = character.GetStats();

        if (stats.ContainsKey("Name")) {
            characterNameText.text = stats["Name"];
            stats.Remove("Name");
        } else {
            characterNameText.text = "Unknown";
        }

        foreach (var kvp in stats) {
            GameObject statGO = Instantiate(statPrefab, statsGrid);
            statGO.transform.Find("StatName").GetComponent<TMP_Text>().text = kvp.Key;
            statGO.transform.Find("StatValue").GetComponent<TMP_Text>().text = kvp.Value;
        }
    }

    private void HidePanel(CharacterComponent character) {
        panelRoot.SetActive(false);
        ClearStats();
    }

    private void ClearStats() {
        foreach (Transform child in statsGrid)
            Destroy(child.gameObject);

        if (characterNameText != null)
            characterNameText.text = string.Empty;
    }

    private void OnFollowButtonClicked() {
        var selected = GlobalCharactersManager.Instance.SelectedCharacter;
        if (selected == null) return;

        CameraControl.Instance.cameraTarget = selected.transform;
    }
}
