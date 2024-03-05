using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CalibrationUI : MonoBehaviour
{
    public TMP_Text labelTextField;
    public TMP_Text timerTextField;

    public void SetLabelText(string label, string timer)
    {
        labelTextField.text = label;
        timerTextField.text = timer;
    }
}
