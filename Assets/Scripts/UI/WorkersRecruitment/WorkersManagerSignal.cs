using UnityEngine;

public class WorkersManagerSignal : MonoBehaviour {
    [Header("UI Reference")]
    [SerializeField] private GameObject workerSignalUI;

    private bool isUIActive = false;

    private void Start() {
        if (workerSignalUI != null)
            workerSignalUI.SetActive(false);
    }

    private void Update() {
        if (isUIActive && Input.GetKeyDown(KeyCode.Escape)) {
            ToggleUI(false);
        }
    }

    private void OnMouseDown() {
        ToggleUI(true);
    }

    private void ToggleUI(bool state) {
        if (workerSignalUI != null) {
            workerSignalUI.SetActive(state);
            isUIActive = state;
        }
    }

    public void forceClose()
    {
        ToggleUI(false);
    }
}
