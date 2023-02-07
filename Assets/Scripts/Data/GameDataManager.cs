using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameDataManager : Singleton<GameDataManager>
{
    // map information - 3 dimensional space array
    private GameObject[,,] initTiles;
    public GameObject[,,] InitTiles { get { return initTiles; } set { initTiles = value; } }
    
    private GameObject[,,] initObjects;
    public GameObject[,,] InitObjects { get { return initObjects; } set { initObjects = value; } }
    private Dictionary<int, SCardView> cardEncyclopedia = new Dictionary<int, SCardView>();
    public Dictionary<int, SCardView> CardEncyclopedia { get { return cardEncyclopedia; } }

    // user information dictionary
    private Dictionary<string, SUserData> userDataDic = new Dictionary<string, SUserData>();
    public Dictionary<string, SUserData> UserDataDic { get { return userDataDic; }}
    
    // level information dictionary
    private Dictionary<int, SLevelData> levelDataDic = new Dictionary<int, SLevelData>();
    public Dictionary<int, SLevelData> LevelDataDic { get { return levelDataDic; } }
    
    // card information dictionary
    private Dictionary<EName, SNameInfo> names = new Dictionary<EName, SNameInfo>();
    public Dictionary<EName, SNameInfo> Names { get { return names; } }

    private Dictionary<EAdjective, SAdjectiveInfo> adjectives = new Dictionary<EAdjective, SAdjectiveInfo>();
    public Dictionary<EAdjective, SAdjectiveInfo> Adjectives { get { return adjectives; } }
    
    // file information
    private string filePath;
    private string tileMapFileName;
    private string objectMapFileName;
    private string objectInfoFileName;
    private string userDataFileName;
    private string levelDataFileName;
    
    private void FilePathInfo()
    {
        filePath = "Assets/Resources/Data/";
        
        tileMapFileName = "tileMapData.csv";
        objectMapFileName = "objectMapData.csv";
        objectInfoFileName = "objectInfoData.json";

        userDataFileName = "user.json";
        levelDataFileName = "levels.json";
    }

#region Map(tile, object) Data 

    public void CreateFile(int level, string userID = "", bool isLevelClear = false)
    {
        FilePathInfo();
        if ((userID == "" && isLevelClear) || (userID != "" && !isLevelClear))
        {
            Debug.Log("인자 값을 확인해주세요. 레벨 클리어했다면 사용자 아이디가 필요합니다.");
            return;
        }
        
        string sceneName = isLevelClear ? levelDataDic[level].sceneName : SceneManager.GetActiveScene().name;
        
        MapReader mapReader = gameObject.AddComponent<MapReader>();
        SMapData mapData = mapReader.GetMapData();
        
        SaveLoadFile saveFile = new SaveLoadFile();
        saveFile.CreateCsvFile(mapData.tileMapData, filePath + sceneName, userID + tileMapFileName);
        saveFile.CreateCsvFile(mapData.objectMapData, filePath + sceneName, userID + objectMapFileName);
        saveFile.CreateJsonFile(mapData.objectInfoData, filePath + sceneName, userID + objectInfoFileName);
    }

    public void CreateMap(int level, string userID = "")
    {
        FilePathInfo();
        
        string sceneName = levelDataDic[level].sceneName;
        if (sceneName == null || sceneName == "")
        {
            Debug.Log("Load Level를 입력해주세요");
            return;
        }

        string frontFileName = "";
        if (userID != "")
        {
            frontFileName = level <= userDataDic[userID].clearLevel ? userID : "";
        }

        SaveLoadFile loadFile = new SaveLoadFile();
        StreamReader tileMapData = loadFile.ReadCsvFile(filePath + sceneName, frontFileName + tileMapFileName);
        StreamReader objectMapData = loadFile.ReadCsvFile(filePath + sceneName, frontFileName + objectMapFileName);
        Dictionary<int, SObjectInfo> objectInfoDic = loadFile.ReadJsonFile<int, SObjectInfo>(filePath + sceneName, frontFileName + objectInfoFileName);

        MapCreator mapCreator = gameObject.AddComponent<MapCreator>();
        initTiles = mapCreator.CreateTileMap(tileMapData);
        initObjects = mapCreator.CreateObjectMap(objectMapData, objectInfoDic);
        
        SetCardEncyclopedia(level, levelDataDic[level].cardView);
        UpdateUserData(level, userID, false);
    }
    
#endregion

#region User And Level Data

    public void GetUserAndLevelData()
    {
        FilePathInfo();
        
        SaveLoadFile loadFile = new SaveLoadFile();
        userDataDic = loadFile.ReadJsonFile<string, SUserData>(filePath + "SaveLoad", userDataFileName);
        levelDataDic = loadFile.ReadJsonFile<int, SLevelData>(filePath + "SaveLoad", levelDataFileName);
    }

    public void AddUserData(string userID)
    {
        if (!userDataDic.ContainsKey("000000"))
        {
            Debug.Log("GameDataManager.GetInstance.GetUserAndLevelData()를 해주세요!");
            return;
        }
        
        if (!userDataDic.ContainsKey(userID))
        {
            userDataDic.Add(userID, userDataDic["000000"]);
        }
        
        SaveLoadFile saveFile = new SaveLoadFile();
        saveFile.UpdateDicDataToJsonFile(userDataDic, filePath + "SaveLoad", userDataFileName);
    }
    
    public void UpdateUserData(int level, string userID, bool isLevelClear)
    {
        if (!userDataDic.ContainsKey(userID))
        {
            userDataDic.Add(userID, new SUserData());
        }
        
        SUserData userData = userDataDic[userID];
        if (isLevelClear)
        {
            userData.clearLevel = level;
            userData.levelScore.Add(10);
        }
        else
        {
            if (userData.cardView.nameRead == null)
            {
                userData.cardView.nameRead = new List<EName>();
            }
            if (userData.cardView.adjectiveRead == null)
            {
                userData.cardView.adjectiveRead = new List<EAdjective>();
            }
            
            userData.cardView.nameRead.AddRange(levelDataDic[level].cardView.nameRead);
            userData.cardView.nameRead = userData.cardView.nameRead.Distinct().ToList();
            userData.cardView.adjectiveRead.AddRange(levelDataDic[level].cardView.adjectiveRead);
            userData.cardView.adjectiveRead = userData.cardView.adjectiveRead.Distinct().ToList();
        }
        userDataDic[userID] = userData;
        
        if (isLevelClear)
        {
            SaveLoadFile saveFile = new SaveLoadFile();
            saveFile.UpdateDicDataToJsonFile(userDataDic, filePath + "SaveLoad", userDataFileName);
        }
    }

    public void UpdateLevelData(int level, SLevelData levelData)
    {
        if (levelDataDic.ContainsKey(level))
        {
            levelDataDic[level] = levelData;
        }
        else
        {
            levelDataDic.Add(level, levelData);
        }
    }

#endregion

#region Card Data

    public void GetCardData()
    {
        SaveLoadFile loadFile = new SaveLoadFile();
        XmlNodeList data = loadFile.ReadXmlFile("Data/SaveLoad/XML/CardData", "root/worksheet");
        
        names = loadFile.GetCardData<EName, SNameInfo>(data, 0);
        adjectives = loadFile.GetCardData<EAdjective, SAdjectiveInfo>(data, 1);
    }

    public void SetCardEncyclopedia(int level, SCardView cardView)
    {
        if (!cardEncyclopedia.ContainsKey(level))
        {
            SCardView sCardView = new SCardView();
            sCardView.nameRead = new List<EName>();
            sCardView.adjectiveRead = new List<EAdjective>();
            
            cardEncyclopedia.Add(level, sCardView);
        }

        for (int i = 0; i < cardView.nameRead.Count; i++)
        {
            if (!cardEncyclopedia[level].nameRead.Contains(cardView.nameRead[i]))
            {
                cardEncyclopedia[level].nameRead.Add(cardView.nameRead[i]);
            }
        }
        
        for (int i = 0; i < cardView.adjectiveRead.Count; i++)
        {
            if (!cardEncyclopedia[level].adjectiveRead.Contains(cardView.adjectiveRead[i]))
            {
                cardEncyclopedia[level].adjectiveRead.Add(cardView.adjectiveRead[i]);
            }
        }
    }
    
    // Create Card Prefabs
    public GameObject[] GetIngameCardEncyclopedia(int level)
    {
        return GetCardPrefabs(CardEncyclopedia[level]);
    }

    public GameObject[] GetMainCardEncyclopedia(string userID)
    {
        return GetCardPrefabs(UserDataDic[userID].cardView);
    }
    
    public GameObject[] GetCardPrefabs(SCardView cardView)
    {
        if (cardView.nameRead.Count == 0 && cardView.adjectiveRead.Count == 0)
        {
            Debug.Log("볼 수 있는 카드가 없어요");
            return null;
        }
        
        List<EName> nameReads = cardView.nameRead;
        List<EAdjective> adjectiveReads = cardView.adjectiveRead;
        
        List<GameObject> cards = new List<GameObject>();
        for (int i = 0; i < nameReads.Count; i++)
        {
            GameObject cardPrefab = Resources.Load("Prefabs/Cards/01. NameCard/" + names[nameReads[i]].cardPrefabName) as GameObject;
            cards.Add(cardPrefab);
        }

        for (int i = 0; i < adjectiveReads.Count; i++)
        {
            GameObject cardPrefab = Resources.Load("Prefabs/Cards/02. AdjustCard/" + adjectives[adjectiveReads[i]].cardPrefabName) as GameObject;
            cards.Add(cardPrefab);
        }
        
        return cards.ToArray();
    }

#endregion

#region LevelClear

    public void UpdateLevelClearData(int level, string userID)
    {
        UpdateUserData(level, userID, true);
        CreateFile(level, userID, true);
    }

#endregion

    
}