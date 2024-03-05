using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Calibration : MonoBehaviour
{
    public List<string> calibrationInputs = new List<string>();
    private float calibrationTimer = 0f;

    private Dictionary<string, List<Vector3>> calibrationData = new Dictionary<string, List<Vector3>>();

    public CalibrationUI calibrationUI;

    private void Update()
    {
        if (calibrationInputs.Count == 0)
            return;
        string calibrationInput = calibrationInputs[0];

        if (calibrationTimer > Mathf.Epsilon)
        {
            calibrationTimer -= Time.deltaTime;
        }

        if (calibrationTimer <= Mathf.Epsilon)
        {
            if (!calibrationData.ContainsKey(calibrationInput))
            {
                calibrationData.Add(calibrationInput, new List<Vector3>());
            }

            calibrationData[calibrationInput].Add(MovementDetect.instance.acceleration);

            ApplyCalibrationData();
            calibrationInputs.RemoveAt(0);
            if (calibrationInputs.Count == 0)
                return;


            ResetTimer();
        }

        calibrationUI.SetLabelText(calibrationInputs[0], calibrationTimer.ToString("F2"));
    }

    private void ApplyCalibrationData()
    {
        foreach (var action in calibrationData)
        {
            Vector3 average = Vector3.zero;
            foreach (var data in action.Value)
            {
                average += data;
            }
            average /= action.Value.Count;

            MovementDetect.ActionThreshold threshold = MovementDetect.instance.actionThresholds[action.Key];
            threshold.magnitudeThreshold = average.magnitude;
            threshold.normDirection = average.normalized;
            MovementDetect.instance.actionThresholds[action.Key] = threshold;
        }
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

    private void ResetTimer()
    {
        calibrationTimer = 3f;
    }
}
