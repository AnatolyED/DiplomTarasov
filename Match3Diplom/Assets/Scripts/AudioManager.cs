using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private List<AudioSource> _audios = new List<AudioSource>();
    private void Awake()
    {
        instance = this;
    }

    public void PlayAudio(ClipType type)
    {
        switch (type)
        {
            case ClipType.Select:
                break;
            case ClipType.Swap:
                break;
            case ClipType.Clear:
                break;
            default:
                break;
        }
    }
}
