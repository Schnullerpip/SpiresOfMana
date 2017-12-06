using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Rewired.Dev.Tools;
using UnityEngine;
using UnityEngine.Networking;

public class ICheck : NetworkBehaviour
{
    public string[] fileName;
    public string preferedFileExtension = ".txt";
    public string preferedFileLocation = "Assets/StatsCollection/";
    /// <summary>
    /// so the different log files can be associated to a certain game
    /// </summary>
    private string mUniqueIdentifier;


    public void Start()
    {

        if (!isServer)
        {
            Destroy(this.gameObject);
            return;
        }

        //if nothing was assigned - use the default path
        if (preferedFileLocation == "")
        {
            preferedFileLocation = "LogData";
        }
        var logDirectoryPath = Application.dataPath + "/StatsCollection";
        var logFileDirectoryPath = logDirectoryPath + "/" + preferedFileLocation;
        if (!System.IO.Directory.Exists(logDirectoryPath))
        {
            Directory.CreateDirectory(logDirectoryPath);
        }
        if (!System.IO.Directory.Exists(logFileDirectoryPath))
        {
            Directory.CreateDirectory(logFileDirectoryPath);
        }
        preferedFileLocation = logFileDirectoryPath;
        if (!preferedFileLocation.EndsWith("/"))
        {
            preferedFileLocation = preferedFileLocation += "/";
        }

        Debug.Log("dataPath: "+preferedFileLocation);

        //initialize unique identifier
	    mUniqueIdentifier = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        //make the dateformate filesystemcompliant
        mUniqueIdentifier = mUniqueIdentifier.Replace('/', '_');
        mUniqueIdentifier = mUniqueIdentifier.Replace('\\', '_');
        mUniqueIdentifier = mUniqueIdentifier.Replace(':', '-');

        for (int i = 0; i < fileName.Length; ++i)
        {
            var val = fileName[i];
            //lead the files to their dedicated destination
            //make sure they are treated as txt files 
            fileName[i] = preferedFileLocation + mUniqueIdentifier + val;
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