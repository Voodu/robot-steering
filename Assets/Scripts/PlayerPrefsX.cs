using UnityEngine;

public class PlayerPrefsX
{
    public static void SetBool(string key, bool booleanValue)
    {
        PlayerPrefs.SetInt(key, booleanValue ? 1 : 0);
    }

    public static bool GetBool(string key)
    {
        return PlayerPrefs.GetInt(key) == 1 ? true : false;
    }

    public static bool GetBool(string key, bool defaultValue)
    {
        if (PlayerPrefs.HasKey(key))
        {
            return GetBool(key);
        }

        return defaultValue;
    }
}