using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FileManager : MonoBehaviour
{
    static string path;
    TextData basedata;
    // Start is called before the first frame update
    void Start()
    {
        path = Application.dataPath.Replace("Assets", "");
        basedata = new TextData("Journal01", "If you see this, please answer me:", 3, new string[] {"hi", "hello", "wow" });
        CreateFile(basedata);
    }

    private void OnApplicationFocus(bool focus)
    {
        int a = CheckFileForAnswers(basedata);
        Debug.Log(a);
        if (a == 0 || a == 1) { Debug.Log("Hello there to you too!"); }
        if (a == 2) { Debug.Log("WOW!"); }
    }

    static void CreateFile(TextData SerData)
    {
        StreamWriter filustreamu = new StreamWriter(path + SerData.filename + ".txt");
        filustreamu.WriteLine(RandomFileStart());
        filustreamu.WriteLine(SerData.WriteText());
        filustreamu.Close();
    }

    static int CheckFileForAnswers(TextData SerData)
    {
        if (SerData == null) { return -1; }
        StreamReader filustreamu = new StreamReader(path + SerData.filename + ".txt");
        filustreamu.ReadLine();
        string answer = filustreamu.ReadLine().Substring(SerData.preamble.Length);
        filustreamu.Close();
        return SerData.GetCase(answer);
    }

    static string RandomFileStart()
    {
        string[] randomizedLines =  new string[]
        {
            "If you have found this, know that you are not alone. We too are trapped here.",
            "Dear Diary:",
            "To any who might find this:",
            "Am I paranoid?",
        };
        return randomizedLines[Random.Range(0, randomizedLines.Length)];
    }
}

public class TextData
{
    public static string Clean(string s)
    {
        return s.ToLower().Replace(" ", "").Replace(".", "").Replace(",", "");
    }

    public TextData(string name, string start, int inputlength, string[] acceptedAnswers)
    {
        filename = name;
        preamble = start;
        inputpoint = "---";
        AcceptedData = acceptedAnswers;
    }

    public string WriteText()
    {
        return preamble + " " + inputpoint;
    }

    public int GetCase(string withInput)
    {
        withInput = Clean(withInput);
        int index = 0;
        foreach (string item in AcceptedData)
        {
            if (withInput == Clean(item)) { return index; }
            index++;
        }
        return index;
    }

    public string filename = "Undefined";
    public string preamble = "[VAR1]:";
    public string inputpoint = "---";
    public string[] AcceptedData = new string[0];
}
