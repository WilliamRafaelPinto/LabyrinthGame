using UnityEngine;

public class MoverObjeto : MonoBehaviour
{
    public float velocidade = 5.0f;
    public float desaceleracao = 10.0f; // A taxa de desaceleração
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float movimentoHorizontal = Input.GetAxis("Horizontal"); // A/D ou seta esquerda/direita
        float movimentoVertical = Input.GetAxis("Vertical"); // W/S ou seta cima/baixo

        Vector3 movimento = new Vector3(movimentoHorizontal, 0.0f, movimentoVertical).normalized; // Normaliza o movimento

        if (movimento.magnitude > 0)
        {
            // Se houver movimento, aplica a velocidade
            rb.linearVelocity = movimento * velocidade;
        }
        else
        {
            // Se não houver movimento, desacelera suavemente
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, desaceleracao * Time.fixedDeltaTime);
        }
    }
}