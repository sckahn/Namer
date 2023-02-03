using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using Newtonsoft.Json;

public class SaveLoadData
{
    // Create, Read
    // Update, Delete
    
#region JSON File

    public void CreateJsonFile<T>(T info, string filePath, string fileName)
    {
        if (!Directory.Exists(filePath + "/JSON/"))
        {
            Directory.CreateDirectory(filePath + "/JSON/");
        }
        
        string data = JsonConvert.SerializeObject(info, Formatting.Indented);
        File.WriteAllText(filePath + "/JSON/" + fileName, data);
    }
    
    public Dictionary<TK, TV> ReadJsonFile<TK,TV> (string filePath, string fileName) where TV : struct
    {
        Dictionary<TK, TV> dataDic = new Dictionary<TK, TV>();
        
        if (!File.Exists(filePath + "/JSON/" + fileName))
        {
            Debug.Log( filePath + "/JSON/" + fileName + " 파일이 없습니다! 원하는 씬으로 가서 맵 정보 파일을 먼저 생성해주세요.");
            return null;
        }
        
        string jsonFilePath = filePath.Replace("Assets/Resources/", "");
        fileName = fileName.Replace(".json", "");
        
        TextAsset textAsset = Resources.Load<TextAsset>(jsonFilePath + "/JSON/" + fileName);
        List<TV> infoList = JsonConvert.DeserializeObject<List<TV>>(textAsset.text);

        Type typeT = typeof(TV);
        foreach (TV info in infoList)
        {
            switch (typeT.FullName)
            {
                case "UserData" :
                    UserData userData = (UserData)(object)info;
                    TK userID = (TK)(object)userData.userID;
                    if (!dataDic.ContainsKey(userID))
                    {
                        dataDic.Add(userID, info);
                    }
                    break;
                case "LevelData" :
                    LevelData levelData = (LevelData)(object)info;
                    TK level = (TK)(object)levelData.level;
                    if (!dataDic.ContainsKey(level))
                    {
                        dataDic.Add(level, info);
                    }
                    break;
                case "ObjectInfo" :
                    ObjectInfo objectInfo = (ObjectInfo)(object)info;
                    TK objectID = (TK)(object)objectInfo.objectID;
                    if (!dataDic.ContainsKey(objectID))
                    {
                        dataDic.Add(objectID, info);
                    }
                    break;
            }
        }
        
        return dataDic;
    }

    public void UpdateJsonFile()
    {
        
    }

    public void DeleteJsonFile()
    {
        
    }
    
#endregion

#region CSV File

    public void CreateCsvFile(StringBuilder data, string filePath, string fileName)
    {
        if (!Directory.Exists(filePath + "/CSV/"))
        {
            Directory.CreateDirectory(filePath + "/CSV/");
        }
            
        StreamWriter outStream = File.CreateText(filePath + "/CSV/"+ fileName);
        outStream.Write(data);
        outStream.Close();
    }

    public StreamReader ReadCsvFile(string filePath, string fileName)
    {
        if (!File.Exists(filePath + "/CSV/" + fileName))
        {
            Debug.Log(fileName + " 파일이 없습니다! 원하는 씬으로 가서 맵 정보 파일을 먼저 생성해주세요.");
            return null;
        }
            
        return new StreamReader(filePath + "/CSV/" + fileName);
    }
    
#endregion

#region XML File
    
    
    
#endregion
}
