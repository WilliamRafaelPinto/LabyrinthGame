using UnityEngine;

public class RotacaoAutomatica : MonoBehaviour
{
    public float velocidadeRotacao = 100.0f; // Velocidade de rotação

    void Update()
    {
        // Calcula a rotação contínua ao longo do eixo Y
        Vector3 rotacao = new Vector3(velocidadeRotacao * Time.deltaTime, 0, 0);

        // Aplica a rotação ao objeto
        transform.Rotate(rotacao);
    }
}
