using Redcode.Pools;
using UnityEngine;

public class Enemy_HoverBot : MonoBehaviour, IPoolObject
{
    public void OnCreatedInPool()
    {
        Debug.Log("Bot created in pool: " + gameObject.name);
    }

    public void OnGettingFromPool()
    {

        Debug.Log("Bot getting from pool: " + gameObject.name);
        // Reset any state or properties if needed
        // For example, reset health, position, etc.
        transform.position = Vector3.zero; // Example reset
        gameObject.SetActive(true); // Ensure the bot is active when retrieved from the pool
    }

}
