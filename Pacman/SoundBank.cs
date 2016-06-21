using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacmanGame
{
    class SoundBank
    {
        Dictionary<string, SoundEffect> listSfx;

        List<SoundEffectInstance> listCurrSfx;

        Song theme;

        Random rand;

        bool enabled = true;

        public SoundBank()
        {
            listSfx = new Dictionary<string, SoundEffect>();
            listCurrSfx = new List<SoundEffectInstance>();

            rand = new Random();
            MediaPlayer.Volume = 0.05f;
        }

        public void setTheme(Song song)
        {
            theme = song;
            MediaPlayer.IsRepeating = true;
        }

        public void update()
        {
            for(int i = 0; i < listCurrSfx.Count; i++)
            {
                if (listCurrSfx[i].State == SoundState.Stopped)
                {
                    listCurrSfx[i].Dispose();
                    listCurrSfx.RemoveAt(i);
                }
            }
        }

        public void addSfx(string name, SoundEffect sfx)
        {
            listSfx.Add(name, sfx);
        }

        public SoundEffectInstance getSoundInstance(string name, float volume, bool loop)
        {
            SoundEffectInstance instance = listSfx[name].CreateInstance();
            instance.Volume = volume;
            instance.IsLooped = loop;

            return instance;
        }

        public void playSfx(string name, float volume, bool loop)
        {
            if (!enabled)
                return;

            SoundEffectInstance instance = getSoundInstance(name, volume, loop);
            instance.Play();
        }

        public void playSfxRandom(string name, int min, int max, float volume, bool loop)
        {
            int id = min + rand.Next(max - min + 1);

            playSfx(name + id, volume, loop);
        }

        public void playTheme()
        {
            if (MediaPlayer.State == MediaState.Playing || !enabled)
                return;


            if (MediaPlayer.State == MediaState.Stopped)
                MediaPlayer.Play(theme);
            else if (MediaPlayer.State == MediaState.Paused)
                MediaPlayer.Resume();
        }

        public void pauseTheme()
        {
            if (MediaPlayer.State == MediaState.Paused)
                return;

            MediaPlayer.Pause();
        }

        public void stopAll()
        {
            for (int i = 0; i < listCurrSfx.Count; i++)
                listCurrSfx[i].Stop();

            pauseTheme();
        }

        public bool Enabled
        {
            get { return enabled; }
            set
            {
                enabled = value;
                if (!enabled)
                    stopAll();
                else
                    playTheme();
            }
        }
    }
}
