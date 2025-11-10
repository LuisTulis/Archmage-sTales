using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdminStationBehaviour : MonoBehaviour
{
    private void OnMouseDown() {
        if(GameManager.Instance.UIOpen == false)
        {
            GameManager.Instance.showDailyStatistics(false);
            GameManager.Instance.UIOpen = true;
        }
    }
}
