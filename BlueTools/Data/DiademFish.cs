using BlueTools.Enums;

namespace BlueTools.Data;

public enum FishingSpotId
{
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
            FishingSpotId.BuffetedCloudtop, 
            new FishingSpotInfo(
                LandingPosition: new Vector3(587.7113f, 222.81683f, -250.81252f),
                FishingPositions: [
                    new Vector3(588.12555f, 222.20378f, -253.84512f),
                    new Vector3(586.257f, 222.10025f, -229.39746f),
                    new Vector3(613.0804f, 222.1041f, -283.7225f),
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

        // { (4, WeatherType.Snow), FishingSpotId.WindbreakingCloudtop },
        // { (4, WeatherType.UmbralDuststorms), FishingSpotId.BuffetedCloudtop },
        // { (4, WeatherType.UmbralFlare), FishingSpotId.WindbreakingCloudtop },
        // { (4, WeatherType.UmbralLevin), FishingSpotId.BuffetedCloudtop },
        // { (4, WeatherType.UmbralTempest), FishingSpotId.WindbreakingCloudtop },
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
        { (3, WeatherType.Snow), BaitType.DiademBalloonBug },

        // Any - need to mooch off Ghost Faeries
        { (3, WeatherType.UmbralFlare), BaitType.DiademHoverworm },
        { (3, WeatherType.UmbralDuststorms), BaitType.DiademHoverworm },

        // { (4, WeatherType.UmbralFlare), BaitType.DiademBalloonBug },
        // { (4, WeatherType.UmbralDuststorms), BaitType.DiademRedBalloon },
        // { (4, WeatherType.UmbralLevin), BaitType.DiademCraneFly },
        // { (4, WeatherType.UmbralTempest), BaitType.DiademHoverworm },
        // { (4, WeatherType.Snow), BaitType.DiademBalloonBug },
    };

    // Which Autohook preset to use based on grade and weather
    public static readonly Dictionary<(int grade, WeatherType weather), string> AutohookPresetStrategy = new()
    {
        // Grade 2 presets
        { (2, WeatherType.Snow), "AH4_H4sIAAAAAAAACs1XS2/jOAz+K4Eue1h7YcvP+JZJHxMgfWCcYg7FHmSbToQ4VkaSp5Mt8t8X8iOJXKedFgV2bwZFfvxIUST9jCaVZFMipJjmSxQ9o8uSJAVMigJFkldgIHU4pyUcD7PuaJahCIdjA91zyjiVOxTZBpqJy19pUWWQHcVKf99g3TCWrhRY/YHVV43jhwa63i5WHMSKFRmKbMvSkF+HrjHGgWZhvUlmuqo2ZwJzbcvtMRr3GHUgrCgglV0krm3Zp2r4bRaMZ5QUZ4jY2Pe1HLut2RUVq8sdiBPHXo+x52mM/e4OyBriFc3lF0Jr3kogOkEsSboWKAoNdFcWu+8rKOu7umXyYdsxexCgHc5md+X0ojsdKhQ/fEnvlNy4JXdPJIUyhZOw/L6dr18E7kw5/QemRDYF1nntW+PeNTqt9WJFCkrW4or8ZFwBaIIuK46hy79Byn4CR5Gtcn0mcFdz2F3DF7q8Jps60Em5LICLzomqmQxFTmC5L9hrUOF+b6DLX5IT7f0eCKj7XLD4iWxnpayopKy8JrTs0mPaBppXHG5ACLIEFCFkoNuaE7plJaAWYbcFFKk8DeDNmZAfxrvnIGCYITLRmfPGY31+5BNvIZWcFNOKcyjlJ0XZQ/20WAfZvoh40HutdcV4CvVjfSKHN1kLMyWti8fCoW20lRVLtlX9gpbLWMK2btTHKNvqm/DPCe4Urmb7UNIfFShcRHLPdrzMMbPcI6ab2IEZ2jgxkzD1IAlg7BGM9gaaUyHvcuVDoOjxufamAugIttGd43hBSQab0Vf1NJ8Y3yjIW8Y3pPjK2FqBdG3mO5D18eGoUwFaVltRE6prB6pPdcax5Kxcvsfcck7M57CEMiN8926EC1YlxYG7poH98UHhyO+sisZhQOtBwILTbcOso9RITt0v6AZ4r/nc0PJwpNreX5aB1MQd0lVyXT9U6jqhwMMq+Ma7Htvb/rHzPv82foPAUOYOqDkpBJwxV49xkkvgU1ItV3JON2oK281B/5XW+1fFmzGvPk4GUTMjvHF/UXl181HLUtdJu8fwDX5UlEMWSyIroQZseKzSwSo7+xB+u97/27J+WcG/VWbvKIX/2Z2f9OAkyTMnxaHpZgk23ZSkZmiHnonHgZ8nWQ4kBLT/u2vC7cb+eBA0ffjxGWkNGYeK07mGfM1JBiN3FK93SUWLDLj4Y3S9YkKOrghwqk8U+7WMzTIoJU1JodKk3DcKkw2rSk0NRa437u9Pjr4Sh8pTxXOSQlyontsund7Ye2Nt9PYGGviZGd4Ae1jhu/5tPgT5+s/PcfZ/eOLHT2SrJFOV9jrjpztAO/nVZyM+qg1V/El1OlmYB+D6ZhI62HRJZpvEcWwzsCAIkiTwMLHq6mxw9YHflBkexSV7Gv05WsBmC0LqK4iV4xxbSWjaPjim6+DQTNI8MW3HysLQHSdJ6qP9v+MU69obDwAA" },
        { (2, WeatherType.UmbralDuststorms), "AH4_H4sIAAAAAAAACs1XSW/rOAz+K4Euc7EHtuNNvqXpMgHSBXWLdyjmoNh0IsSx8iT59WWK/PeBvCSW63RDgZmbQZEfP1IUSb+gSSnZlAgpptkSRS/ooiCLHCZ5jiLJSzCQOpzTAo6HaXs0S1HkhNhAd5wyTuUORbaBZuLid5KXKaRHsdLf11jXjCUrBVZ9OOqrwvFDA11tH1YcxIrlKYpsy9KQ34auMHCgWVjvkpmuys2JwFzbcnuMcI9RC8LyHBLZRuLalt1Vc95nwXhKSX6CiO34vpZjtzG7pGJ1sQPRcez1GHuexthv74CsIV7RTJ4RWvFWAtEKYkmStUBRaKDbIt/9WEFR3dUNk4/bltmjAO1wNrstpuft6VCh+OFrel1yuCF3RySFIoFOWH7fztcvwmlNOf0HpkTWBdZ67Vs7vWscN9YPK5JTshaX5BfjCkATtFkZG7r8HhL2CziKbJXrE4G7msP2Gs7o8opsqkAnxTIHLlonqmZSFI0Dy33FXoMK93sDXfyWnGjv90BA3ecDi5/JdlbIkkrKiitCizY9pm2gecnhGoQgS0ARQga6qTihG1YAahB2W0CRytMA3pwJ+WW8Ow4ChhkiE504rz1W50c+8RYSyUk+LTmHQn5TlD3Ub4t1kO2riAe9V1qXjCdQPdZncniTlTBV0qp4LCfARlNZsWRb1S9osYwlbKtGfYyyqb4J/57gunAV28eC/ixB4aLMs9LAwZlphQmYrhM6ZkhSzwyx7WQptn0yTtDeQHMq5G2mfAgUPb1U3lQALcEmulMczylJYTO6h3R0RvKcsUKB3jC+IflfjK0VTNtofgBZH5+OOhWg5bUR1cG6dqA6VWscS86K5WfMrXHHfA5LKFLCd59GOGflIj9w1zQcHx8UjvxOqmgcBrQeBTxwuq2ZtZRqSdf9A90A77Wfa1ocjlTj+9MykJq5Q7pKruuHSl0nFHiOCr72rsf2Af/h5/zb1jsEhjJ3QM1ILuCEuXqOk0wCn5JyuZJzulFz2K4P+u+02sBKXg969dEZRfWU8HB/VXlz91HrUttL28dwDz9LyiGNJZGlUCM2PFbpYJWdfAgfrvf/tqxfV/CHyuwTpfA/u/NOF8bYxY5NFiYEC9t0IQtNnGDfzNzMsxaOj4OFjfZ/t2242dmfDoK6Ez+9IK0lO6HidKolX3GSwsgdxevdoqR5Clz8MbpaMSFHlwQ41WeK/VbGZikUkiYkV2lS7muFyYaVhaaGItfD/Q1qrC/FofJU8owkEOeq5zZrp4e9dxZHb2+ggd+Z4R2whxV+6u/mS5Bv//4cp/+XZ378TLZKMlVprzLe3QKa2a8+a/FRbajiO9VpAQTYCkMz8YlrusTB5mIRBGbou8QbW2PHcoKqOmtcfeTXZeaMzkshhWR8I/QFxPYDnNnj1HR91zddcMYmtkLb9PEi8cGyEtdaoP2/QhVpkhkPAAA=" },
        { (2, WeatherType.UmbralFlare), "AH4_H4sIAAAAAAAACs1XS4+jSAz+K1Fd9gIrIAUBbun0YyOlHxq6NYfWHhwwSSmEylQV05Nt5b+vCkICNOmXWpq5IZf9+bPLZZtnMi4Un4BUcpIuSPhMLnKYZzjOMhIqUaBB9OGM5Xg8TOqjaUJCxw8McicYF0xtSWgbZCovfsVZkWByFGv9XYV1zXm81GDlh6O/ShzPN8jV5n4pUC55lpDQtqwW8uvQJUYwallYb5KZLIv1icCobdEOo6DDqAbhWYaxqiOhtmU31Zy3WXCRMMhOELEdz2vlmO7NLplcXmxRNhy7Hcau22Ls1XcAK4yWLFVnwEreWiBrQaQgXkkS+ga5zbPt9yXm5V3dcPWwqZk9SGwdTqe3+eS8Pu0rFM9/Sa9JLtiTuwPFMI+xEZbXtfPaF+HUpoL9hxNQVYHVXrvWTucah3vr+yVkDFbyEn5yoQFagjorQ6Mt/4Yx/4mChLbO9YnAacthfQ1nbHEF6zLQcb7IUMjaia6ZhITDkUVfsG9B+budQS5+KQGt93sgoO/znkdPsJnmqmCK8fwKWF6nx7QNMisEXqOUsEASEmKQm5ITueE5kj3CdoMk1HnqwZtxqT6NdydQYj9DYpIT55XH8vzIJ9pgrARkk0IIzNUXRdlB/bJYe9m+iLjXe6l1yUWM5WN9gsObLIWJlpbFYzkj39hXVqT4RvcLli8ihZuyUR+j3FffWHxNcE24ku1Dzn4UqHGJ66dBStEygyFNTRonlhl4c8uc257rebFvI6RkZ5AZk+o21T4kCR+fS286gJrgPrpTHM8ZJLgenEGWcZ4PzoqFBr3hYg3ZP5yvNEzdaL4jrI5PR59KbOV1L6qCpfZId6raOFKC54uPmFvDhvkMF5gnILYfRjjnxTw7cG9pOF5wUDjyO6nS4tCj9SDxXrBNxaymVEma7u/ZGkWn/Vyz/HCkG9/flkH0zO3T1fK2vq/V24RGrqODr7y3Y3uHf/9j/m3rDQJ9mTugppBJPGGun+M4VSgmUCyWasbWeg7b1UH3nZYbWCGqQa8/GqOomhJu0F1VXt199LpU99L6MXzDHwUTmEQKVCH1iPWPVdpbZScfwrvr/feW9csKfleZfaAU/rA7b3RhBzxvhIlnOgkNTJrAyJxTC0zqJzZSdOcWjcnu37oN73f2x4Og6sSPz6TVkh1fczrVkq8EJDigg2i1nRcsS1DIvwZXSy7V4BJQsPZMsV/L2DTBXLEYMp0m7b5SGK95kbfUSEjdoLtBDdtLsa89FSKFGKNM99z92ukG7huLo7szSM/vTP8O2MHyP/R38ynI139/jtP/0zM/eoKNlkx02suMN7eA/ezXn5X4qNZX8Y3qHFo0tuLUMWNvTk1qO5YJgR+bYMexm45cd+hCWZ0VbnvkV2XmDC4zEJpyA5cGaCcOzM3YchyTogsmxEMw7eEo8FPLoSMPyO5//VoE2RQPAAA=" },
        { (2, WeatherType.UmbralLevin), "AH4_H4sIAAAAAAAACs1XTW/jOAz9K4Eue1h7YclyYvuWST+2QNopxi3mUOxBselEiGNlJLmdTJH/vpAdJ5HrtNOiwO7NoMjHR4oi6Wc0rrSYMKXVJJ+j+Bmdl2xWwLgoUKxlBQ4yh1NewuEwa4+uMhSTMHLQreRCcr1BMXbQlTr/mRZVBtlBbPS3Dda1EOnCgNUfxHzVOMPQQZfru4UEtRBFhmLseRby69A1RjSyLLw3yUwW1epEYBR7tMMo6jBqQURRQKrbSCj28LEaeZuFkBlnxQkimAyHVo7pzuyCq8X5BtSR46DDOAgsxsP2DtgSkgXP9RfGa95GoFpBolm6VCgOHfS1LDbfF1DWd3Uj9P26ZXavwDq8uvpaTs7a075CGYYv6R2Ti3bkbpnmUKZwFNawaze0L4K0ppL/ggnTTYG1XrvWpHON/s76bsEKzpbqgj0KaQAsQZsV37Hl3yAVjyBRjE2uTwROLYftNXzh80u2qgMdl/MCpGqdmJrJUOyPPPqCvQUVbrcOOv+pJbPe756Auc87kTyx9VWpK665KC8ZL9v0uNhB00rCNSjF5oBihBx0U3NCN6IEtEPYrAHFJk89eFOh9IfxbiUo6GeIXHTivPFYnx/4JGtItWTFpJISSv1JUXZQPy3WXrYvIu71XmtdCJlC/Vif2P5N1sLMSOvi8UjoObvKSrRYm37By3miYV036kOUu+oby88J7hiuZntf8h8VGFzk4YhiEnouYyNwaUbBDTH2XRZmGAibMS8K0NZBU67019z4UCh+eK69mQBagrvoTnE84yyD1WAiWQmDi2JjIG+EXLHibyGWBqRtM9+BLQ8Px5wqsLK6EzWhUjwyfao1TrQU5fw95p5/ZD6FOZQZk5t3I5yJalbsuVsaZBjtFQ78TqpYHHq07hXcSb5umLWUGsmx+zu+AtlpPte83B+ZtveX5yAzcft0jdzWD426TWgUEBN8492O7W3/xH+ff0zeINCXuT1qzgoFJ8zNYxznGuSEVfOFnvKVmcK4Oei+0nr/qmQz5s3H0SBqZkQQdReVVzcfsyy1nbR9DN/gR8UlZIlmulJmwIaHKu2tspMP4bfr/b8t65cV/Ftl9o5S+J/d+VEPzrMcD/Nh6I5YylxKvMxlKVCXBtinUeB5xCNo+0/bhHcb+8Ne0PThh2dkNWQSGk6nGvKlZBkM6CBZbmYVLzKQ6o/B5UIoPbhgILk9UfBrGbvKoNQ8ZYVJk3HfKIxXoiotNRTTIOruT769EofGUyVzlkJSmJ67WzqDKHhjbQy2Dur5menfADtY4bv+bT4E+frPz2H2f3jiJ09sbSQTk/Y648c7wG7ym89GfFDrq/ij6gywP5rhCLtZNIpcShh2IyDMDUmWziIyoh716+pscO2B35QZGSSleBr8OZjCIy/tBcQPIcAe9VwIPebSPEjdkGbg5tksjHJMsE8J2v4LzCG9rRkPAAA=" },
        { (2, WeatherType.UmbralTempest), "AH4_H4sIAAAAAAAACs1XS2/jOAz+K4Eue1h7YcvP+JZJHxMgfWCcYg7FHmSbToQ4VkaSp5Mt8t8X8iOJXKedFgV2bwZFfvxIUST9jCaVZFMipJjmSxQ9o8uSJAVMigJFkldgIHU4pyUcD7PuaJahCIdjA91zyjiVOxTZBpqJy19pUWWQHcVKf99g3TCWrhRY/YHVV43jhwa63i5WHMSKFRmKbMvSkF+HrjHGgWZhvUlmuqo2ZwJzbcvtMRr3GHUgrCgglV0krm3Zp2r4bRaMZ5QUZ4jY2Pe1HLut2RUVq8sdiBPHXo+x52mM/e4OyBriFc3lF0Jr3kogOkEsSboWKAoNdFcWu+8rKOu7umXyYdsxexCgHc5md+X0ojsdKhQ/fEnvlNy4JXdPJIUyhZOw/L6dr18E7kw5/QemRDYF1nntW+PeNTqt9WJFCkrW4or8ZFwBaIIuK46hy79Byn4CR5Gtcn0mcFdz2F3DF7q8Jps60Em5LICLzomqmQxFTmC5L9hrUOF+b6DLX5IT7f0eCKj7XLD4iWxnpayopKy8JrTs0mPaBppXHG5ACLIEFCFkoNuaE7plJaAWYbcFFKk8DeDNmZAfxrvnIGCYITLRmfPGY31+5BNvIZWcFNOKcyjlJ0XZQ/20WAfZvoh40HutdcV4CvVjfSKHN1kLMyWti8fCoW20lRVLtlX9gpbLWMK2btTHKNvqm/DPCe4Urmb7UNIfFShcRHLPdrzMMbPcI6ab2IEZ2jgxkzD1IAlg7BGM9gaaUyHvcuVDoOjxufamAugIttGd43hBSQab0Vf1NJ8Y3yjIW8Y3pPjK2FqBdG3mO5D18eGoUwFaVltRE6prB6pPdcax5Kxcvsfcck7M57CEMiN8926EC1YlxYG7poH98UHhyO+sisZhQOtBwILTbcOso9RITt0v6AZ4r/nc0PJwpNreX5aB1MQd0lVyXT9U6jqhwMMq+Ma7Htvb/rHzPv82foPAUOYOqDkpBJwxV49xkkvgU1ItV3JON2oK281B/5XW+1fFmzGvPk4GUTMjvHF/UXl181HLUtdJu8fwDX5UlEMWSyIroQZseKzSwSo7+xB+u97/27J+WcG/VWbvKIX/2Z2f9OAkyTMnxaHpZgk23ZSkZmiHnonHgZ8nWQ4kBLT/u2vC7cb+eBA0ffjxGWkNGYeK07mGfM1JBiN3FK93SUWLDLj4Y3S9YkKOrghwqk8U+7WMzTIoJU1JodKk3DcKkw2rSk0NRa437u9Pjr4Sh8pTxXOSQlyontsund7Ye2Nt9PYGGviZGd4Ae1jhu/5tPgT5+s/PcfZ/eOLHT2SrJFOV9jrjpztAO/nVZyM+qg1V/El1OlmYB+D6ZhI62HRJZpvEcWwzsCAIkiTwMLHq6mxw9YHflBkexSV7Gv05WsBmC0LqK4iV4xxbSWjaPjim6+DQTNI8MW3HysLQHSdJ6qP9v+MU69obDwAA" },

        // Grade 3 presets
        // { (3, WeatherType.Snow), "Grade3_Snow" },
        // { (3, WeatherType.UmbralDuststorms), "Grade3_Duststorms" },
        // { (3, WeatherType.UmbralFlare), "Grade3_Flare" },
        // { (3, WeatherType.UmbralLevin), "Grade3_Levin" },
        // { (3, WeatherType.UmbralTempest), "Grade3_Tempest" },

        // // Grade 4 presets
        // { (4, WeatherType.Snow), "Grade4_Snow" },
        // { (4, WeatherType.UmbralDuststorms), "Grade4_Duststorms" },
        // { (4, WeatherType.UmbralFlare), "Grade4_Flare" },
        // { (4, WeatherType.UmbralLevin), "Grade4_Levin" },
        // { (4, WeatherType.UmbralTempest), "Grade4_Tempest" },
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
}