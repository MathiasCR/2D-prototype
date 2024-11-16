using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public Transform cameraTransform;  // R�f�rence au transform de la cam�ra
    public float shakeDuration = 0.5f; // Dur�e du tremblement
    public float shakeAmount = 0.1f;   // Intensit� du tremblement
    public float decreaseFactor = 1.0f; // Vitesse � laquelle l'effet de tremblement s'att�nue

    private Vector3 originalPos;        // Position originale de la cam�ra
    private float currentShakeDuration; // Dur�e actuelle du tremblement

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
            // Applique un l�ger d�placement al�atoire autour de la position originale
            cameraTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;

            // R�duit la dur�e restante du tremblement
            currentShakeDuration -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            // Reset � la position initiale une fois le tremblement termin�
            currentShakeDuration = 0f;
            cameraTransform.localPosition = originalPos;
        }
    }

    // Fonction � appeler pour d�marrer le tremblement
    public void TriggerShake(float duration = -1)
    {
        currentShakeDuration = (duration > 0.0f) ? duration : shakeDuration;
        originalPos = cameraTransform.localPosition; // Enregistre la position initiale
    }
}
