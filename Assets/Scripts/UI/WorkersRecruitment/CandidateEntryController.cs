using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CandidateEntryController : MonoBehaviour {
    [Header("UI refs")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text statsText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private Button hireButton;

    private CharacterData data;
    private Action<CharacterData> onHire;

    public void SetData(CharacterData characterData, Action<CharacterData> onHireCallback) {
        data = characterData;
        onHire = onHireCallback;

        if (nameText != null) nameText.text = data.Name;
        if (statsText != null) statsText.text = $"Velocidad: {data.Speed}";
        if (priceText != null) priceText.text = $"Precio: {data.HirePrice}";

        hireButton.onClick.RemoveAllListeners();
        hireButton.onClick.AddListener(OnHireClicked);
    }

    private void OnHireClicked() {
        onHire?.Invoke(data);
    }

    private void OnDestroy() {
        if (hireButton != null)
            hireButton.onClick.RemoveAllListeners();
    }
}
