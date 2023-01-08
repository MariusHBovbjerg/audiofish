using System.Text;

namespace Audio_based_trove_fisher;

public class FishSettings
{
    // Audio Fields
    public string AudioDevice = "CABLE Input (VB-Audio Virtual Cable)";
    public float AudioThreshold = 0.10f;
    public float AudioThresholdMax = 0.3f;

    // Fish Bot Fields
    public int GetCatchDelay => new Random().Next(RandomMin, RandomMax) + CatchDelay;

    public int GetCastDelay => new Random().Next(RandomMin, RandomMax) + CastDelay;
    
    private int CatchDelay = 1000;
    private int CastDelay = 1500;
    public int RecordDelay = 3500;
    private int RandomMin = 100;
    private int RandomMax = 1000;
        
    // Fish Bot Keys
    public VKeys CastKey = VKeys.KEY_F;
    public VKeys ToggleKey = VKeys.KEY_5;
        
    public void LoadFromIniFile(string iniFilePath)
    {
        // Check if the file exists
        if (!File.Exists(iniFilePath))
        {
            Console.WriteLine($"The INI file does not exist, creating it: {iniFilePath}");
            
            // Write the default values to the file
            string[] defaultLines = {
                $"AudioDevice={AudioDevice}",
                $"AudioThreshold={AudioThreshold}",
                $"AudioThresholdMax={AudioThresholdMax}",
                $"CatchDelay={CatchDelay}",
                $"CastDelay={CastDelay}",
                $"RecordDelay={RecordDelay}",
                $"RandomMin={RandomMin}",
                $"RandomMax={RandomMax}",
                $"CastKey={CastKey}",
                $"ToggleKey={ToggleKey}"
            };
            File.WriteAllLines(iniFilePath, defaultLines);
            return;
        }

        // Read all lines from the file
        var lines = File.ReadAllLines(iniFilePath);

        // Iterate through each line
        foreach (var line in lines)
        {
            // Ignore comments and empty lines
            if (line.StartsWith(";") || string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            // Split the line into key and value
            string[] keyValue = line.Split('=');
            if (keyValue.Length != 2)
            {
                Console.WriteLine($"Invalid INI file format: {line}");
                continue;
            }

            string key = keyValue[0].Trim();
            string value = keyValue[1].Trim();

            // Update the corresponding field in the FishSettings class
            switch (key)
            {
                case "AudioDevice":
                    AudioDevice = value;
                    break;
                case "AudioThreshold":
                    float.TryParse(value, out AudioThreshold);
                    break;
                case "AudioThresholdMax":
                    float.TryParse(value, out AudioThresholdMax);
                    break;
                case "CatchDelay":
                    int.TryParse(value, out CatchDelay);
                    break;
                case "CastDelay":
                    int.TryParse(value, out CastDelay);
                    break;
                case "RecordDelay":
                    int.TryParse(value, out RecordDelay);
                    break;
                case "CastKey":
                    // convert char to ascii
                    VKeys.TryParse(value, out CastKey);
                    break;
                case "ToggleKey":
                    // convert char to ascii
                    VKeys.TryParse(value, out ToggleKey);
                    break;
            }
        }
    }
}