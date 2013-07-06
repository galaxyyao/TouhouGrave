using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Configuration;

namespace TouhouSpring.Services
{
    [LifetimeDependency(typeof(ResourceManager))]
    [LifetimeDependency(typeof(CurrentProfile))]
    partial class Sound : GameService
    {
        bool m_isMusicOn = false;
        bool m_isSoundOn = false;
        float m_musicVolume = 0;
        float m_soundVolume = 0;

        public enum SoundEffectEnum
        {
            Menu
        }

        public enum MusicEnum
        {
            BlueTearsNight, kagamiM
        }

        private Dictionary<SoundEffectEnum, SoundEffectInstance> m_sounds;
        private Dictionary<MusicEnum, Song> m_musics;
        private SoundEffectInstance m_soundEffectInstance = null;
        private Song m_songInstance = null;

        public override void Startup()
        {
            FillSoundMusicList();
            PlayMusic(MusicEnum.BlueTearsNight);
        }

        private void FillSoundMusicList()
        {
            ResourceManager rm = GameApp.Service<Services.ResourceManager>();
            //Fill Sound
            m_sounds = new Dictionary<SoundEffectEnum, SoundEffectInstance>();
            m_sounds.Add(SoundEffectEnum.Menu, GameApp.Service<Services.ResourceManager>().Acquire<SoundEffect>("Audio/MenuClick").CreateInstance());

            //Fill Music
            m_musics = new Dictionary<MusicEnum, Song>();
            m_musics.Add(MusicEnum.BlueTearsNight, GameApp.Service<Services.ResourceManager>().Acquire<Song>("Audio/BlueTearsNightMp3"));
            m_musics.Add(MusicEnum.kagamiM, GameApp.Service<Services.ResourceManager>().Acquire<Song>("Audio/kagamiM"));
        }

        public void PlaySound(SoundEffectEnum sound)
        {
            if (!Settings.Instance.MyAppSettings.GameSettings.IsSoundOn)
                return;
            m_soundEffectInstance = m_sounds[sound];
            m_soundEffectInstance.Volume = Settings.Instance.MyAppSettings.GameSettings.SoundVolume;
            m_soundEffectInstance.Play();
        }

        public void PlayMusic(MusicEnum music)
        {
            if (!Settings.Instance.MyAppSettings.GameSettings.IsMusicOn)
                return;
            m_songInstance = m_musics[music];
            MediaPlayer.Play(m_songInstance);
            MediaPlayer.Volume = Settings.Instance.MyAppSettings.GameSettings.MusicVolume;
            MediaPlayer.IsRepeating = true;
        }
    }
}
