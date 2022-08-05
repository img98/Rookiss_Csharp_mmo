using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawningPool : MonoBehaviour
{
    [SerializeField]
    int _monsterCount = 0;
    [SerializeField]
    int _reserveCount = 0;
    [SerializeField]
    int _keepMonsterCount = 0;
    [SerializeField]
    Vector3 _spawnPos;
    [SerializeField]
    float _spawnRadius = 15.0f;
    [SerializeField]
    float _spawnTime = 5.0f;

    public void AddMonsterCount(int value) { _monsterCount += value; }
    public void SetKeepMonsterCount(int count) { _keepMonsterCount = count; } 

    void Start()
    {
        Managers.Game.OnSpawnEvent -= AddMonsterCount;
        Managers.Game.OnSpawnEvent += AddMonsterCount; //����ī��Ʈ �Լ��� OnSpawnEvent�� �����Ͽ� Invoke�� ��������
    }

    void Update()
    {
        while(_reserveCount+_monsterCount<_keepMonsterCount ) 
        {
            StartCoroutine("ReserveSpawn");
        }
    }
    IEnumerator ReserveSpawn() //Managers.Game.Spawnó�� ���� ������ �ϴ°� �ƴ϶�, ������ġ, �����ð� ��ɱ��� �߰����ٰŶ� ���� �Լ��� ����°��̴�.
    {
        _reserveCount++;

        // 0�ʿ��� ���������ð� ���̿�, ������Ű�� ������ �ð��� ��� �����Ҽ������� => �ڷ�ƾ�� �̿��ϸ� ����.
        yield return new WaitForSeconds(Random.Range(0, _spawnTime));
        //�� �����ð��� ������ �Ʒ� �ڵ� �۵�
        GameObject obj = Managers.Game.Spawn(Define.WorldObject.Monster,"Knight");

        NavMeshAgent nma = obj.GetOrAddComponent<NavMeshAgent>(); //������ġ�� ��ȿ���� Ȯ���Ϸ��� ��ã�Ⱑ ȿ������. ��ã���Ϸ��� NMA�� �޾����.
        
        Vector3 randPos;
        while (true) 
        {
            Vector3 randDir = Random.insideUnitSphere * (Random.Range(0, _spawnRadius)); //Random.insideUnitSphere => ���� ���͸� ����µ� ������ �����ϴ�!. ���⿡ Radius�� ���� ũ����� ��������.
            randDir.y = 0; //3d�̱�� �ϳ� �����̳� �ϴÿ� ������ų�� �ƴϴ� y���� 0���� �о���ߵ�

            randPos = _spawnPos + randDir; //������ �߽��� + �������⺤�� = �������� ������ ��ġ
            NavMeshPath path = new NavMeshPath();
            if(nma.CalculatePath(randPos, path)) //�����ִ��� Ȯ��
                break;
        }

        obj.transform.position = randPos; //��ã�⸦ ���� �� ����pos�� �����ִ°����� �˾�����, ���� �ʴ� �ű⼭ �¾��.
        _reserveCount--;
    }
}
