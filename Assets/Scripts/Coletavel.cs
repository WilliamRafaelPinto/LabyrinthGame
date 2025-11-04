// using UnityEngine;

// public class Collectible : MonoBehaviour
// {
//     void OnTriggerEnter(Collider other)
//     {
//         if (other.CompareTag("Player"))
//         {
//             GameManager.Instance.Collect();
//             Destroy(gameObject);
//         }
//     }
// }


using UnityEngine;

public class Collectible : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
                GameManager.Instance.CollectCoin();

            Destroy(gameObject);
        }
    }
}
