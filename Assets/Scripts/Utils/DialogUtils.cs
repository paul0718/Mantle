using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogUtils 
{
    public static Dictionary<string, Dictionary<string, string>> ParseJsonToDictionary(string jsonString)
    {
        try
        {
            var parsedData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(jsonString);
            return parsedData;
        }
        catch (JsonException ex)
        {
            Debug.LogError("Error parsing JSON: " + ex.Message);
            return null;
        }
    }
}
