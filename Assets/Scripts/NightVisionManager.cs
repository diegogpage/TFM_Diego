using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class NightVisionManager : MonoBehaviour
{
    [SerializeField] private InputManagerSO inputManager;
    [SerializeField] private Volume nightVisionVolume;
    [SerializeField] private Volume darknessVolume;
    [SerializeField] private Image iconoVisionNocturna;

    [Header("TutorialCard")]
    [SerializeField] private TutorialCard tutorialCard;
    [SerializeField] private Sprite imagen;
    [TextArea]
    [SerializeField] private string mensaje;

    private bool tieneGafas = false;
    private bool nightVisionOn = false;
    private bool enZonaOscura = false;


    private void OnEnable()
    {
        inputManager.OnVisionNocturna += VisionNocturna;
    }


    private void Start()
    {
        if (nightVisionVolume != null)
        {
            nightVisionVolume.gameObject.SetActive(true);
            nightVisionVolume.enabled = false;
        }
        if (darknessVolume != null)
        {
            darknessVolume.gameObject.SetActive(true);
            darknessVolume.enabled = false;
        }

        if (SaveSystem.HayCheckpoint())
        {
            tieneGafas = SaveSystem.CargarGafas();
        }

    }

    private void Update()
    {
        if (tieneGafas)
        {
            iconoVisionNocturna.gameObject.SetActive(true);
        }
    }


    private void VisionNocturna()
    {
        Debug.Log("Vision nocturna");
        if (!tieneGafas || !enZonaOscura)
        {
            return;
        }

        nightVisionOn = !nightVisionOn;  // cambio el estado

        if (nightVisionVolume != null)
        {
            nightVisionVolume.enabled = nightVisionOn;  // se pone true o false en funcion del bool
        }

    }

    public void EntrarZonaOscura()
    {
        Debug.Log("Entra en zona oscura");
        enZonaOscura = true;
        nightVisionOn = false;
        
        if (darknessVolume != null)
        {
            darknessVolume.enabled = true;
        }
    }

    public void SalirZonaOscura()
    {
        Debug.Log("Sale de la zona oscura");
        enZonaOscura = false;
        nightVisionOn = false;

        if (darknessVolume != null)
        {
            darknessVolume.enabled = false;
        }
        if(nightVisionVolume != null)
        {
            nightVisionVolume.enabled = false;
        }
    }

    public void RecogerGafas()
    {
        tieneGafas = true;
        tutorialCard.Mostrar(imagen, mensaje);
    }

    public bool TieneGafas()
    {
        return tieneGafas;
    }
}
