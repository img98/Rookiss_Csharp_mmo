using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    int _mask = (1 << (int)Define.Layer.Ground) | (1 << (int)Define.Layer.Monster);

    Texture2D _attackIcon; //���־� �������̶� �׳� �ѹ� ����ͼ� �����Ű�ڴ�.
    Texture2D _handIcon;

    enum CursorType
    {
        None,
        Attack,
        Hand,

    }
    CursorType _cursorType = CursorType.None;

    void Start()
    {
        _attackIcon = Managers.Resource.Load<Texture2D>("Textures/Cursor/Attack"); //���־� �������̶� �׳� �ѹ� ����ͼ� �����Ű�ڴ�.
        _handIcon = Managers.Resource.Load<Texture2D>("Textures/Cursor/Hand");

    }

    void Update()
    {
        if (Input.GetMouseButton(0))
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100.0f, _mask))
        {
            if (hit.collider.gameObject.layer == (int)Define.Layer.Ground)
            {
                if (_cursorType != CursorType.Attack) //Ŀ���̹����� �ʹ� ���ֹٲ��(�����Ӹ���) �̹����� �����Ÿ�. �׷��� ���°� ����������, Ŀ���̹����� ���ϰ� ���ִ°�.
                {
                    Cursor.SetCursor(_handIcon, new Vector2(_handIcon.width / 5, 0), CursorMode.Auto); //���� (�̹���, �̹����� ���۵���ǥ, ���콺��� )
                    _cursorType = CursorType.Attack;
                }
            }
            if (hit.collider.gameObject.layer == (int)Define.Layer.Monster)
            {
                if (_cursorType != CursorType.Hand)
                {
                    Cursor.SetCursor(_attackIcon, new Vector2(_attackIcon.width / 3, 0), CursorMode.Auto);
                    _cursorType = CursorType.Hand;
                }
            }
        }

    }
}
