using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Card Data", menuName = "Scriptable Object/Card Data", order = 0)]
public class CardData : ScriptableObject
{
    public ECardType cardType;
    public string uiText;
    public EName addedName;
    public EAdjective[] addedAdjectives;
}

[CustomEditor(typeof(CardData))]
public class CustomInspectorEditor : Editor
{
    private SerializedProperty Option_CardType;
    private SerializedProperty Option_UIText;
    private SerializedProperty Option_AddedName;
    private SerializedProperty Option_AddedAdjectives;

    private void Awake()
    {
        Option_CardType = serializedObject.FindProperty("cardType");
        Option_UIText = serializedObject.FindProperty("uiText");
        Option_AddedName = serializedObject.FindProperty("addedName");
        Option_AddedAdjectives = serializedObject.FindProperty("addedAdjectives");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(Option_CardType);
        EditorGUILayout.PropertyField(Option_UIText);

        if ((ECardType)Option_CardType.enumValueIndex == ECardType.Name)
        {
            EditorGUILayout.LabelField("Name의 Adjective들은 NameData에서 확인해주세요!", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(Option_AddedName);
        }
        else if ((ECardType)Option_CardType.enumValueIndex == ECardType.Adjective)
        {
            EditorGUILayout.PropertyField(Option_AddedAdjectives);
        }

        serializedObject.ApplyModifiedProperties();
    }
}