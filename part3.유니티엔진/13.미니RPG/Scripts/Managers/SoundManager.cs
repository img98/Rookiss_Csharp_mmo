using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager
{
    AudioSource[] _audioSources = new AudioSource[(int)Define.Sound.MaxCount]; //Bgm, Effect�� �ҽ��� ���� �ʿ��ѵ�, �׳� �迭�� ���� �ڵ带 �ѹ��� ����.

    Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>(); //�����Ŭ������ ��Ƴ��� ��ųʸ�, ���⼭ string�� ���(path)�� �����Ű�, AudioClip�� ���״�� Ŭ���� �������̴�.

    // MP3 �����ġ => AudioSource
    // MP3 ���� => AudioClup
    // ����(��) => AudioListener
    
    public void Init()
    {
        GameObject root = GameObject.Find("@Sound");
        if(root==null)
        {
            root = new GameObject { name = "@Sound" };
            Object.DontDestroyOnLoad(root);

            string[] soundNames = System.Enum.GetNames(typeof(Define.Sound)); //enum Sound�� ����ִ� �̸����� �迭�� ��´�.
            for(int i=0;i<soundNames.Length-1;i++) //�̸��� ������ŭ(MaxCount�� �ҽ�Ÿ���� ���� �̸��� �ƴϴ�, �ϳ��� ��޾��Ѵ� -1)
            {
                GameObject go = new GameObject { name = soundNames[i] }; //���ο� ������Ʈgo�� ����� �̸��� enum Sound�� �ִ� �̸����� �����ش�.
                _audioSources[i] = go.AddComponent<AudioSource>(); //������ ���� ������Ʈgo�� audioSource ������Ʈ�� �޾Ƽ�, ��׸� ȣ���ϱ� ����, ������ �غ��ص� _audioSources �迭�� �����Ѵ�.
                go.transform.parent = root.transform; //go�� root(@Sound)�� ���ϰ���� �������.
            }

            _audioSources[(int)Define.Sound.Bgm].loop = true; //Bgm�� ����ϴ� �ҽ��� loop�� true��
        }
    }

    public void Clear() //@Sound�� ���� ���� �Ѿ�� ���� ����������, �޸� ��������ʰ�, ���������� �Ⱦ��°� �ϴ� ������ߵ�
    {
        foreach(AudioSource audioSource in _audioSources)
        {
            audioSource.Stop();
            audioSource.clip = null;
        }

        _audioClips.Clear();
    }


    public void Play(string path, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f)
    {
        AudioClip audioClip = GetOrAddAudioClip(path, type);
        Play(audioClip, type, pitch); // Play������ �ٸ� ������Play �ҷ�����
    }
    public void Play(AudioClip audioClip, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f) //���߿� ���������ϸ� path�� ��μ������ִ°� ���ѵ�, �׶������� ��� ��� ���ִ°� �����, ���� Ŭ���� �־��ִ� ������ ������ ���θ�������.
    {
        if (audioClip == null)
            return;

        if (type == Define.Sound.Bgm)
        {
            AudioSource audioSource = _audioSources[(int)Define.Sound.Bgm];
            if (audioSource.isPlaying) //�̹� �ٸ� bgm�� �����ִٸ� ���߰��ѵ� Ʋ�����
                audioSource.Stop();

            audioSource.pitch = pitch;
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        else
        {
            AudioSource audioSource = _audioSources[(int)Define.Sound.Effect];
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(audioClip);

        }
    }

    AudioClip GetOrAddAudioClip(string path, Define.Sound type = Define.Sound.Effect) //�Ź� Managers.Resource.Load�� ����ϸ� �޸� �δ�Ǵ�, �̹� ã�Ҵ��Ŵ� ��Ƶ״ٰ� �ٽ� ȣ���Ҷ� ���������ϴ� �Լ�
    {
        if (path.Contains("Sounds/") == false) //path�� ������ Sounds/ ���Ͽ� �����ϴµ�, ���� �Է��Ҷ� ���ִ°� �����ߴٸ� �߰����ִ��ڵ�
            path = $"Sounds/{path}";

        AudioClip audioClip = null;

        if (type == Define.Sound.Bgm)
        {
            AudioSource audioSource = _audioSources[(int)Define.Sound.Bgm];
        }
        else 
        {
            if (_audioClips.TryGetValue(path, out audioClip) == false)
            {
                audioClip = Managers.Resource.Load<AudioClip>(path);
                _audioClips.Add(path, audioClip);
            }
        }
        if (audioClip == null)
            Debug.Log($"AudioClip Missing ! {path}");

        return audioClip;
    }

}
