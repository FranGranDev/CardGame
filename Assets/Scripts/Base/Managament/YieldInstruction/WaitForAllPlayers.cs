using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WaitForAllPlayers : CustomYieldInstruction
{
    public override bool keepWaiting
    {
        get
        {
            return false;
        }
    }
}
