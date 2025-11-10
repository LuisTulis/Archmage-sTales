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
        if(GameManager.Instance.UIOpen != true)
        { 
            if (shopStateText != null)
            {
                shopStateText.text = (gameManager.isOpen ? "Close" : "Open") + " shop?";
            }

            GameManager.Instance.UIOpen = true;
            openCloseUI.enabled = true;
        }
    }

    public void OnConfirm()
    {
        gameManager.Open(!gameManager.isOpen);
        openCloseUI.enabled = false;
        GameManager.Instance.UIOpen = false;
    }

    public void OnClose()
    {
        GameManager.Instance.UIOpen = false;
        openCloseUI.enabled = false;
    }

}
