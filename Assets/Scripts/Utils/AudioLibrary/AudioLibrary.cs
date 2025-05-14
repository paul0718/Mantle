using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioLibrary", menuName = "ScriptableObjects/Audio Library", order = 1)]
public class AudioLibrary : ScriptableObject
{
    public List<AudioClip> library = new List<AudioClip>();
}
