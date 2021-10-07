using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolydayRun
{
    public class PlaySound
    {
        IWavePlayer wave;
        AudioFileReader file;


        public PlaySound(string FILE)
        {
            file = new AudioFileReader(FILE);
            wave = new WaveOut();
        }

        public void playSound()
        {
            wave.Init(file);
            wave.Play();
        }

    }
}
