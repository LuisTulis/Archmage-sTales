using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdminStationBehaviour : MonoBehaviour
{
    private void OnMouseDown() {
        GameManager.Instance.showDailyStatistics(false);
    }
}
