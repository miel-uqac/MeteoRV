using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;

public class WeatherRequest : MonoBehaviour
{
    public TMPro.TextMeshProUGUI temperatureText;
    public GameObject lightningStormCloud;

    private bool isUpdating = false;
    
    // Météo à Montréal
    private string url =
        "https://api.open-meteo.com/v1/forecast?latitude=45.50&longitude=-73.56&current_weather=true";


    void Start()
    {
        UpdateWeather(); 
    }

    // Cette fonction pourrait être appelée par un bouton
    // ou appelée automatiquement, p. ex., InvokeRepeating(nameof(UpdateWeather), 0f, 600f);
    public void UpdateWeather() 
    {
        if (!isUpdating)
            StartCoroutine(GetWeather());
    }

    IEnumerator GetWeather()
    {
        isUpdating = true;
        temperatureText.text = "Loading...";

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            WeatherData data =
                JsonUtility.FromJson<WeatherData>(request.downloadHandler.text);

            float temp = data.current_weather.temperature;
            int code = data.current_weather.weathercode;

            temperatureText.text = temp + "°C";

            // Reset
            lightningStormCloud.SetActive(false);
            
            // Code interpretation
            if (code == 0) { /* clear sky */ }
            else if (code >= 51 && code <= 67) { /* rain */ }
            else if (code >= 71 && code <= 77) { /* snow */ }
            else if (code >= 95) // thunderstorm
                lightningStormCloud.SetActive(true);
        }
        else
        {
            temperatureText.text = "Error";
            Debug.LogError(request.error);
        }

        isUpdating = false;
    }
}

[System.Serializable]
public class WeatherData
{
    public CurrentWeather current_weather;
}

[System.Serializable]
public class CurrentWeather
{
    public float temperature;
    public int weathercode;
}

