using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface ILoader<Key, Value> //key�� value�� ���� ������ Ÿ���� �ִ°Ծƴ�, ������ Ÿ���� �ʰ� ���ؼ� ������¶�. ���ʿ� �̰� �������̽��ϱ�, �̷������� ������ ���̵幮
{
    Dictionary<Key, Value> MakeDict();
}

public class DataManager
{
    public Dictionary<int, Data.Stat> StatDict { get; private set; } = new Dictionary<int, Data.Stat>();

    public void Init()
    {
        StatDict = LoadJson<Data.StatData, int, Data.Stat>("StatData").MakeDict(); //!!LoadJson�� ������ StatDataŸ���� ��ȯ���״�, �츮 �ű⼭ �߰��� MakeDict()�� �����Ͽ� dictionary�� ������ش�
    }

    Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key,Value> //Loader�� �츮�� �ҷ��� �������� Ÿ���� �ǹ���. ��, <Ű�͹���� ����ִ� ���� Ÿ��, Ű�� Ÿ��, ����� Ÿ��>
    {
        //�����͸� �Ľ��ϴ� �Լ���

        TextAsset textAsset = Managers.Resource.Load<TextAsset>($"Data/{path}"); 
        return JsonUtility.FromJson<Loader>(textAsset.text); //����Ƽ�� Json�� �˾Ƽ� �Ľ����ִ� ���̺귯���� �ִ�.

    }



    //Data�� �׻� ����־�� �ϱ⿡  Clear�����ʴ´�.
}
