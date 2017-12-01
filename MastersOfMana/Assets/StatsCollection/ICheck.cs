using System.Collections;
using System.Collections.Generic;
using System.IO;
using Rewired.Dev.Tools;
using UnityEngine;

public class ICheck : MonoBehaviour
{
    public string[] fileName;
    public string preferedFileExtension = ".txt";
    public string preferedFileLocation = "Assets/StatsCollection/";


    public void Start()
    {
        for (int i = 0; i < fileName.Length; ++i)
        {
            var val = fileName[i];
            //lead the files to their dedicated destination
            //make sure they are treated as txt files 
            fileName[i] = preferedFileLocation + val + (val.EndsWith(preferedFileExtension) ? "" : preferedFileExtension);
        }
    }

    protected void WriteString(string msg, string path, bool append = true)
    {
        //Write some text to the test.txt file
        using (StreamWriter writer = File.AppendText(path))
        {
            writer.WriteLine(msg);
            writer.Close();
        }
    }
}