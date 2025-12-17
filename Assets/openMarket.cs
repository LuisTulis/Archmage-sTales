using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class openMarket : MonoBehaviour
{
    
   public void OnMouseDown()
   {
        Debug.Log("Toqué carretilla");
        if (!GameManager.Instance.UIOpen && GameManager.Instance.isPlaying)
        {
            GameManager.Instance.openMarket();
        }
   }
}
