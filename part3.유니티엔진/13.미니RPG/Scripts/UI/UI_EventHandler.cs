using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EventHandler : MonoBehaviour, IDragHandler, IPointerClickHandler
{
    public Action<PointerEventData> OnClickHandler = null; //�ݹ� ����� �������� Action����
    public Action<PointerEventData> OnDragHandler = null;


    //IBeginDragHandler�� �����ߴ�. �ʿ��ϸ� �߰�����.
    public void OnDrag(PointerEventData eventData) //�巡�װ� ���ӵǴ� ����
    {
        if (OnDragHandler != null)
            OnDragHandler.Invoke(eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnClickHandler != null)
            OnClickHandler.Invoke(eventData);
    }
}
