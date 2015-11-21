using IrrKlang;
using System.Collections.Generic;
using System.IO;

//TODO: Singletonize?
public class SoundManager
{
    //A dictionary containing all of the sounds in the directory.
    private static Dictionary<string, ISoundSource> SoundHash;
    //A sound engine for our sound needs.
    private static ISoundEngine Engine;
    //Volume to play all of our sounds at.
    private static readonly float VolumeMax= .3f;

    //Load sound from path, relative to PathToAssets.
    public static void Load(string path)
    {
        Engine = new ISoundEngine();
        SoundHash = GetAllSoundFilesFromPath(path);
    }

    //Play a sound.
    public static void Play(string name,bool looped = false)
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

    //Stop all Sounds.
    public static void Stop()
    {
        Engine.RemoveAllSoundSources();
    }

    //Generate all sounds from a file.
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