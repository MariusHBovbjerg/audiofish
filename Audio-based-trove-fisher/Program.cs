using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace Audio_based_trove_fisher;

internal class Program
{
    private static void Main(string[] args)
    {
        var fishSettings = new FishSettings();
        
        fishSettings.LoadFromIniFile("settings.ini");
        
        var enumerator = new MMDeviceEnumerator();
        var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
        var device = devices.FirstOrDefault(d => d.FriendlyName == fishSettings.AudioDevice);
        
        var waveIn = new WasapiLoopbackCapture(device);

        new AudioDeviceDataHandler(waveIn, fishSettings, "Trove");
        
    }
}