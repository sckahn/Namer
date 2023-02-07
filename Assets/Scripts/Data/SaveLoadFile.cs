using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Newtonsoft.Json;

public class SaveLoadFile
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
        
        string data = JsonConvert.SerializeObject(info, Newtonsoft.Json.Formatting.Indented);
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

        Type typeTV = typeof(TV);
        foreach (TV info in infoList)
        {
            switch (typeTV.FullName)
            {
                case "SUserData" :
                    SUserData userData = (SUserData)(object)info;
                    TK userID = (TK)(object)userData.userID;
                    if (!dataDic.ContainsKey(userID))
                    {
                        dataDic.Add(userID, info);
                    }
                    break;
                case "SLevelData" :
                    SLevelData levelData = (SLevelData)(object)info;
                    TK level = (TK)(object)levelData.level;
                    if (!dataDic.ContainsKey(level))
                    {
                        dataDic.Add(level, info);
                    }
                    break;
                case "SObjectInfo" :
                    SObjectInfo objectInfo = (SObjectInfo)(object)info;
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
    
    // 업데이트가 필요한 자료가 딕션너리인 경우 사용하는 함수
    // 만약 업데이트가 필요한 자료가 리스트인 경우, CreateJsonFile을 사용할 것!
    public void UpdateDicDataToJsonFile<TK, TV>(Dictionary<TK, TV> dataDic, string filePath, string fileName)
    {
        List<TV> dataList = new List<TV>();
        foreach (var data in dataDic)
        {
            dataList.Add(data.Value);
        }
        
        CreateJsonFile(dataList, filePath, fileName);
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

    public XmlNodeList ReadXmlFile(string filePath, string xPath)
    {
        TextAsset textAsset = Resources.Load(filePath) as TextAsset;
        XmlDocument xmlFile = new XmlDocument();
        xmlFile.LoadXml(textAsset.text);

        return xmlFile.SelectNodes(xPath);
    }
    
    public Dictionary<TK, TV> GetCardData<TK, TV>(XmlNodeList data, int index)
    {
        if (data == null)
        {
            Debug.Log("XML 데이터를 제대로 가져오지 못헀습니다. 확인 부탁요!");
            return null;
        }
        
        Dictionary<TK, TV> dataDic = new Dictionary<TK, TV>();
        
        // index == 0 : Name, index == 1 : Adjective
        XmlNodeList nodeList = data.Item(index).SelectNodes("Row");
        foreach (XmlNode item in nodeList)
        {
            if (index == 0)
            {
                int priority = Convert.ToInt32(item["우선순위"]?.InnerText);
                string uiText = item["UI표시텍스트"]?.InnerText;
                EName itemName = (EName)Enum.Parse(typeof(EName), item["이름명"]?.InnerText);
            
                string adjText = item["보유꾸밈성질"]?.InnerText;
                EAdjective[] adjNames = null;
                if (adjText != "")
                {
                    adjNames = adjText.Split(", ").Select(item => (EAdjective)Enum.Parse(typeof(EAdjective), item)).ToArray();
                }
                
                string cardPrefabName = item["카드프리팹이름"]?.InnerText;
                string content = item["설명"]?.InnerText;

                TK key = (TK)(object)itemName;
                if (!dataDic.ContainsKey(key))
                {
                    TV value = (TV)(object)new SNameInfo(priority, uiText, itemName, adjNames, cardPrefabName, content);
                    dataDic.Add(key, value);
                }
            }
            else if (index == 1)
            {
                int priority = Convert.ToInt32(item["우선순위"]?.InnerText);
                string uiText = item["UI표시텍스트"]?.InnerText;
                EAdjective adjName = (EAdjective)Enum.Parse(typeof(EAdjective), item["이름명"]?.InnerText);
                string cardPrefabName = item["카드프리팹이름"]?.InnerText;
                string content = item["설명"]?.InnerText;
                    
                TK key = (TK)(object)adjName;
                if (!dataDic.ContainsKey(key))
                {
                    TV value = (TV)(object)new SAdjectiveInfo(priority, uiText, adjName, cardPrefabName, content);
                    dataDic.Add(key, value);
                }
            }
        }

        return dataDic;
    }

#endregion
}
