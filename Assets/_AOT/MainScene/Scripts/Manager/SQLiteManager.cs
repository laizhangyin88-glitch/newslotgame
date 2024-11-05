using UnityEngine;
using Mono.Data.Sqlite;
using System.IO;
using System.Text;
using System;
using System.Collections.Generic;
using SlotMaker;
using System.Collections;

public partial class SQLiteManager : MonoSingleton<SQLiteManager>
{
    //string filePath = Path.Combine(Application.streamingAssetsPath, "SQLiteData.db");

    private static SqliteConnection connection;
    private static SqliteCommand command;
    private static SqliteDataReader dataReader;


    private string connectString;

    private bool _isReady = false;
    public bool isReady
    {
        get
        {
            return _isReady;
        }
    }

    private void Awake()
    {

#if UNITY_EDITOR
        connectString = "URI=file:" + Path.Combine(Application.streamingAssetsPath, "SQLiteData.db");
        _isReady = true;
        Debug.Log($"db url = {connectString}");
#elif UNITY_ANDROID
        StartCoroutine(copyDbFile());
#endif

    }

    IEnumerator copyDbFile()
    {
        string dataSandBoxPath = Application.persistentDataPath + "/SQLiteData.db";
        if (!Directory.Exists(Application.persistentDataPath))
        {
            Directory.CreateDirectory(Application.persistentDataPath);
        }

        WWW loadWWW = new WWW(Path.Combine(Application.streamingAssetsPath, "SQLiteData.db"));
        Debug.Log(Path.Combine(Application.streamingAssetsPath, "SQLiteData.db"));
        yield return loadWWW;
        File.WriteAllBytes(dataSandBoxPath, loadWWW.bytes);
        connectString = "URI=file:" + dataSandBoxPath;
        _isReady = true;
        Debug.Log($"db url = {connectString}");
    }


    /// <summary>
    /// 执行sql命令并返回结果
    /// </summary>
    /// <param name="sql"></param>
    /// <returns></returns>
    private  SqliteDataReader ExcuteSql(string sqlCmd)
    {
        command = connection.CreateCommand();

        command.CommandText = sqlCmd;

        dataReader = command.ExecuteReader();

        return dataReader;
    }





    /// <summary>
    /// 连接数据库
    /// </summary>
    /// <param name="path">数据库路径</param>
    public  void OpenSQLiteFile(string path = null) //connectString
    {
        try
        {
            //connection = new SqliteConnection($"URI=file:{path}");
            connection = new SqliteConnection(connectString);

            connection.Open();

            Debug.Log("SQLiteFile Open...");
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
    }


    /// <summary>
    /// 清空表
    /// </summary>
    /// <param name="tableName"></param>
    public void ClearTable(string tableName)
    {
        StringBuilder cmdSrt = new StringBuilder(20);

        cmdSrt.Append($"TRUNCATE TABLE {tableName};");

        ExcuteSql(cmdSrt.ToString());
    }


    /// <summary>
    /// 创建表
    /// sqlCmd:CREATE TABLE tableName (column1 datatype1,column2 datatype2,column3 ///datatype3,.....columnN datatypeN)
    /// </summary>
    /// <param name="tableName"> 表的名称</param>
    /// <param name="columns"></param>
    /// <param name="columnType">类型</param>
    public void CreateTable(string tableName, string[] columns, string[] columnType)
    {
        if (columns.Length != columnType.Length)
        {
            Debug.LogError("Colum's Length != ColumType's Length");
            return;
        }

        StringBuilder cmdSrt = new StringBuilder(20);

        cmdSrt.Append($"CREATE TABLE {tableName}(");

        for (int i = 0; i < columns.Length; i++)
        {
            if (i > 0)
            {
                cmdSrt.Append(",");
            }

            cmdSrt.Append($"{columns[i]} {columnType[i]}");
        }

        cmdSrt.Append(")");

        ExcuteSql(cmdSrt.ToString());
    }


    /// <summary>
    /// 检查表是否存在
    /// sqlCmd:SELECT COUNT(*) FROM sqlite_master WHERE type ='table' and name=tableName
    /// </summary>
    public  bool CheckTableExists(string tableName)
    {
        string sql = $"SELECT COUNT(*) FROM sqlite_master WHERE type ='table' and name='{tableName}';";

        command = connection.CreateCommand();
        command.CommandText = sql;

        int count = Convert.ToInt32(command.ExecuteScalar());

        bool isExists = count == 1;

        return isExists;
    }


    /// <summary>
    /// 向表中插入数据
    /// sqlCmd:INSERT INTO tableName VALUES(value1, value2, value3,...valueN)
    /// </summary>
    /// <param name="tableName"></param>
    /// <param name="insertDatas"></param>
    public  void InsertTableData(string tableName, string[] insertDatas)
    {
        if (insertDatas.Length == 0)
        {
            Debug.LogError("Values's length == 0");
        }

        StringBuilder cmdSrt = new StringBuilder(20);

        cmdSrt.Append($"INSERT INTO {tableName} VALUES(");

        for (int i = 0; i < insertDatas.Length; i++)
        {
            if (i > 0)
            {
                cmdSrt.Append(",");
            }

            cmdSrt.Append($"{insertDatas[i].ToString()}");
        }

        cmdSrt.Append(")");

        ExcuteSql(cmdSrt.ToString());

    }

    /// <summary>
    /// 向表中更新数据
    /// sqlCmd:UPDATE tableName SET column1 = value1, column2 = value2...., columnN = valueN
    /// </summary>
    /// <param name="tableName"></param>
    /// <param name="updateDatas"></param>
    public void UpdateTableData(string tableName, string[] updateDatas, string[] where = null)
    {
        if (updateDatas.Length == 0)
            Debug.LogError("Values's length == 0");

        StringBuilder cmdSrt = new StringBuilder(20);

        cmdSrt.Append($"UPDATE {tableName} SET ");

        for (int i = 0; i < updateDatas.Length; i += 2)
        {
            if (i > 0)
            {
                cmdSrt.Append(",");
            }
            cmdSrt.Append($"{updateDatas[i]}={updateDatas[i + 1]}");
        }

        if (where != null)
        {
            string whereStr = "";
            if (where != null)
            {
                for (int i = 0; i < where.Length; i += 2)
                {
                    if (i > 0)
                    {
                        whereStr += (",");
                    }
                    whereStr += $"{where[i]}={where[i + 1]}";
                }
            }
            cmdSrt.Append($" WHERE {whereStr}");
        }


        string _cmd = cmdSrt.ToString();
        //Debug.Log($"sql = {_cmd}");

        ExcuteSql(_cmd);

    }


    /// <summary>
    /// 获取表中所有的数据 
    /// sqlCmd:SELECT * FROM tableName
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="tableName"></param>
    /// <returns></returns>
    public  List<Dictionary<string, T>> GetTableAllData<T>(string tableName, string[] where = null)
    {
        List<Dictionary<string, T>> dataList = new List<Dictionary<string, T>>(20);

        string whereStr = "";
        if (where != null)
        {
            for (int i = 0; i < where.Length; i += 2)
            {
                if (i > 0)
                {
                    whereStr += (",");
                }
                whereStr += $"{where[i]}={where[i + 1]}";
            }
        }


        string sql = where == null ?  $"SELECT * FROM {tableName}" : $"SELECT * FROM {tableName} WHERE {whereStr}";

        //Debug.Log($"sql = {sql}");
        using (var reader = ExcuteSql(sql))
        {

            while (reader.Read())
            {
                Dictionary<string, T> dataDic = new Dictionary<string, T>(5);

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string key = reader.GetName(i);

                    object value = reader.GetValue(i);
                    dataDic.Add(key, (T)value);
                }

                dataList.Add(dataDic);
            }
        }

        return dataList;
    }

}

public partial class SQLiteManager : MonoSingleton<SQLiteManager>
{
    private void _GetPlayerPrefsTab()
    {
        if (connection == null)
        {
            //OpenSQLiteFile(filePath);
            OpenSQLiteFile();
        }

        if (!CheckTableExists("PlayerPrefs"))
        {
            CreateTable("PlayerPrefs", new string[] { "Key", "Value", "Type" }, new string[] { "TEXT", "TEXT", "TEXT" });
        }
    }
    public string GetString(string key, string defaultValue = null)
    {
        _GetPlayerPrefsTab();

        List<Dictionary<string, object>> data = GetTableAllData<object>("PlayerPrefs", new string[] { "Key", $"'{key}'" });

        if (data.Count > 0)
        {
            return (string)data[0]["Value"];

        }
        else if (defaultValue != null)
        {
            InsertTableData("PlayerPrefs", new string[] { $"'{key}'", $"'{defaultValue}'", $"'string'" });
            return defaultValue;
        }

        return null;
    }

    public void SetString(string key, string value)
    {
        _GetPlayerPrefsTab();

        List<Dictionary<string, object>> data = GetTableAllData<object>("PlayerPrefs", new string[] { "Key", $"'{key}'" });

        if (data.Count > 0)
        {
            UpdateTableData("PlayerPrefs", new string[] { "Value", $"'{value}'" }, new string[] { "Key", $"'{key}'" });
        }
        else
        {
            InsertTableData("PlayerPrefs", new string[] { $"'{key}'", $"'{value}'", $"'string'" });
        }
    }


    public float GetFloat(string key, float defaultValue)
    {

        _GetPlayerPrefsTab();

        List<Dictionary<string, object>> data = GetTableAllData<object>("PlayerPrefs", new string[] { "Key", $"'{key}'" });

        if (data.Count > 0)
        {
            return float.Parse((string)data[0]["Value"]);
        }
        else
        {
            InsertTableData("PlayerPrefs", new string[] { $"'{key}'", $"'{defaultValue}'", $"'float'" });
            return defaultValue;
        }
    }

    public void SetFloat(string key, float value)
    {
        _GetPlayerPrefsTab();

        List<Dictionary<string, object>> data = GetTableAllData<object>("PlayerPrefs", new string[] { "Key", $"'{key}'" });

        if (data.Count > 0)
        {
            UpdateTableData("PlayerPrefs", new string[] { "Value", $"'{value}'" }, new string[] { "Key", $"'{key}'" });
        }
        else
        {
            InsertTableData("PlayerPrefs", new string[] { $"'{key}'", $"'{value}'", $"'float'" });
        }
    }


    public int GetInt(string key, int defaultValue)
    {
        _GetPlayerPrefsTab();

        List<Dictionary<string, object>> data = GetTableAllData<object>("PlayerPrefs", new string[] { "Key", $"'{key}'" });

        if (data.Count > 0)
        {

            return int.Parse((string)data[0]["Value"]);
        }
        else
        {
            InsertTableData("PlayerPrefs", new string[] { $"'{key}'", $"'{defaultValue}'", $"'int'" });
            return defaultValue;
        }
    }

    public void SetInt(string key, int value)
    {
        _GetPlayerPrefsTab();

        List<Dictionary<string, object>> data = GetTableAllData<object>("PlayerPrefs", new string[] { "Key", $"'{key}'" });

        if (data.Count > 0)
        {
            UpdateTableData("PlayerPrefs", new string[] { "Value", $"'{value}'" }, new string[] { "Key", $"'{key}'" });
        }
        else
        {
            InsertTableData("PlayerPrefs", new string[] { $"'{key}'", $"'{value}'", $"'int'" });
        }
    }

}
