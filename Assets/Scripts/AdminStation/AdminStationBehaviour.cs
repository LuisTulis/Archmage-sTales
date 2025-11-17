using UnityEngine;

public class AdminStationBehaviour : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (GameManager.Instance.UIOpen == false)
        {
            GameManager.Instance.showDailyStatistics(false);
            GameManager.Instance.UIOpen = true;

            AudioManager.Instance.PlaySound("Madera1");
        }
    }
}
