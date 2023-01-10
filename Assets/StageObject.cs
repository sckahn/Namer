using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//public class Word : ScriptableObject
//{
//    public string name;
//    public NewAdjective[] newAdjectives;
//}

//[CreateAssetMenu(menuName = "create Name", order = 0)]
//public class Name : Word
//{
//    public Nametype nametype;  // 생성할 때만 쓸것, 카드로 줄 때는 안 씁니다.
//}

//[CreateAssetMenu(menuName = "create Adjective", order = 1)]
//public class Adjective : Word
//{

//}

public struct NewAdjective
{
    public int index;
    public bool value;
}

//public class TestButton : MonoBehaviour
//{
//    public Adjective[] adjectives;
//    public Name name;
//    public GameObject targetObj;

//    public void SetObj()
//    {
//        StageObject sObj = targetObj.GetComponent<StageObject>();
//        sObj.SetObject(name.name, name.newAdjectives);
//        foreach (Adjective adj in adjectives)
//            sObj.SetObject(adj.name + " " + sObj.Name, adj.newAdjectives);
//    }
//}

//public enum Nametype
//{
//    none = -1,
//    tile = 0,
//    rock = 1
//}

//public class LevelEditor
//{
//    public void CreatePrefab(Nametype nt)
//    {

//        GameObject obj = Resources.Load(nt.ToString()) as GameObject;
//        GameObject.Instantiate(obj);
//    }
//}

public class StageObject : MonoBehaviour
{
    private string name;
    public string Name
    {
        get { return name; }
        set { if (value == "") return; name = value; }
    }
    [SerializeField] private bool[] specificity;

    public StageObject(string name, NewAdjective[] adjectives)
    {
        SetObject(name, adjectives);
    }

    public void SetObject(string name, NewAdjective[] adjectives)
    {
        this.name = name;
        SetSpeficity(adjectives);
    }

    public void SetSpeficity(NewAdjective[] adjectives)
    {
        foreach (NewAdjective adjective in adjectives)
        {
            specificity[adjective.index] = adjective.value;
        }
    }

    //public void Interact(Player player)
    //{

    //}

    public void Interact(StageObject obj)
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        //if (collision.transform.CompareTag("Player"))
            //Interact(collision.transform.GetComponent<Player>());
        //else
            Interact(collision.transform.GetComponent<StageObject>());
    }
}