using TMPro;
using UnityEngine;

public class OpenSign : MonoBehaviour
{
    private GameManager gameManager;

    [Header("UI Elements")]
    [SerializeField] private Canvas openCloseUI;
    [SerializeField] private TMP_Text shopStateText;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        openCloseUI.enabled = false;
    }

    private void OnMouseDown()
    {
        if (shopStateText != null)
        {
            shopStateText.text = (gameManager.isOpen ? "Close" : "Open") + " shop?";
        }

        openCloseUI.enabled = true;
    }

    public void OnConfirm()
    {
        gameManager.Open(!gameManager.isOpen);
        openCloseUI.enabled = false;
    }

    public void OnClose()
    {
        openCloseUI.enabled = false;
    }

}
