using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class LeftControllerManager : BaseControllerManager
{
    public override void GripReleased()
    {
        EventCenter.Broadcast(EventDefine.LeftHandGrab, false);
    }

    public override void GripPressed()
    {
        EventCenter.Broadcast(EventDefine.LeftHandGrab, true);
    }
    public override void TouchpadPressed()
    {
        EventCenter.Broadcast(EventDefine.LeftReloadMagazine);
    }
    public override void TouchpadReleased()
    {

    }
    public override void TriggerPressed()
    {
        EventCenter.Broadcast(EventDefine.LeftShot);
    }
    public override void TriggerReleased()
    {

    }
}
