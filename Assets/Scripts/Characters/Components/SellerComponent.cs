using System.Collections.Generic;
using UnityEngine;

public class SellerComponent : NpcComponent {

    public override void MoveToTalkPoint()
    {
        if (talkPoint != null && locomotion != null)
        {
            locomotion.MoveTo(talkPoint.position);
        }
    }

}

