using UnityEngine;
using UnityEngine.UIElements;

public class LaserBehaviour : MonoBehaviour
{
    [SerializeField] private float speed;
    private PlayerLife player;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider elOtro)
    {
       if (elOtro.gameObject.GetComponent<PlayerLife>())
        {
            player = elOtro.gameObject.GetComponent<PlayerLife>();
            player.QuitarVida();
            Destroy(gameObject);
        }
    }
}
