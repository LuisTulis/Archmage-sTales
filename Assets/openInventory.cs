using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class openInventory : MonoBehaviour
{
    public void OnMouseDown()
    {
        if (!GameManager.Instance.UIOpen && GameManager.Instance.isPlaying)
        {
            ItemController.Instance.openBackpack();

        }
    }
}
