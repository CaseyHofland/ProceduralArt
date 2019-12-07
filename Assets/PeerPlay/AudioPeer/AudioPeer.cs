//Copyright (c) 2019 Peter Olthof, Peer Play
//http://www.peerplay.nl, info @ peerplay.nl 
//--------------------------------------------
//This script can be used in commercial and non-commercial software
//Please credit either Peer Play, or Peter Olthof in the final product

using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

[RequireComponent (typeof (AudioSource))]
public class AudioPeer : MonoBehaviour {
	AudioSource _audioSource;

    //Microphone input
    public AudioClip _audioClip;
    public bool _useMicrophone;
    public string _selectedDevice;

	//FFT values
	private float[] _samplesLeft = new float[512];
	private float[] _samplesRight = new float[512];

	private float[] _freqBand = new float[8];
	private float[] _bandBuffer = new float[8];
	private float[] _bufferDecrease = new float[8];
	private float[] _freqBandHighest = new float[8];

	//audio band values
	[HideInInspector]
	public float[] _audioBand, _audioBandBuffer;


	//Amplitude variables
	[HideInInspector]
	public float _Amplitude, _AmplitudeBuffer;
	private float _AmplitudeHighest;

	//stereo channels
	public enum _channel {Stereo, Left, Right};
	public _channel channel = new _channel ();


    //Audio64
    float[] _freqBand64 = new float[64];
	float[] _bandBuffer64 = new float[64];
	float[] _bufferDecrease64 = new float[64];
	float[] _freqBandHighest64 = new float[64];
	//audio band64 values
	[HideInInspector]
	public float[] _audioBand64, _audioBandBuffer64;


    // Use this for initialization
    void Start ()
    {
        _audioBand = new float[8];
		_audioBandBuffer = new float[8];
		_audioBand64 = new float[64];
		_audioBandBuffer64 = new float[64];
		_audioSource = GetComponent<AudioSource> ();
		AudioProfile (0.5f);


        //Microphone input

        if (_useMicrophone)
        {
            if (Microphone.devices.Length > 0)
            {
                _selectedDevice = Microphone.devices[0].ToString();
                _audioSource.clip = Microphone.Start(_selectedDevice, true, 1, AudioSettings.outputSampleRate);
                while (Microphone.GetPosition(_selectedDevice) <= 0)
                {
                    System.Threading.Thread.Sleep(8);
                }

            }
            else
            {
                _useMicrophone = false;
            }
        }
        if (!_useMicrophone)
        {
            _audioSource.clip = _audioClip;
        }

        _audioSource.Play();
     //   _audioSource.time += 110;
    }

	// Update is called once per frame
	void Update ()
    {

        if (_audioSource.clip != null)
        {
            GetSpectrumAudioSource();
            MakeFrequencyBands();
            MakeFrequencyBands64();
            BandBuffer();
            BandBuffer64();
            CreateAudioBands();
            CreateAudioBands64();
            GetAmplitude();

        }

    }


    void AudioProfile(float audioProfile)
	{
		for (int i = 0; i < 8; i++) {
			_freqBandHighest [i] = audioProfile;
		}
        for (int i = 0; i < 64; i++)
        {
            _freqBandHighest64[i] = audioProfile;
        }
        _AmplitudeHighest = audioProfile;
    }

	void GetAmplitude()
	{
		float _CurrentAmplitude = 0;
		float _CurrentAmplitudeBuffer = 0;
		for (int i = 0; i < 8; i++) {
			_CurrentAmplitude += _audioBand [i];
			_CurrentAmplitudeBuffer += _audioBandBuffer [i];
		}
		if (_CurrentAmplitude > _AmplitudeHighest) {
			_AmplitudeHighest = _CurrentAmplitude;
		}
		_Amplitude = _CurrentAmplitude / _AmplitudeHighest;
		_AmplitudeBuffer = _CurrentAmplitudeBuffer / _AmplitudeHighest;
	}

	void CreateAudioBands()
	{
		for (int i = 0; i < 8; i++) 
		{
			if (_freqBand [i] > _freqBandHighest [i]) {
				_freqBandHighest [i] = _freqBand [i];
			}
			_audioBand [i] = Mathf.Clamp((_freqBand [i] / _freqBandHighest [i]), 0, 1);
			_audioBandBuffer [i] = Mathf.Clamp((_bandBuffer [i] / _freqBandHighest [i]), 0, 1);
		}
	}

	void CreateAudioBands64()
	{
		for (int i = 0; i < 64; i++) 
		{
			if (_freqBand64 [i] > _freqBandHighest64 [i]) {
				_freqBandHighest64 [i] = _freqBand64 [i];
			}
			_audioBand64 [i] = Mathf.Clamp((_freqBand64 [i] / _freqBandHighest64 [i]), 0, 1);
			_audioBandBuffer64 [i] = Mathf.Clamp((_bandBuffer64 [i] / _freqBandHighest64 [i]), 0, 1);
		}
	}

	void GetSpectrumAudioSource()
	{
		_audioSource.GetSpectrumData(_samplesLeft, 0, FFTWindow.Hanning);
		_audioSource.GetSpectrumData(_samplesRight, 1, FFTWindow.Hanning);
	}


	void BandBuffer()
	{
		for (int g = 0; g < 8; ++g) {
			if (_freqBand [g] > _bandBuffer [g]) {
				_bandBuffer [g] = _freqBand [g];
				//_bufferDecrease [g] = 0.005f;
			}

			if ((_freqBand [g] < _bandBuffer [g]) && (_freqBand [g] > 0)) {
                _bufferDecrease[g] = (_bandBuffer[g] - _freqBand[g]) / 8;
                _bandBuffer[g] -= _bufferDecrease[g];

			}

		}
	}

    private void OnAudioFilterRead(float[] data, int channels)
    {
        if (_useMicrophone) System.Array.Clear(data, 0, data.Length);
    }

    void BandBuffer64()
	{
		for (int g = 0; g < 64; ++g) {
			if (_freqBand64 [g] > _bandBuffer64 [g]) {
				_bandBuffer64 [g] = _freqBand64 [g];
			}

			if ((_freqBand64 [g] < _bandBuffer64 [g]) && (_freqBand64 [g] > 0)) {
                _bufferDecrease64[g] = (_bandBuffer64[g] - _freqBand64[g]) / 8;
                _bandBuffer64[g] -= _bufferDecrease64 [g];
			}

		}
	}

	void MakeFrequencyBands()
	{
		int count = 0;

		for (int i = 0; i < 8; i++) {


			float average = 0;
			int sampleCount = (int)Mathf.Pow (2, i) * 2;

			if (i == 7) {
				sampleCount += 2;
			}
			for (int j = 0; j < sampleCount; j++) {
				if (channel == _channel.Stereo) {
					average += (_samplesLeft [count] + _samplesRight [count]) * (count + 1);
				}
				if (channel == _channel.Left) {
					average += _samplesLeft [count] * (count + 1);
				}
				if (channel == _channel.Right) {
					average += _samplesRight [count] * (count + 1);
				}
				count++;

			}

			average /= count;

			_freqBand [i] = average * 10;

		}
	}
	void MakeFrequencyBands64()
	{
			int count = 0;
			int sampleCount = 1;
			int power = 0;
			for (int i = 0; i < 64; i++) {
				float average = 0;

				if (i == 16 || i == 32 || i == 40 || i == 48 || i == 56) {
					sampleCount = (int)Mathf.Pow (2, power);
					if (power == 3) {
						sampleCount -= 2;
					}
					power++;
				}

				for (int j = 0; j < sampleCount; j++) {
					if (channel == _channel.Stereo) {
						average += (_samplesLeft [count] + _samplesRight [count]) * (count + 1);
					}
					if (channel == _channel.Left) {
						average += _samplesLeft [count] * (count + 1);
					}
					if (channel == _channel.Right) {
						average += _samplesRight [count] * (count + 1);
					}
					count++;

				}

				average /= count;
				_freqBand64 [i] = average * 80;
			}
	}
}