using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Calibration : MonoBehaviour
{
    public List<string> calibrationInputs = new List<string>();
    private float calibrationTimer = 0f;
    public float preCalibrationWindow = 0.25f;
    public float postCalibrationWindow = 0.1f;

    private Dictionary<string, List<Vector3>> calibrationData = new Dictionary<string, List<Vector3>>();
    private List<Vector3> calibrationAverages = new List<Vector3>();

    private Vector3 restingAcceleration;

    public CalibrationUI calibrationUI;

    private Vector3 Average(List<Vector3> calibrationAverages)
    {
        Vector3 average = Vector3.zero;
        foreach (var data in calibrationAverages)
        {
            average += data;
        }
        average /= calibrationAverages.Count;
        return average;
    }

    private void Update()
    {
        if (calibrationInputs.Count == 0)
            return;

        string calibrationInput = calibrationInputs[0];
        calibrationTimer -= Time.deltaTime;

        if (calibrationTimer <= preCalibrationWindow)
        {
            calibrationAverages.Add(MovementDetect.instance.acceleration);
        }

        if (calibrationTimer <= -postCalibrationWindow)
        {
            // get average of data
            Vector3 accelerationAverage = Average(calibrationAverages);

            if (calibrationInput == "Stay")
                restingAcceleration = accelerationAverage;
            else
            {
                if (!calibrationData.ContainsKey(calibrationInput))
                {
                    calibrationData.Add(calibrationInput, new List<Vector3>());
                }

                calibrationData[calibrationInput].Add(accelerationAverage);
                ApplyCalibrationData(calibrationInput);
            }
            calibrationInputs.RemoveAt(0);
            calibrationAverages.Clear();

            if (calibrationInputs.Count == 0)
            {
                foreach (var action in MovementDetect.instance.actionThresholds)
                {
                    Debug.Log(action.Key + " " + action.Value.magnitudeThreshold + " " + action.Value.normDirection);
                }

                GameManager.gameState = GameManager.GameState.Playing;
                return;
            }

            ResetTimer(2f);
        }

        calibrationUI.SetLabelText(calibrationInputs[0],
            Mathf.Max(calibrationTimer, 0f).ToString("F2")
        );
    }

    private void ApplyCalibrationData(string input)
    {
        var action = calibrationData[input];
        Vector3 average = Vector3.zero;
        foreach (var data in action)
        {
            average += data;
        }
        average /= action.Count;
        average -= restingAcceleration;

        MovementDetect.ActionThreshold threshold = MovementDetect.instance.actionThresholds[input];
        threshold.magnitudeThreshold = average.magnitude;
        threshold.normDirection = average.normalized;
        threshold.enabled = true;
        MovementDetect.instance.actionThresholds[input] = threshold;
    }

    public void StartCalibration(string input)
    {
        calibrationInputs.Add(input);
        ResetTimer();
    }

    public void StartCalibrations(List<string> inputs)
    {
        calibrationInputs.AddRange(inputs);
        ResetTimer();
    }

    public void StopCalibration()
    {
        calibrationInputs.Clear();
    }

    private void ResetTimer(float time = 3f)
    {
        calibrationTimer = time;
    }
}
