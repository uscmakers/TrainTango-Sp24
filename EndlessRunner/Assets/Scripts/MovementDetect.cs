using System;
using System.IO.Ports;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class MovementDetect : MonoBehaviour
{
    public static MovementDetect instance;

    [System.Serializable]
    public struct ActionThreshold
    {
        public Vector3 normDirection;
        public float magnitudeThreshold;
        public float dotThreshold;
        public bool enabled;
    }

    [Header("Action Thresholds")]
    [SerializeField]
    public Dictionary<string, ActionThreshold> actionThresholds = new Dictionary<string, ActionThreshold>
    {
        { "Jump", new ActionThreshold { normDirection = new Vector3(0, 1, 0), magnitudeThreshold = 1.5f, dotThreshold = 0.75f, enabled = false } },
        { "Left", new ActionThreshold { normDirection = new Vector3(-1, 0, 0), magnitudeThreshold = 1.5f, dotThreshold = 0.75f, enabled = false } },
        { "Right", new ActionThreshold { normDirection = new Vector3(1, 0, 0), magnitudeThreshold = 1.5f, dotThreshold = 0.75f, enabled = false } }
    };

    private List<Vector3> directionBuffer = new List<Vector3>();
    public int maxDirectionBuffer = 3;
    private List<Vector3> magnitudeBuffer = new List<Vector3>();
    public int maxMagnitudeBuffer = 8;

    [Header("Action Callbacks")]
    [SerializeField]
    public Dictionary<string, UnityEvent> actionCallbacks = new Dictionary<string, UnityEvent>
    {
        { "Jump", new UnityEvent() },
        { "Left", new UnityEvent() },
        { "Right", new UnityEvent() }
    };

    SerialPort serialPort;
    public string portName = "COM8"; // Example port name
    public int baudRate = 115200; // Example baud rate

    [SerializeField] private PlayerMovement playerMovement;

    public float totalCooldown = 0.5f;
    private float cooldownTimer = 0f;

    public Vector3 acceleration;
    public Vector3 gyroscope;

    private void Awake()
    {
        instance = this;

        foreach (var action in actionCallbacks)
        {
            action.Value.AddListener(() => Debug.Log("Action " + action.Key + " triggered"));
        }

        actionCallbacks["Jump"].AddListener(OnJumpDetected);
        actionCallbacks["Left"].AddListener(OnLeftMotionDetected);
        actionCallbacks["Right"].AddListener(OnRightMotionDetected);
    }

    private void Start()
    {
        OpenConnection();
    }


    private void Update()
    {
        string dataString;
        try
        {
            dataString = serialPort.ReadLine();
        }
        catch (Exception e)
        {
            return;
        }

        try
        {
            if (cooldownTimer >= totalCooldown)
            {
                HandleData(dataString);
            }
            else
            {
                cooldownTimer += Time.deltaTime;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("Error reading from serial port: " + e.Message);
        }
    }


    void OpenConnection()
    {
        serialPort = new SerialPort(portName, baudRate)
        {
            ReadTimeout = 5000 // Prevents blocking if no data is available to read
        };

        try
        {
            serialPort.Open();
        }
        catch (Exception e)
        {
            Debug.LogWarning("Could not open serial port: " + e.Message);
        }
    }


    void HandleData(string dataString)
    {
        try
        {
            // Assuming the dataString is in the format "A:x,y,z;G:x,y,z;"
            // We split the data into accelerometer and gyroscope values
            if (!string.IsNullOrEmpty(dataString) && dataString.Contains("A:") && dataString.Contains("G:"))
            {
                string[] splitData = dataString.Split(';');
                if (splitData.Length >= 2)
                {
                    string[] accData = splitData[0].Split(':')[1].Split(',');
                    string[] gyroData = splitData[1].Split(':')[1].Split(',');


                    if (accData.Length == 3 && gyroData.Length == 3)
                    {
                        magnitudeBuffer.Add(new Vector3(
                            float.Parse(accData[0]),
                            float.Parse(accData[1]),
                            float.Parse(accData[2])));
                        if (magnitudeBuffer.Count > maxMagnitudeBuffer)
                            magnitudeBuffer.RemoveAt(0);


                        directionBuffer.Add(new Vector3(
                            float.Parse(gyroData[0]),
                            float.Parse(gyroData[1]),
                            float.Parse(gyroData[2])));
                        if (directionBuffer.Count > maxDirectionBuffer)
                            directionBuffer.RemoveAt(0);

                        acceleration = Vector3.zero;
                        foreach (var magnitude in magnitudeBuffer)
                        {
                            acceleration += magnitude;
                        }
                        acceleration /= magnitudeBuffer.Count;

                        gyroscope = Vector3.zero;
                        foreach (var direction in directionBuffer)
                        {
                            gyroscope += direction;
                        }
                        gyroscope /= directionBuffer.Count;

                        // Use the parsed data
                        //Debug.Log("Accelerometer data received: " + acceleration);
                        // Debug.Log("Gyroscope data received: " + gyroscope);

                        foreach (var action in actionThresholds)
                        {
                            if (ThresholdCheck(acceleration, action.Value))
                            {
                                actionCallbacks[action.Key].Invoke();
                                cooldownTimer = 0f;
                            }
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error handling data: " + e.Message);
        }
    }

    private bool ThresholdCheck(Vector3 acceleration, ActionThreshold threshold)
    {
        if (threshold.enabled)
        {
            float dot = Vector3.Dot(acceleration.normalized, threshold.normDirection);
            float magnitude = acceleration.magnitude;

            if (dot > threshold.dotThreshold && magnitude > threshold.magnitudeThreshold)
            {
                return true;
            }
        }

        return false;
    }

    private void OnDestroy()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }

        instance = null;
    }

    void OnJumpDetected()
    {
        Debug.LogWarning("Jump detected at " + Time.time);
        // Add your jump handling code here
        playerMovement.Jump();
    }


    void OnLeftMotionDetected()
    {
        Debug.LogWarning("Left motion detected at " + Time.time);
        // Add your left motion handling code here
        playerMovement.MoveLeft();
    }


    void OnRightMotionDetected()
    {
        Debug.LogWarning("Right motion detected at " + Time.time);
        // Add your right motion handling code here
        playerMovement.MoveRight();
    }
}
