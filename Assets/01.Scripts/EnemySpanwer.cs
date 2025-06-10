using Redcode.Pools;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpanwer : MonoBehaviour
{
    //3초마다 Enemy_HoverBot 정해진 위치들에 스폰
    //최대 10마리를 넘기지 않음

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
            randomPos.y = 0; // 필요에 따라 y값 조정

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
