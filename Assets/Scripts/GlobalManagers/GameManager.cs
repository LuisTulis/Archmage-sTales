using System.Xml.Linq;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour 
{
    public int horo;
    public TMP_Text horo_mostrar;


    public void addGold(int amount)
    {
        horo += amount;
        horo_mostrar.text = horo.ToString() + "$";
    }

}