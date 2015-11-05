using IrrKlang;
using System.Collections.Generic;
using System.IO;

public class SoundManager
{
    Dictionary<string, ISoundSource> soundHash;
    ISoundEngine engine;

    public SoundManager(string path)
    {
        engine = new ISoundEngine();
        soundHash = new Dictionary<string, ISoundSource>();
        string[] files = Directory.GetFiles(Utils.pathToAssets + path);
        foreach(string s in files)
        {
            ISoundSource sound= engine.AddSoundSourceFromFile(s);
            string id = Path.GetFileName(s);
            soundHash.Add(id, sound);
        }
    }

    public void play(string name,bool looped = false)
    {
        ISoundSource sound = soundHash[name];
        engine.Play2D(sound, looped, false, false);
    }

    public void stop()
    {
        engine.RemoveAllSoundSources();
    }
}

