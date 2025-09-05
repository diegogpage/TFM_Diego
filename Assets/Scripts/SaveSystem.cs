using UnityEngine;

public static class SaveSystem
{
    public static void GuardarCheckpoint(Vector3 pos, int vida, bool tieneGafas)
    {
        PlayerPrefs.SetFloat("x", pos.x);
        PlayerPrefs.SetFloat("y", pos.y);
        PlayerPrefs.SetFloat("z", pos.z);

        PlayerPrefs.SetInt("vida", vida);
        PlayerPrefs.SetInt("tieneGafas", tieneGafas ? 1 : 0);

        PlayerPrefs.SetInt("checkpointAlcanzado", 1);
        PlayerPrefs.Save();
    }

    public static Vector3 CargarCheckpoint()
    {
        if (PlayerPrefs.GetInt("checkpointAlcanzado", 0) == 1)
        {
            float x = PlayerPrefs.GetFloat("x");
            float y = PlayerPrefs.GetFloat("y");
            float z = PlayerPrefs.GetFloat("z");
            return new Vector3(x, y, z);
        }
        return Vector3.zero;
    }

    public static int CargarVida()
    {
        return PlayerPrefs.GetInt("vida", 100);
    }

    public static bool CargarGafas()
    {
        return PlayerPrefs.GetInt("tieneGafas", 0) == 1;
    }

    public static bool HayCheckpoint()
    {
        return PlayerPrefs.GetInt("checkpointAlcanzado", 0) == 1;
    }

    public static void ResetProgress()
    {
        PlayerPrefs.DeleteAll();
    }
}
