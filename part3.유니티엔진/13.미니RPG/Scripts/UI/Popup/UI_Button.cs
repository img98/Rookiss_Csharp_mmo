using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Button : UI_Popup //������ ��� ui�� UI_Base ������� ����Ŵϱ�, �긦 ��ӹް��ϸ��
{
    enum Buttons // Bind�� üũ�Ҷ� ��(Buttons)�� ����(PointButton)�̶� ���� �̸��� �ִ��� Ȯ��.
    {
        PointButton,
    }
    enum Texts
    {
        PointText,
        ScoreText,
    }
    enum GameObjects
    {
        TestObject,
    }
    enum Images
    {
        ItemIcon,
    }


    public override void Init()
    {
        base.Init(); //�� �θ��� init�� ȣ���ϱ�

        Bind<Button>(typeof(Buttons)); //�̰� reflection�����ε�, ��Ȯ�� enum�� �ѱ�� �ƴϰ�, enumŸ���� �ѱ�ŷ�
        Bind<TextMeshProUGUI>(typeof(Texts)); // Generic�� ��������=Texts�� ���پ��ǵ�, �ű⼭ TextMeshProUGUI��� Ŭ������ ���¾ָ� �������ּ��� ��� ��
        Bind<GameObject>(typeof(GameObjects)); //������Ʈ�� �ƴ϶�, ����Ÿ���� ���� �����ϴ� ��쵵 ������.
        Bind<Image>(typeof(Images));

        // Get<TextMeshProUGUI>((int)Texts.ScoreText).text = "Test Get";
        //GetText((int)Texts.ScoreText).text = "Text Get v2."; //����� (int�� ���������� Texts.ScoreText�� int�������� �޾Ƶ��̱����� �̶�� �����ϸ�ȴ�.)

        GameObject go = GetImage((int)Images.ItemIcon).gameObject; //gameObject�� �ҷ��� ������, ItemIcon�̶�� ������Ʈ�� UI_EventHandler�� �߰��ϰų�, UI_EventHandler�� ��ȭ�� �ٰ��̱⶧���̴�.
        BindEvent(go, (PointerEventData data) => { go.transform.position = data.position; }, Define.UIEvent.Drag);

        GetButton((int)Buttons.PointButton).gameObject.BindEvent(OnButtonClicked);
    }

    int _score = 0;
    public void OnButtonClicked(PointerEventData data) 
    {
        _score++;
        GetText((int)Texts.ScoreText).text = $"���� : {_score}";
    }
}
