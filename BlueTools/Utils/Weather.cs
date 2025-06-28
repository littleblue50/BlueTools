using BlueTools.Enums;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Graphics.Environment;
using Lumina.Excel.Sheets;

namespace BlueTools.Utils;

public unsafe class Weather
{
    private static EnvManager* EM => EnvManager.Instance();

    public static WeatherType? GetCurrentWeather()
    {
        if (EM == null) return null;

        var weatherId = EM->ActiveWeather;
        if (Enum.IsDefined(typeof(WeatherType), (int)weatherId))
        {
            return (WeatherType)weatherId;
        }
        
        return null;
    }

    public static int GetCurrentWeatherId()
    {
        if (EM == null) return 0;
        return (int)EM->ActiveWeather;
    }

    public static string GetWeatherName(WeatherType? weather = null)
    {
        int weatherId;
        if (weather.HasValue)
        {
            weatherId = (int)weather.Value;
        }
        else
        {
            weatherId = GetCurrentWeatherId();
        }
        
        return GetWeatherName(weatherId);
    }

    public static string GetWeatherName(int weatherId)
    {
        var weatherRow = Svc.Data.GetExcelSheet<Lumina.Excel.Sheets.Weather>()?.GetRow((uint)weatherId);
        if (weatherRow.HasValue)
        {
            return weatherRow.Value.Name.ToString();
        }
        
        return $"Unknown Weather ({weatherId})";
    }
} 