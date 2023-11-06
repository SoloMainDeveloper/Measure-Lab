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
    public InputField resultsToSave;
    public InputField resultToLoad;
    public InputField output;
    private int caretPosition;

    public void Quit() => Application.Quit();

    public void Run() => Debugger.RunCode(output, inputCodeField.text, 0);

    public void Debug() => Debugger.RunCode(output, inputCodeField.text, 1);

    public void Save() => PlayerPrefs.SetString(fileNameToSave.text, inputCodeField.text);

    public void SaveResults() => PlayerPrefs.SetString("results/" + resultsToSave.text, output.text);

    public void LoadResults()
    {
        if (PlayerPrefs.HasKey("results/" + resultToLoad.text))
            output.text = $"Результаты компиляции программы \'<b>{resultToLoad.text}</b>\':\n{PlayerPrefs.GetString("results/" + resultToLoad.text)}";
        else
            resultToLoad.text = "Не найден файл с таким названием";
    }

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
        string stringToAdd = null;
        switch (commandName)
        {
            case "Comment":
                stringToAdd = "//";
                break;
            case "Move":
                stringToAdd = "MOVE( , , );\n";
                break;
            case "Point":
                stringToAdd = "POINT_ ( , , ) NORMAL[ , , ];\n";
                break;
            case "Plane":
                stringToAdd = "PLANE_ (POINT_ , POINT_ , POINT_ );\n";
                break;
            case "Circle":
                stringToAdd = "CIRCLE_ (POINT_ , POINT_ , POINT_ );\n";
                break;
            case "DeviationPoint":
                stringToAdd = "DEVIATION-POINT_ ( );\n";
                break;
            case "DeviationCircle":
                stringToAdd = "DEVIATION-CIRCLE_ ( , );\n";
                break;
            case "Projection":
                stringToAdd = "POINT-BY-PROJECTION_ (PLANE_ , POINT_ );\n";
                break;
            case "Clear":
                inputCodeField.text = "";
                UpdateCaretPosition();
                break;
            case "ClearOutput":
                output.text = "";
                Debugger.ReloadCommandNumber();
                break;
            default:
                break;
        }
        if (stringToAdd != null)
        {
            inputCodeField.text = inputCodeField.text.Insert(caretPosition, stringToAdd);
            UpdateCaretPosition(stringToAdd.Length);
        }
            
    }
}
