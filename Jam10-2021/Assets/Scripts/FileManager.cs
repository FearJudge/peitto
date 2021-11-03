using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FileManager : MonoBehaviour
{
    static string path;
    List<TextData> ruleDatas = new List<TextData>
    {
        new TextData("I can not jump twice", "In order to jump, one must", "Exert force against a surface.", new string[] { "Exert force against a surface." }),
        new TextData("Trees are tangible", "Even the most freeminded spirit can not", "push through solid matter.", new string[] { "push through solid matter.", "collide with Trees." }),
    };
    // Start is called before the first frame update
    void Start()
    {
        path = Application.dataPath + "\\..\\";
        CreateFile(ruleDatas[0]);
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus) { return; }
        int a = CheckFileForAnswers(ruleDatas[0]);
        Debug.Log(a);
        if (a == 0 || a == 1) { Debug.Log("Hello there to you too!"); }
        if (a == 2) { Debug.Log("WOW!"); }
    }

    static void CreateFile(TextData SerData)
    {
        StreamWriter filustreamu = new StreamWriter(path + SerData.filename + ".txt");
        filustreamu.WriteLine(FileStart(0));
        filustreamu.WriteLine(SerData.WriteText(0));
        filustreamu.Close();
    }

    static int CheckFileForAnswers(TextData SerData)
    {
        if (SerData == null) { return -2; }
        if (!File.Exists(path + SerData.filename + ".txt")) { return 0; }
        StreamReader filustreamu = new StreamReader(path + SerData.filename + ".txt");
        filustreamu.ReadLine();
        string answer = filustreamu.ReadLine().Substring(SerData.lines[0].preamble.Length);
        filustreamu.Close();
        return SerData.GetCase(answer, 0);
    }

    static string FileStart(int linestart)
    {
        string[] fileStartLines =  new string[]
        {
            "If you have found this, know that you are not alone. We too are trapped here.",
            "Dear Diary:",
            "To any who might find this:",
            "Am I paranoid?",
        };
        return fileStartLines[0];
    }
}

public class TextData
{
    public static string Clean(string s)
    {
        return s.ToLower().Replace(" ", "").Replace(".", "").Replace(",", "");
    }

    public TextData(string name)
    {
        filename = name;
    }

    public TextData(string name, string start, string basevalue, string[] acceptedAnswers)
    {
        filename = name;
        lines = new DataLine[1]
        {
            new DataLine()
            {
                preamble = start,
                inputpoint = basevalue,
                AcceptedData = acceptedAnswers
            }
        };
    }

    public string WriteText(int line)
    {
        return lines[line].preamble + " " + lines[line].inputpoint;
    }

    public int GetCase(string withInput, int line)
    {
        withInput = Clean(withInput);
        int index = 0;
        foreach (string item in lines[line].AcceptedData)
        {
            if (withInput == Clean(item)) { return index; }
            index++;
        }
        return -1;
    }

    public string filename = "Undefined";
    public DataLine[] lines;
    public class DataLine
    {
        public string preamble = "[VAR1]:";
        public string inputpoint = "---";
        public string[] AcceptedData = new string[0];
    }
    
}
