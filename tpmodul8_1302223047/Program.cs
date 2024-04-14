using System.Text.Json;

internal class Program
{
    private static void Main(string[] args)
    {
        double suhuBadan;
        int hariDemam;

        CovidData defaultConf = new CovidData();

        Console.Write("Berapa suhu badan Anda saat ini? ");
        suhuBadan = Convert.ToDouble(Console.ReadLine());
        Console.Write("Berapa hari yang lalu (perkiraan) anda terakhir memiliki gejala demam? ");
        hariDemam = Convert.ToInt32(Console.ReadLine());

        bool tepatWaktu = hariDemam < defaultConf.covidConf.batas_hari_demam;
        bool terimaFahrenheit = (defaultConf.covidConf.satuan_suhu == "fahrenheit") &&
                                (suhuBadan >= 97.7 && suhuBadan <= 99.5);
        bool terimaCelcius = (defaultConf.covidConf.satuan_suhu == "celcius") &&
                             (suhuBadan >= 36.5 && suhuBadan <= 37.5);

        if (tepatWaktu && (terimaCelcius || terimaFahrenheit))
        {
            Console.WriteLine(defaultConf.covidConf.pesan_diterima);
        }

        else
        {
            Console.WriteLine(defaultConf.covidConf.pesan_ditolak);
        }
    }
}

public class CovidConfig
{
    public string satuan_suhu { get; set; }
    public int batas_hari_demam { get; set; }
    public string pesan_ditolak { get; set; }
    public string pesan_diterima { get; set; }

    public CovidConfig() { }
    public CovidConfig(string satuan_suhu, int batas_hari_demam, string pesan_ditolak, string pesan_diterima)
    {
        this.satuan_suhu = satuan_suhu;
        this.batas_hari_demam = batas_hari_demam;
        this.pesan_ditolak = pesan_ditolak;
        this.pesan_diterima = pesan_diterima;
    }
}

public class CovidData
{
    public CovidConfig covidConf;
    public const string filePath = @"covidconfig.json";

    public CovidData()
    {
        try
        {
            ReadConfig();
        }

        catch (Exception)
        {
            SetDefault();
            WriteNewConfig();
        }
    }

    private CovidConfig ReadConfig()
    {
        string jsonData = File.ReadAllText(filePath);
        covidConf = JsonSerializer.Deserialize<CovidConfig>(jsonData);
        return covidConf;
    }

    private void SetDefault()
    {
        covidConf = new CovidConfig("celcius", 14, "Anda tidak diperbolehkan masuk ke gedung ini", "Anda dipersilakan untuk masuk ke gedung ini");
    }

    private void WriteNewConfig()
    {
        JsonSerializerOptions opts = new JsonSerializerOptions()
        {
            WriteIndented = true,
        };

        string JsonString = JsonSerializer.Serialize(covidConf, opts);
        File.WriteAllText(filePath, JsonString);
    }

    public void UbahSatuan(string satuanBaru)
    {
        bool satuanValid = (satuanBaru == "celcius" || satuanBaru == "fahrenheit");

        if (satuanBaru == null || !satuanValid)
        {
            throw new ArgumentException();
        }

        if (satuanValid)
        {
            covidConf.satuan_suhu = satuanBaru;

            JsonSerializerOptions opts = new JsonSerializerOptions()
            {
                WriteIndented = true,
            };

            string updateSuhu = JsonSerializer.Serialize(covidConf, opts);
            File.WriteAllText(filePath, updateSuhu);
        }
    }
}