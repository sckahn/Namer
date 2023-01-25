using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct NameInfo
{
    public string uiText;
    public EName name;
    public EAdjective[] adjectives;
    public string contentText;
}

public class NameData : MonoBehaviour
{
    [Header("Name enum 순서를 확인해서 입력해주세요!")]
    [SerializeField] NameInfo[] nameInfos = new NameInfo[10];
    public NameInfo[] NameInfos { get { return nameInfos; } }
}