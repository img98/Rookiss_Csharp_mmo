using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BaseScene : MonoBehaviour
{
    public Define.Scene SceneType { get; protected set; } = Define.Scene.Unknown; //��Ÿ���� ��𼭵� �޾Ƽ� Ȯ���Ҽ�������(public) ������ ��ӹ����ڼո� ����(protected), �׸��� �̰��� Unknown���� �����صΰڴ�.

    void Awake() // Awake�� GameScene�̶�� ������Ʈ(��ũ��Ʈ���� ������Ʈ ��ü�� ���ϰ��ִ�.)�� ���� �۵��Ѵ�. �˸�����
    {
        Init();
    }

    protected virtual void Init()
    {
        Object obj = GameObject.FindObjectOfType(typeof(EventSystem));
        if (obj == null)
            Managers.Resource.Instantiate("UI/EventSystem").name = "@EventSystem";
    }

    public abstract void Clear(); //���⼭ ����� �ƴ϶� abstract�� //�� Scene�� �����Ű�°� Clear(���״�� ���տ��� ġ����.)
}
