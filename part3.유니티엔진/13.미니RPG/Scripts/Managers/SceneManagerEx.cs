using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx
{
    public BaseScene CurrentScene { get { return GameObject.FindObjectOfType<BaseScene>(); }  } //Ȥ�ó� �ܺο��� ����scene�� Ÿ���� �ʿ���ϸ� �� �Լ�

    public void LoadScene(Define.Scene type) //�츰 enum��ϰ����� ��Ÿ���� �̿��ϰ� ������,
    {
        Managers.Clear();
        SceneManager.LoadScene(GetSceneName(type));
    }

    string GetSceneName(Define.Scene type) //Define.Scene type�� ���״�� Define������ string�� �ƴ�, �׷��� ���ڿ��� Ȱ���Ҽ� ���⿡, Define���� ���ڿ��� �������ִ� �Լ��� ������.
    {
        string name = System.Enum.GetName(typeof(Define.Scene), type); //�̷��� �������� ���°� reflectionŸ���̶� �ϳ���.
        return name;
    }

    public void Clear() //���� �긦 �׳� LoadScene�� �־ �Ǵµ�, Managers���� ��� Clear�� ����ϰ� �Ϸ��� ���� ����
    {
        CurrentScene.Clear();
    }
}
