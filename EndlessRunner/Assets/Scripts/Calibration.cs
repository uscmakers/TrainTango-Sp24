using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calibration : MonoBehaviour
{
    public bool calibrating = false;
    public string calibrationInput;
    private float calibrationTimer = 0f;

    public MovementDetect movementDetect;

    private Dictionary<string, List<Vector3>> calibrationData = new Dictionary<string, List<Vector3>>();

    private void Update()
    {
        if (!calibrating) return;

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

            calibrationData[calibrationInput].Add(movementDetect.acceleration);

            ApplyCalibrationData();
            calibrating = false;
        }
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

            MovementDetect.ActionThreshold threshold = movementDetect.actionThresholds[action.Key];
            threshold.magnitudeThreshold = average.magnitude;
            threshold.normDirection = average.normalized;
            movementDetect.actionThresholds[action.Key] = threshold;
        }
    }

    public void StartCalibration(string input)
    {
        calibrating = true;
        calibrationInput = input;
        calibrationTimer = 3f;
    }
}
