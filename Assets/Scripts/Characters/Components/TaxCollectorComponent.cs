using System.Collections.Generic;
using UnityEngine;

public class TaxCollectorComponent : NpcComponent {

    private void Start() {
        SetTarget(GlobalNpcManager.Instance.taxCollectorPoint);
        MoveToTalkPoint();
    }

    public override void MoveToTalkPoint() {
        if (talkPoint != null && locomotion != null) {
            locomotion.MoveTo(talkPoint.position);
             
        }
    }
}

