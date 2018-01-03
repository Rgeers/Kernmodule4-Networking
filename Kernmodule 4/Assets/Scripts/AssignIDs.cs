using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AssignIDs : NetworkBehaviour {
    public Player[] players;
    private bool finished = false;

    private int newPlayerCount;
    private int oldPlayerCount = 0;
	
	private void Update () {
        players = FindObjectsOfType<Player>();

        newPlayerCount = players.Length;


        if (newPlayerCount != oldPlayerCount) {
            oldPlayerCount = newPlayerCount;
            finished = false;
        }

        if (isServer) {
            if (!finished) {
                RpcAssignIDtoPlayers();
            }
        }
	}

    [ClientRpc]
    private void RpcAssignIDtoPlayers() {
        for (int i = 0; i < players.Length; i++) {
            players[i].GetComponent<Player>().identity = i;
        }
        finished = true;
    }
}
