using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using CommonUtilLib.ThreadSafe;
using UnityEngine.UI;

/// <summary>
/// 사운드 관리
/// BGM, SFX 키고 끌수 있으며 상황에 따라 사운드 크기가 서서히 바뀌던지 BGM을 무작위로 루프할 수 있음
/// </summary>
public class SoundManager : SingleTonForGameObject<SoundManager>
{
    public enum BGM
    {
        CasualMusicLoop01,
        CasualMusicLoop04_1,
        CasualMusicLoop08_1,
        Count
    }
    public enum SFX
    {
        CraftSFX00,
        SwapClickSFX01,
        Count
    }

    public AudioMixer Mixer;
    public float MasterVolume
    {
        get { return m_masterVolume; }
        set
        {
            m_masterVolume = value;
            MasterSoundVolume(m_masterVolume);
        }
    }
    private float m_masterVolume = 0.5f;

    internal bool BIsOpened
    {
        get
        {
            return m_soundSettingsUI.activeSelf;
        }
    }


    [Header("BGM")]
    public AudioClip[] bgmClips = new AudioClip[(int)BGM.Count];
    public float BgmVolume
    {
        get { return m_BgmVolume; }
        set
        {
            m_BgmVolume = value;
            BGMSoundVolume(m_BgmVolume);
        }
    }
    private float m_BgmVolume = 0.5f;

    
    public AudioSource bgmPlayer;
    public AudioSource bgmBuffer;

    [Header("SFX")]
    public AudioClip[] sfxClips = new AudioClip[(int)SFX.Count];
    // 클래스 상단에 추가
    private Dictionary<SFX, int> sfxPlayingCount = new Dictionary<SFX, int>();
    public int maxSimultaneousSFX = 2;
    public float SfxVolume
    {
        get { return m_SfxVolume; }
        set
        {
            m_SfxVolume = value;
            SFXSoundVolume(m_SfxVolume);
        }
    }
    private float m_SfxVolume = 0.5f;

    public int channels = 20;
    AudioSource[] sfxPlayers;
    int channelIndex;

    public bool IsMuteBGM
    {
        get { return m_isMuteBGM; }
        set
        {
            m_isMuteBGM = value;
            bgmPlayer.mute = IsMuteBGM | IsMuteMaster;
            bgmBuffer.mute = IsMuteBGM | IsMuteMaster;
        }
    }
    private bool m_isMuteBGM = false;

    public bool IsMuteSFX
    {
        get { return m_isMuteSFX; }
        set
        {
            m_isMuteSFX = value;
            for (int i = 0; i < sfxPlayers.Length; i++)
            {
                sfxPlayers[i].mute = IsMuteSFX | IsMuteMaster;
            }
        }
    }

    private bool m_isMuteSFX = false;

    public bool IsMuteMaster
    {
        get { return m_isMuteMaster; }
        set
        {
            m_isMuteMaster = value;
            bgmPlayer.mute = IsMuteBGM | IsMuteMaster;
            bgmBuffer.mute = IsMuteBGM | IsMuteMaster;
            for (int i = 0; i < sfxPlayers.Length; i++)
            {
                sfxPlayers[i].mute = IsMuteSFX | IsMuteMaster;
            }
        }
    }
    private bool m_isMuteMaster = false;
    [SerializeField] private GameObject m_soundSettingsUI;
    [SerializeField] private Slider m_BGMSlider;
    [SerializeField] private Slider m_SFXSlider;


    public void Awake()
    {
        SetInstance(this);
        Init();
    }

    // 초기화 BGM은 메인과 버퍼 2개가 있으며 SFX는 채널수를 지정해서 그 갯수만큼 만듦
    void Init()
    {
        Mixer = Resources.Load<AudioMixer>($"Sound/Mixer");

        GameObject bgmObject = new GameObject("BgmPlayer");

        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.outputAudioMixerGroup = Mixer.FindMatchingGroups("BGM")[0];
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = BgmVolume;
        bgmPlayer.dopplerLevel = 0.0f;
        bgmPlayer.reverbZoneMix = 0.0f;

        GameObject bufferObject = new GameObject("BgmPlayer");
        bufferObject.transform.parent = transform;
        bgmBuffer = bufferObject.AddComponent<AudioSource>();
        bgmBuffer.outputAudioMixerGroup = Mixer.FindMatchingGroups("BGM")[0];
        bgmBuffer.playOnAwake = false;
        bgmBuffer.loop = true;
        bgmBuffer.volume = BgmVolume;
        bgmBuffer.dopplerLevel = 0.0f;
        bgmBuffer.reverbZoneMix = 0.0f;

        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;

        sfxPlayers = new AudioSource[channels];

        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            sfxPlayers[index] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[index].outputAudioMixerGroup = Mixer.FindMatchingGroups("SFX")[0];
            sfxPlayers[index].playOnAwake = false;
            sfxPlayers[index].volume = SfxVolume;
            sfxPlayers[index].dopplerLevel = 0.0f;
            sfxPlayers[index].reverbZoneMix = 0.0f;
        }


        bgmClips = new AudioClip[(int)BGM.Count];
        sfxClips = new AudioClip[(int)SFX.Count];
        
        // BGM 클립 초기화
        for (int i = 0; i < bgmClips.Length; i++)
        {
            bgmClips[i] = Resources.Load<AudioClip>($"Sound/BGM/{(BGM)i}");
            Debug.Log(bgmClips[i].name);
        }
        // SFX 클립 초기화
        for (int i = 0; i < sfxClips.Length; i++)
        {
            sfxClips[i] = Resources.Load<AudioClip>($"Sound/SFX/{(SFX)i}");
        }

        PlayBgm(BGM.CasualMusicLoop08_1, true);
    }
    private void Start()
    {
        m_BGMSlider.value = SaveDataBuffer.Instance.Data.BGMVolume;
        m_SFXSlider.value = SaveDataBuffer.Instance.Data.SFXVolume;
    }

    // BGM을 실행
    public void PlayBgm(BGM bgm, bool isLoop)
    {
        if (bgmPlayer.isPlaying)
        {
            bgmBuffer.clip = bgmClips[(int)bgm];
            bgmBuffer.Play();
            bgmBuffer.volume = BgmVolume;
            StartCoroutine(SoundSmooth(bgmPlayer, true));
            StartCoroutine(SoundSmooth(bgmBuffer, false));

            var temp = bgmBuffer;
            bgmBuffer = bgmPlayer;
            bgmPlayer = temp;

            bgmPlayer.loop = isLoop;
            bgmBuffer.loop = isLoop;
        }
        else
        {
            bgmPlayer.clip = bgmClips[(int)bgm];
            bgmPlayer.volume = BgmVolume;
            bgmPlayer.Play();
            StartCoroutine(SoundSmooth(bgmPlayer, false));
            bgmPlayer.loop = isLoop;
        }
    }
    // BGM을 멈춤
    public void StopBgm()
    {
        StartCoroutine(SoundSmooth(bgmPlayer, true));
        StopCoroutine("BGMRandomLoop");
    }


    // SFX를 실행
    public void PlaySfx(SFX sfx, float Pitch = 1, bool isLoop = false, float volume = 0)
    { 
        // 효과음 재생 전 체크 현재 재생 중인 효과음 개수 체크
        if (!sfxPlayingCount.ContainsKey(sfx))
            sfxPlayingCount[sfx] = 0;

        if (sfxPlayingCount[sfx] >= maxSimultaneousSFX)
            return;

        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            int loopIndex = (index + channelIndex) % sfxPlayers.Length;

            if (sfxPlayers[loopIndex].isPlaying)
                continue;

            channelIndex = loopIndex;
            sfxPlayers[loopIndex].clip = sfxClips[(int)sfx];
            sfxPlayers[loopIndex].loop = isLoop;
            sfxPlayers[loopIndex].pitch = Pitch;
            sfxPlayers[loopIndex].volume = volume > float.Epsilon ? volume : SfxVolume;
            sfxPlayingCount[sfx]++;
            sfxPlayers[loopIndex].Play();
            StartCoroutine(ReleaseSfxCountAfterPlay(sfx, sfxPlayers[loopIndex].clip.length));
            break;
        }
    }
    // SFX를 서서히 실행
    public void SmoothPlaySfx(SFX sfx, float Pitch = 1, bool isLoop = false)
    {
        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            int loopIndex = (index + channelIndex) % sfxPlayers.Length;

            if (sfxPlayers[loopIndex].isPlaying)
                continue;

            channelIndex = loopIndex;
            sfxPlayers[loopIndex].clip = sfxClips[(int)sfx];
            sfxPlayers[loopIndex].loop = isLoop;
            sfxPlayers[loopIndex].pitch = Pitch;
            sfxPlayers[loopIndex].Play();
            StartCoroutine(SoundSmooth(sfxPlayers[loopIndex], false));
            break;
        }
    }

    // SFX를 멈춤
    public void StopSfx(SFX sfx)
    {
        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            if (sfxPlayers[index].clip == sfxClips[(int)sfx])
            {
                StartCoroutine(SoundSmooth(sfxPlayers[index], true));
            }
        }
    }
    
    // 모든 SFX를 멈춤
    public void StopSfx()
    {
        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            if (sfxPlayers[index].isPlaying)
            {
                StartCoroutine(SoundSmooth(sfxPlayers[index], true));
            }
        }
    }

    // Mixer를 이용해 BGM 크기 조절
    public void MasterSoundVolume(float val)
    {
        Mixer.SetFloat("MasterSound", Mathf.Log10(val) * 20);
    }
    public void MasterSoundVolume(Slider sliderBar)
    {
        Mixer.SetFloat("MasterSound", Mathf.Log10(sliderBar.value) * 20);
        var data = SaveDataBuffer.Instance.Data;
        data.MasterVolume = sliderBar.value;
        SaveDataBuffer.Instance.Data = data;
    }

    // Mixer를 이용해 BGM 크기 조절
    public void BGMSoundVolume(float val)
    {
        Mixer.SetFloat("BGMSound", Mathf.Log10(val) * 20);
    }
    public void BGMSoundVolume()
    {
        Mixer.SetFloat("BGMSound", Mathf.Log10(m_BGMSlider.value) * 20);
    }

    // Mixer를 이용해 SFX 크기 조절
    public void SFXSoundVolume(float val)
    {
        Mixer.SetFloat("SFXSound", Mathf.Log10(val) * 20);
    }
    public void SFXSoundVolume()
    {
        Mixer.SetFloat("SFXSound", Mathf.Log10(m_SFXSlider.value) * 20);
    }

    // 특정 값까지 BGM을 서서히 줄임
    public void SetBGMSoundVolume(float val)
    {
        StartCoroutine(BGMSmoothVolum(val, 1));
    }

    // BGM을 실행하고 끝났을시 랜덤으로 다시 돌림
    public void StartBGMRandomLoop(int num)
    {
        StopCoroutine("BGMRandomLoop");
        StartCoroutine("BGMRandomLoop", num);
    }

    // 사운드 크기가 특정값까지 자연스럽게 바뀜
    IEnumerator BGMSmoothVolum(float endVolum, float time)
    {
        float DeltaVolum = (endVolum - bgmPlayer.volume) * 0.1f;
        float second = time * 0.1f;

        for (int i = 0; i < 10; i++)
        {
            bgmPlayer.volume += DeltaVolum;
            yield return new WaitForSeconds(second);
        }
    }

    // 사운드의 크기를 서서히 줄이거나 늘릴때 사용 
    IEnumerator SoundSmooth(AudioSource audio, bool isDown)
    {
        float startvolume = audio.volume;
        float start = 0;
        float num = 0;
        if (isDown)
        {
            start = audio.volume;
            num = -audio.volume / 10;
        }
        else
        {
            start = 0;
            num = audio.volume / 10;
        }

        audio.volume = start;

        for (int i = 0; i < 10; i++)
        {
            audio.volume += num;
            yield return new WaitForSeconds(0.095f);
        }

        if (isDown)
        {
            audio.Stop();
            audio.volume = startvolume;
        }
    }

    // 2초마다 한번씩 BGM이 끝났는지 검사 후 끝났으면 다음 BGM을 틀음
    IEnumerator BGMRandomLoop(int num)
    {
        int[] temp = new int[num];
        for (int i = 0; i < num; i++)
            temp[i] = i;

        SuffleArray(temp);

        int index = 0;

        while (true)
        {
            if (!bgmPlayer.isPlaying)
            {
                PlayBgm((BGM)temp[index], false);
                index++;

                if (index >= temp.Length)
                {
                    index = 0;
                    SuffleArray(temp);
                }
            }
            yield return new WaitForSeconds(2);
        }
    }

    /// <summary>
    /// 정수 배열을 무작위로 섞습니다.
    /// </summary>
    public void SuffleArray(int[] array)
    {
        int n = array.Length;
        for (int i = n - 1; i > 0; i--)
        {
            // 0부터 i 사이의 랜덤한 인덱스를 선택
            int randomIndex = Random.Range(0, i + 1);

            // 현재 요소(i)와 랜덤하게 선택된 요소(randomIndex)를 교체(Swap)
            int temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }
    // SFX가 재생 중인 개수를 줄이는 코루틴
    IEnumerator ReleaseSfxCountAfterPlay(SFX sfx, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (sfxPlayingCount.ContainsKey(sfx))
            sfxPlayingCount[sfx] = Mathf.Max(0, sfxPlayingCount[sfx] - 1);
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }

    public void SetActiveSoundPanel()
    {
        m_soundSettingsUI.SetActive(!m_soundSettingsUI.activeSelf);

        if(!m_soundSettingsUI.activeSelf)
        {
            var data = SaveDataBuffer.Instance.Data;
            data.BGMVolume = m_BGMSlider.value;
            data.SFXVolume = m_SFXSlider.value;
            SaveDataBuffer.Instance.Data = data;
            SaveDataBuffer.Instance.SaveData();
        }
    }
}