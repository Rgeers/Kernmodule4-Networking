using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Player : NetworkBehaviour {
    [SyncVar(hook = "IDChanged")] public int identity;

    public GameObject cam;
    public GameObject canvas;

    public GameObject racing,
        repairing,
        timeRunning,
        raceButton,
        wheelButton,
        bodyButton,
        engineButton,
        running,
        endgame;

    public TimeKeeper timeKeeper;
    public TurnManager turnManager;

    public int dmgPercentage;
    public float timeRaceTook, TEMP_time;
    public bool myTurn, P1turn, P2turn, startedRacing, countdownToNextTurn = false;
    private bool wheelDamage, engineDamage, bodyDamage;

    private float raceCompletingAVG,
        wheelDamagePercentage,
        engineDamagePercentage,
        bodyDamagePercentage,
        wheelFixTime,
        engineFixTime,
        bodyFixTime;

    private int amountOfCheckpoints;

    void Start() {
        raceCompletingAVG = 60;
        amountOfCheckpoints = 3;
        dmgPercentage = 1;
        wheelFixTime = 25;
        engineFixTime = 50;
        bodyFixTime = 30;

        timeKeeper = FindObjectOfType<TimeKeeper>();
        turnManager = FindObjectOfType<TurnManager>();

        if (!isLocalPlayer) {
            Destroy(cam);
            Destroy(canvas);
        }


        if (isLocalPlayer) {
        }
    }

    void Update() {
        if (startedRacing) {
            raceButton.SetActive(false);
            running.SetActive(true);
            if (TEMP_time < timeRaceTook) {
                TEMP_time += Time.deltaTime;
            }
            else {
                startedRacing = false;
            }
            timeRunning.GetComponent<Text>().text = "Time :" + TEMP_time;
        }
        bool myTurn = false;
        if (turnManager.turns[identity]) {
            myTurn = true;
        }
        if (isLocalPlayer) {
            if (myTurn) {
                Debug.Log("It's my turn");
                racing.SetActive(true);
                raceButton.SetActive(true);
                repairing.SetActive(false);
            }
            else {
                Debug.Log("It's not my turn");
                racing.SetActive(false);
                repairing.SetActive(true);
            }
        }


        if (identity == 0) {
            myTurn = P1turn;
        }
        else if (identity == 1) {
            myTurn = P2turn;
        }
    }

    public void NextTurnInvoke() {
        TEMP_time = 0;
        Invoke("NextTurnCountdown", timeRaceTook);
    }

    public void EndGame(int indexWon) {
        endgame.SetActive(true);
        endgame.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "Pro winner is player " + indexWon + 1;
    }

    private void NextTurnCountdown() {
        CmdNextTurnCountdown();
    }

    private void IDChanged(int val) {
        identity = val;
    }

    public void StartRacing() {
        if (isLocalPlayer) {
            for (int i = 0; i < amountOfCheckpoints; i++) {
                if (dmgPercentage > Random.Range(0, 10)) {
                    switch (Random.Range(0, 2)) {
                        case (0):
                            wheelDamage = true;
                            break;
                        case (1):
                            engineDamage = true;
                            break;
                        case (2):
                            bodyDamage = true;
                            break;
                    }
                }
            }
            timeRaceTook = (raceCompletingAVG * (Random.Range(0.8f, 1.2f))) / 3;

            if (engineDamage) {
                timeRaceTook *= engineDamagePercentage;
            }
            if (wheelDamage) {
                timeRaceTook *= wheelDamagePercentage;
            }
            if (bodyDamage) {
                timeRaceTook *= bodyDamagePercentage;
            }

            CmdSyncTime(identity, timeRaceTook);
            startedRacing = true;
        }
    }

    public void FixEngineDamage() {
        Invoke("EngineFix", engineFixTime);
    }

    public void FixWheelDamage() {
        Invoke("WheelFix", wheelFixTime);
    }

    public void FixBodyDamage() {
        Invoke("BodyFix", bodyFixTime);
    }

    public void EngineFix() {
        engineDamage = false;
    }

    public void WheelFix() {
        wheelDamage = false;
    }

    public void BodyFix() {
        bodyDamage = false;
    }

    [Command]
    private void CmdSyncTime(int id, float time) {
        if (id == 0) {
            timeKeeper.P1times.Add(time);
        }

        if (id == 1) {
            timeKeeper.P2times.Add(time);
        }
    }

    [Command]
    private void CmdNextTurnCountdown() {
        turnManager.NewTurn();
    }
}