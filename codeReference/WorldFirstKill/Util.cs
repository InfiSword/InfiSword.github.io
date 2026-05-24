using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using static Define;

public class Util
{
    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
		if (component == null)
            component = go.AddComponent<T>();
        return component;
	}
    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform == null)
            return null;

        return transform.gameObject;
    }
    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;

        if (recursive == false)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || transform.name == name)
                {
                    T component = transform.GetComponent<T>();
                    if (component != null)
                        return component;
                }
            }
		}
        else
        {
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }                

        return null;
    }

    // int 리스트를 셔플시킴
    public static List<int> SuffleList(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randNum = UnityEngine.Random.Range(0, list.Count);
            int temp = list[i];
            list[i] = list[randNum];
            list[randNum] = temp;
        }

        return list;
    }

    // int 배열을 셔플시킴
    public static int[] SuffleArray(int[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            int randNum = UnityEngine.Random.Range(0, array.Length);
            int temp = array[i];
            array[i] = array[randNum];
            array[randNum] = temp;
        }

        return array;
    }

    public static Equipment PickRandomWeapon()
    {
        var weaponDict = Managers.Data.WeaponData;
        if (weaponDict == null || weaponDict.Count == 0)
            return null;

        // 키 목록으로부터 랜덤 ID 추출
        var keys = weaponDict.Keys.ToList();
        int randIndex = UnityEngine.Random.Range(0, keys.Count);
        int weaponId = keys[randIndex];

        return DataFactory.MakeEquip(EquipRarity.Normal, weaponId, 0, 0, 0);
    }

    public static Color32 PlayerColor(Define.UIColor playerColor)
    {
        switch (playerColor)
        {
            case UIColor.Red:
                return new Color32(255, 130, 130, 255);
            case UIColor.Green:
                return new Color32(130, 255, 130, 255);
            case UIColor.Blue:
                return new Color32(130, 230, 255, 255);
            case UIColor.Yellow:
                return new Color32(255, 230, 130, 255);
            case UIColor.White:
                return new Color32(255, 255, 255, 255);
            case UIColor.Gray:
                return new Color32(200, 200, 200, 255);
            case UIColor.Black:
                return new Color32(70, 70, 90, 255);
            default:
                return new Color32(255, 255, 255, 255);
        }
    }
    public static Color32 RarityColor(Define.EquipRarity rarityColor)
    {
        switch (rarityColor)
        {
            case EquipRarity.Normal:
                return new Color32(200, 200, 200, 255);
            case EquipRarity.Rare:
                return new Color32(100, 150, 255, 255);
            case EquipRarity.Epic:
                return new Color32(200, 150, 255, 255);
            case EquipRarity.Unique:
                return new Color32(255, 255, 125, 255);
            case EquipRarity.Legendary:
                return new Color32(255, 100, 100, 255);
            default:
                return new Color32(255, 255, 255, 255);
        }
    }

    /// <summary>
    ///  제네릭 메서드로 모든 Enum 타입에서 랜덤 선택 ( 매개변수로 특정 값 제외 가능)   
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="excludeValues"></param>
    /// <returns></returns>
    public static T GetRandomEnum<T>(params T[] excludeValues) where T : Enum
    {
        var allValues = Enum.GetValues(typeof(T)).Cast<T>();

        if (excludeValues != null && excludeValues.Length > 0)
        {
            allValues = allValues.Where(v => !excludeValues.Contains(v));
        }

        T[] validValues = allValues.ToArray();

        int randomIndex = UnityEngine.Random.Range(0, validValues.Length);
        return validValues[randomIndex];
    }

    public static T GetRandomEnum<T>() 
    { 
        Array values = Enum.GetValues(typeof(T));
        return (T)values.GetValue(new System.Random().Next(0, values.Length)); 
    }

    public static int GenerateRandomBaseSeed()
    {
        // 시스템 난수 기반으로 int 전체 범위에서 시드 생성
        return RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue);
    }

    public static bool TryParseSeed(string input, out long seed)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            seed = 0;
            return false;
        }
        return long.TryParse(input.Trim(), out seed);
    }
}
