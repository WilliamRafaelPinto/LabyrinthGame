using UnityEngine;

public class Obistacle : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
                GameManager.Instance.SubtractTime(5);

            Destroy(gameObject);
        }
    }
}