using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public enum GameState
    {
        None,
        MainMenu,
        Calibration,
        Playing,
        GameOver
    }

    private GameState prevGameState = GameState.None;
    public static GameState gameState = GameState.Calibration;

    public GameObject spawnerPrefab;
    private GameObject spawner;

    public GameObject calibrationPrefab;
    private GameObject calibration;
    public GameObject calibrationUI;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    void Update()
    {
        if (prevGameState != gameState)
        {
            OnStateEnd(prevGameState);
            OnStateStart(gameState);
            prevGameState = gameState;
        }
    }

    private void OnStateStart(GameState newState)
    {
        switch (newState)
        {
            case GameState.MainMenu:
                break;
            case GameState.Calibration:
                calibrationUI.SetActive(true);

                calibration = Instantiate(calibrationPrefab);
                Calibration calibrationScript = calibration.GetComponent<Calibration>();
                calibrationScript.calibrationUI = calibrationUI.GetComponent<CalibrationUI>();
                calibrationScript.StartCalibrations(new List<string> { "Stay", "Jump", "Left", "Right" });
                break;
            case GameState.Playing:
                spawner = Instantiate(spawnerPrefab);
                break;
            case GameState.GameOver:
                break;
        }
    }

    private void OnStateEnd(GameState newState)
    {
        switch (newState)
        {
            case GameState.MainMenu:
                break;
            case GameState.Calibration:
                calibrationUI.SetActive(false);
                if (calibration)
                    Destroy(calibration);
                break;
            case GameState.Playing:
                if (spawner)
                    Destroy(spawner);
                break;
            case GameState.GameOver:
                break;
        }
    }
}
