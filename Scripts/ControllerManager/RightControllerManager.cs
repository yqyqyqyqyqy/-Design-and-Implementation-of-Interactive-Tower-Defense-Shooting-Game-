using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class RightControllerManager : BaseControllerManager
{
    public override void GripPressed()
    {
        EventCenter.Broadcast(EventDefine.RightHandGrab, true);
    }

    public override void GripReleased()
    {
        EventCenter.Broadcast(EventDefine.RightHandGrab, false);
    }
    public override void TouchpadPressed()
    {
        EventCenter.Broadcast(EventDefine.RightReloadMagazine);
    }
    public override void TouchpadReleased()
    {

    }
    public override void TriggerPressed()
    {
        EventCenter.Broadcast(EventDefine.RightShot);
    }
    public override void TriggerReleased()
    {

    }
}
