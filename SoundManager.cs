using IrrKlang;
using System.Collections.Generic;
using System.IO;

public class SoundManager
{
    //A dictionary containing all of the sounds in the directory.
    public static Dictionary<string, ISoundSource> soundHash;
    //A sound engine for our sound needs.
    public static ISoundEngine engine;
    //Volume to play all of our sounds at.
    public static readonly float VOLUME_MAX = .3f;

    public static void load(string path)
    {
        engine = new ISoundEngine();
        soundHash = getAllSoundFilesFromPath(path);
    }

    public void play(string name,bool looped = false)
    {
        if (soundHash.ContainsKey(name))
        {
            ISoundSource sound = soundHash[name];
            sound.DefaultVolume = VOLUME_MAX;
            engine.Play2D(sound, looped, false, false);
        } else
        {
            Utils.log("Couldn't play sound: " + name);
        }
    }

    public void stop()
    {
        engine.RemoveAllSoundSources();
    }

    private static Dictionary<string, ISoundSource> getAllSoundFilesFromPath(string path)
    {
        Dictionary<string, ISoundSource> newSoundHash = new Dictionary<string, ISoundSource>();
        string[] files = Directory.GetFiles(Utils.pathToAssets + path);
        foreach (string s in files)
        {
            ISoundSource sound = engine.AddSoundSourceFromFile(s);
            string id = Path.GetFileName(s);
            newSoundHash.Add(id, sound);
        }
        return newSoundHash;
    }
}