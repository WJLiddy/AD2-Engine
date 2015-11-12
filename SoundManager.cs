using IrrKlang;
using System.Collections.Generic;
using System.IO;

public class SoundManager
{
    //A dictionary containing all of the sounds in the directory.
    public static Dictionary<string, ISoundSource> SoundHash;
    //A sound engine for our sound needs.
    public static ISoundEngine Engine;
    //Volume to play all of our sounds at.
    public static readonly float VolumeMax= .3f;

    public static void Load(string path)
    {
        Engine = new ISoundEngine();
        SoundHash = GetAllSoundFilesFromPath(path);
    }

    public void Play(string name,bool looped = false)
    {
        if (SoundHash.ContainsKey(name))
        {
            ISoundSource sound = SoundHash[name];
            sound.DefaultVolume = VolumeMax;
            Engine.Play2D(sound, looped, false, false);
        } else
        {
            Utils.Log("Couldn't play sound: " + name);
        }
    }

    public void Stop()
    {
        Engine.RemoveAllSoundSources();
    }

    private static Dictionary<string, ISoundSource> GetAllSoundFilesFromPath(string path)
    {
        Dictionary<string, ISoundSource> newSoundHash = new Dictionary<string, ISoundSource>();
        string[] files = Directory.GetFiles(Utils.PathToAssets + path);
        foreach (string s in files)
        {
            ISoundSource sound = Engine.AddSoundSourceFromFile(s);
            string id = Path.GetFileName(s);
            newSoundHash.Add(id, sound);
        }
        return newSoundHash;
    }
}