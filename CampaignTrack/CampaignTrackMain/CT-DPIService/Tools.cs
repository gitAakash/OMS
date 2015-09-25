//Microsoft NameSpaces
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Xml;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Diagnostics;

class Tools
{
    #region FileLoggingMethods

    private static string _LogPath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
    private static string _InfoFilePath = _LogPath + "\\Info_" + DateTime.Now.ToString("MMM-yyyy") + ".txt";
    private static string _ErrorFilePath = _LogPath + "\\Error_" + DateTime.Now.ToString("MMM-yyyy") + ".txt";

    private static EventLog _MyLog = new EventLog();
    public static EventLog CurrentLog
    {
        get
        {
            return _MyLog;
        }
        set
        {
            _MyLog = value;
        }
    }




    public static string LogPath
    {
        get
        { return _LogPath; }

    }

    public static void CreateLogFiles(string pNameFormat)
    {

        _InfoFilePath = _LogPath + "\\Info_" + DateTime.Now.ToString(pNameFormat) + ".txt";
        _ErrorFilePath = _LogPath + "\\Error_" + DateTime.Now.ToString(pNameFormat) + ".txt";

        if ((Directory.Exists(_LogPath) == false))
        {
            Directory.CreateDirectory(_LogPath);
        }
        if ((File.Exists(_ErrorFilePath) == false))
        {
            FileStream objFs;
            objFs = File.Create(_ErrorFilePath);
            objFs.Close();
        }
        if ((File.Exists(_InfoFilePath) == false))
        {
            FileStream objFs;
            objFs = File.Create(_InfoFilePath);
            objFs.Close();
        }
    }

    public static bool WriteInfo(string message)
    {
        FileStream objFs;
        StreamWriter objSw;
        string strData = "";
        try
        {

            strData = DateTime.Now.ToString() + "::" + message;
            strData = ("\r\n" + strData);
            objFs = File.Open(_InfoFilePath, FileMode.Append, FileAccess.Write, FileShare.Write);
            objSw = new StreamWriter(objFs);
            objSw.Write(strData);
            objSw.Close();
            objFs.Close();
            return true;
        }
        catch (Exception Ex)
        {
            _MyLog.WriteEntry((DateTime.Now.ToString() + (":" + Ex.Message)), EventLogEntryType.Error);
            return false;
            //throw Ex;
        }
    }

    public static bool WriteError(string strData)
    {
        FileStream objFs;
        StreamWriter objSw;
        try
        {
            strData = ("\r\n" + strData);
            objFs = File.Open(_ErrorFilePath, FileMode.Append, FileAccess.Write, FileShare.Write);
            objSw = new StreamWriter(objFs);
            objSw.Write(strData);
            objSw.Close();
            objFs.Close();
            return true;
        }
        catch (Exception Ex)
        {
            //MyLog.WriteEntry((DateTime.Now.ToString() + (":" + ex.Message)), EventLogEntryType.Error);
            throw Ex;
        }
    }
    public static void WriteError(string Method, string Msg)
    {
        try
        {

            string fullmsg = "[Method Name-" + Method + "][ErrorMessage-" + Msg + "]";
            WriteError((DateTime.Now.ToString() + ("::" + fullmsg)));
            //UploadSuccess = false;
        }
        catch (Exception ex)
        {
            // _MyLog.WriteEntry((DateTime.Now.ToString() + (":" + ex.Message)), EventLogEntryType.Error);
        }
    }

    public static void DeleteOldLogFiles()
    {
        Tools.WriteInfo("Entered DeleteOldLogFiles()");

        try
        {

            DirectoryInfo ofs = new DirectoryInfo(Tools.LogPath);

            foreach (FileInfo oFile in ofs.GetFiles())
            {
                if ((oFile.Extension.ToLower() == ".txt") && ((oFile.Name.ToLower().Contains("info_") == true || oFile.Name.ToLower().Contains("error_") == true)))//&&(oFile.Name.Contains("Volumetric") == true)
                {
                    TimeSpan t1 = System.DateTime.Today.Date.Subtract(oFile.CreationTime.Date);
                    if (t1.TotalDays > 30)
                    {
                        oFile.Delete();
                    }
                }
            }
            ofs = null;

        }
        finally
        {
            Tools.WriteInfo("Exit DeleteOldLogFiles() Successfully.");
            Tools.WriteInfo("--------------------------------------------------------------------------------------------------------------");
        }
    }


    #endregion

    #region Config Methods

    public static string GetConfigValue(string pKeyName)
    {
        string str = System.Configuration.ConfigurationSettings.AppSettings[pKeyName].ToString();
        return str;
    }

    public static string GetConnectionString()
    {
        string str = GetConfigValue("ConnectionString"); //ConfigurationSettings.AppSettings("ConnectionString").ToString();
        return str;
    }

    #endregion



}