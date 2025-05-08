using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class Extensions
{
    #region Vector3
    public static Vector3 With(this Vector3 original, float? x = null, float? y = null, float? z = null)
    {
        return new Vector3(x ?? original.x, y ?? original.y, z ?? original.z);
    }

    public static float DistanceTo(this Vector3 original, Vector3 target)
    {
        return Vector3.Distance(original, target);
    }

    public static Vector3 Add(this Vector3 original, float? x = null, float? y = null, float? z = null)
    {
        return new Vector3(original.x + (x ?? 0.0f), original.y + (y ?? 0.0f), original.z + (z ?? 0.0f));
    }
    #endregion

    #region List
    public static T Random<T>(this List<T> original)
    {
        if (original.Count > 0)
        {
            return original[UnityEngine.Random.Range(0, original.Count)];
        }
        else
        {
            return default;
        }
    }

    private static System.Random rng = new System.Random();

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static T Random<T>(this List<T> original, out int? index)
    {
        if (original.Count > 0)
        {
            index = UnityEngine.Random.Range(0, original.Count);
            return original[(int)index];
        }
        else
        {
            index = null;
            return default;
        }
    }

    public static List<T> Random<T>(this List<T> original, int amount)
    {
        List<T> list = new List<T>(original);
        List<T> elements = new List<T>();

        amount = Mathf.Clamp(amount, 0, original.Count);
        for (int i = 0; i < amount; i++)
        {
            int index = UnityEngine.Random.Range(0, list.Count);
            elements.Add(list[index]);
            list.RemoveAt(index);
        }

        return elements;
    }

    public static List<T> Random<T>(this List<T> original, int amount, out List<int> indexes)
    {
        List<T> list = new List<T>(original);
        List<T> elements = new List<T>();

        amount = Mathf.Clamp(amount, 0, original.Count);
        indexes = new List<int>();
        for (int i = 0; i < amount; i++)
        {
            int index = UnityEngine.Random.Range(0, list.Count);
            elements.Add(list[index]);
            indexes.Add(index);
            list.RemoveAt(index);
        }

        return elements;
    }

    public static bool Find<T>(this List<T> original, System.Predicate<T> match, out T element)
    {
        element = original.Find(match);
        return element != null;
    }

    public static bool FindAll<T>(this List<T> original, System.Predicate<T> match, out List<T> elements)
    {
        elements = original.FindAll(match);
        return elements.Count > 0;
    }

    public static bool FindLast<T>(this List<T> original, System.Predicate<T> match, out T element)
    {
        element = original.FindLast(match);
        return element != null;
    }

    public static bool FindAmount<T>(this List<T> original, int amount, System.Predicate<T> match, out List<T> elements)
    {
        List<T> matchElements = new List<T>(original.FindAll(match));

        if (matchElements.Count > amount)
        {
            elements = new List<T>(matchElements.Random(amount));
            return true;
        }

        elements = new List<T>();
        return false;
    }

    public static void Move<T>(this List<T> original, int oldIndex, int newIndex)
    {
        T item = original[oldIndex];
        original.RemoveAt(oldIndex);
        original.Insert(newIndex, item);
    }

    public static int IndexOf<T>(this IReadOnlyList<T> self, T elementToFind)
    {
        int i = 0;
        foreach (T element in self)
        {
            if (Equals(element, elementToFind))
            {
                return i;
            }

            i++;
        }
        return -1;
    }

    public static T Random<T>(this IReadOnlyList<T> original)
    {
        if (original.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, original.Count);
            return original[index];
        }

        return default;
    }

    public static List<T> Random<T>(this IReadOnlyList<T> original, int amount)
    {
        List<T> list = new List<T>(original);
        List<T> elements = new List<T>();

        amount = Mathf.Clamp(amount, 0, original.Count);
        for (int i = 0; i < amount; i++)
        {
            int index = UnityEngine.Random.Range(0, list.Count);
            elements.Add(list[index]);
            list.RemoveAt(index);
        }

        return elements;
    }

    public static bool Exists<T>(this IReadOnlyList<T> original, System.Predicate<T> match)
    {
        List<T> list = new List<T>(original);
        return list.Exists(match);
    }

    public static bool Contains<T>(this IReadOnlyList<T> original, T match)
    {
        List<T> list = new List<T>(original);
        return list.Contains(match);
    }
    #endregion

    #region Array
    public static T Random<T>(this T[] original)
    {
        if (original.Length > 0)
        {
            return original[UnityEngine.Random.Range(0, original.Length)];
        }
        else
        {
            return default;
        }
    }

    public static T Random<T>(this T[] original, out int? index)
    {
        if (original.Length > 0)
        {
            index = UnityEngine.Random.Range(0, original.Length);
            return original[(int)index];
        }
        else
        {
            index = null;
            return default;
        }
    }

    public static List<T> Random<T>(this T[] original, int amount)
    {
        List<T> list = new List<T>(original);
        return list.Random(amount);
    }

    public static List<T> Random<T>(this T[] original, int amount, System.Predicate<T> exclude)
    {
        List<T> list = new List<T>(original);
        list.RemoveAll(exclude);
        return list.Random(amount);
    }

    public static List<T> Random<T>(this T[] original, int amount, out List<int> indexes)
    {
        List<T> list = new List<T>(original);
        List<T> elements = new List<T>();

        amount = Mathf.Clamp(amount, 0, original.Length);
        indexes = new List<int>();
        for (int i = 0; i < amount; i++)
        {
            int index = UnityEngine.Random.Range(0, list.Count);
            elements.Add(list[index]);
            indexes.Add(index);
            list.RemoveAt(index);
        }

        return elements;
    }
    #endregion

    #region Transform
    public static float DistanceTo(this Transform original, Transform target)
    {
        return Vector3.Distance(original.position, target.position);
    }

    public static bool GetComponent<T>(this Transform original, out T component)
    {
        component = original.GetComponent<T>();
        return component != null;
    }

    public static bool GetComponentInParent<T>(this Transform original, out T component)
    {
        component = original.GetComponentInParent<T>();
        return component != null;
    }

    public static bool GetComponentInChildren<T>(this Transform original, out T component)
    {
        component = original.GetComponentInChildren<T>();
        return component != null;
    }
    #endregion

    #region Color
    public static Color With(this Color original, float? r = null, float? g = null, float? b = null, float? a = null)
    {
        return new Color(r ?? original.r, g ?? original.g, b ?? original.b, a ?? original.a);
    }

    public static Color GetTransparent(this Color color)
    {
        return new Color(color.r, color.g, color.b, 0.0f);
    }
    #endregion

    #region GameObject
    public static void ToggleActive(this GameObject original, bool? active = null)
    {
        original.SetActive(active ?? !original.activeSelf);
    }
    #endregion

    #region Object
    public static bool Chance(this Object original, int chance)
    {
        return UnityEngine.Random.Range(0, 100) < chance;
    }

    public static bool Chance(this Object original, int chance, out int randomNumber)
    {
        randomNumber = UnityEngine.Random.Range(0, 100);
        return randomNumber < chance;
    }
    #endregion

    #region int
    public static bool IsBetween(this int original, int minInclusive, int maxInclusive)
    {
        return original >= minInclusive && original <= maxInclusive;
    }
    #endregion

    #region string

    public static string GetBetween(this string strSource, string strStart, string strEnd)
    {
        if (strSource.Contains(strStart) && strSource.Contains(strEnd))
        {
            int Start, End;
            Start = strSource.IndexOf(strStart, 0) + strStart.Length;
            End = strSource.IndexOf(strEnd, Start);
            return strSource.Substring(Start, End - Start);
        }

        return "";
    }

    #endregion

    #region Texture2D

    public static void SaveTextureAsPNG(this Texture2D texture, string directoryPath, string fileName)
    {
        byte[] bytes = texture.EncodeToPNG();
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        File.WriteAllBytes(directoryPath + fileName + ".png", bytes);
    }

    #endregion

    #region MeshRenderer
    public static void ToggleEmission(this Material material, bool? active = null)
    {
        bool enable = active ?? !material.IsKeywordEnabled("_EMISSION");

        if (enable)
        {
            material.EnableKeyword("_EMISSION");
        }
        else
        {
            material.DisableKeyword("_EMISSION");
        }
    }
    #endregion
}


public static class PrisonSimulatorExtensions
{
    #region NPCInfo
    public static bool ValidateNameInput(this TMPro.TMP_InputField inputField)
    {
        if (inputField.text.Contains("<"))
        {
            inputField.text = inputField.text.Replace("<", "");
            return false;
        }

        if (inputField.text.Contains(">"))
        {
            inputField.text = inputField.text.Replace(">", "");
            return false;
        }

        if (inputField.text.Contains("{"))
        {
            inputField.text = inputField.text.Replace("{", "");
            return false;
        }

        if (inputField.text.Contains("}"))
        {
            inputField.text = inputField.text.Replace("}", "");
            return false;
        }

        if (inputField.text.Contains("\\"))
        {
            inputField.text = inputField.text.Replace("\\", "");
            return false;
        }

        if (inputField.text.Contains("/"))
        {
            inputField.text = inputField.text.Replace("/", "");
            return false;
        }

        return true;
    }
    #endregion
}