using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Xml;

public struct SNameInfo
{
    public readonly int priority;
    public readonly string uiText;
    public readonly EName name;
    public readonly EAdjective[] adjectives;
    public readonly string contentText;

    public SNameInfo(int priority, string uiText, EName name, EAdjective[] adjectives, string contentText = "")
    {
        this.priority = priority;
        this.uiText = uiText;
        this.name = name;
        this.adjectives = adjectives;
        this.contentText = contentText;
    }
}

public struct SAdjectiveInfo
{
    public readonly int priority;
    public readonly string uiText;
    public readonly EAdjective adjectiveName;
    public readonly IAdjective adjective;
    public readonly string contentText;

    public SAdjectiveInfo(int priority, string uiText, EAdjective adjectiveName, string contentText = "")
    {
        this.priority = priority;
        this.uiText = uiText;
        this.adjectiveName = adjectiveName;
        
        Type type = Type.GetType(adjectiveName + "Adj");
        var adjFunc = Activator.CreateInstance(type) as IAdjective;
        this.adjective = adjFunc;
        
        this.contentText = contentText;
    }
}

public class CardDataManager : Singleton<CardDataManager>
{
    private Dictionary<EName, SNameInfo> names = new Dictionary<EName, SNameInfo>();
    public Dictionary<EName, SNameInfo> Names { get { return names; } }

    private Dictionary<EAdjective, SAdjectiveInfo> adjectives = new Dictionary<EAdjective, SAdjectiveInfo>();
    public Dictionary<EAdjective, SAdjectiveInfo> Adjectives { get { return adjectives; } }

    private void Awake()
    {
        ReadXmlFile();
    }

    /*
    private void SetCardData()
    {
        if (nameData == null)
        {
            nameData = FindObjectOfType<NameData>();
        }

        foreach (NameInfo nameInfo in nameData.NameInfos)
        {
            Adjective[] enumAdjectives = nameInfo.adjectives;
            List<IAdjective> iAdjectives = new List<IAdjective>();
            for (int i = 0; i < enumAdjectives.Length - 1; i++)
            {
                Type type = Type.GetType(enumAdjectives[i] + "Adj");
                var adjective = Activator.CreateInstance(type) as IAdjective;
                if (adjective == null)
                {
                    Debug.Log( enumAdjectives[i] + " Adjective 인터페이스의 이름를 확인해주세요!");
                    return;
                }
                
                iAdjectives.Add(adjective);
            }

            names.Add(nameInfo.name, iAdjectives.ToArray());
        }

        int count = Enum.GetValues(typeof(Adjective)).Length;

        for (int i = 0; i < count - 1; i++)
        {
            Type type = Type.GetType((Adjective)i + "Adj");
            var adjective = Activator.CreateInstance(type) as IAdjective;
            if (adjective == null)
            {
                Debug.Log( (Adjective)i + " Adjective 인터페이스의 이름를 확인해주세요!");
                return;
            }
            
            adjectives.Add(adjective);
        }
    }
    */

    void ReadXmlFile()
    {
        TextAsset textAsset = Resources.Load("Data/Card/CardData") as TextAsset;
        XmlDocument xmlFile = new XmlDocument();
        xmlFile.LoadXml(textAsset.text);

        XmlNodeList nodeList = xmlFile.SelectNodes("root/worksheet");
        
        // Read Name Data of Card Data
        XmlNodeList nameNodeList = nodeList.Item(0).SelectNodes("Row");
        foreach (XmlNode nameItem in nameNodeList)
        {
            int priority = Convert.ToInt32(nameItem["우선순위"]?.InnerText);
            string uiText = nameItem["UI표시텍스트"]?.InnerText;
            EName itemName = (EName)Enum.Parse(typeof(EName), nameItem["이름명"]?.InnerText);
            
            string adjText = nameItem["보유꾸밈성질"]?.InnerText;
            EAdjective[] adjNames = null;
            if (adjText != "")
            {
                adjNames = adjText.Split(", ").Select(item => (EAdjective)Enum.Parse(typeof(EAdjective), item)).ToArray();
            }
            
            names.Add(itemName, new SNameInfo(priority, uiText, itemName, adjNames));
        }
        
        // Read Adjective Data of Card Data
        XmlNodeList adjNodeList = nodeList.Item(1).SelectNodes("Row");
        foreach (XmlNode adjItem in adjNodeList)
        {
            int priority = Convert.ToInt32(adjItem["우선순위"]?.InnerText);
            string uiText = adjItem["UI표시텍스트"]?.InnerText;
            EAdjective adjName = (EAdjective)Enum.Parse(typeof(EAdjective), adjItem["이름명"]?.InnerText);
            
            adjectives.Add(adjName, new SAdjectiveInfo(priority, uiText, adjName));
        }
    }
}
