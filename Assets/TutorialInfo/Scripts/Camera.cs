using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform alvo; // O objeto que a câmera deve seguir
    public Vector3 offset; // Deslocamento da câmera em relação ao alvo
    public float suavizacao = 0.125f; // Fator de suavização para o movimento da câmera

    void LateUpdate()
    {
        if (alvo != null)
        {
            Vector3 posicaoDesejada = alvo.position + offset;
            Vector3 posicaoSuave = Vector3.Lerp(transform.position, posicaoDesejada, suavizacao);
            transform.position = posicaoSuave;

            transform.LookAt(alvo); // Faz a câmera olhar para o alvo
        }
    }
}