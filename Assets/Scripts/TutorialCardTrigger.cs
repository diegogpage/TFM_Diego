using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class TutorialCardTrigger : MonoBehaviour
{

    [SerializeField] private TutorialCard tutorialCard;
    [SerializeField] private Sprite imagen;
    [TextArea]
    [SerializeField] private string mensaje;

    private bool yaMostrado = false;

    
    private void OnTriggerEnter(Collider elOtro)
    {
        if (yaMostrado) return;

        if (elOtro.tag == "Player")
        {
            tutorialCard.Mostrar(imagen, mensaje);
            yaMostrado = true;
        }
    }
}
