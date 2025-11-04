// using UnityEngine;

// public class MoverObjeto : MonoBehaviour
// {
//     public float velocidade = 0.0f;

//     public float velocidade_maxima = 10.0f;
//     public float aceleracao = 3.0f; // A taxa de desaceleração
//     public float desaceleracao = 1.0f; // A taxa de desaceleração
//     private Rigidbody rb;

//     void Start()
//     {
//         rb = GetComponent<Rigidbody>();
//     }

//     void FixedUpdate()
//     {
//         float movimentoHorizontal = Input.GetAxis("Horizontal"); // A/D ou seta esquerda/direita
//         float movimentoVertical = Input.GetAxis("Vertical"); // W/S ou seta cima/baixo

//         Vector3 movimento = new Vector3(movimentoHorizontal, 0.0f, movimentoVertical).normalized; // Normaliza o movimento

//         if (movimento.magnitude > 0)
//         {
//             // Se houver movimento, aplica a velocidade
//             rb.linearVelocity = movimento * velocidade;
//         }
//         else
//         {
//             // Se não houver movimento, desacelera suavemente
//             rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, desaceleracao * Time.fixedDeltaTime);
//         }
//     }
// }

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementRB : MonoBehaviour
{
    public float maxSpeed = 10f;
    public float acceleration = 5f;
    public float deceleration = 5f;

    private Rigidbody rb;
    private Vector3 velocity;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //rb.freezeRotation = true; // evita que tombe
    }

    void Update()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");
        Vector3 inputDir = new Vector3(inputX, 0, inputZ).normalized;

        if (inputDir.magnitude > 0.1f)
        {
            // 🚀 Aceleração exponencial
            velocity = Vector3.Lerp(velocity, inputDir * maxSpeed,
                1f - Mathf.Exp(-acceleration * Time.deltaTime));
        }
        else
        {
            // 🛑 Desaceleração suave
            velocity = Vector3.Lerp(velocity, Vector3.zero,
                1f - Mathf.Exp(-deceleration * Time.deltaTime));
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
    }
}
