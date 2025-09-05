using UnityEngine;

public class Inventario : MonoBehaviour
{
    private bool tieneGafas = false;

    public void DarGafasInventario()
    {
        tieneGafas = true;
        Debug.Log("Gafas en el inventario");
    }

    public bool TieneGafas()
    {
        return tieneGafas;
    }
}
