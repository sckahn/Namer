using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameDataManager : Singleton<GameDataManager>
{
    // Test
    private string firstUserID = "";
    
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
            Debug.LogError("인자 값을 확인해주세요. 레벨 클리어했다면 사용자 아이디가 필요합니다.");
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

    public void CreateMap(int level)
    {
        FilePathInfo();
        
        string sceneName = levelDataDic[level].sceneName;
        if (sceneName == null || sceneName == "")
        {
            Debug.LogError("Load Level를 입력해주세요");
            return;
        }
        
        SetCardEncyclopedia(levelDataDic[level].cardView);
        
        SaveLoadFile loadFile = new SaveLoadFile();
        StreamReader tileMapData = loadFile.ReadCsvFile(filePath + sceneName, tileMapFileName);
        StreamReader objectMapData = loadFile.ReadCsvFile(filePath + sceneName, objectMapFileName);
        Dictionary<int, SObjectInfo> objectInfoDic = loadFile.ReadJsonFile<int, SObjectInfo>(filePath + sceneName, objectInfoFileName);

        MapCreator mapCreator = gameObject.AddComponent<MapCreator>();
        initTiles = mapCreator.CreateTileMap(tileMapData);
        initObjects = mapCreator.CreateObjectMap(objectMapData, objectInfoDic);
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
            Debug.LogError("GameDataManager.GetInstance.GetUserAndLevelData()를 해주세요!");
            return;
        }
        
        if (!userDataDic.ContainsKey(userID))
        {
            SUserData userData = new SUserData(userID);
            userDataDic.Add(userID, userData);

            SaveLoadFile saveFile = new SaveLoadFile();
            saveFile.UpdateDicDataToJsonFile(userDataDic, filePath + "SaveLoad", userDataFileName);
        }
        
        // Test
        if (GameManager.GetInstance.userId == "000000")
        {
            GameManager.GetInstance.userId = userID;
            firstUserID = userID;
        }
        //
    }
    
    public string GetUserID()
    {
        return firstUserID;
    }

    public void SetLevelName(string levelName)
    {
        string userID = GameManager.GetInstance.userId;
        int level = GameManager.GetInstance.Level;
        bool hasLevelName = false;

        if (userDataDic[userID].levelNames.Count > 0)
        {
            for (int i = 0; i < UserDataDic[userID].levelNames.Count; i++)
            {
                if (UserDataDic[userID].levelNames[i].level == level)
                {
                    UserDataDic[userID].levelNames[i] = new SLevelName(level, levelName);
                    hasLevelName = true;
                    break;
                }
            }
        }
        
        if (!hasLevelName)
        {
            UserDataDic[userID].levelNames.Add(new SLevelName(level, levelName));
        }
    }

    public string GetLevelName(int level)
    {
        string userID = GameManager.GetInstance.userId;

        if (UserDataDic[userID].levelNames != null)
        {
            foreach (var levelName in UserDataDic[userID].levelNames)
            {
                if (levelName.level == level)
                {
                    return levelName.levelName;
                }
            }
        }
        
        return "";
    }

    public void UpdateUserData(bool isLevelClear)
    {
        if (isLevelClear)
        {
            string userID = GameManager.GetInstance.userId;
            int level = GameManager.GetInstance.Level;
            
            SUserData userData = userDataDic[userID];
            userData.clearLevel = level;
            userDataDic[userID] = userData;
        }

        SaveLoadFile saveFile = new SaveLoadFile();
        saveFile.UpdateDicDataToJsonFile(userDataDic, filePath + "SaveLoad", userDataFileName);
    }

    public void SetGameSetting(SGameSetting gameSetting)
    {
        string userID = GameManager.GetInstance.userId;
        
        SUserData userData = userDataDic[userID];
        userData.gameSetting = gameSetting;
        userDataDic[userID] = userData;
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
        
        SaveLoadFile saveFile = new SaveLoadFile();
        saveFile.UpdateDicDataToJsonFile(levelDataDic, filePath + "SaveLoad", levelDataFileName);
    }

#endregion

#region Get/Set Card Data & Create Card Prefabs

    public void GetCardData()
    {
        SaveLoadFile loadFile = new SaveLoadFile();
        XmlNodeList data = loadFile.ReadXmlFile("Data/SaveLoad/XML/CardData", "root/worksheet");
        
        names = loadFile.GetCardData<EName, SNameInfo>(data, 0);
        adjectives = loadFile.GetCardData<EAdjective, SAdjectiveInfo>(data, 1);
    }

    public void SetCardEncyclopedia(SCardView cardView)
    {
        int level = GameManager.GetInstance.Level;
        
        if (!cardEncyclopedia.ContainsKey(level))
        {
            SCardView sCardView = new SCardView();
            sCardView.nameRead = new List<EName>();
            sCardView.adjectiveRead = new List<EAdjective>();
            
            cardEncyclopedia.Add(level, sCardView);
        }

        List<EAdjective> adjectiveReads = new List<EAdjective>();
        for (int i = 0; i < cardView.nameRead.Count; i++)
        {
            if (!cardEncyclopedia[level].nameRead.Contains(cardView.nameRead[i]))
            {
                cardEncyclopedia[level].nameRead.Add(cardView.nameRead[i]);
            }
            
            if (Names[cardView.nameRead[i]].adjectives != null)
            {
                adjectiveReads.AddRange(Names[cardView.nameRead[i]].adjectives);
            }
        }
        
        if (adjectiveReads.Count > 0)
        {
            adjectiveReads.AddRange(cardView.adjectiveRead); 
            adjectiveReads.Distinct().ToList();
            adjectiveReads.OrderBy(item => Adjectives[item].priority).ToList();
        }
        
        for (int i = 0; i < adjectiveReads.Count; i++)
        {
            if (!cardEncyclopedia[level].adjectiveRead.Contains(adjectiveReads[i]))
            {
                cardEncyclopedia[level].adjectiveRead.Add(adjectiveReads[i]);
            }
        }
    }
    
    // Create Card Prefabs
    public GameObject[] GetIngameCardEncyclopedia()
    {
        return GetCardPrefabs(CardEncyclopedia[GameManager.GetInstance.Level]);
    }

    public GameObject[] GetMainCardEncyclopedia()
    {
        return GetCardPrefabs(UserDataDic[GameManager.GetInstance.userId].cardView);
    }

    public GameObject[] GetRewardCardEncyclopedia()
    {
        int level = GameManager.GetInstance.Level;
        string userID = GameManager.GetInstance.userId;
        
        HashSet<EName> nameReads = new HashSet<EName>();
        HashSet<EAdjective> adjectiveReads = new HashSet<EAdjective>();

        foreach (EName name in CardEncyclopedia[level].nameRead)
        {
            if (!UserDataDic[userID].cardView.nameRead.Contains(name))
            {
                nameReads.Add(name);

                if (Names[name].adjectives != null)
                {
                    foreach (var adjective in Names[name].adjectives)
                    {
                        if (!UserDataDic[userID].cardView.adjectiveRead.Contains(adjective))
                        {
                            adjectiveReads.Add(adjective);
                        }
                    }
                }
            }
        }

        foreach (var adjective in CardEncyclopedia[level].adjectiveRead)
        {
            if (!UserDataDic[userID].cardView.adjectiveRead.Contains(adjective))
            {
                adjectiveReads.Add(adjective);
            }
        }
        
        nameReads = Enumerable.ToHashSet(nameReads.OrderBy(item => Names[item].priority));
        adjectiveReads = Enumerable.ToHashSet(adjectiveReads.OrderBy(item => Adjectives[item].priority));

        // update user data
        SUserData userData = UserDataDic[userID];
        userData.cardView.nameRead.AddRange(nameReads);
        userData.cardView.nameRead.OrderBy(item => Names[item].priority).ToList();
        userData.cardView.adjectiveRead.AddRange(adjectiveReads);
        userData.cardView.adjectiveRead.OrderBy(item => Adjectives[item].priority).ToList();
        // 

        return GetCardPrefabs(new SCardView(nameReads.ToList(), adjectiveReads.ToList()));
    }
    
    public GameObject[] GetCardPrefabs(SCardView cardView)
    {
        if (cardView.nameRead.Count == 0 && cardView.adjectiveRead.Count == 0)
        {
            return null;
        }
        
        List<EName> nameReads = cardView.nameRead;
        List<EAdjective> adjectiveReads = cardView.adjectiveRead;
        
        List<GameObject> cards = new List<GameObject>();
        for (int i = 0; i < nameReads.Count; i++)
        {
            if ((int)nameReads[i] > names.Count - 1)
            {
                Debug.LogError("입력할 수 있는 네임 카드 번호를 벗어났어요!");
            }

            GameObject cardPrefab = Resources.Load("Prefabs/Cards/01. NameCard/" + names[nameReads[i]].cardPrefabName) as GameObject;
            cards.Add(cardPrefab);
        }

        for (int i = 0; i < adjectiveReads.Count; i++)
        {
            if ((int)adjectiveReads[i] > adjectives.Count - 1)
            {
                Debug.LogError("입력할 수 있는 꾸밈 성질 카드 번호를 벗어났어요!");
            }

            GameObject cardPrefab = Resources.Load("Prefabs/Cards/02. AdjustCard/" + adjectives[adjectiveReads[i]].cardPrefabName) as GameObject;
            cards.Add(cardPrefab);
        }
        
        return cards.ToArray();
    }

#endregion


}