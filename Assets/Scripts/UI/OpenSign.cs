using UnityEngine;

public class OpenSign : MonoBehaviour
{

    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnMouseDown()
    {
        gameManager.Open(!gameManager.isOpen);
    }

}
