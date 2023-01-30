using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.WSA;

public class DetectManager : Singleton<DetectManager>
{
    private GameObject[,,] currentObjects;
    private GameObject[,,] previousObjects;

    protected GameObject[,,] currentTiles;

    MapDataManager mapManager;
    LevelInfos levelInfos;
    Dictionary<Vector3, GameObject> scaleChangedObjects = new Dictionary<Vector3, GameObject>();

    int maxX = 20;
    int maxY = 9;
    int maxZ = 20;

    int tileMaxX = 20;
    int tileMaxY = 9;
    int tileMaxZ = 20;

    // test
    private GameObject target;
    private Dir ECheckDir;
    public List<GameObject> ForTestPurpos;

    #region Detect region
    /* 1. 포룹돌려서 하나씩빼는거?
       2. 검출 로직 돌릴때 파일 가져와서 로드할때 
       3. 인디케이터는 배열의 하나 하나 씩 확인하는거 
     */

    //1. 인디케이터 돌면서 블럭들의 현재위치 확인
    //2. prevBlockObjs = blockObjs
    //3. blockObj = objIndicator


    /*
     * TestCases
     * 
     */
    #region ChangesInBlockObjArrRelatied

    /*
   * TestCases
   * 
   */
    //[ContextMenu("update blockObjs")]
    //public void MapChanged()
    //{
    //    Array.Clear(currentObjects, 0, currentObjects.Length);
    //    bool delay = Indicator.GetInstance.CreateNewLevel();
    //    StartCoroutine(WaitForCreateNewLevel(delay));
    //}

    //IEnumerator WaitForCreateNewLevel(bool delay)
    //{
    //    while (!delay)
    //    {
    //        yield return null;
    //    }
    //    Debug.Log("CreateNewLevel Finished");

    //}

    //이전 배열과 현재 배열을 비교해서 포지션 변화가 있는 오브젝트를 리턴해준다.
    // 배열이 바뀌고 prev 랑 현재 값이 바뀐걸 알아야 한다.
    /* ---DetectMovedGameObject TestCases---
     * 1. 아무것도 없는 스페이스로 오브젝트가 이동한경우
     * 2. 2가지가 이동된 경우
     * 3. 물체가 떨어지는 경우는 떨어지기 시작한 지점에서 부르면=> 변화가 없음으로 안된다.
     *      물체가 다떨어진 경우는 => 배열상 변화가 되어서 될듯
     */
    public HashSet<GameObject> DetectMovedGameObject()
    {
        HashSet<GameObject> changedGameObj = new HashSet<GameObject>();

        for (int x = 0; x < currentObjects.GetLength(0); x++)
        {
            for (int y = 0; y < currentObjects.GetLength(1); y++)
            {
                for (int z = 0; z < currentObjects.GetLength(2); z++)
                {
                    //null 인경우는 pass
                    if (currentObjects[x, y, z] == null) continue;

                    // 물체가 원래 아무것도 없는 곳으로 이동한 경우
                    if (currentObjects[x, y, z] != null && previousObjects[x, y, z] == null)
                    {
                        changedGameObj.Add(currentObjects[x, y, z]);
                    }

                    //물체가 원래 무언가 있는곳으로 이동한 경우
                    if (currentObjects[x, y, z] != previousObjects[x, y, z] && previousObjects[x, y, z] != null)
                    {
                        changedGameObj.Add(currentObjects[x, y, z]);
                    }
                }
            }
        }
        UpdatePrevBlockObjs();
        return changedGameObj;
    }
    /*
        ----- InteractionDetector TestCases ---------------------
        * 1. 움직인 물체가 flame이면서 주변 물체가 flameable 인경우 *
        * 2. 움직인 물체가 flameable 이면서 주변 물체가 flame 인 경우 *
        * 3. 움직인 물체가 flame 이면서 주변 물체에  flame인 경우 *
     * 움ㅈ딕인 물체가 flame - flammable, flame 가치 있는경우
        * 4. 움직인 물체가 flameable 이면서 주변 물체가 flameable인 경우 *
        * 5. 움직인 물체가 flame 이면서 주변에 flameable 이 없는경우 *
        * 6.  
        ==========================================================
    */

    // 변화가 있는 오브젝트가 무엇인지 모를때 인터랙션 해야하는 오브젝트 와 형용사들을 밷어준다.
    [ContextMenu("TestInteractionDetector")]
    public List<Dictionary<GameObject, List<IAdjective>>> InteractionDetector()
    {
        List<Dictionary<GameObject, List<IAdjective>>> influenceInfluencerPairDict =
            new List<Dictionary<GameObject, List<IAdjective>>>();
        var changedObjs = DetectMovedGameObject();

        if (changedObjs.Count == 0)
        {
            Debug.LogWarning("There Are No Changed In the Scene!!!");
            return null;
        }

        foreach (var gameObject in changedObjs)
        {
            //gameObject == 위치가 변경된 오브젝트
            //1. 만약 gameObject 가 interactive 가 않일 경우
            //2. gameobject 의 특성이 없을 경우 
            if (gameObject.tag != "InteractObj" && gameObject.GetComponent<InteractiveObject>().Adjectives.Any() == false) continue;

            var adjacentObjects = GetAdjacentObjects(gameObject);
            for (int i = 0; i < adjacentObjects.Count; i++)
            {
                //adjacentObject 주변에 있는 오브젝트
                if (adjacentObjects[i] == null
                   || adjacentObjects[i] == gameObject // 자기 자신이거나
                   || adjacentObjects[i].tag != "InteractObj") continue;

                if (adjacentObjects[i].GetComponent<InteractiveObject>().Adjectives.Any() == false) continue;

                // problem how to find influencer and follower? 
                //문제점 만약 양쪽둘다 주체 객체 형용사 가지고 있으면 어떻할 것인가

                var checkNeighborObj = IsInfluencer(adjacentObjects[i].GetComponent<InteractiveObject>());
                var checkMovedObj = IsInfluencer(gameObject.GetComponent<InteractiveObject>());
                
                
                // Neighbor 가 주차자인경우
                // movedObj 가 주체자인경우
                //neighbor 가 게체자인경우
                // movedObj 가 개체자인 경우
                // 둘다 주체자인 경우
                // 둘다 아닌 경우

                if (checkNeighborObj)
                {
                    influenceInfluencerPairDict.Add(CompareTwoAdjs(adjacentObjects[i], gameObject));
                }
                if (checkMovedObj)
                {
                    influenceInfluencerPairDict.Add(CompareTwoAdjs(gameObject, adjacentObjects[i]));
                }

                // var influencerAdj = adjacentObjects[i].GetComponent<InteractiveObject>().Adjectives.Where(x => x != null)
                //     .FirstOrDefault(x => x.GetAdjectiveName() == EAdjective.Flame);// 어차피 forloop 
                // var influencedAdj = adjacentObjects[i].GetComponent<InteractiveObject>().Adjectives.Where(x => x != null)
                //     .FirstOrDefault(x => x.GetAdjectiveName() == EAdjective.Flammable);// 어차피 forloop 


                // bool influencerAdj =
                //     (from adj in adjacentObjects[i].GetComponent<InteractiveObject>().Adjectives
                //         select adj.GetAdjectiveName()).Contains(EAdjective.Flame); // throws null
                //
                // bool influencedAdj =
                //     (from adj in adjacentObjects[i].GetComponent<InteractiveObject>().Adjectives
                //         select  adj.GetAdjectiveName()).Contains(EAdjective.Flammable);

                // 주변 물채가 둘다 널인경우..
                // if (influencedAdj == null && influencerAdj == null) continue;
                // //주변 물체중 객차자가 없는 경우 -> 주체자 == 
                // if (influencedAdj == null)
                // {
                //     influenceInfluencerPairDict.Add(CompareTwoAdjs(adjacentObjects[i], gameObject));
                // }
                // //주변물체중 주체자가 없는 경우 -> 주체자 == game
                // if (influencerAdj == null)
                // {
                //     influenceInfluencerPairDict.Add(CompareTwoAdjs(gameObject, adjacentObjects[i]));
                // }

            }
        }
        foreach (var item in influenceInfluencerPairDict)
        {
            foreach (var ite in item)
            {
                Debug.Log(ite.Key, ite.Key.transform);
                foreach (var adjs in ite.Value)
                {
                    Debug.Log(adjs.GetAdjectiveName());
                }
            }
        }
        return influenceInfluencerPairDict;
    }
    /*
   * TestCases
   * flame 이 들어가있을때
     * flame -- flameable
     * flame  -- flame
     * flamable -- flame
     * flammable -- flammable
   */

    //변화가 있는 오브젝트를 알경우 그 오브젝트를 중심으로 인터렉션이 있어야할 오브젝트와 형용사를 밷어준다.

    public List<Dictionary<GameObject, List<IAdjective>>> InteractionDetector(List<GameObject> changedObjects)
    {
        List<Dictionary<GameObject, List<IAdjective>>> influenceInfluencerPairDict =
            new List<Dictionary<GameObject, List<IAdjective>>>();

        foreach (var gameObject in changedObjects)
        {
            var adjacentObjects = GetAdjacentObjects(gameObject);

            for (int i = 0; i < adjacentObjects.Count; i++)
            {
                
                if (adjacentObjects[i] == null
                   || adjacentObjects[i] == gameObject // 자기 자신이거나
                   || adjacentObjects[i].tag != "InteractObj") continue;

                if (adjacentObjects[i].GetComponent<InteractiveObject>().Adjectives.Any() == false) continue;
                
                var checkNeighborObj = IsInfluencer(adjacentObjects[i].GetComponent<InteractiveObject>());
                var checkMovedObj = IsInfluencer(gameObject.GetComponent<InteractiveObject>());
                
                
                // Neighbor 가 주차자인경우
                // movedObj 가 주체자인경우
                //neighbor 가 게체자인경우
                // movedObj 가 개체자인 경우
                // 둘다 주체자인 경우
                // 둘다 아닌 경우

                if (checkNeighborObj)
                {
                    influenceInfluencerPairDict.Add(CompareTwoAdjs(adjacentObjects[i], gameObject));
                }
                if (checkMovedObj)
                {
                    influenceInfluencerPairDict.Add(CompareTwoAdjs(gameObject, adjacentObjects[i]));
                }
                 // var influencerAdj = adjacentObjects[i].GetComponent<InteractiveObject>().Adjectives.Where(x => x != null)
                 //     .FirstOrDefault(x => x.GetAdjectiveName() == EAdjective.Flame);// 어차피 forloop 
                 // var influencedAdj = adjacentObjects[i].GetComponent<InteractiveObject>().Adjectives.Where(x => x != null)
                 //     .FirstOrDefault(x => x.GetAdjectiveName() == EAdjective.Flammable);// 어차피 forloop 
                // 주변 물채가 둘다 널인경우..
                // if (influencedAdj == null && influencerAdj == null) continue;

                // if (isInfluencer) //만약 객체가 주체자면서 주변애가 객체자면 // 만약 주체가 
                // {
                //     influenceInfluencerPairDict.Add(CompareTwoAdjs(adjacentObjects[i], gameObject));
                // }

                // if (influencedAdj == null)
                // {
                //     influenceInfluencerPairDict.Add(CompareTwoAdjs(adjacentObjects[i], gameObject));
                // }
                // //주변물체중 주체자가 없는 경우 -> 주체자 == game
                // if (influencerAdj == null)
                // {
                //     influenceInfluencerPairDict.Add(CompareTwoAdjs(gameObject, adjacentObjects[i]));
                // }
            }
        }
        foreach (var item in influenceInfluencerPairDict)
        {
        
            foreach (var ite in item)
            {
                
                Debug.Log(ite.Key, ite.Key.transform);
                foreach (var adjs in ite.Value)
                {
                    Debug.Log(adjs.GetAdjectiveName());
                }
            }
        }
        return influenceInfluencerPairDict;
    }
    #endregion


    // 1. 뭔가 변화 

    //인디케이터 랑 검출 과는 때어놓는다. 
    //인디케이터는 지정만 한다.

    // 맵에 있을때는 지칭하는애 


    /*행동후 바뀐배열이랑 바낀에들만 리스트를 해서 개내들만 따로검출
     1. transposition 변경시 불을 현 전 비교 매소드 불값이랑 바뀐 개임오브젝트 리스트 반환 or list 만 반환
     2. scale 변경시
     3. 특성변경시, 
     4. 플레이어 인터랙션시
     예시
     
     
     무버블 할때 
     1. 먼저 무버블 가능한지 체크 -> gameobj 받으면 그주변에 뭐있는지 확인 (gameobj, direction)
     2. 변경된 상태에서 다시 검출 
     3. 배열을 넘겨준다. 
     4. 상태가 또바뀌니까 -> 직전에 한번하고 -> 바꾼후에 하고 -> 반복 배열이 안바뀔때까지. prev == current
     */

    /*
     * check cases
     * 1. 처음 모두 찾거
     * 2. 오브젝트 병경시
     *      2-1 오브젝트 위치변화
     *      2-2 오브젝트 사라지거나
     * 3.카드 부여시 그주변 오브젝트 가져오기         
     * findinteraction 매겨변수 없이줄때는 -> 배열 비교해서 찾아주세요
     * findInteraction 매겨변수에 오브젝트를 던질시 -> 그안에서 찾으면 된다. 예 길쭉이 -> game -> 호성님 코드 접근 가져옴-> 리턴 값으로 그위 치에있는 오브젝트 + 특성 ->
     * flame, flameable 이 두 오브젝트에 둘다 들어있을시 --> 나중에 토론토론
     */

    //IEnumerator WaitForInstantiateIndicator()
    //{
    //    yield return new WaitUntil(() => detectSurroundingHs == true);
    //    currentObjects = detectSurroundingHs.GetTileMap();
    //    UpdatePrevBlockObjs();
    //}
    //1. 주최자를 이넘 -로 하던가
    //2. 주최자 adj안에 개 불값

    private void UpdatePrevBlockObjs()
    {
        previousObjects = new GameObject[currentObjects.GetLength(0), currentObjects.GetLength(1), currentObjects.GetLength(2)];
        for (int i = 0; i < currentObjects.GetLength(0); i++)
        {
            for (int j = 0; j < currentObjects.GetLength(1); j++)
            {
                for (int k = 0; k < currentObjects.GetLength(2); k++)
                {
                    previousObjects[i, j, k] = currentObjects[i, j, k];
                }
            }
        }
    }
    #region ITerateThroughMap
    [ContextMenu("IterateThroughMap")]
    public List<Dictionary<GameObject, List<IAdjective>>> IterateThroughMap()
    {
        List<Dictionary<GameObject, List<IAdjective>>> interactionDictCollection =
            new List<Dictionary<GameObject, List<IAdjective>>>();
        //전체 오브젝트 돌기
        for (int i = 0; i < currentObjects.GetLength(0); i++)
        {
            for (int j = 0; j < currentObjects.GetLength(1); j++)
            {
                for (int k = 0; k < currentObjects.GetLength(2); k++)
                {
                    //interactive obj만 받아오기
                    if (currentObjects[i, j, k] == null || currentObjects[i, j, k].tag != "InteractObj") continue;
                    //특성값들 받아오기
                    currentObjects[i, j, k].TryGetComponent(out InteractiveObject targetObj);
                    //특성이 없으면 다음 오브젝트
                    if (targetObj.Adjectives.Any() == false) continue;
                    // 주체자인지 체크 하는 기능
                    if (!IsInfluencer(targetObj)) continue;
                    // 주체자면 검출 돌기
                    var adjacentObjects = GetAdjacentObjects(currentObjects[i, j, k]);
                    // 주변 게임오브젝트 도는 것
                    foreach (var item in adjacentObjects)
                    {

                        if (item == null) continue; // 주변 게임 오브젝트가 널이면 뛰넘기
                        if (item == currentObjects[i, j, k]) continue; // 주변 오브젝트가 자기자신이면 건너 뛰기
                        //주변에 있는 오브젝트 특성 받아오기
                        item.TryGetComponent(out InteractiveObject neighbor);
                        //없으면 다음 주변에 있는에
                        if (neighbor == null || neighbor.Adjectives.Any() == false) continue;
                        var eachAbjs = CompareTwoAdjs(currentObjects[i, j, k], item);
                        if (eachAbjs.Count != 0)
                        {
                            interactionDictCollection.Add(eachAbjs);
                        }
                    }
                }
            }

        }

        // for testing purpose need to be deleted --------
        /*testcases
         
         * 1. 주변 둘러싸여 있을때 flameable로만
         * 2. 대각선에 있는에 체크 안된다.
         * 3. flameable이 양쪽에 flame 에 끼어 있을때
         * 4. flame 한테 flameable 도 있을때
         */

        foreach (var item in interactionDictCollection)
        {
            foreach (var ite in item)
            {
                Debug.Log(ite.Key, ite.Key.transform);
                foreach (var adjs in ite.Value)
                {
                    Debug.Log(adjs.GetAdjectiveName());
                }
            }
        }

        // 이전 배열을 업데이트를 해준다.
        UpdatePrevBlockObjs();
        //  it testing purpose ends here -------------
        return interactionDictCollection;

        // 리턴 값은 선택사항인데
        // 1. 리턴 하는 애들이 주체,당하는애,adj가 있어야 한다. 
    }


    private bool IsInfluencer(InteractiveObject interactiveObject)
    {
        var adjs = interactiveObject.Adjectives;
        for (int i = 0; i < adjs.Length; i++)
        {
            if (adjs[i] == null) continue;
            if (adjs[i].GetAdjectiveName() == EAdjective.Flame)
            {
                return true;
            }
        }
        return false;
    }

    private List<IAdjective> GetAdjectives(GameObject targetObj)
    {
        var adjectives = targetObj.GetComponent<InteractiveObject>().Adjectives;
        List<IAdjective> adjList = new List<IAdjective>();
        for (int i = 0; i < adjectives.Length; i++)
        {
            if (adjectives[i] == null) continue;
            adjList.Add(adjectives[i]);
        }

        return adjList;

    }

    //여기서 만약 주체 형용사 1개 이상이거나 객체가 1개 이상일 경우 
    //flame - flameable
    //water - flame 
    //n - s 
    //dictionary<GameObject, List<IAdjective>> 
    private Dictionary<GameObject, List<IAdjective>> CompareTwoAdjs(GameObject influence, GameObject influenced)
    {
        Dictionary<GameObject, List<IAdjective>> eachAdjDict = new Dictionary<GameObject, List<IAdjective>>(2);

        var influenceAdjs = GetAdjectives(influence);
        var influencedAdjs = GetAdjectives(influenced);



        for (int i = 0; i < influenceAdjs.Count; i++)
        {
            for (int j = 0; j < influencedAdjs.Count; j++)
            {
                if (influenceAdjs[i] == null || influencedAdjs[j] == null) continue;
                if (influenceAdjs[i] == influencedAdjs[j]) continue; // 둘이 같으면 넘기기
                if (influenceAdjs[i].GetAdjectiveName() == EAdjective.Flame &&
                    influencedAdjs[j].GetAdjectiveName() == EAdjective.Flammable)
                {
                    if (!eachAdjDict.ContainsKey(influence))
                    {
                        eachAdjDict.Add(influence, new List<IAdjective>());
                    }
                    if (!eachAdjDict.ContainsKey(influenced))
                    {
                        eachAdjDict.Add(influenced, new List<IAdjective>());
                    }

                    eachAdjDict[influence].Add(influenceAdjs[i]);
                    eachAdjDict[influenced].Add(influencedAdjs[j]);
                }
            }
        }

        return eachAdjDict;
    }



    #endregion


    

   

    #endregion

    #region Check Adjacent region
    // LevelInfos 컴포넌트에서 씬이 열리고 바로 해당 함수를 호출
    // 호출시에 바로 모드를 파악하고, 맵을 로드하거나 (에디터모드인 경우는) 맵 파일을 저장함 
    public void Init(LevelInfos infos)
    {
        GameObject player = GameObject.Find("Player");
        if (player != null) player.SetActive(false);
        this.levelInfos = infos;
        mapManager = MapDataManager.GetInstance;
        if (levelInfos.IsCreateMode)
        {
            mapManager.CreateFile();
        }
        else
        {
            Debug.Log("here");
            mapManager.CreateMap(levelInfos.LevelName);
            SetMapData();
            if (player != null) player.SetActive(true);
        }
    }

    // 맵을 로드할 때에 한 번 배열을 가져오는 메서드로 따로 사용하면 안 됨
    // 기존 맵 배열을 사용하는 것이 아니라 인게임용 배열을 가지고 검출, 수정 등을 할 예정 
    private void SetMapData()
    {
        currentObjects = (GameObject[,,])mapManager.InitObjects.Clone();
        //이전배열 == currentOBJECTS 배열을 만드는 것을 추가 했습니다.
        UpdatePrevBlockObjs();
        currentTiles = (GameObject[,,])mapManager.InitTiles.Clone();

        maxX = currentObjects.GetLength(0) - 1;
        maxY = currentObjects.GetLength(1) + 2;
        maxZ = currentObjects.GetLength(2) - 1;

        tileMaxX = currentTiles.GetLength(0) - 1;
        tileMaxY = currentTiles.GetLength(1) - 1;
        tileMaxZ = currentTiles.GetLength(2) - 1;
    }

    // 오브젝트 데이터 배열 전체를 가져오는 메서드 
    public GameObject[,,] GetObjectsData()
    {
        return this.currentObjects;
    }

    // 타일 데이터 배열 전체를 가져오는 메서드 
    public GameObject[,,] GetTilesData()
    {
        return this.currentTiles;
    }

    // 어떤 오브젝트의 스케일을 건드릴 경우 반드시 호출해야 하는 함수들 
    #region On Any Object's Scale Changed
    // 길어질 수 있는지 체크 후에 실제로 변화시키기 전에 꼭 먼저 이 메서드를 호출하세요
    // 길어진 것이라면, isStretched = true / 줄어든 것이라면, isStretched = false
    public void OnObjectScaleChanged(Vector3 changedScale, Transform targetObj, bool isStreched = true)
    {
        if (isStreched)
        {
            for (int x = 0; x < changedScale.x; x++)
            {
                for (int y = 0; y < changedScale.y; y++)
                {
                    for (int z = 0; z < changedScale.z; z++)
                    {
                        Vector3 stretchedPos = new Vector3(x, y, z);
                        if (stretchedPos == Vector3.zero) continue;
                        Vector3 vec = (targetObj.position + stretchedPos);
                        vec.x = Mathf.RoundToInt(vec.x);
                        vec.y = Mathf.RoundToInt(vec.y);
                        vec.z = Mathf.RoundToInt(vec.z);
                        scaleChangedObjects[vec] = targetObj.gameObject;
                    }
                }
            }
        }
        else
        {
            Vector3 LostScale = targetObj.lossyScale - changedScale;
            for (int x = 0; x <= LostScale.x; x++)
            {
                for (int y = 0; y <= LostScale.y; y++)
                {
                    for (int z = 0; z <= LostScale.z; z++)
                    {
                        Vector3 newPos = new Vector3(x, y, z);
                        if (newPos == Vector3.zero) continue;
                        Vector3 vec = (targetObj.position + changedScale + newPos);
                        vec.x = Mathf.RoundToInt(vec.x);
                        vec.y = Mathf.RoundToInt(vec.y);
                        vec.z = Mathf.RoundToInt(vec.z);
                        scaleChangedObjects.Remove(vec);
                    }
                }
            }
        }
    }

    // 스케일이 1,1,1이 아닌 오브젝트가 이동할 때, 없어질 때에 해당 오브젝트도 처리하기 위해 가져오는 메서드
    // 가져온 벡터3 리스트로 배열의 값을 수정하세요 
    public List<Vector3> GetStretchedObject(Vector3 scale, GameObject targetObj)
    {
        List<Vector3> returnObjs = new List<Vector3>();
        for (int x = 0; x < scale.x; x++)
        {
            for (int y = 0; y < scale.y; y++)
            {
                for (int z = 0; z < scale.z; z++)
                {
                    Vector3 stretchedPos = new Vector3(x, y, z);
                    if (stretchedPos == Vector3.zero) continue;
                    returnObjs.Add(targetObj.transform.position + scale);
                }
            }
        }
        return returnObjs;
    }
    #endregion

    #region GetVector in TileMap & isRight?
    private Vector3Int GetAdjacentVector3(Vector3 position, string value, int addValue)
    {
        Vector3Int returnVector = new Vector3Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), Mathf.RoundToInt(position.z));
        switch (value)
        {
            case ("x"):
                returnVector += Vector3Int.right * addValue;
                break;
            case ("y"):
                returnVector += Vector3Int.up * addValue;
                break;
            case ("z"):
                returnVector += Vector3Int.forward * addValue;
                break;
            default:
                break;
        }
        return returnVector;
    }

    private bool isRightPos(Vector3Int pos, bool isObject = true)
    {
        if (isObject)
        {
            if (pos.x < 0 || pos.x > maxX || pos.y < 0 || pos.y > maxY || pos.z < 0 || pos.z > maxZ)
            {
                return false;
            }
            return true;
        }
        else
        {
            if (pos.x < 0 || pos.x > tileMaxX || pos.y < 0 || pos.y > tileMaxY || pos.z < 0 || pos.z > tileMaxZ)
            {
                return false;
            }
            return true;
        }
    }
    #endregion

    #region Get Adjacent Object(s)
    private GameObject GetExistTileOrNull(int x, int y, int z)
    {
        if (!isRightPos(new Vector3Int(x, y, z), false))
            return null;
        return currentTiles[x, y, z];
    }

    private GameObject GetStretchedObjOrNull(Vector3 vec)
    {
        if (scaleChangedObjects.Keys.Contains(vec))
            return scaleChangedObjects[vec];
        return null;
    }

    private GameObject GetStretchedObjOrNull(Vector3Int vec)
    {
        if (scaleChangedObjects.Keys.Contains(vec))
            return scaleChangedObjects[vec];
        return null;
    }

    protected GameObject GetBlockOrNull(GameObject indicatedObj, string value, int addValue, GameObject[,,] mapData = null)
    {
        return GetBlockOrNull(indicatedObj.transform.position, value, addValue, mapData);
    }

    protected GameObject GetBlockOrNull(Transform indicatedObj, string value, int addValue, GameObject[,,] mapData = null)
    {
        return GetBlockOrNull(indicatedObj.position, value, addValue, mapData);
    }

    protected GameObject GetBlockOrNull(Vector3 indicatedPos, string value, int addValue, GameObject[,,] mapData = null)
    {
        if (mapData == null)
            mapData = this.currentObjects;

        if (mapData.Length == 0)
            return null;

        Vector3Int newPos = GetAdjacentVector3(indicatedPos, value, addValue);
        Vector3 newPosFloat = new Vector3(newPos.x, newPos.y, newPos.z);
        if (!isRightPos(newPos, true)) return null;
        if (mapData.GetLength(1) <= newPos.y) return null; // 수정중 
        if (mapData[newPos.x, newPos.y, newPos.z] != null)
            return mapData[newPos.x, newPos.y, newPos.z];
        else
        {
            GameObject stretchedObj = GetStretchedObjOrNull(newPosFloat);
            if (stretchedObj == null)
            {
                if (!isRightPos(newPos)) return null;
                GameObject tile = GetExistTileOrNull(newPos.x, newPos.y, newPos.z);
                return tile;
            }
            else
                return stretchedObj;
        }
    }

    protected GameObject GetObjectInArray(Vector3 vec, GameObject[,,] mapData = null)
    {
        if (mapData == null)
            mapData = this.currentObjects;

        if (mapData.Length == 0)
            return null;

        Vector3Int newPos = new Vector3Int(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y), Mathf.RoundToInt(vec.z));
        if (!isRightPos(newPos)) return null;
        if (mapData[newPos.x, newPos.y, newPos.z] != null)
            return mapData[newPos.x, newPos.y, newPos.z];
        return null;
    }

    // 특정 오브젝트의 한 방향을 검출하는 로직 --> GameObject
    public GameObject GetAdjacentObjectWithDir(GameObject indicatedObj, Dir dir, int length = 1)
    {
        GameObject returnObj = null;
        switch (dir)
        {
            // 다른 맵 데이터에서 탐색하기 원한다면, GetObjectOrNull 네번째 파라미터로 3차원 배열 맵 데이터를 넣으세요
            case (Dir.right):
                returnObj = GetBlockOrNull(indicatedObj, "x", length);
                break;
            case (Dir.left):
                returnObj = GetBlockOrNull(indicatedObj, "x", -length);
                break;
            case (Dir.up):
                returnObj = GetBlockOrNull(indicatedObj, "y", length);
                break;
            case (Dir.down):
                returnObj = GetBlockOrNull(indicatedObj, "y", -length);
                break;
            case (Dir.forward):
                returnObj = GetBlockOrNull(indicatedObj, "z", length);
                break;
            case (Dir.back):
                returnObj = GetBlockOrNull(indicatedObj, "z", -length);
                break;
        }
        return returnObj;
    }

    // 특정 오브젝트의 한 방향을 검출하는 로직 --> GameObject
    public GameObject GetAdjacentObjectWithDir(Transform indicatedObj, Dir dir, int length = 1)
    {
        GameObject returnObj = null;
        switch (dir)
        {
            // 다른 맵 데이터에서 탐색하기 원한다면, GetObjectOrNull 네번째 파라미터로 3차원 배열 맵 데이터를 넣으세요
            case (Dir.right):
                returnObj = GetBlockOrNull(indicatedObj, "x", length);
                break;
            case (Dir.left):
                returnObj = GetBlockOrNull(indicatedObj, "x", -length);
                break;
            case (Dir.up):
                returnObj = GetBlockOrNull(indicatedObj, "y", length);
                break;
            case (Dir.down):
                returnObj = GetBlockOrNull(indicatedObj, "y", -length);
                break;
            case (Dir.forward):
                returnObj = GetBlockOrNull(indicatedObj, "z", length);
                break;
            case (Dir.back):
                returnObj = GetBlockOrNull(indicatedObj, "z", -length);
                break;
        }
        return returnObj;
    }

    // 스케일이 변경된 특정 오브젝트의 한 방향을 검출하는 로직 --> GameObject
    // 생각해보니 여러개를 가져올 수도 있는데... 일단 그건 나중에 수정할 예정 
    public GameObject GetAdjacentObjectWithDir(GameObject indicatedObj, Dir dir, Vector3 objScale)
    {
        int length = 1;
        if ((int)dir % 2 == 1)
        {
            length = (int)dir < 2 ? Mathf.RoundToInt(objScale.x)
                : ((int)dir < 4 ? Mathf.RoundToInt(objScale.y)
                : Mathf.RoundToInt(objScale.z));
        }
        return GetAdjacentObjectWithDir(indicatedObj, dir, length);
    }

    // 특정 오브젝트의 6 방향을 검출하는 로직 1 --> List<GameObject>
    public List<GameObject> GetAdjacentObjects(GameObject indicatedObj)
    {
        Vector3 objScale = indicatedObj.transform.lossyScale;
        if (objScale != Vector3.one)
        {
            return GetAdjacentObjects(indicatedObj, objScale);
        }

        Vector3 indicatedObjPos = indicatedObj.transform.position;
        string[] values = new string[3] { "x", "y", "z" };
        int[] addValues = new int[2] { 1, -1 };

        List<GameObject> returnObjects = new List<GameObject>(6);

        for (int i = 0; i < 6; i++)
        {
            GameObject go = GetAdjacentObjectWithDir(indicatedObj, (Dir)i);
            if (go == null) continue;
            returnObjects.Add(go);
        }
        return returnObjects;
    }

    // 스케일이 변경된 특정 오브젝트의 6++ 방향을 검출하는 로직 --> List<GameObject>
    public List<GameObject> GetAdjacentObjects(GameObject indicatedObj, Vector3 objScale)
    {
        Vector3 indicatedObjPos = indicatedObj.transform.position;
        string[] values = new string[3] { "x", "y", "z" };
        int[] addValues = new int[2] { 1, -1 };

        int scaleX = Mathf.RoundToInt(objScale.x);
        int scaleY = Mathf.RoundToInt(objScale.y);
        int scaleZ = Mathf.RoundToInt(objScale.z);
        int count = ((scaleX * scaleY) + (scaleY * scaleZ) + (scaleZ * scaleX)) * 2;

        List<GameObject> returnObjects = new List<GameObject>(count);

        for (int i = 0; i < 2; i++)
        {
            for (int y = 0; y < scaleY; y++)
            {
                for (int z = 0; z < scaleZ; z++)
                {
                    Transform newPos = indicatedObj.transform;
                    Vector3Int newPosition = GetAdjacentVector3(newPos.position, "y", y);
                    newPosition = GetAdjacentVector3(newPosition, "z", z);
                    GameObject go = GetBlockOrNull(newPosition, "x", i % 2 == 1 ? scaleX : -1);
                    if (go == null || returnObjects.Contains(go)) continue;
                    returnObjects.Add(go);
                }
            }
        }
        for (int i = 2; i < 4; i++)
        {
            for (int z = 0; z < scaleZ; z++)
            {
                for (int x = 0; x < scaleX; x++)
                {
                    Transform newPos = indicatedObj.transform;
                    Vector3Int newPosition = GetAdjacentVector3(newPos.position, "z", z);
                    newPosition = GetAdjacentVector3(newPosition, "x", x);
                    GameObject go = GetBlockOrNull(newPosition, "y", i % 2 == 1 ? scaleY : -1);
                    if (go == null || returnObjects.Contains(go)) continue;
                    returnObjects.Add(go);
                }
            }
        }
        for (int i = 4; i < 6; i++)
        {
            for (int x = 0; x < scaleX; x++)
            {
                for (int y = 0; y < scaleY; y++)
                {
                    Transform newPos = indicatedObj.transform;
                    Vector3Int newPosition = GetAdjacentVector3(newPos.position, "x", x);
                    newPosition = GetAdjacentVector3(newPosition, "y", y);
                    GameObject go = GetBlockOrNull(newPosition, "z", i % 2 == 1 ? scaleZ : -1);
                    if (go == null || returnObjects.Contains(go)) continue;
                    returnObjects.Add(go);
                }
            }
        }
        return returnObjects;
    }

    // 특정 오브젝트의 6 방향을 검출하는 로직 2 --> Dictionary<Dir, GameObject>
    public Dictionary<Dir, GameObject> GetAdjacentDictionary(GameObject indicatedObj)
    {
        Vector3 objScale = indicatedObj.transform.lossyScale;
        if (objScale != Vector3.one)
        {
            Debug.LogError("Use GetAdjacentsDictionary");
            return null;
        }

        Vector3 indicatedObjPos = indicatedObj.transform.position;
        string[] values = new string[3] { "x", "y", "z" };
        int[] addValues = new int[2] { 1, -1 };

        Dictionary<Dir, GameObject> returnObjects = new Dictionary<Dir, GameObject>(6);

        for (int i = 0; i < 6; i++)
        {
            GameObject go = GetAdjacentObjectWithDir(indicatedObj, (Dir)i);
            if (go == null) continue;
            returnObjects[(Dir)i] = go;
        }
        return returnObjects;
    }

    // 스케일이 변경된 특정 오브젝트의 6++ 방향을 검출하는 로직 --> Dictionary<Dir, List<GameObject>>
    public Dictionary<Dir, List<GameObject>> GetAdjacentsDictionary(GameObject indicatedObj, Vector3 objScale)
    {
        Vector3 indicatedObjPos = indicatedObj.transform.position;
        string[] values = new string[3] { "x", "y", "z" };
        int[] addValues = new int[2] { 1, -1 };

        int scaleX = Mathf.RoundToInt(objScale.x);
        int scaleY = Mathf.RoundToInt(objScale.y);
        int scaleZ = Mathf.RoundToInt(objScale.z);
        int count = ((scaleX * scaleY) + (scaleY * scaleZ) + (scaleZ * scaleX)) * 2;

        Dictionary<Dir, List<GameObject>> returnObjects = new Dictionary<Dir, List<GameObject>>(6);

        for (int i = 0; i < 2; i++)
        {
            for (int y = 0; y < scaleY; y++)
            {
                for (int z = 0; z < scaleZ; z++)
                {
                    Transform newPos = indicatedObj.transform;
                    Vector3Int newPosition = GetAdjacentVector3(newPos.position, "y", y);
                    newPosition = GetAdjacentVector3(newPosition, "z", z);
                    GameObject go = GetBlockOrNull(newPosition, "x", i % 2 == 1 ? scaleX : -1);
                    if (go == null || (returnObjects.Keys.Contains((Dir)i) && returnObjects[(Dir)i].Contains(go))) continue;
                    if (returnObjects.Keys.Contains((Dir)i))
                        returnObjects[(Dir)i].Add(go);
                    else
                    {
                        returnObjects.Add((Dir)i, new List<GameObject>());
                        returnObjects[(Dir)i].Add(go);
                    }
                }
            }
        }
        for (int i = 2; i < 4; i++)
        {
            for (int z = 0; z < scaleZ; z++)
            {
                for (int x = 0; x < scaleX; x++)
                {
                    Transform newPos = indicatedObj.transform;
                    Vector3Int newPosition = GetAdjacentVector3(newPos.position, "z", z);
                    newPosition = GetAdjacentVector3(newPosition, "x", x);
                    GameObject go = GetBlockOrNull(newPosition, "y", i % 2 == 1 ? scaleY : -1);
                    if (go == null || (returnObjects.Keys.Contains((Dir)i) && returnObjects[(Dir)i].Contains(go))) continue;
                    if (returnObjects.Keys.Contains((Dir)i))
                        returnObjects[(Dir)i].Add(go);
                    else
                    {
                        returnObjects.Add((Dir)i, new List<GameObject>());
                        returnObjects[(Dir)i].Add(go);
                    }
                }
            }
        }
        for (int i = 4; i < 6; i++)
        {
            for (int x = 0; x < scaleX; x++)
            {
                for (int y = 0; y < scaleY; y++)
                {
                    Transform newPos = indicatedObj.transform;
                    Vector3Int newPosition = GetAdjacentVector3(newPos.position, "x", x);
                    newPosition = GetAdjacentVector3(newPosition, "y", y);
                    GameObject go = GetBlockOrNull(newPosition, "z", i % 2 == 1 ? scaleZ : -1);
                    if (go == null || (returnObjects.Keys.Contains((Dir)i) && returnObjects[(Dir)i].Contains(go))) continue;
                    if (returnObjects.Keys.Contains((Dir)i))
                        returnObjects[(Dir)i].Add(go);
                    else
                    {
                        returnObjects.Add((Dir)i, new List<GameObject>());
                        returnObjects[(Dir)i].Add(go);
                    }
                }
            }
        }
        return returnObjects;
    }

#endregion

#region Get or Change Array Data
    // 맵 배열에서 Vector3의 값에 해당하는 게임 오브젝트들을 가져오는 메서드 
    private Dictionary<Vector3, GameObject> GetArrayObjects(params Vector3[] blocks)
    {
        Dictionary<Vector3, GameObject> returnDict = new Dictionary<Vector3, GameObject>();
        foreach (Vector3 block in blocks)
        {
            GameObject go = GetObjectInArray(block);
            returnDict.Add(block, go);
        }
        return returnDict;
    }

    // 맵 배열 데이터에서 두 개의 값을 교환하는 메서드 
    public void SwapBlockInMap(Vector3 block1, Vector3 block2)
    {
        Dictionary<Vector3, GameObject> dict = GetArrayObjects(block1, block2);
        GameObject go1 = dict[block1];
        GameObject go2 = dict[block2];

        ChangeValueInMap(block1, go2);
        ChangeValueInMap(block2, go1);

        Dictionary<Vector3, GameObject> newDict = GetArrayObjects(block1, block2);

        //Debug.Log(go1 + " -> " + newDict[block1]);
        //Debug.Log(go2 + " -> " + newDict[block2]);
    }

    // 맵 배열 데이터에서 한 개의 값을 새로운 값으로 변경하는 메서드 
    public void ChangeValueInMap(Vector3 block, GameObject curObject = null)
    {
        int x = Mathf.RoundToInt(block.x);
        int y = Mathf.RoundToInt(block.y);
        int z = Mathf.RoundToInt(block.z);

        currentObjects[x, y, z] = curObject;
    }
#endregion

#endregion

#region Test
    
   
    public void TestSetForTestPurposeList()
    {
        ForTestPurpos = new List<GameObject>();
        Debug.Log(currentObjects.Length);
        foreach (var item in currentObjects)
        {
            if(item == null) continue;
            string goName = item.name;
            if (goName.Contains('W'))
            {
                ForTestPurpos.Add(item);
            }
        }
            
    }
        
    [ContextMenu("TestGameObjListDetector")]
    public void TestGameObjs()
    {
        TestSetForTestPurposeList();
        InteractionDetector(ForTestPurpos);
    }
  
    [ContextMenu("TestDetectMoveGameObj")]
    public void TestDetectMoveGameObj()
    {
        var test = DetectMovedGameObject();
        foreach (var item in test)
        {
            Debug.Log(item, item.transform);
        }
    }

    private void SetTarget()
    {
        target = levelInfos.target;
        ECheckDir = levelInfos.ECheckDir;
    }

    [ContextMenu("TestGetAdjacentObjs")]
    public void TestGetAdjacentObjs()
    {
        SetTarget();
        foreach (GameObject go in GetAdjacentObjects(target))
        {
            if (go == null) continue;
            Debug.Log(go.name + ", pos : " + go.transform.position.ToString(), go.transform);
        }
    }

    [ContextMenu("TestGetAdjacentObjWithDir")]
    public void TestGetAdjacentDict()
    {
        SetTarget();
        Dictionary<Dir, GameObject> dict = GetAdjacentDictionary(target);
        for (int i = 0; i < 6; i++)
        {
            if (!dict.Keys.Contains((Dir)i)) continue;
            GameObject go = dict[(Dir)i];
            Debug.Log("Dir " + ((Dir)i) + " " + go.name + ", pos : " + go.transform.position.ToString(), go.transform);
        }
    }

    [ContextMenu("TestGetAdjacentsDict")]
    public void TestGetAdjacentsDict()
    {
        SetTarget();
        Dictionary<Dir, List<GameObject>> dict = GetAdjacentsDictionary(target, target.transform.lossyScale);
        foreach (Dir d in dict.Keys)
        {
            foreach (GameObject go in dict[d])
            {
                Debug.Log("Dir " + d + " " + go.name + ", pos : " + go.transform.position.ToString(), go.transform);
            }
        }
    }

    [ContextMenu("TestDir")]
    public void TestAdjacentObjWithDir()
    {
        SetTarget();
        GameObject go = GetAdjacentObjectWithDir(target, ECheckDir);
        if (go == null) Debug.Log("There is nothing!");
        else Debug.Log(go.name, go.transform);
    }

    [ContextMenu("TestSwap")]
    public void TestSwapValue()
    {
        Dictionary<Vector3, GameObject> dict = GetArrayObjects(levelInfos.block1, levelInfos.block2);
        Debug.Log("Before");
        Debug.Log("block1 : " + dict[levelInfos.block1]);
        Debug.Log("block2 : " + dict[levelInfos.block2]);
        SwapBlockInMap(levelInfos.block1, levelInfos.block2);
        Dictionary<Vector3, GameObject> dict2 = GetArrayObjects(levelInfos.block1, levelInfos.block2);
        Debug.Log("After");
        Debug.Log("block1 : " + dict2[levelInfos.block1]);
        Debug.Log("block2 : " + dict2[levelInfos.block2]);
    }

    [ContextMenu("TestChange")]
    public void TestChnageValue()
    {
        Dictionary<Vector3, GameObject> dict = GetArrayObjects(levelInfos.block1);
        Debug.Log("block1 : " + dict[levelInfos.block1]);
        ChangeValueInMap(levelInfos.block1, levelInfos.newValue);
        Dictionary<Vector3, GameObject> dict2 = GetArrayObjects(levelInfos.block1);
        Debug.Log("block1 : " + dict2[levelInfos.block1]);
    }

    [ContextMenu("TestAddScaledObject")]
    public void TestAddScaledObject()
    {
        OnObjectScaleChanged(levelInfos.changeScale, levelInfos.target.transform, levelInfos.isStretched);
    }
#endregion
}

// y축으로 둥둥 시에 기존에 y축 length을 넘어버리면, out of range