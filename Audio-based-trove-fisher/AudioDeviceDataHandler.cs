using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace Audio_based_trove_fisher;

public class AudioDeviceDataHandler
{
    private readonly FishSettings _fishSettings;
    private readonly WindowInteracter _windowInteracter;
    private readonly WasapiCapture _wasapiCapture;

    
    public AudioDeviceDataHandler(WasapiCapture wasapiCapture, FishSettings fishSettings, string processName)
    {
        _wasapiCapture = wasapiCapture;
        _fishSettings = fishSettings;
        _windowInteracter = new WindowInteracter(processName);
        
        _wasapiCapture.DataAvailable += OnDataAvailable;
        _wasapiCapture.RecordingStopped += OnRecordingStopped;

        Loop();
    }

    private void Loop()
    {
        // Create Task to wait for the window to be active
        var task = _windowInteracter.AwaitKeyPress(_fishSettings.ToggleKey);

        task.WaitAsync(cancellationToken: CancellationToken.None);
        
        if (task.Result)
        {
            _wasapiCapture.StartRecording();
            _windowInteracter.SendKeystroke(_fishSettings.CastKey);
        }
        
        Thread.Sleep(100);
        
        var task2 = _windowInteracter.AwaitKeyPress(_fishSettings.ToggleKey);

        task2.WaitAsync(cancellationToken: CancellationToken.None);
        if (task2.Result)
        {
            _wasapiCapture.StopRecording();
        }

    }
    
    // Event handler for when new audio data is available
    private void OnDataAvailable(object? sender, WaveInEventArgs e)
    {
        var silence = e.Buffer.All(x => x == 0);

        if (silence)
            return;

        // Convert the audio data to a float array
        var audioData = new float[e.Buffer.Length / 4];
        Buffer.BlockCopy(e.Buffer, 0, audioData, 0, e.Buffer.Length);

        // turn audioData into a list
        var audioList = audioData.ToList();

        // Increase the volume of the audio data
        var audioListGain = audioList.Select(x => x * 200).ToList();

        // Calculate the average amplitude of the audio data
        var averageAmplitude = audioListGain.Select(x => Math.Abs(x)).Average();

        Console.WriteLine("Average amplitude: " + averageAmplitude);

        // Check if the average amplitude is above a certain threshold

        if (!(averageAmplitude > _fishSettings.AudioThreshold)) return;
        if (averageAmplitude > _fishSettings.AudioThresholdMax) return;
        
        Console.WriteLine("yo!");

        _wasapiCapture.StopRecording();
    }
    
    private void OnRecordingStopped(object? sender, StoppedEventArgs e)
    {
        Console.WriteLine("Recording stopped, we must've caught something");
        
        Thread.Sleep(_fishSettings.GetCatchDelay);
        _windowInteracter.SendKeystroke(_fishSettings.CastKey);
        Thread.Sleep(_fishSettings.GetCastDelay);
        _windowInteracter.SendKeystroke(_fishSettings.CastKey);
        Thread.Sleep(_fishSettings.RecordDelay);
        
        _wasapiCapture.StartRecording();
    }
}