# CMusic

This is a simple wrapper for the Windows WASAPI that currently just supports audio rendering of WAV files. 

Currently you are able to natively load in <i><b>.wav</b></i> files with limited support.

```cs

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

```

## Feature Bucket List

<ul>
  <li>.mp3 Support</li>
  <li>Audio Conversion for support of odd files</li>
  <li>Audio recording</li>
  <li>Audio Control like volume</li>
  <li>More WASAPI native features</li>
</ul>
