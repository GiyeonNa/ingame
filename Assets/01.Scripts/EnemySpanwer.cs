using Redcode.Pools;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpanwer : MonoBehaviour
{
    //3�ʸ��� Enemy_HoverBot ������ ��ġ�鿡 ����
    //�ִ� 10������ �ѱ��� ����

    public GameObject enemyPrefab;
    public int maxEnemyCount = 10;
    public float spawnInterval = 3f;
    public float spawnRadius = 20f;

    private float timer;
    private int currentEnemyCount;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval && currentEnemyCount < maxEnemyCount)
        {
            Vector3 randomPos = transform.position + Random.insideUnitSphere * spawnRadius;
            randomPos.y = 0; // �ʿ信 ���� y�� ����

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPos, out hit, 5f, NavMesh.AllAreas))
            {
                Instantiate(enemyPrefab, hit.position, Quaternion.identity);
                currentEnemyCount++;
            }
            timer = 0f;
        }
    }

}
