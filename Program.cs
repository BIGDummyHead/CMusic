using System.Runtime.InteropServices;
using CMusic.WASAPI;

namespace CMusic
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var htmlData = await FileTools.ReadWaveFileURLAsync("https://file-examples.com/storage/fe6a71582967c9a269c25cd/2017/11/file_example_WAV_1MG.wav");

            using Player wav = new Player(htmlData.format);
            wav.QueuedAudioStreams.Enqueue(htmlData.audioData);

            do
            {
                await wav.Play();

            } while(wav.Next());
        }
    }


}