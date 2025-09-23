using UnityEngine;

public class RotacaoAutomatica : MonoBehaviour
{
    public float velocidadeRotacao_X = 0.0f; // Velocidade de rotação
    public float velocidadeRotacao_Y = 0.0f; // Velocidade de rotação
    public float velocidadeRotacao_Z = 0.0f; // Velocidade de rotação

    void Update()
    {
        // Calcula a rotação contínua ao longo do eixo Y
        Vector3 rotacao = new Vector3(velocidadeRotacao_X * Time.deltaTime, velocidadeRotacao_Y * Time.deltaTime, velocidadeRotacao_Z * Time.deltaTime);

        // Aplica a rotação ao objeto
        transform.Rotate(rotacao);
    }
}
