using System.Runtime.InteropServices;
using CMusic.WASAPI;

namespace CMusic
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            ParsedFile parsed = await FileTools.ReadWaveFileAsync(@"C:\Users\shawn\Downloads\example.wav");
            ParsedFile parsed2 = await FileTools.ReadWaveFileAsync(@"C:\Users\shawn\Downloads\myfile.wav");

            using Player wav = new Player(parsed.format);
            wav.QueuedAudioStreams.Enqueue(parsed.AsMemoryStream);
            wav.QueuedAudioStreams.Enqueue(parsed2.AsMemoryStream);
            wav.QueuedAudioStreams.Enqueue(parsed.AsMemoryStream);

            do
            {
                await wav.Play();

            } while(wav.Next());

            wav.Previous();
            await wav.Play();


            //Read up on the following for WASAPI
            //https://learn.microsoft.com/en-us/windows/win32/api/
            //ole32.dll
            //avert.dll
            //mmdeviceapi.dll
            //wasapi interfaces
            //https://learn.microsoft.com/en-us/windows/win32/api/audioclient/nn-audioclient-iaudioclient
            //https://learn.microsoft.com/en-us/windows/win32/api/audioclient/nn-audioclient-iaudiorenderclient
        }

        public static async Task Test(){
            HRESULT hr = Tools.GetDefaultDevice(out IMMDevice immDevice);
            System.Console.WriteLine($"Device Enumeration: 0x{hr:X}");

            hr = Tools.ActivateDevice(immDevice, out IAudioClient client);
            System.Console.WriteLine($"Device Activation: 0x{hr:X}");

            ParsedFile fileData = await FileTools.ReadWaveFileAsync(@"C:\Users\shawn\Downloads\example.wav");
            //var fileData = CreatePreferredSineWave(client);
            WAVEFORMATEX format = fileData.format;

            hr = client.GetDevicePeriod(out long defaultDevicePeriod, out _);
            System.Console.WriteLine($"Device Period: 0x{hr:X}");

            System.Console.WriteLine("Format -> " + format.ToString());
            hr = Tools.EnsureFormat(client, ref format);
            System.Console.WriteLine($"Format Enforcement Result: 0x{hr:X}");
            System.Console.WriteLine("Enforced Format -> " + format.ToString());



            hr = client.Initialize(
                AUDCLNT_SHAREMODE.AUDCLNT_SHAREMODE_SHARED,
                0, // No flags
                (uint)defaultDevicePeriod,
                0,
                ref format,
                IntPtr.Zero);

            System.Console.WriteLine($"Device Initialization: 0x{hr:X}");

            hr = Tools.GetRenderClient(client, out IAudioRenderClient? renderClient);
            System.Console.WriteLine($"Service Request: 0x{hr:X}");

            hr = client.GetBufferSize(out uint bufferSize);

            System.Console.WriteLine($"Buffer Size Request: 0x{hr:X}");

            hr = client.Start();
            System.Console.WriteLine($"Start Request: 0x{hr:X}");


            if (hr != HRESULT.S_OK)
                return;

            Stream memStream = new MemoryStream(fileData.audioData);

            int dataOffset = 0; //controls the position, we can then verify how far through we are in the this.
            while (dataOffset <= memStream.Length)
            {

                //gets the amount of frames already in the buffer...
                hr = client.GetCurrentPadding(out uint padding);

                if (hr != HRESULT.S_OK)
                {
                    System.Console.WriteLine("Abort. Failed to get the padding.");
                    break;
                }

                //take the actual buffer size and remove the frames already inside of the buffer
                uint actualFramesAvaiable = bufferSize - padding;
                if (actualFramesAvaiable == 0)
                    continue; //just keep going until we are given some

                uint bytesPerFrame = format.nBlockAlign;
                uint copyAmnt = Math.Min(actualFramesAvaiable * bytesPerFrame, (uint)(memStream.Length - dataOffset));
                uint countWrite = copyAmnt / bytesPerFrame;

                hr = renderClient.GetBuffer(countWrite, out IntPtr bufferPtr);
                if (hr != HRESULT.S_OK || bufferPtr == IntPtr.Zero)
                {
                    System.Console.WriteLine($"Could not get buffer to write: 0x{hr:X} , point to {bufferPtr}");
                    break;
                }

                //finally to the easier steps, copy to the buffer and release.

                //copy raw pcm wav data (+ offset) to the buffer pointer with the amnt 

                byte[] bufferData = new byte[copyAmnt];
                System.Console.WriteLine($"Stream Length: {memStream.Length} | Buffer Length: {copyAmnt} | Offset {dataOffset}");
                System.Console.WriteLine($"Total: {fileData.audioData.Length}");

                memStream.Position = dataOffset;

                int read = await memStream.ReadAsync(bufferData, 0, (int)copyAmnt);
                Marshal.Copy(bufferData, 0, bufferPtr, bufferData.Length);

                renderClient.ReleaseBuffer(countWrite, 0);

                dataOffset += (int)copyAmnt;
            }

            hr = client.Stop();
            System.Console.WriteLine($"Client Stop: 0x{hr:X}");

            Marshal.ReleaseComObject(renderClient);
            Marshal.ReleaseComObject(client);
            System.Console.WriteLine("Com Cleaned Up.");
        }

        /*static async Task<byte[]?> GetWavFileData(string path)
        {
            if (!File.Exists(path) || !Path.GetExtension(path).Equals(".wav", StringComparison.OrdinalIgnoreCase))
                return default;

            return await File.ReadAllBytesAsync(path);
        }*/


    }


}