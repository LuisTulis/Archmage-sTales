using System.Collections.Generic;
using UnityEngine;

public class SellerComponent : NpcComponent {

    public void Start()
    {
        SetTarget(GlobalNpcManager.Instance.talkPoint);
        MoveToTalkPoint();
    }
    public override void MoveToTalkPoint()
    {
        if (talkPoint != null && locomotion != null)
        {
            locomotion.MoveTo(talkPoint.position);
        }
    }

}

