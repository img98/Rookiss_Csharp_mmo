using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager //���ҽ� �Ŵ����� ����ؼ�, Pooling�� �����ϴ� �Ŵ���

{
    #region pool
    class Pool
    {
        public GameObject Original { get; private set; }
        public Transform Root { get; set; } //@Pool_Root �Ʒ��� �ڱ������� �´� Ǯ ���Ͽ� ��ġ�ϵ��� �߰����� transform. �׳� ������������ �������.  

        Stack<Poolable> _poolStack = new Stack<Poolable>();

        public void Init(GameObject original, int count = 5) // Count��, ������� original_Root Ǯ�� ����ϰ�����, Ǯ����(Poolable) �ֵ��� �� �� �ǹ��Ѵ�.
        {
            Original = original;
            Root = new GameObject().transform;
            Root.name = $"{original.name}_Root";

            for(int i=0;i<count;i++)
                Push(Create());
        }

        Poolable Create() //Ǯ���� ��ü ����� �Լ�
        {
            GameObject go = Object.Instantiate<GameObject>(Original); //������ �����ؾߵǴ� Instantiate
            go.name = Original.name; //Clone�Ⱥٰ� �̸��� �׳� original���
            return go.GetOrAddComponent<Poolable>(); //Poolable ������Ʈ�� �ٿ�����.
        }

        public void Push(Poolable poolable) //create�� ���� Ǯ�������, ������ٰ� �ٷ� ���ÿ� Push���ִ°� �ƴ϶� ��Ȱ��ȭ��Ű��, original_root���Ϸ� �ű�� ���� ������ ó���ϰ�, ���ÿ� �ִ� ���
        {
            if (poolable == null) //poolalble�� ������ �հ� �߸��Ȱ���
                return;

            poolable.transform.parent = Root; //�θ� root�� �޾��ֱ�
            poolable.gameObject.SetActive(false); //��Ȱ��ȭ��Ű��
            poolable.IsUsing = false; //poolalble���� �ִ� ������ �׳༮. ������� �ƴ϶�� ����

            _poolStack.Push(poolable);
        }

        public Poolable Pop(Transform parent) //����parent�� Ȱ��ȭ�� Ǯ������ ��� ��ġ������ �Է����ָ�ȴ�.
        {
            Poolable poolable;

            if (_poolStack.Count > 0) 
                poolable = _poolStack.Pop();
            else
                poolable = Create(); //���� ���þȿ� �ִ°� ���ٸ�, ���� ���� �����ֱ�
            

            poolable.gameObject.SetActive(true);

            if (parent == null) //DontDestroyOnLoad ���� �뵵 (������ �ֵ��� �غ�� 5�� �̻��̸�, ���Ӱ� ������ �ȴ�. �׷��� ���Ӱ� �����̵Ǹ� parent�� null�̱⿡ DontDestroyOnLoad�� �پ������.)
                poolable.transform.parent = Managers.Scene.CurrentScene.transform; //��¦ �ļ��ε�, ���� �����Ǽ� parent�� ���¾ֵ���, ���� ������ �����ϴ� CurrentScene�� ���Ͽ� �ְڴٴ��ǹ�.
                                                                                   //�̷��ԵǸ� �Ʒ� poolable.transform.parent = parent=null;�� �Ǳ⿡, CurrentScene���� ������ ��������, �׷� �ڿ������� Scene�� �ִ� ��ġ�� ������ġ�� ���Եɰ��̴�.
            poolable.transform.parent = parent;
            poolable.IsUsing = true;

            return poolable;
        }
    }
    #endregion

    Dictionary<string, Pool> _pool = new Dictionary<string, Pool>(); //Ǯ�Ŵ����� �������� �̸�(string)�� ���� Pool���� ���������Ű�, �׷����� ���������� Ǯ�� ���� �ɰ��̴�.
    Transform _root; //��� transform�����ϵ� GameObject�� �ϵ� �������. ��¼�� ���� ������Ʈ���� transform�� �ֱ⶧��
    
    public void Init()
    {
        if (_root == null)
        {
            _root = new GameObject { name = "@Pool_Root" }.transform;
            Object.DontDestroyOnLoad(_root);
        }
    }

    public void Push(Poolable poolable) //Ǯ���� �ֵ��� ����ִ� �Լ�. ����ó�� Push Pop�Լ��� ���� ���ٰ��Ѵ�.
    {
        string name = poolable.gameObject.name;
        if(_pool.ContainsKey(name)==false) //���� �پ��� ����������µ�, �̳༮�� Ǯ�� ���ٸ�? �������� ����̱���
        {
            GameObject.Destroy(poolable.gameObject); //�׳� �����ع����ڴ�.
            return;
        }

        _pool[name].Push(poolable);
    }

    public Poolable Pop(GameObject original, Transform parent = null)
    {
        if (_pool.ContainsKey(original.name) == false) //���� Ǯ�� �����Ǳ� ���� Ǯ������, Pop�� ��û�ߴٸ�
            CreatePool(original); //�ش� Ǯ�� ���� ����ڴ�

        return _pool[original.name].Pop(parent);
    }
    public void CreatePool(GameObject original, int count = 5)
    {
        Pool pool = new Pool();
        pool.Init(original, count); //PoolŬ�������� ���� Init�� ����ϸ� �ڵ� �ߺ��� ��������.
        pool.Root.parent = _root;

        _pool.Add(original.name, pool);
    }

    public GameObject GetOriginal(string name) //���ҽ��� Load�� �ش��ϴ� �κ�, Ǯ���Ҿֵ��߿��� ����(������ ����?)�� �������
    {
        if (_pool.ContainsKey(name) == false)
            return null;

        return _pool[name].Original;
    }

    public void Clear() //���� �Ѿ��, Ǯ�� �־���� �������� �������ұ�? ���Ӹ��� �ٸ����̱��ѵ�.. �ϴ� ������ mmo��ǥ�� ������������.
    {
        foreach (Transform child in _root)
            GameObject.Destroy(child.gameObject);

        _pool.Clear();
    }
}
