using BlueTools.Enums;

namespace BlueTools.Data;

public enum FishingSpotId
{
    BlusteryCloudtop,
    BuffetedCloudtop,
    WindbreakingCloudtop,
}

public record FishingSpotInfo(Vector3 LandingPosition, Vector3[] FishingPositions);

public static class DiademFish
{
    private static readonly Dictionary<FishingSpotId, int> CurrentPositionIndex = new();
    private static readonly Random Random = new();
    
    public static readonly Dictionary<FishingSpotId, FishingSpotInfo> FishingSpots = new()
    {
        { 
            FishingSpotId.BlusteryCloudtop, 
            new FishingSpotInfo(
                LandingPosition: new Vector3(540.2755f, 192.361f, -527.81384f),
                FishingPositions: [
                    new Vector3(533.8707f, 191.95576f, -536.36755f),
                    new Vector3(519.7842f, 193.44038f, -522.0181f),
                    new Vector3(548.39087f, 191.7401f, -507.4081f),
                    new Vector3(572.55225f, 188.91255f, -500.94052f),
                ]
            ) 
        },
        { 
            FishingSpotId.BuffetedCloudtop, 
            new FishingSpotInfo(
                LandingPosition: new Vector3(587.7113f, 222.81683f, -250.81252f),
                FishingPositions: [
                    new Vector3(588.12555f, 222.20378f, -253.84512f),
                    new Vector3(586.257f, 222.10025f, -229.39746f),
                    new Vector3(620.8661f, 221.44586f, -293.0172f),
                    new Vector3(600.39264f, 222.28319f, -269.51248f),
                ]
            ) 
        },
        { 
            FishingSpotId.WindbreakingCloudtop, 
            new FishingSpotInfo(
                LandingPosition: new Vector3(301.67618f, 284.98096f, -632.194f),
                FishingPositions: [
                    new Vector3(302f, 281.95523f, -625.8858f),
                    new Vector3(289.85043f, 282.36798f, -627.15826f),
                    new Vector3(279.60147f, 282.427f, -632.9205f),
                    new Vector3(258.8478f, 283.8888f, -650.54376f),
                ]
            )
        },
    };

    // Which spot to go to based on grade and weather
    public static readonly Dictionary<(int grade, WeatherType weather), FishingSpotId> SpotStrategy = new()
    {
        { (2, WeatherType.Snow), FishingSpotId.BuffetedCloudtop },
        { (2, WeatherType.UmbralDuststorms), FishingSpotId.WindbreakingCloudtop },
        { (2, WeatherType.UmbralFlare), FishingSpotId.WindbreakingCloudtop },
        { (2, WeatherType.UmbralLevin), FishingSpotId.BuffetedCloudtop },
        { (2, WeatherType.UmbralTempest), FishingSpotId.BuffetedCloudtop },

        { (3, WeatherType.Snow), FishingSpotId.WindbreakingCloudtop },
        { (3, WeatherType.UmbralDuststorms), FishingSpotId.BuffetedCloudtop },
        { (3, WeatherType.UmbralFlare), FishingSpotId.BuffetedCloudtop },
        { (3, WeatherType.UmbralLevin), FishingSpotId.WindbreakingCloudtop },
        { (3, WeatherType.UmbralTempest), FishingSpotId.WindbreakingCloudtop },

        { (4, WeatherType.Snow), FishingSpotId.BlusteryCloudtop },
        { (4, WeatherType.UmbralDuststorms), FishingSpotId.BlusteryCloudtop },
        { (4, WeatherType.UmbralFlare), FishingSpotId.BlusteryCloudtop },
        { (4, WeatherType.UmbralLevin), FishingSpotId.BlusteryCloudtop },
        { (4, WeatherType.UmbralTempest), FishingSpotId.BlusteryCloudtop },
    };

    // Which bait to use based on grade and weather
    public static readonly Dictionary<(int grade, WeatherType weather), BaitType> BaitStrategy = new()
    {
        { (2, WeatherType.UmbralFlare), BaitType.DiademBalloonBug },
        { (2, WeatherType.UmbralDuststorms), BaitType.DiademRedBalloon },
        { (2, WeatherType.UmbralLevin), BaitType.DiademCraneFly },
        { (2, WeatherType.UmbralTempest), BaitType.DiademHoverworm },
        { (2, WeatherType.Snow), BaitType.DiademHoverworm },

        { (3, WeatherType.UmbralLevin), BaitType.DiademRedBalloon },
        { (3, WeatherType.UmbralTempest), BaitType.DiademCraneFly },
        { (3, WeatherType.Snow), BaitType.DiademCraneFly },

        // Any - for now don't bother moving to catch the greens in these spots, keep going for Oscars and Mantas
        { (3, WeatherType.UmbralFlare), BaitType.DiademCraneFly },
        { (3, WeatherType.UmbralDuststorms), BaitType.DiademCraneFly },

        { (4, WeatherType.UmbralFlare), BaitType.DiademHoverworm },
        { (4, WeatherType.UmbralDuststorms), BaitType.DiademHoverworm },
        { (4, WeatherType.UmbralLevin), BaitType.DiademHoverworm },
        { (4, WeatherType.UmbralTempest), BaitType.DiademHoverworm },
        { (4, WeatherType.Snow), BaitType.DiademHoverworm },
    };

    // Which Autohook preset to use based on grade and weather
    public static readonly Dictionary<(int grade, WeatherType weather), string> AutohookPresetStrategy = new()
    {
        { (2, WeatherType.Snow), "AH4_H4sIAAAAAAAACs1XS2/jOAz+K4Eue1h7IT8j+5ZJHxMgfWCcYg7FHmSbToQ4VkaSp5Mt8t8XsuPEdp12WhTYvRkU+fEjRZH0M5qUik+pVHKaLVH4jC4LGucwyXMUKlGCgfThnBVwOkybo1mKQpsEBroXjAumdii0DDSTl7+SvEwhPYm1/r7GuuE8WWmw6sPWXxWOTwx0vV2sBMgVz1MUWhh3kF+HrjCCcccCv0lmuio3DQPXwm6PQtCj0FjxPIdEtQyttpr9tlsuUkbzMym1bN/vJNU9mF0xubrcgWw59nqMPa/D2G+STtcQrVimvlBW8dYC2QgiRZO1RCEx0F2R776voKgu55arh23D7EFC53A2uyumF83pUGX45CW9NrngQO6eKgZFAq2w/L6d370IuzEV7B+YUlVXVOO1b233rtE5WC9WNGd0La/oTy40QEfQZMUxuvJvkPCfIFBo6VyfCdztOGyu4QtbXtNNFeikWOYgZONE10yKQmeM3RfsO1BkvzfQ5S8laOfBHgno+1zw6IluZ4UqmWK8uKasaNJjWgaalwJuQEq6BBQiZKDbihO65QWgA8JuCyjUeRrAm3OpPox3L0DCMENkojPntcfq/MQn2kKiBM2npRBQqE+Ksof6abEOsn0R8aD3SuuKiwSqx/pEj2+yEqZaWhUPtollHCorUnyr+wUrlpGCbdWZT1Eeqm8iPie4NlzF9qFgP0rQuAgH2LHiDJsOJb7pBtgyKckcM3YgyYC4WUIA7Q00Z1LdZdqHROHjc+VNB9AQPER3juMFoylsRl/103ziYqMhb7nY0Pwr52sN0rSZ70DXp4ejTyV0snoQ1aG61lj3qcY4UoIXy/eYY6dlPoclFCkVu3cjXPAyzo/cOxq2HxwVTvzOqnQ4DGg9SFgItq2ZNZRqSdv9gm1A9JrPDSuOR7rt/YUNpEfskK6Wd/WJVu8SGnu2Dr723o3tbf+28z7/lv0GgaHMHVEzmks4Y64f4yRTIKa0XK7UnG30FLbqg/4rrRauUtRjXn+0BlE9I7ygv6i8uuro7ajppM1j+AY/SiYgjRRVpdQDlpyqdLDKzj6E3673/7asX1bwb5XZO0rhf3bnrR6cYRdbPo3NOIg907UoNolFsOlmPo4t6o+x56L9300TPqzoj0dB3Ycfn1GnIdtEczrXkK8FTWHkjqL1Li5ZnoKQf4yuV1yq0RUFwboTxXotY7MUCsUSmus0afe1wmTDy6KjhkLXC/r7k9NdiYn2VIqMJhDluucelk4v8N5YG729gQb+XoY3wB4WedfPzIcgX//bOc3+D0/86IlutWSq015lvL0DHCa//qzFJ7Whim9Vp0PG2B57nulmY2K6SeyYNCGuiWmQYQI0IeBX1Vnjdgd+XWb2KCr40+jP0QI2W5Cqt4JkdmbjmJiWD47pOjYx4ySLTcvBKSFuEMeJj/b/Ai9vTXAMDwAA" },
        { (2, WeatherType.UmbralDuststorms), "AH4_H4sIAAAAAAAACs1XSW/rOAz+K4Euc7EHXuT1lqbLBEgX1C3eoZiDbDOJEMfKk+TXlyny3wfykkSu0w0FZm4GRX78SFEk/YLGlWQTIqSYzBcofkEXJUkLGBcFiiWvwEDqcEZLOBzm3dE0R7ETRga645RxKrcotg00FRe/s6LKIT+Ilf6uwbpmLFsqsPrDUV81jh8a6GrzsOQglqzIUWxblob8NnSNEQWahfUumcmyWncMsG3hHoWoR6GzYkUBmTwytI/VnPfdMp5TUpxIqe34vpZU3JpdUrG82II4cuz1GHuextjvkk5WkCzpXJ4RWvNWAtEJEkmylUBxaKDbstj+WEJZX84Nk4+bjtmjAO1wOr0tJ+fd6VBl+OFresfkopbcHZEUygyOwvL7dr5+EU5nyuk/MCGyqajOa9/a6V2j21o/LElByUpckl+MKwBN0GXFNXT5PWTsF3AU2yrXJwLHmsPuGs7o4oqs60DH5aIALjonqmZyFLuBhV+x16DC3c5AF78lJ9qD3RNQ9/nAkmeymZayopKy8orQskuPaRtoVnG4BiHIAlCMkIFuak7ohpWAWoTtBlCs8jSAN2NCfhnvjoOAYYbIRCfOG4/1+YFPsoFMclJMKs6hlN8UZQ/122IdZPsq4kHvtdYl4xnUj/WZ7N9kLcyVtC4eywkio62sRLKN6he0XCQSNnVnPkTZVt+Yf09wx3A128eS/qxA4SKA1PGJH5nYh9TEOfbMFHtg4tBxI8fKgsAP0M5AMyrk7Vz5ECh+eqm9qQA6gm10pzieU5LDenQP+eiMFAVjpQK9YXxNir8YWymYrtH8ALI6PB11KkDLaytqgsV2oDpVZ5xIzsrFZ8wt98h8Bgsoc8K3n0Y4Z1Va7LlrGo4f7RUO/E6qaBwGtB4FPHC6aZh1lBrJsfsHugbeaz/XtNwfqcb3p2UgNWSHdJVc1w+Vuk4o8BwVfONdj+0D/sPP+betdwgMZW6POieFgBPm6jmO5xL4hFSLpZzRtZrDdnPQf6f1ylXxZtCrj6NR1EwJL+qvKm8uO2o/6npp9xju4WdFOeSJJLISasSGhyodrLKTD+HD9f7flvXrCv5QmX2iFP5nd653YZz5kW1afgAmxmFkpm6IzdyzIPNy38/mLtr93bXhdkl/2guaTvz0grSW7ISK06mWfMVJDiM8SlbbtKJFDlz8MbpaMiFHlwQ41WeK/VbGpjmUkmakUGlS7huF8ZpVpaaGYuxF/Q3K1ZfiUHmq+JxkkBSq57Zrpxd57yyO3s5AA/8vwztgDyv81O/MlyDf/t85TP8vz/zkmWyUZKLSXmf8eAtoZ7/6bMQHtaGK16rTx+A4genmJDWxG1hmOHcd07ZtN8epE2HHqauzwdVHflNmzui8ElJIxtdCX0BsP4jmtpub2Me+icFxzcgKbdOP0swHy8qwlaLdv0WBLlwKDwAA" },
        { (2, WeatherType.UmbralFlare), "AH4_H4sIAAAAAAAACs1XWW/jOAz+K4Fe9sVe+JDPtzQ9NkB6YNxiHop9kG06EeJYGUmeTrbIf1/IR2K7Ti8UmHkzKPLjR4oi6Wc0LSWbESHFLFui8BldFCTOYZrnKJS8BA2pwwUt4HiYtkfzFIWWH2jojlPGqdyh0NTQXFz8SvIyhfQoVvr7GuuasWSlwKoPS31VOK6voavt/YqDWLE8RaFpGD3k16ErjMDrWRhvkpmtyk3LAJsGHlAIBhRaK5bnkMiOodlVs952y3hKSX4ipablur2k4sbskorVxQ5Ex7EzYOw4PcZum3SyhmhFM3lGaMVbCUQriCRJ1gKFvoZui3z3fQVFdTk3TD5sW2YPAnqH8/ltMTtvT8cqw/Vf0uuSCxpyd0RSKBLohOUO7dz+RVitKaf/wYzIuqJar0Nra3CNdmN9vyI5JWtxSX4yrgB6gjYrttaXf4OE/QSOQlPl+kTguOewvYYzurwimyrQabHMgYvWiaqZFIW2Z+AX7HtQ/n6voYtfkpPegz0QUPd5z6Insp0XsqSSsuKK0KJNj25qaFFyuAYhyBJQiJCGbipO6IYVgBqE3RZQqPI0grdgQn4a746DgHGGSEcnzmuP1fmRT7SFRHKSz0rOoZBfFOUA9ctiHWX7IuJR75XWJeMJVI/1iRzeZCVMlbQqHsPyfK2prEiyreoXtFhGErZVZz5G2VTflH9NcF24iu1DQX+UoHBRjH3Pcomvm37s6Nggrk5cSPTMjyG1vczKghTtNbSgQt5myodA4eNz5U0F0BJsojvF8ZySFDaTM5LnjBWTs3KpQG8Y35D8H8bWCqZtNN+BrI9PR50K6OW1EdXBYtNTnao1jiRnxfIj5obdMV/AEoqU8N2HEc5ZGecH7j0Nyw0OCkd+J1V6HEa0HgTcc7qtmbWUaknX/T3dAB+0n2taHI5U4/vb0JAasmO6St7X95V6n5DnWCr42ns/tnf49z/m3zTeIDCWuQNqRnIBJ8zVc5xmEviMlMuVXNCNmsNmfTB8p9XKVfJ60KuPziiqp4QTDFeVV5cdtR+1vbR9DN/gR0k5pJEkshRqxPrHKh2tspMP4d31/nvL+mUFv6vMPlAKf9idd7owTuLASHCsu8S3dBxgrMdp7OnYw35mxq6bGDba/9u24WZJfzwI6k78+Ix6LdnyFadTLfmKkxQmeBKtd3FJ8xS4+GtytWJCTi4JcNqfKeZrGZunUEiakFylSbmvFaYbVhY9NRRiJxhuUHZ/KfaVp5JnJIEoVz23WTudwHljcXT2Ghr5fxnfAQdY/od+Zz4F+fr/znH6f3rmR09kqyQzlfYq490toJn96rMWH9XGKr5TnZ5vGp7nGnqaYVPHGbZ1385M3cwwWKaVeUliVdVZ4/ZHfl1m1uQyJ1xR7lZ9AGZqkVhPDMvSMThEJ4lNdNP2Aj8zLOy5BO3/B6FH02gFDwAA" },
        { (2, WeatherType.UmbralLevin), "AH4_H4sIAAAAAAAACs1XS2/jOAz+K4Euexh74ffrlkkfGyB9YNxiDsUeZJtOhDhWRpLbyRT57wv5kViu006LArM3gyI/fqQokn5G00rQGeaCz/Ilip7ReYmTAqZFgSLBKtCQPFyQEo6HWXc0z1BkBaGGbhmhjIgdikwNzfn5z7SoMsiOYqm/b7CuKE1XEqz+sORXjeMFGrrc3q0Y8BUtMhSZhqEgvw5dY4S+YmG8SWa2qjYdA8c0nAGFcEChs6JFAanoGZp9Nettt5RlBBcnUmpanqck1WnNLghfne+A9xy7A8auqzD2uqTjNcQrkouvmNS8pYB3gljgdM1RFGjopix231dQ1pdzTcX9tmN2z0E5nM9vytlZdzpWGV7wkl6fXNiSu8WCQJlCLyxvaOepF2F1poz8ghkWTUV1XofW1uAa7db6boULgtf8Aj9SJgEUQZcVW1Pl3yClj8BQZMpcnwjcURx21/CVLC/xpg50Wi4LYLxzImsmQ5HtG84L9gpUsN9r6PynYFh5sAcC8j7vaPyEt/NSVEQQWl5iUnbp0U0NLSoGV8A5XgKKENLQdc0JXdMSUIuw2wKKZJ5G8BaUiw/j3TLgMM4Q6ejEeeOxPj/yibeQCoaLWcUYlOKTohygflqso2xfRDzqvda6oCyF+rE+4cObrIWZlNbFY1iBobWVFQu6lf2ClMtYwLbuzMco2+qbss8Jrg9Xs70vyY8KJC4ycidPMGA9dPxEd8BP9SC0DD1PMbh+DuDmKdpraEG4uMmlD46ih+famwygI9hGd4rjGcEZbCYzhkuYXBQ7CXlN2QYX/1C6liBdm/kOeH18OPKUg5LVVtSE6pi+7FOdcSwYLZfvMTfsnvkCllBmmO3ejXBGq6Q4cFc0LC88KBz5nVRROIxo3XO4Y2TbMOsoNZK++zuyATZoPlekPBzJtve3oSE5Ysd0pVzVD6S6Ssh3LRl8412N7W3/lv0+/6b1BoGxzB1Qc1xwOGEuH+M0F8BmuFquxIJs5BQ2m4PhK60Xroo1Y15+9AZRMyPccLiovLrqyO2o66TdY/gGPyrCIIsFFhWXAzY4VulolZ18CL9d73+2rF9W8G+V2TtK4X92570enFlmavuZpUPmpbrjpaaemLah+zZ4qeNnHvgB2v/bNeF2RX84CJo+/PCMlIZsBZLTqYZ8yXAGE2cSr3dJRYoMGP9rcrmiXEwuMDCiThTztYzNMygFSXEh0yTdNwrTDa1KRQ1FjhsO9ydbXYkD6aliOU4hLmTPbZdON3TfWBvdvYZG/l7GN8ABVvCun5kPQb7+t3Oc/R+e+PET3krJTKa9znh/B2gnv/xsxEe1sYrvVaePPcsJ7UTHiZvoTu5kepAaiW65qWl4qYcNO6urs8FVB35TZtYkLunT5MtkAY+kVBcQOwDXNBxDh8DAupO7qR44Geh5lgRhblqm7Vho/x8paxbsCg8AAA==" },
        { (2, WeatherType.UmbralTempest), "AH4_H4sIAAAAAAAACs1XS2/jOAz+K4Eue1h7IT8j+5ZJHxMgfWCcYg7FHmSbToQ4VkaSp5Mt8t8XsuPEdp12WhTYvRkU+fEjRZH0M5qUik+pVHKaLVH4jC4LGucwyXMUKlGCgfThnBVwOkybo1mKQpsEBroXjAumdii0DDSTl7+SvEwhPYm1/r7GuuE8WWmw6sPWXxWOTwx0vV2sBMgVz1MUWhh3kF+HrjCCcccCv0lmuio3DQPXwm6PQtCj0FjxPIdEtQyttpr9tlsuUkbzMym1bN/vJNU9mF0xubrcgWw59nqMPa/D2G+STtcQrVimvlBW8dYC2QgiRZO1RCEx0F2R776voKgu55arh23D7EFC53A2uyumF83pUGX45CW9NrngQO6eKgZFAq2w/L6d370IuzEV7B+YUlVXVOO1b233rtE5WC9WNGd0La/oTy40QEfQZMUxuvJvkPCfIFBo6VyfCdztOGyu4QtbXtNNFeikWOYgZONE10yKQmeM3RfsO1BkvzfQ5S8laOfBHgno+1zw6IluZ4UqmWK8uKasaNJjWgaalwJuQEq6BBQiZKDbihO65QWgA8JuCyjUeRrAm3OpPox3L0DCMENkojPntcfq/MQn2kKiBM2npRBQqE+Ksof6abEOsn0R8aD3SuuKiwSqx/pEj2+yEqZaWhUPtollHCorUnyr+wUrlpGCbdWZT1Eeqm8iPie4NlzF9qFgP0rQuAgH2LHiDJsOJb7pBtgyKckcM3YgyYC4WUIA7Q00Z1LdZdqHROHjc+VNB9AQPER3juMFoylsRl/103ziYqMhb7nY0Pwr52sN0rSZ70DXp4ejTyV0snoQ1aG61lj3qcY4UoIXy/eYY6dlPoclFCkVu3cjXPAyzo/cOxq2HxwVTvzOqnQ4DGg9SFgItq2ZNZRqSdv9gm1A9JrPDSuOR7rt/YUNpEfskK6Wd/WJVu8SGnu2Dr723o3tbf+28z7/lv0GgaHMHVEzmks4Y64f4yRTIKa0XK7UnG30FLbqg/4rrRauUtRjXn+0BlE9I7ygv6i8uuro7ajppM1j+AY/SiYgjRRVpdQDlpyqdLDKzj6E3673/7asX1bwb5XZO0rhf3bnrR6cYRdbPo3NOIg907UoNolFsOlmPo4t6o+x56L9300TPqzoj0dB3Ycfn1GnIdtEczrXkK8FTWHkjqL1Li5ZnoKQf4yuV1yq0RUFwboTxXotY7MUCsUSmus0afe1wmTDy6KjhkLXC/r7k9NdiYn2VIqMJhDluucelk4v8N5YG729gQb+XoY3wB4WedfPzIcgX//bOc3+D0/86IlutWSq015lvL0DHCa//qzFJ7Whim9Vp0PG2B57nulmY2K6SeyYNCGuiWmQYQI0IeBX1Vnjdgd+XWb2KCr40+jP0QI2W5Cqt4JkdmbjmJiWD47pOjYx4ySLTcvBKSFuEMeJj/b/Ai9vTXAMDwAA" },

        { (3, WeatherType.Snow), "AH4_H4sIAAAAAAAACs1XS5OjOAz+Kylf9gJbvAPcMunHpir9qCFdc+jagwERXCE4Y5uZyXblv28ZcAJ00un09GFuxJI+fZJlSXlBk0rQKeaCT7MlCl/QdYnjAiZFgULBKtCQFM5JCQdhqkSzFIWWH2jokRHKiNii0NTQjF//SooqhfRwLPV3DdYdpUkuweoPS37VOJ6vodvNImfAc1qkKDQNo4f8NnSNEYx7FsZZMtO8WisGjmk4AwrBgIKyokUBiegYml0167xbylKCixMpNS3P6yXVac1uCM+vt8A7jt0BY9ftMfZU0vEKopxk4gsmNW95wNVBJHCy4ij0NfRQFttvOZT15dxT8bRRzJ449ISz2UM5vVLSY5Xh+a/pdckFLblHLAiUCXTC8oZ2Xv8iLGXKyH8wxaKpKOV1aG0NrtFurRc5Lghe8Rv8gzIJ0DtQWbG1/vlXSOgPYCg0Za5PBO70HKpr+EKWt3hdBzoplwUwrpzImklRaI8N5xX7HpS/22no+pdguH2w8gIXNPqJN7NSVEQQWt5iUqp86KaG5hWDO+AcLwGFCGnoviaB7mkJSGsQthtAoUzMEbw55eLDeI8MOBxniHR0Qt54rOUHPtEGEsFwMa0Yg1J8UpQD1E+L9SjbVxEf9V5rNQUSCbqRz56Uy0jApm6wB+5tEU3Y51DuwtUcnkryvQKJi2AcxHbsJroZW5nuBImjY2Ns6XHi27HrOJkb22inoTnh4iGTPjgKn5vylAEogrZh+cZpjlcEp7AeTRkuYXRTbCXkPWVrXPxD6UqCqG7xDXD9u3mAUspByCjUU2yPmlAdcyzbjTKOBKPl8hJzw+6Yz2EJZYrZ9mKEK1rFxZ671FiQNbBBF7kj5V4ku93fxgDN8oI92CGWkyo9vq3W3mWGCz5k2xg/cVgwsmmCU9yak9+PYOxaMiENXD+GdwD6ZwAvj7g1lw9ukglgU1wtczEnazkwzUYwfIn1blSxZiLLj87MaNq5Gwx3ije3ErnIqB6oCv4rfK8IgzQSWFRczkL/UIndezhf7O+u6feV7meWY1frdYm9q2wuKIU/7M47fdY3x05g4kRPLcfUnfHY0LEfxLqbBBjsJPWDsYV2/6pG227Tz/uDptc+v6Be07V8yelU071lOIWRM4pW27giRQqM/zW6zSkXoxsMjPSnhvlWxmYplIIkuJBpku4bhcmaVmVPDYWOGwxXHbu/vfrSU8UynEBUyL7a7odu4J7Z8Nydho780Ti+rA2w/Iv+d3wI8u0/Jof5/uGpHv3EG3kylWmvM96d8+10l5/N8UHtWMV3qtP0sBsnbqI7senrjhvbepDFjh4HqW2YsW/HDq6rs8HtD/WmzOzBXpHZqWsEgR7gzNSd1Ev0wHNTPQbXMGzfTAFMtPsftO20NagOAAA=" },
        { (3, WeatherType.UmbralDuststorms), "AH4_H4sIAAAAAAAACs1XS5OjOAz+Kylf9gJbvAPcMunHpir9qCFdc+jagwERXCE4Y5uZyXblv28ZcAJ00un09GFuxJI+fZJlSXlBk0rQKeaCT7MlCl/QdYnjAiZFgULBKtCQFM5JCQdhqkSzFIWWH2jokRHKiNii0NTQjF//SooqhfRwLPV3DdYdpUkuweoPS37VOJ6vodvNImfAc1qkKDQNo4f8NnSNEYx7FsZZMtO8WisGjmk4AwrBgIKyokUBiegYml0167xbylKCixMpNS3P6yXVac1uCM+vt8A7jt0BY9ftMfZU0vEKopxk4gsmNW95wNVBJHCy4ij0NfRQFttvOZT15dxT8bRRzJ449ISz2UM5vVLSY5Xh+a/pdckFLblHLAiUCXTC8oZ2Xv8iLGXKyH8wxaKpKOV1aG0NrtFurRc5Lghe8Rv8gzIJ0DtQWbG1/vlXSOgPYCg0Za5PBO70HKpr+EKWt3hdBzoplwUwrpzImklRaI8N5xX7HpS/22no+pdguH2w8gIXNPqJN7NSVEQQWt5iUqp86KaG5hWDO+AcLwGFCGnoviaB7mkJSGsQthtAoUzMEbw55eLDeI8MOBxniHR0Qt54rOUHPtEGEsFwMa0Yg1J8UpQD1E+L9SjbVxEf9V5rNQUSCbqRz56Uy0jApm6wB+5tEU3Y51DuwtUcnkryvQKJi2AcxHbsJroZW5nuBImjY2Ns6XHi27HrOJkb22inoTnh4iGTPjgKn5vylAEogrZh+cZpjlcEp7AeTRkuYXRTbCXkPWVrXPxD6UqCqG7xDXD9u3mAUspByCjUU2yPmlAdcyzbjTKOBKPl8hJzw+6Yz2EJZYrZ9mKEK1rFxZ671FiQNbBBF7kj5V4ku93fxgDN8oI92CGWkyo9vq3W3mWGCz5k2xg/cVgwsmmCU9yak9+PYOxaMiENXD+GdwD6ZwAvj7g1lw9ukglgU1wtczEnazkwzUYwfIn1blSxZiLLj87MaNq5Gwx3ije3ErnIqB6oCv4rfK8IgzQSWFRczkL/UIndezhf7O+u6feV7meWY1frdYm9q2wuKIU/7M47fdY3x05g4kRPLcfUnfHY0LEfxLqbBBjsJPWDsYV2/6pG227Tz/uDptc+v6Be07V8yelU071lOIWRM4pW27giRQqM/zW6zSkXoxsMjPSnhvlWxmYplIIkuJBpku4bhcmaVmVPDYWOGwxXHbu/vfrSU8UynEBUyL7a7odu4J7Z8Nydho780Ti+rA2w/Iv+d3wI8u0/Jof5/uGpHv3EG3kylWmvM96d8+10l5/N8UHtWMV3qtP0sBsnbqI7senrjhvbepDFjh4HqW2YsW/HDq6rs8HtD/WmzOzBXpHZqWsEgR7gzNSd1Ev0wHNTPQbXMGzfTAFMtPsftO20NagOAAA=" },
        { (3, WeatherType.UmbralFlare), "AH4_H4sIAAAAAAAACs1XS5OjOAz+Kylf9gJbvAPcMunHpir9qCFdc+jagwERXCE4Y5uZyXblv28ZcAJ00un09GFuxJI+fZJlSXlBk0rQKeaCT7MlCl/QdYnjAiZFgULBKtCQFM5JCQdhqkSzFIWWH2jokRHKiNii0NTQjF//SooqhfRwLPV3DdYdpUkuweoPS37VOJ6vodvNImfAc1qkKDQNo4f8NnSNEYx7FsZZMtO8WisGjmk4AwrBgIKyokUBiegYml0167xbylKCixMpNS3P6yXVac1uCM+vt8A7jt0BY9ftMfZU0vEKopxk4gsmNW95wNVBJHCy4ij0NfRQFttvOZT15dxT8bRRzJ449ISz2UM5vVLSY5Xh+a/pdckFLblHLAiUCXTC8oZ2Xv8iLGXKyH8wxaKpKOV1aG0NrtFurRc5Lghe8Rv8gzIJ0DtQWbG1/vlXSOgPYCg0Za5PBO70HKpr+EKWt3hdBzoplwUwrpzImklRaI8N5xX7HpS/22no+pdguH2w8gIXNPqJN7NSVEQQWt5iUqp86KaG5hWDO+AcLwGFCGnoviaB7mkJSGsQthtAoUzMEbw55eLDeI8MOBxniHR0Qt54rOUHPtEGEsFwMa0Yg1J8UpQD1E+L9SjbVxEf9V5rNQUSCbqRz56Uy0jApm6wB+5tEU3Y51DuwtUcnkryvQKJi2AcxHbsJroZW5nuBImjY2Ns6XHi27HrOJkb22inoTnh4iGTPjgKn5vylAEogrZh+cZpjlcEp7AeTRkuYXRTbCXkPWVrXPxD6UqCqG7xDXD9u3mAUspByCjUU2yPmlAdcyzbjTKOBKPl8hJzw+6Yz2EJZYrZ9mKEK1rFxZ671FiQNbBBF7kj5V4ku93fxgDN8oI92CGWkyo9vq3W3mWGCz5k2xg/cVgwsmmCU9yak9+PYOxaMiENXD+GdwD6ZwAvj7g1lw9ukglgU1wtczEnazkwzUYwfIn1blSxZiLLj87MaNq5Gwx3ije3ErnIqB6oCv4rfK8IgzQSWFRczkL/UIndezhf7O+u6feV7meWY1frdYm9q2wuKIU/7M47fdY3x05g4kRPLcfUnfHY0LEfxLqbBBjsJPWDsYV2/6pG227Tz/uDptc+v6Be07V8yelU071lOIWRM4pW27giRQqM/zW6zSkXoxsMjPSnhvlWxmYplIIkuJBpku4bhcmaVmVPDYWOGwxXHbu/vfrSU8UynEBUyL7a7odu4J7Z8Nydho780Ti+rA2w/Iv+d3wI8u0/Jof5/uGpHv3EG3kylWmvM96d8+10l5/N8UHtWMV3qtP0sBsnbqI7senrjhvbepDFjh4HqW2YsW/HDq6rs8HtD/WmzOzBXpHZqWsEgR7gzNSd1Ev0wHNTPQbXMGzfTAFMtPsftO20NagOAAA=" },
        { (3, WeatherType.UmbralLevin), "AH4_H4sIAAAAAAAACs1XS5OjOAz+Kylf9gJbvAPcMunHpir9qCFdc+jagwERXCE4Y5uZyXblv28ZcAJ00un09GFuxJI+fZJlSXlBk0rQKeaCT7MlCl/QdYnjAiZFgULBKtCQFM5JCQdhqkSzFIWWH2jokRHKiNii0NTQjF//SooqhfRwLPV3DdYdpUkuweoPS37VOJ6vodvNImfAc1qkKDQNo4f8NnSNEYx7FsZZMtO8WisGjmk4AwrBgIKyokUBiegYml0167xbylKCixMpNS3P6yXVac1uCM+vt8A7jt0BY9ftMfZU0vEKopxk4gsmNW95wNVBJHCy4ij0NfRQFttvOZT15dxT8bRRzJ449ISz2UM5vVLSY5Xh+a/pdckFLblHLAiUCXTC8oZ2Xv8iLGXKyH8wxaKpKOV1aG0NrtFurRc5Lghe8Rv8gzIJ0DtQWbG1/vlXSOgPYCg0Za5PBO70HKpr+EKWt3hdBzoplwUwrpzImklRaI8N5xX7HpS/22no+pdguH2w8gIXNPqJN7NSVEQQWt5iUqp86KaG5hWDO+AcLwGFCGnoviaB7mkJSGsQthtAoUzMEbw55eLDeI8MOBxniHR0Qt54rOUHPtEGEsFwMa0Yg1J8UpQD1E+L9SjbVxEf9V5rNQUSCbqRz56Uy0jApm6wB+5tEU3Y51DuwtUcnkryvQKJi2AcxHbsJroZW5nuBImjY2Ns6XHi27HrOJkb22inoTnh4iGTPjgKn5vylAEogrZh+cZpjlcEp7AeTRkuYXRTbCXkPWVrXPxD6UqCqG7xDXD9u3mAUspByCjUU2yPmlAdcyzbjTKOBKPl8hJzw+6Yz2EJZYrZ9mKEK1rFxZ671FiQNbBBF7kj5V4ku93fxgDN8oI92CGWkyo9vq3W3mWGCz5k2xg/cVgwsmmCU9yak9+PYOxaMiENXD+GdwD6ZwAvj7g1lw9ukglgU1wtczEnazkwzUYwfIn1blSxZiLLj87MaNq5Gwx3ije3ErnIqB6oCv4rfK8IgzQSWFRczkL/UIndezhf7O+u6feV7meWY1frdYm9q2wuKIU/7M47fdY3x05g4kRPLcfUnfHY0LEfxLqbBBjsJPWDsYV2/6pG227Tz/uDptc+v6Be07V8yelU071lOIWRM4pW27giRQqM/zW6zSkXoxsMjPSnhvlWxmYplIIkuJBpku4bhcmaVmVPDYWOGwxXHbu/vfrSU8UynEBUyL7a7odu4J7Z8Nydho780Ti+rA2w/Iv+d3wI8u0/Jof5/uGpHv3EG3kylWmvM96d8+10l5/N8UHtWMV3qtP0sBsnbqI7senrjhvbepDFjh4HqW2YsW/HDq6rs8HtD/WmzOzBXpHZqWsEgR7gzNSd1Ev0wHNTPQbXMGzfTAFMtPsftO20NagOAAA=" },
        { (3, WeatherType.UmbralTempest), "AH4_H4sIAAAAAAAACs1XS5OjOAz+Kylf9gJbvAPcMunHpir9qCFdc+jagwERXCE4Y5uZyXblv28ZcAJ00un09GFuxJI+fZJlSXlBk0rQKeaCT7MlCl/QdYnjAiZFgULBKtCQFM5JCQdhqkSzFIWWH2jokRHKiNii0NTQjF//SooqhfRwLPV3DdYdpUkuweoPS37VOJ6vodvNImfAc1qkKDQNo4f8NnSNEYx7FsZZMtO8WisGjmk4AwrBgIKyokUBiegYml0167xbylKCixMpNS3P6yXVac1uCM+vt8A7jt0BY9ftMfZU0vEKopxk4gsmNW95wNVBJHCy4ij0NfRQFttvOZT15dxT8bRRzJ449ISz2UM5vVLSY5Xh+a/pdckFLblHLAiUCXTC8oZ2Xv8iLGXKyH8wxaKpKOV1aG0NrtFurRc5Lghe8Rv8gzIJ0DtQWbG1/vlXSOgPYCg0Za5PBO70HKpr+EKWt3hdBzoplwUwrpzImklRaI8N5xX7HpS/22no+pdguH2w8gIXNPqJN7NSVEQQWt5iUqp86KaG5hWDO+AcLwGFCGnoviaB7mkJSGsQthtAoUzMEbw55eLDeI8MOBxniHR0Qt54rOUHPtEGEsFwMa0Yg1J8UpQD1E+L9SjbVxEf9V5rNQUSCbqRz56Uy0jApm6wB+5tEU3Y51DuwtUcnkryvQKJi2AcxHbsJroZW5nuBImjY2Ns6XHi27HrOJkb22inoTnh4iGTPjgKn5vylAEogrZh+cZpjlcEp7AeTRkuYXRTbCXkPWVrXPxD6UqCqG7xDXD9u3mAUspByCjUU2yPmlAdcyzbjTKOBKPl8hJzw+6Yz2EJZYrZ9mKEK1rFxZ671FiQNbBBF7kj5V4ku93fxgDN8oI92CGWkyo9vq3W3mWGCz5k2xg/cVgwsmmCU9yak9+PYOxaMiENXD+GdwD6ZwAvj7g1lw9ukglgU1wtczEnazkwzUYwfIn1blSxZiLLj87MaNq5Gwx3ije3ErnIqB6oCv4rfK8IgzQSWFRczkL/UIndezhf7O+u6feV7meWY1frdYm9q2wuKIU/7M47fdY3x05g4kRPLcfUnfHY0LEfxLqbBBjsJPWDsYV2/6pG227Tz/uDptc+v6Be07V8yelU071lOIWRM4pW27giRQqM/zW6zSkXoxsMjPSnhvlWxmYplIIkuJBpku4bhcmaVmVPDYWOGwxXHbu/vfrSU8UynEBUyL7a7odu4J7Z8Nydho780Ti+rA2w/Iv+d3wI8u0/Jof5/uGpHv3EG3kylWmvM96d8+10l5/N8UHtWMV3qtP0sBsnbqI7senrjhvbepDFjh4HqW2YsW/HDq6rs8HtD/WmzOzBXpHZqWsEgR7gzNSd1Ev0wHNTPQbXMGzfTAFMtPsftO20NagOAAA=" },

        { (4, WeatherType.Snow), "AH4_H4sIAAAAAAAACs1WS2+jMBD+K5XPsALC+5amTyntVk2qPVR7MHhIrBCc2qaPrfLfVwbcAEmbtNvD3sg8vvm+yYztVzQsJRthIcUom6H4FZ0WOMlhmOcolrwEAynnmBawcRLtuiQodsLIQDecMk7lC4ptA12K0+c0LwmQjVnFr2usK8bSuQKrPhz1VeH4oYHOV9M5BzFnOUGxbVkd5I+hK4wo6GRYe8mM5uVSM3Bty91DQWexPIdUthLtdpizvyzjhOL8nZb6ttvBc5usMyrmpy8gWnW9HmHP6xD2dc/xAiZzmsljTCvayiC0YSJxuhAo9pou+uE2bhs1alBvsKRQpNDi4/fz/G4DHZ3K6R8YYVlPgq7az3Z67R802dM5zileiDP8yLgC6Bi0nIHRtd9Cyh6Bo9hWTdo1yn6oJqBVUPfvmM7O8bISOixmOXChi6j/mqB4EFjuFvsOVLheG+j0WXLcWbQ3AuqPmLLJE15dFrKkkrLiHNNCt8e0DTQuOVyBEHgGKEbIQNcVJ3TNCkANwssKUKz6tANvzIT8Mt4NBwG7GSITveOvK1b+DZ/JClLJcT4qOYdCfpPKHuq3ad3JdkvxzupVVD0vE8lWan1pMZtIWFXn5IZ7M1ND/j2U23AVh7uCPpSgcJGXuUnq+wMzSfzAdAcJmDgJUzP1I+wlkRdhC9DaQGMq5M9M1RAovn+tqikBmuDAcsIPOJ5QTGB5dKEW7onxpYK8ZnyJ8wvGFgpEHx6/AC8266C8AqRSoRdDmaZ0Cby3MFe0eHOh2HZ/WHWsakPVFtcO1EmlC00kZ0W1eU3UG16GcwHGxwxaqNaghTqGGRQE85e9GvoIdwJOWNnE68DaonvyZemOr5TXYHt1v5vZ0XZ48p2AKaerrrDa8u/CAs9RzavhPimtk/t5cU26WuVhJoGPcDmbyzFdqivVrh39Ha8eTyWv72z10bqc6nvDi7YfHR+8H9RLR5+uepVu4aGkHMhEYlmqe1w9pfr79anV2D/quwK3h/ewgTxs8tpR29N06IQcOgr/2X9+tznBCXghCTwwM4+kpmsNwMRpZJtJYjmQOSm4loPWv/UR3jy3798M9Smuftd3RvfEPueYwJHbvTSA+CS0XcsMSIZNlwSZmRDLMkMviCLX9h2cAFr/BbOWAMdMDAAA" },
        { (4, WeatherType.UmbralDuststorms), "AH4_H4sIAAAAAAAACs1WS2+jMBD+K5XPsALC+5amTyntVk2qPVR7MHhIrBCc2qaPrfLfVwbcAEmbtNvD3sg8vvm+yYztVzQsJRthIcUom6H4FZ0WOMlhmOcolrwEAynnmBawcRLtuiQodsLIQDecMk7lC4ptA12K0+c0LwmQjVnFr2usK8bSuQKrPhz1VeH4oYHOV9M5BzFnOUGxbVkd5I+hK4wo6GRYe8mM5uVSM3Bty91DQWexPIdUthLtdpizvyzjhOL8nZb6ttvBc5usMyrmpy8gWnW9HmHP6xD2dc/xAiZzmsljTCvayiC0YSJxuhAo9pou+uE2bhs1alBvsKRQpNDi4/fz/G4DHZ3K6R8YYVlPgq7az3Z67R802dM5zileiDP8yLgC6Bi0nIHRtd9Cyh6Bo9hWTdo1yn6oJqBVUPfvmM7O8bISOixmOXChi6j/mqB4EFjuFvsOVLheG+j0WXLcWbQ3AuqPmLLJE15dFrKkkrLiHNNCt8e0DTQuOVyBEHgGKEbIQNcVJ3TNCkANwssKUKz6tANvzIT8Mt4NBwG7GSITveOvK1b+DZ/JClLJcT4qOYdCfpPKHuq3ad3JdkvxzupVVD0vE8lWan1pMZtIWFXn5IZ7M1ND/j2U23AVh7uCPpSgcJGXuUnq+wMzSfzAdAcJmDgJUzP1I+wlkRdhC9DaQGMq5M9M1RAovn+tqikBmuDAcsIPOJ5QTGB5dKEW7onxpYK8ZnyJ8wvGFgpEHx6/AC8266C8AqRSoRdDmaZ0Cby3MFe0eHOh2HZ/WHWsakPVFtcO1EmlC00kZ0W1eU3UG16GcwHGxwxaqNaghTqGGRQE85e9GvoIdwJOWNnE68DaonvyZemOr5TXYHt1v5vZ0XZ48p2AKaerrrDa8u/CAs9RzavhPimtk/t5cU26WuVhJoGPcDmbyzFdqivVrh39Ha8eTyWv72z10bqc6nvDi7YfHR+8H9RLR5+uepVu4aGkHMhEYlmqe1w9pfr79anV2D/quwK3h/ewgTxs8tpR29N06IQcOgr/2X9+tznBCXghCTwwM4+kpmsNwMRpZJtJYjmQOSm4loPWv/UR3jy3798M9Smuftd3RvfEPueYwJHbvTSA+CS0XcsMSIZNlwSZmRDLMkMviCLX9h2cAFr/BbOWAMdMDAAA" },
        { (4, WeatherType.UmbralFlare), "AH4_H4sIAAAAAAAACs1WS2+jMBD+K5XPsALC+5amTyntVk2qPVR7MHhIrBCc2qaPrfLfVwbcAEmbtNvD3sg8vvm+yYztVzQsJRthIcUom6H4FZ0WOMlhmOcolrwEAynnmBawcRLtuiQodsLIQDecMk7lC4ptA12K0+c0LwmQjVnFr2usK8bSuQKrPhz1VeH4oYHOV9M5BzFnOUGxbVkd5I+hK4wo6GRYe8mM5uVSM3Bty91DQWexPIdUthLtdpizvyzjhOL8nZb6ttvBc5usMyrmpy8gWnW9HmHP6xD2dc/xAiZzmsljTCvayiC0YSJxuhAo9pou+uE2bhs1alBvsKRQpNDi4/fz/G4DHZ3K6R8YYVlPgq7az3Z67R802dM5zileiDP8yLgC6Bi0nIHRtd9Cyh6Bo9hWTdo1yn6oJqBVUPfvmM7O8bISOixmOXChi6j/mqB4EFjuFvsOVLheG+j0WXLcWbQ3AuqPmLLJE15dFrKkkrLiHNNCt8e0DTQuOVyBEHgGKEbIQNcVJ3TNCkANwssKUKz6tANvzIT8Mt4NBwG7GSITveOvK1b+DZ/JClLJcT4qOYdCfpPKHuq3ad3JdkvxzupVVD0vE8lWan1pMZtIWFXn5IZ7M1ND/j2U23AVh7uCPpSgcJGXuUnq+wMzSfzAdAcJmDgJUzP1I+wlkRdhC9DaQGMq5M9M1RAovn+tqikBmuDAcsIPOJ5QTGB5dKEW7onxpYK8ZnyJ8wvGFgpEHx6/AC8266C8AqRSoRdDmaZ0Cby3MFe0eHOh2HZ/WHWsakPVFtcO1EmlC00kZ0W1eU3UG16GcwHGxwxaqNaghTqGGRQE85e9GvoIdwJOWNnE68DaonvyZemOr5TXYHt1v5vZ0XZ48p2AKaerrrDa8u/CAs9RzavhPimtk/t5cU26WuVhJoGPcDmbyzFdqivVrh39Ha8eTyWv72z10bqc6nvDi7YfHR+8H9RLR5+uepVu4aGkHMhEYlmqe1w9pfr79anV2D/quwK3h/ewgTxs8tpR29N06IQcOgr/2X9+tznBCXghCTwwM4+kpmsNwMRpZJtJYjmQOSm4loPWv/UR3jy3798M9Smuftd3RvfEPueYwJHbvTSA+CS0XcsMSIZNlwSZmRDLMkMviCLX9h2cAFr/BbOWAMdMDAAA" },
        { (4, WeatherType.UmbralLevin), "AH4_H4sIAAAAAAAACs1WS2+jMBD+K5XPsALC+5amTyntVk2qPVR7MHhIrBCc2qaPrfLfVwbcAEmbtNvD3sg8vvm+yYztVzQsJRthIcUom6H4FZ0WOMlhmOcolrwEAynnmBawcRLtuiQodsLIQDecMk7lC4ptA12K0+c0LwmQjVnFr2usK8bSuQKrPhz1VeH4oYHOV9M5BzFnOUGxbVkd5I+hK4wo6GRYe8mM5uVSM3Bty91DQWexPIdUthLtdpizvyzjhOL8nZb6ttvBc5usMyrmpy8gWnW9HmHP6xD2dc/xAiZzmsljTCvayiC0YSJxuhAo9pou+uE2bhs1alBvsKRQpNDi4/fz/G4DHZ3K6R8YYVlPgq7az3Z67R802dM5zileiDP8yLgC6Bi0nIHRtd9Cyh6Bo9hWTdo1yn6oJqBVUPfvmM7O8bISOixmOXChi6j/mqB4EFjuFvsOVLheG+j0WXLcWbQ3AuqPmLLJE15dFrKkkrLiHNNCt8e0DTQuOVyBEHgGKEbIQNcVJ3TNCkANwssKUKz6tANvzIT8Mt4NBwG7GSITveOvK1b+DZ/JClLJcT4qOYdCfpPKHuq3ad3JdkvxzupVVD0vE8lWan1pMZtIWFXn5IZ7M1ND/j2U23AVh7uCPpSgcJGXuUnq+wMzSfzAdAcJmDgJUzP1I+wlkRdhC9DaQGMq5M9M1RAovn+tqikBmuDAcsIPOJ5QTGB5dKEW7onxpYK8ZnyJ8wvGFgpEHx6/AC8266C8AqRSoRdDmaZ0Cby3MFe0eHOh2HZ/WHWsakPVFtcO1EmlC00kZ0W1eU3UG16GcwHGxwxaqNaghTqGGRQE85e9GvoIdwJOWNnE68DaonvyZemOr5TXYHt1v5vZ0XZ48p2AKaerrrDa8u/CAs9RzavhPimtk/t5cU26WuVhJoGPcDmbyzFdqivVrh39Ha8eTyWv72z10bqc6nvDi7YfHR+8H9RLR5+uepVu4aGkHMhEYlmqe1w9pfr79anV2D/quwK3h/ewgTxs8tpR29N06IQcOgr/2X9+tznBCXghCTwwM4+kpmsNwMRpZJtJYjmQOSm4loPWv/UR3jy3798M9Smuftd3RvfEPueYwJHbvTSA+CS0XcsMSIZNlwSZmRDLMkMviCLX9h2cAFr/BbOWAMdMDAAA" },
        { (4, WeatherType.UmbralTempest), "AH4_H4sIAAAAAAAACs1WS2+jMBD+K5XPsALC+5amTyntVk2qPVR7MHhIrBCc2qaPrfLfVwbcAEmbtNvD3sg8vvm+yYztVzQsJRthIcUom6H4FZ0WOMlhmOcolrwEAynnmBawcRLtuiQodsLIQDecMk7lC4ptA12K0+c0LwmQjVnFr2usK8bSuQKrPhz1VeH4oYHOV9M5BzFnOUGxbVkd5I+hK4wo6GRYe8mM5uVSM3Bty91DQWexPIdUthLtdpizvyzjhOL8nZb6ttvBc5usMyrmpy8gWnW9HmHP6xD2dc/xAiZzmsljTCvayiC0YSJxuhAo9pou+uE2bhs1alBvsKRQpNDi4/fz/G4DHZ3K6R8YYVlPgq7az3Z67R802dM5zileiDP8yLgC6Bi0nIHRtd9Cyh6Bo9hWTdo1yn6oJqBVUPfvmM7O8bISOixmOXChi6j/mqB4EFjuFvsOVLheG+j0WXLcWbQ3AuqPmLLJE15dFrKkkrLiHNNCt8e0DTQuOVyBEHgGKEbIQNcVJ3TNCkANwssKUKz6tANvzIT8Mt4NBwG7GSITveOvK1b+DZ/JClLJcT4qOYdCfpPKHuq3ad3JdkvxzupVVD0vE8lWan1pMZtIWFXn5IZ7M1ND/j2U23AVh7uCPpSgcJGXuUnq+wMzSfzAdAcJmDgJUzP1I+wlkRdhC9DaQGMq5M9M1RAovn+tqikBmuDAcsIPOJ5QTGB5dKEW7onxpYK8ZnyJ8wvGFgpEHx6/AC8266C8AqRSoRdDmaZ0Cby3MFe0eHOh2HZ/WHWsakPVFtcO1EmlC00kZ0W1eU3UG16GcwHGxwxaqNaghTqGGRQE85e9GvoIdwJOWNnE68DaonvyZemOr5TXYHt1v5vZ0XZ48p2AKaerrrDa8u/CAs9RzavhPimtk/t5cU26WuVhJoGPcDmbyzFdqivVrh39Ha8eTyWv72z10bqc6nvDi7YfHR+8H9RLR5+uepVu4aGkHMhEYlmqe1w9pfr79anV2D/quwK3h/ewgTxs8tpR29N06IQcOgr/2X9+tznBCXghCTwwM4+kpmsNwMRpZJtJYjmQOSm4loPWv/UR3jy3798M9Smuftd3RvfEPueYwJHbvTSA+CS0XcsMSIZNlwSZmRDLMkMviCLX9h2cAFr/BbOWAMdMDAAA" },
    };

    /// <summary>
    /// Get the fishing spot info for a specific grade and weather
    /// </summary>
    public static FishingSpotInfo? GetFishingSpot(int grade, WeatherType weather)
    {
        if (SpotStrategy.TryGetValue((grade, weather), out var spotId))
        {
            return FishingSpots.TryGetValue(spotId, out var spotInfo) ? spotInfo : null;
        }
        return null;
    }

    /// <summary>
    /// Get the current fishing position for a specific grade and weather (rotates between available positions)
    /// </summary>
    public static Vector3? GetFishingPosition(int grade, WeatherType weather)
    {
        if (!SpotStrategy.TryGetValue((grade, weather), out var spotId))
            return null;
            
        var spotInfo = GetFishingSpot(grade, weather);
        if (spotInfo == null || spotInfo.FishingPositions.Length == 0)
            return null;
            
        // Get current index for this spot (default to random if not set)
        if (!CurrentPositionIndex.TryGetValue(spotId, out var currentIndex))
        {
            currentIndex = Random.Next(spotInfo.FishingPositions.Length);
            CurrentPositionIndex[spotId] = currentIndex;
            PluginLog.Information($"Starting at random fishing position {currentIndex + 1}/{spotInfo.FishingPositions.Length} for {spotId}");
        }
        
        return spotInfo.FishingPositions[currentIndex];
    }
    
    /// <summary>
    /// Advance to the next fishing position for the current spot (call when "fish sense something amiss")
    /// </summary>
    public static Vector3? GetNextFishingPosition(int grade, WeatherType weather)
    {
        if (!SpotStrategy.TryGetValue((grade, weather), out var spotId))
            return null;
            
        var spotInfo = GetFishingSpot(grade, weather);
        if (spotInfo == null || spotInfo.FishingPositions.Length == 0)
            return null;
            
        // Get current index and advance it
        if (!CurrentPositionIndex.TryGetValue(spotId, out var currentIndex))
            currentIndex = Random.Next(spotInfo.FishingPositions.Length);
            
        // Advance to next position (wrap around if at end)
        currentIndex = (currentIndex + 1) % spotInfo.FishingPositions.Length;
        CurrentPositionIndex[spotId] = currentIndex;
        
        PluginLog.Information($"Cycling to fishing position {currentIndex + 1}/{spotInfo.FishingPositions.Length} for {spotId}");
        return spotInfo.FishingPositions[currentIndex];
    }

    /// <summary>
    /// Get the landing position for a specific grade and weather
    /// </summary>
    public static Vector3? GetLandingPosition(int grade, WeatherType weather)
    {
        return GetFishingSpot(grade, weather)?.LandingPosition;
    }

    /// <summary>
    /// Get the bait to use for a specific grade and weather
    /// </summary>
    public static BaitType? GetBait(int grade, WeatherType weather)
    {
        return BaitStrategy.TryGetValue((grade, weather), out var bait) ? bait : null;
    }

    /// <summary>
    /// Get the Autohook preset to use for a specific grade and weather
    /// </summary>
    public static string? GetAutohookPreset(int grade, WeatherType weather)
    {
        return AutohookPresetStrategy.TryGetValue((grade, weather), out var preset) ? preset : null;
    }

    /// <summary>
    /// Clear all cached position indices to force fresh random selection on next access
    /// </summary>
    public static void ClearPositionCache()
    {
        CurrentPositionIndex.Clear();
    }
}