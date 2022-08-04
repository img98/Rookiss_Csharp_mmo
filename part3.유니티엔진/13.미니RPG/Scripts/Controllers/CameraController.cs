using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    Define.CameraMode _mode = Define.CameraMode.QuarterView;

    [SerializeField]
    Vector3 _delta = new Vector3(0.0f, 6.0f, -5.0f); //ĳ���ͷκ��� �󸶳� �������ִ°�. //�Ʊ� �ſ��� �������� y=-6, z=5��°��� ã�Ҵ�.

    [SerializeField]
    GameObject _player = null; //������Ʈ �巡�׵������������ ���� �ʱ�ȭ

    public void SetPlayer(GameObject player) { _player = player; } //����̷��� ī�޶�Ŵ��� ������ ����°Ը±��ѵ�, �����ϱ� �׳� ��������

    void Start()
    { 
        
    }

    void LateUpdate()
    {
        if (_mode == Define.CameraMode.QuarterView)
        {
            if (_player.IsValid()==false)
                return;

            RaycastHit hit;
            if (Physics.Raycast(_player.transform.position, _delta, out hit, _delta.magnitude, LayerMask.GetMask("Wall"))) //�÷��̾� ��ġ����, ķ��������, ����ĳ��Ʈ�ؼ� wall�� �ݸ��� �ȴٸ�
            {
                float dist = (hit.point - _player.transform.position).magnitude * 0.8f; //hit��ǥ�� �÷��̾� ��ǥ�� ���� ����� ũ�⸦ �˼��ְ�, ���� 0.8�� ���ؼ� ������ ���� ������ �������Ѵ�. 
                transform.position = _player.transform.position + _delta.normalized * dist; //normalized �� ������, ũ�Ⱑ �ƴ϶� ���⸸�� ������� ���ؼ�(ũ��� dist�� ��Ҵ�)
            }
            else //���� ���ٸ� �׳� �̵�
            {
                //��ƽ���� �÷��̾� ��ǥ�� �������� ī�޶� ��ġ�� ����ؾ��Ѵ�. �÷��̾ �巡�׵������ �ҷ�����.(���� �÷��̾ ��� �ٷ��� �������� ���ؼ� �ӽù���)
                transform.position = _player.transform.position + _delta; // �÷��̾��� ��ġ�� ��Ÿ��ġ�� �����ذ� = ī�޶��� ��ġ
                transform.LookAt(_player.transform);//ī�޶� ����� ��ǥ�� �ֽ��ϵ��� ����
            }
        }

    }

    public void SetQuarterView(Vector3 delta)
    {
        _mode = Define.CameraMode.QuarterView;
        _delta = delta;
    }
}
