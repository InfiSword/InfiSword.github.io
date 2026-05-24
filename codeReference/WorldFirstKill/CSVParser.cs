using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

public class CSVParser
{
    // CSV 속성값 / Data 필드값 
    protected Dictionary<string, string> Data_Dict = new Dictionary<string, string>();

    // index / CSV 속성값
    protected Dictionary<int, string> DataIndex_Dict = new Dictionary<int, string>();

    string[] SmartSplit(string line)
    {
        return Regex.Matches(line, @"(?:^|,)(?:""(?<val>[^""]*)""|(?<val>[^,""]*))")
                    .Cast<Match>()
                    .Select(m => m.Groups["val"].Value)
                    .ToArray();
    }


    public List<T> parseCSV<T>(string csvData, bool isCutColumn, int start, int end) where T : BaseData, new()
    {
        if(isCutColumn)
            return parseCSV<T>(csvData, start, end);


        List<T> csvDataList = new List<T>();

        FieldInfo[] myFieldInfo;
        Type myType = typeof(T);
        myFieldInfo = myType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance
            | BindingFlags.Public | BindingFlags.Static);


        // 각 CSV 파일내용을 저장
        string[] csvIndexData = csvData.Split('\n')
            .Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

        if (csvData.Length == 0)
            return null;

        // attribute
        string[] attributeColumnArray = csvIndexData[0].Trim().Split(',');

        SetupFieldDict(attributeColumnArray, myFieldInfo);

        for (int rowIndex = 1; rowIndex < csvIndexData.Length; rowIndex++)
        {
            string[] column = SmartSplit(csvIndexData[rowIndex].Trim());
            if (column.Length == 0 || string.IsNullOrWhiteSpace(column[0]))
                continue;

            T objectData = Activator.CreateInstance<T>(); ;

            for (int i = 0; i < column.Length; i++)
            {
                if (DataIndex_Dict.TryGetValue(i, out string attribute) &&
                    Data_Dict.TryGetValue(attribute, out string fieldName))
                {
                    FieldInfo fieldInfo = myType.GetField(fieldName);
                    string columnData = column[i];

                    if (fieldInfo != null)
                    {
                        SetFieldValue(objectData, fieldInfo, columnData);
                    }
                }
            }

            csvDataList.Add(objectData);

        }
        return csvDataList;
    }

    protected List<T> parseCSV<T>(string csvData, int _startIndex, int _endIndex) where T : BaseData, new()
    {
        List<T> csvDataList = new List<T>();

        FieldInfo[] myFieldInfo;
        Type myType = typeof(T);
        myFieldInfo = myType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance
            | BindingFlags.Public | BindingFlags.Static);


        // 각 CSV 파일내용을 저장
        string[] csvIndexData = csvData.Split('\n')
            .Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

        if (csvData.Length == 0)
            return null;

        // attribute
        string[] attributeColumnArray = csvIndexData[0].Trim().Split(',');

        string[] slicedHeader = attributeColumnArray
            .Skip(_startIndex)
            .Take(_endIndex - _startIndex + 1)
            .ToArray();

        SetupFieldDict(slicedHeader, myFieldInfo);

        for (int rowIndex = 1; rowIndex < csvIndexData.Length; rowIndex++)
        {
            string[] column = csvIndexData[rowIndex].Trim().Split(',');
            if (column.Length == 0 || string.IsNullOrWhiteSpace(column[0]))
                continue;

            T objectData = Activator.CreateInstance<T>(); ;

            for (int i = 0; i < column.Length; i++)
            {
                if (DataIndex_Dict.TryGetValue(i, out string attribute) &&
                    Data_Dict.TryGetValue(attribute, out string fieldName))
                {
                    FieldInfo fieldInfo = myType.GetField(fieldName);
                    string columnData = column[i];

                    if (fieldInfo != null)
                    {
                        SetFieldValue(objectData, fieldInfo, columnData);
                    }
                }
            }

            csvDataList.Add(objectData);

        }
        return csvDataList;
    }

    protected virtual void SetupFieldDict(string[] attribute, FieldInfo[] fields)
    {
        Data_Dict.Clear();
        DataIndex_Dict.Clear();

        for (int i = 0; i < attribute.Length; i++)
        {
            string header = attribute[i].Trim();
            Data_Dict[header] = fields[i].Name;
            DataIndex_Dict[i] = header;
        }
    }
    protected virtual void SetFieldValue(BaseData instance, FieldInfo fieldInfo, string value)
    {
        if (fieldInfo.FieldType == typeof(string))
            fieldInfo.SetValue(instance, value);
        else if (fieldInfo.FieldType == typeof(int) && int.TryParse(value, out int intVal))
            fieldInfo.SetValue(instance, intVal);
        else if (fieldInfo.FieldType == typeof(float) && float.TryParse(value, out float floatVal))
            fieldInfo.SetValue(instance, floatVal);
    }
}

