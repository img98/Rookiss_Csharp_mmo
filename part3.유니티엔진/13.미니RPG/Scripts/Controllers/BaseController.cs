using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseController : MonoBehaviour
{
    [SerializeField]
    protected Vector3 _destPos;

    [SerializeField]
    protected Define.State _state = Define.State.Idle; //�ʱ� ���´� Idle�� ���� //_state�� ���� �Ⱦ��µ� �̰� �ʿ��Ѱ�?=>�ʿ��ϴ�. State�� �ᱹ�� _state�� ���¸� �ٲ��ִ� �뵵�̱⿡.

    [SerializeField]
    protected GameObject _lockTarget;

    public Define.WorldObject WorldObjectType { get; protected set; } = Define.WorldObject.Unknown;

    public virtual Define.State State //��ӹ����ֵ��� �߰��ϰ���� �����ִ� �ֵ��� �̰Ű��� �ٲٶ�¶�
    {
        get { return _state; }
        set
        {
            _state = value;

            Animator anim = GetComponent<Animator>();
            switch (_state)
            {
                case Define.State.Die:
                    break;
                case Define.State.Idle:
                    anim.CrossFade("WAIT", 0.1f);
                    break;
                case Define.State.Moving:
                    anim.CrossFade("RUN", 0.1f);
                    break;
                case Define.State.Skill:
                    anim.CrossFade("ATTACK", 0.1f, -1, 0); //����= Ʋ���� ���, transition�Ѿ�µ� �ɸ��ð�, ���̾�(�Ű�Ⱦ��Ŵ� -1), normalizedTimeOffset(0�����ϸ� ����� ������ ó������ ���ư��� �ٽ�ư�ٴµ�?)
                    break;
            }
        }
    }
    public void Start()
    {
        Init();

    }

    void Update()
    {

        switch (State)
        {
            case Define.State.Die:
                UpdateDie();
                break;
            case Define.State.Moving:
                UpdateMoving();
                break;
            case Define.State.Idle:
                UpdateIdle();
                break;
            case Define.State.Skill:
                UpdateSkill();
                break;
        }
    }

    public abstract void Init();

    protected virtual void UpdateDie() { } //��� ĳ���Ϳ� ������Ʈ ������ ���ٴ� ������ ������, �������̽��� �ĵ���
    protected virtual void UpdateMoving() { }
    protected virtual void UpdateIdle() { }
    protected virtual void UpdateSkill() { }

}
