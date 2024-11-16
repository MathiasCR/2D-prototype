using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public Transform cameraTransform;  // Référence au transform de la caméra
    public float shakeDuration = 0.5f; // Durée du tremblement
    public float shakeAmount = 0.1f;   // Intensité du tremblement
    public float decreaseFactor = 1.0f; // Vitesse à laquelle l'effet de tremblement s'atténue

    private Vector3 originalPos;        // Position originale de la caméra
    private float currentShakeDuration; // Durée actuelle du tremblement

    private void Start()
    {
        if (cameraTransform == null)
        {
            cameraTransform = GetComponent<Transform>();
        }
        originalPos = cameraTransform.localPosition;
    }

    private void Update()
    {
        if (currentShakeDuration > 0)
        {
            // Applique un léger déplacement aléatoire autour de la position originale
            cameraTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;

            // Réduit la durée restante du tremblement
            currentShakeDuration -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            // Reset à la position initiale une fois le tremblement terminé
            currentShakeDuration = 0f;
            cameraTransform.localPosition = originalPos;
        }
    }

    // Fonction à appeler pour démarrer le tremblement
    public void TriggerShake(float duration = -1)
    {
        currentShakeDuration = (duration > 0.0f) ? duration : shakeDuration;
        originalPos = cameraTransform.localPosition; // Enregistre la position initiale
    }
}
