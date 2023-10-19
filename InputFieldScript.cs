using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;

public class InputFieldScript : MonoBehaviour
{
    public InputField inputCodeField;
    public InputField fileNameToSave;
    public InputField fileNameToLoad;
    private int caretPosition;

    public void Debug() => Debugger.DebugCode(inputCodeField.text);

    public void Save() => PlayerPrefs.SetString(fileNameToSave.text, inputCodeField.text);

    public void Load()
    {
        if (PlayerPrefs.HasKey(fileNameToLoad.text))
            inputCodeField.text = PlayerPrefs.GetString(fileNameToLoad.text);
        else
            fileNameToLoad.text = "Не найден файл с таким названием";
    }

    public void UpdateCaretPosition(int moveCaret = 0)
    {
        if (inputCodeField.caretPosition > inputCodeField.text.Length)
        {
            caretPosition = 0;
            return;
        }
        if (moveCaret == 0)
            caretPosition = inputCodeField.caretPosition;
        else
            caretPosition += moveCaret;
    }

    public void AddCommandIntoInput(string commandName)
    {
        //UnityEngine.Debug.Log(caretPosition);
        switch (commandName)
        {
            case "Comment":
                inputCodeField.text = inputCodeField.text.Insert(caretPosition, "//");
                UpdateCaretPosition(2);
                break;
            case "Move":
                inputCodeField.text = inputCodeField.text.Insert(caretPosition, "MOVE( , , );\n");
                UpdateCaretPosition(13);
                break;
            case "Point":
                inputCodeField.text = inputCodeField.text.Insert(caretPosition, "POINT_ ( , , ) NORMAL[ , , ];\n");
                UpdateCaretPosition(30);
                break;
            case "Plane":
                inputCodeField.text = inputCodeField.text.Insert(caretPosition, "PLANE_ (POINT_ , POINT_ , POINT_ );\n");
                UpdateCaretPosition(36);
                break;
            case "Circle":
                inputCodeField.text = inputCodeField.text.Insert(caretPosition, "CIRCLE_ (POINT_ , POINT_ , POINT_ );\n");
                UpdateCaretPosition(37);
                break;
            case "DeviationPoint":
                inputCodeField.text = inputCodeField.text.Insert(caretPosition, "DEVIATION-POINT_ ;\n");
                UpdateCaretPosition(19);
                break;
            case "DeviationCircle":
                inputCodeField.text = inputCodeField.text.Insert(caretPosition, "DEVIATION-CIRCLE_ ;\n");
                UpdateCaretPosition(20);
                break;
            case "Clear":
                inputCodeField.text = "";
                UpdateCaretPosition();
                break;
            default:
                break;
        }
    }
}
