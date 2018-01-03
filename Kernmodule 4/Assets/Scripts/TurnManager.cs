using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TurnManager : NetworkBehaviour {

    [SyncVar]
    int currentTurn = 0;

    public SyncListBool turns = new SyncListBool();
    public Player[] players;
    public float countdownTime, TEMP_Time;
    public TimeKeeper timeKeeper;

    private bool finished = false, countingDown = false;

    private int newPlayerCount;
    private int oldPlayerCount = 0;

    private void Update() {
        players = FindObjectsOfType<Player>();
    
        newPlayerCount = players.Length;

        if (newPlayerCount != oldPlayerCount) {
            oldPlayerCount = newPlayerCount;
            turns.Clear();

            for (int i = 0; i < newPlayerCount; i++) {
                turns.Add(false);
            }

            turns[0] = true;
        }
    }

    public void Countdown() {
        countingDown = true;
    }

    [ClientRpc]
    public void RpcSetNextTurn() {
        currentTurn++;
        
        foreach(Player p in players) {
            if (p.gameObject.transform.childCount >0) {
                p.running.SetActive(false);

                
                if (currentTurn == 5) {

                    int playerWon = 1337;

                    float p0time = 0;
                    float p1time = 1;

                    foreach(float p0f in timeKeeper.P1times) {
                        p0time += p0f;
                    }

                    foreach (float p1f in timeKeeper.P2times) {
                        p1time += p1f;
                    }

                    float p0result = p0time / 3;
                    float p1result = p1time / 3;


                    if (p0result < p1result) {
                        playerWon = 0;
                    } else {
                        playerWon = 1;
                    }
                    //TODO: Add canvas object showing you which player you are
                    //TODO: Add object showing the winner's time, comparing it to yours
                    p.EndGame(playerWon);
                }
            }
        }
    }
    
    [Server]
    public void NewTurn() {
        RpcSetNextTurn();
        for (int i = 0; i < turns.Count; i++) {
            if (turns[i] == true) {
                turns[i] = false;
                if (i + 1 < turns.Count) {
                    turns[i + 1] = true;
                } else {
                    turns[0] = true;
                }
                return;
            }
        }
    }
}
