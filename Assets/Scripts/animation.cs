using UnityEngine;

public class AnimatedVortex : MonoBehaviour
{
    public float spinSpeed = 20f; // Velocidade do giro
    public float pulseSpeed = 1f; // Velocidade da pulsação
    public float pulseIntensity = 0.5f; // Força da pulsação
    public Color baseColor = Color.cyan; // Cor base do vortex

    private Material mat;
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
        mat = rend.material; // Obtém uma cópia do material para não afetar outros
    }

    void Update()
    {
        // 1. GIRA a textura (modifica o offset no eixo U)
        float currentOffset = mat.mainTextureOffset.x;
        mat.mainTextureOffset = new Vector2(currentOffset + (spinSpeed * Time.deltaTime / 360f), 100f);

        // 2. PULSA a emissão (brilho)
        float pulseFactor = Mathf.Sin(Time.time * pulseSpeed) * pulseIntensity + 1f;
        Color emissiveColor = baseColor * pulseFactor;
        mat.SetColor("_EmissionColor", emissiveColor);

        // (Opcional) 3. GIRA os ponteiros (se for um objeto separado)
        // transform.Rotate(0, spinSpeed * 2 * Time.deltaTime, 0);
    }

    void OnDestroy()
    {
        // Destrói a cópia do material criada no Start para evitar vazamento de memória
        if (rend != null)
        {
            Destroy(mat);
        }
    }
}