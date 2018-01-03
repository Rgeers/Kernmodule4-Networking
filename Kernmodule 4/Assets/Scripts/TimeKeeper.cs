using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TimeKeeper : NetworkBehaviour {
    public SyncListFloat P1times = new SyncListFloat();
    public SyncListFloat P2times = new SyncListFloat();
}
