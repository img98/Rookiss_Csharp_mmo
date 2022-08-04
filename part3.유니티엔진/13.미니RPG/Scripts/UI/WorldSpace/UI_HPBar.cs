using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HPBar : UI_Base
{
    enum GameObjects
    {
        HPBar,
    }

    Stat _stat;

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        _stat = transform.parent.GetComponent<Stat>();
    }

    private void Update()
    {
        Transform parent = gameObject.transform.parent; //��.Ʈ��������.�θ� (���⼭ '��'�� ��������)
        transform.position = parent.position + Vector3.up * (parent.GetComponent<Collider>().bounds.size.y); // HP�ٰ� ����� ��ġ = ���θ� + ����������*�θ��� �ݶ��̴� y������ŭ
        transform.rotation = Camera.main.transform.rotation; //HP�ٰ� ī�޶���⿡ �°� ���ư�����,(������)

        float ratio = _stat.Hp / (float)_stat.MaxHp; // float�� int�� �����°� �Ҽ��� ������ֱ⿡, �и� floatĳ�������൵ �ȴ�.
        SetHPRatio(ratio);
    }

    public void SetHPRatio(float ratio)
    {
        GetObject((int)GameObjects.HPBar).GetComponent<Slider>().value = ratio;
    }
}
