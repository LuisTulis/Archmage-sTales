using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WorkerSignalUI : MonoBehaviour {
    [Header("UI References")]
    [SerializeField] private Transform candidatesContainer;
    [SerializeField] private GameObject candidateEntryPrefab;

    private void OnEnable() {
        PopulateCandidates();
    }

    private void PopulateCandidates() {
        foreach (Transform child in candidatesContainer) {
            Destroy(child.gameObject);
        }

        List<WorkerModel> candidates = GlobalCharactersManager.Instance.Candidates;

        foreach (var candidate in candidates) {
            GameObject entry = Instantiate(candidateEntryPrefab, candidatesContainer);

            TMP_Text nameText = entry.transform.Find("NameText").GetComponent<TMP_Text>();
            TMP_Text statsText = entry.transform.Find("StatsText").GetComponent<TMP_Text>();
            TMP_Text speedText = entry.transform.Find("SpeedText").GetComponent<TMP_Text>();
            TMP_Text priceText = entry.transform.Find("PriceText").GetComponent<TMP_Text>();
            Image icon = entry.transform.Find("Image").GetComponent<Image>();
            Button hireButton = entry.transform.Find("HireButton").GetComponent<Button>();

            nameText.text = candidate.CharacterName;
            statsText.text = $"Alq: {candidate.Stats.alchemyStat} - Inv: {candidate.Stats.summonStat} - Enc: {candidate.Stats.enchantStat} - Adv: {candidate.Stats.adivinationStat}";
            speedText.text = $"Velocidad: {candidate.Speed}";
            priceText.text = $"Salario: {candidate.salary}";

            WorkerModel candidateCopy = candidate;
            hireButton.onClick.AddListener(() => OnHireCandidate(candidateCopy));
        }
    }

    private void OnHireCandidate(WorkerModel candidate) {
        Debug.Log($"Contrataste a {candidate.CharacterName} por {candidate.salary}");

        if (GameManager.Instance.horo < candidate.salary) {
            Debug.Log("No tienes suficiente oro para contratar a este trabajador.");
            return;
        }

        GameManager.Instance.addGold(-candidate.salary);
        GameManager.Instance.perdidas += candidate.salary;
        GlobalCharactersManager.Instance.HireWorker(candidate);

        PopulateCandidates();
    }
}
