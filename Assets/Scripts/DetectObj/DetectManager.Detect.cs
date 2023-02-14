using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class DetectManager : Singleton<DetectManager>
{
#region Detect region By.JS

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
            //Debug.LogWarning("There Are No Changed In the Scene!!!");
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
                //Debug.Log(ite.Key, ite.Key.transform);
                foreach (var adjs in ite.Value)
                {
                    //Debug.Log(adjs.GetAdjectiveName());
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
        // foreach (var item in influenceInfluencerPairDict)
        // {
        //
        //     foreach (var ite in item)
        //     {
        //
        //         //Debug.Log(ite.Key, ite.Key.transform);
        //         foreach (var adjs in ite.Value)
        //         {
        //             //Debug.Log(adjs.GetAdjectiveName());
        //         }
        //     }
        // }
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
                //Debug.Log(ite.Key, ite.Key.transform);
                foreach (var adjs in ite.Value)
                {
                    //Debug.Log(adjs.GetAdjectiveName());
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
            if (adjs[i].GetAdjectiveName() == EAdjective.Flame 
                || adjs[i].GetAdjectiveName() == EAdjective.Extinguisher
                ||adjs[i].GetAdjectiveName()==EAdjective.Freeze)
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
                if (influenceAdjs[i].GetAdjectiveName() == EAdjective.Extinguisher &&
                         influencedAdjs[j].GetAdjectiveName() == EAdjective.Flame)
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
                if (influenceAdjs[i].GetAdjectiveName() == EAdjective.Freeze &&
                    influencedAdjs[j].GetAdjectiveName() == EAdjective.Flow)
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
}
