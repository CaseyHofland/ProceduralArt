//Copyright (c) 2018 Peter Olthof, Peer Play
//http://www.peerplay.nl, info @ peerplay.nl 
//--------------------------------------------
//This script can be used in commercial and non-commercial software
//Please credit either Peer Play, or Peter Olthof in the final product

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KochTrail : KochGenerator {

    public class TrailObject
    {
        public GameObject GO { get; set; }
        public TrailRenderer Trail { get; set; }
        public int CurrentTargetNum { get; set; }
        public Vector3 TargetPosition { get; set; }
        public Color EmissionColor { get; set; }
    }

    [HideInInspector]
    public List<TrailObject> _trail;

    [Header("Trail Properties")]
    public AnimationCurve _trailWidthCurve;
    [Range(0,8)]
    public int _trailEndCapVertices;
    public Material _trailMaterial;
    public Gradient _trailColor;
    public Color[] _trailColorArray;
    public string _colorName;

    [Header("Audio")]
    public AudioPeer _audioPeer;
    [Range(0, 7)]
    public int[] _audioBand;
    [MinMaxSlider(0f, 2000f)]
    public Vector2 _speedMinMax;
    [MinMaxSlider(0f, 10f)]
    public Vector2 _widthMinMax;
    [MinMaxSlider(0f, 10f)]
    public Vector2 _trailTimeMinMax;
    [Range(1f,4f)]
    public float _colorMultiplier;
    [Range(0,7)]
    public int _audioBandSpeed;

    [Header("Speed")]
    public bool _speedOnAudio;

    [Header("Width")]
    public bool _widthOnAudio;

    [Header("Time")]
    public bool _timeOnAudio;

    [Header("Color")]
    public bool _colorOnAudio;

    [Range(0f,4000f)]
    public float _fixedSpeed;
    [Range(0f, 50f)]
    public float _fixedWidth;
    [Range(0f, 20f)]
    public float _fixedTime;

    //Private Variables
    private float _lerpPosSpeed;
    private float _distanceSnap;
    private Color _startColor;

    protected enum _audioSpeedSetting { Band, BandBuffer, Amplitude, AmplitudeBuffer };
    [SerializeField]
    protected _audioSpeedSetting audioSpeedSetting = new _audioSpeedSetting();

    protected enum _audioColorSetting { Band, BandBuffer, Amplitude, AmplitudeBuffer };
    [SerializeField]
    protected _audioColorSetting audioColorSetting = new _audioColorSetting();

    protected enum _audioTrailWidthSetting { Band, BandBuffer, Amplitude, AmplitudeBuffer };
    [SerializeField]
    protected _audioTrailWidthSetting audioTrailWidthSetting = new _audioTrailWidthSetting();

    protected enum _audioTrailTimeSetting { Band, BandBuffer, Amplitude, AmplitudeBuffer };
    [SerializeField]
    protected _audioTrailTimeSetting audioTrailTimeSetting = new _audioTrailTimeSetting();






    // Use this for initialization
    void Start () {
        _startColor = new Color(0, 0, 0, 0);
        _trailColorArray = new Color[8];
        _trail = new List<TrailObject>();
        for (int a = 0; a < 8; a++)
        {
            _trailColorArray[a] = _trailColor.Evaluate(a * 0.125f);
        }
        for (int i = 0; i < _initiatorPointAmount; i ++)
        {
            GameObject trailInstance = new GameObject("KochTrail" + i.ToString());
            trailInstance.transform.parent = this.transform;
            trailInstance.AddComponent<TrailRenderer>();
            TrailObject trailObjectInstance = new TrailObject();
            trailObjectInstance.GO = trailInstance;
            trailObjectInstance.Trail = trailInstance.GetComponent<TrailRenderer>();
            trailObjectInstance.Trail.material = new Material(_trailMaterial);
            trailObjectInstance.EmissionColor = _trailColor.Evaluate(i * (1.0f / _initiatorPointAmount));
            trailObjectInstance.Trail.numCapVertices = _trailEndCapVertices;
            trailObjectInstance.Trail.widthCurve = _trailWidthCurve;

            

            Vector3 instantiatePosition;

            if (_generationCount > 0)
            {
                int step;
                if (_useBezierCurves)
                {
                    step = _bezierPosition.Length / _initiatorPointAmount;
                    instantiatePosition = _bezierPosition[i * step];
                    trailObjectInstance.CurrentTargetNum = (i * step) + 1;
                    trailObjectInstance.TargetPosition = _bezierPosition[trailObjectInstance.CurrentTargetNum];
                }
                else
                {
                    step = _position.Length / _initiatorPointAmount;
                    instantiatePosition = _position[i * step];
                    trailObjectInstance.CurrentTargetNum = (i * step) + 1;
                    trailObjectInstance.TargetPosition = _position[trailObjectInstance.CurrentTargetNum];

                }
            }
            else
            {
                instantiatePosition = _position[i];
                trailObjectInstance.CurrentTargetNum = i + 1;
                trailObjectInstance.TargetPosition = _position[trailObjectInstance.CurrentTargetNum];
            }

            trailObjectInstance.GO.transform.localPosition = instantiatePosition;
            _trail.Add(trailObjectInstance);



        }
	}
	
    void Movement()
    {
        if (_speedOnAudio)
        {
            _lerpPosSpeed = Mathf.Lerp(_speedMinMax.x, _speedMinMax.y, _audioSpeedFloat(_audioBandSpeed));
        }
        if (!_speedOnAudio)
        {
            _lerpPosSpeed = _fixedSpeed;
        }
        for (int i = 0; i < _trail.Count; i ++)
        {
            
            _distanceSnap = Vector3.Distance(_trail[i].GO.transform.localPosition, _trail[i].TargetPosition);

            if (_distanceSnap < 0.05f)
            {
                _trail[i].GO.transform.localPosition = _trail[i].TargetPosition;
                if (_useBezierCurves && _generationCount > 0)
                {
                    if (_trail[i].CurrentTargetNum < _bezierPosition.Length - 1)
                    {
                        _trail[i].CurrentTargetNum += 1;
                    }
                    else
                    {
                        _trail[i].CurrentTargetNum = 1;
                    }
                    _trail[i].TargetPosition = _bezierPosition[_trail[i].CurrentTargetNum];
                }
                else
                {
                    if (_trail[i].CurrentTargetNum < _position.Length - 1)
                    {
                        _trail[i].CurrentTargetNum += 1;
                    }
                    else
                    {
                        _trail[i].CurrentTargetNum = 1;
                    }
                    _trail[i].TargetPosition = _targetPosition[_trail[i].CurrentTargetNum];
                }
            }
            _trail[i].GO.transform.localPosition = Vector3.MoveTowards(_trail[i].GO.transform.localPosition, _trail[i].TargetPosition, Time.deltaTime * _lerpPosSpeed);
        }

    }

    void AudioBehaviour()
    {
        for (int i = 0; i < _initiatorPointAmount; i ++)
        {
            Color colorLerp = new Color(1,1,1,1);
            if (_colorOnAudio)
            {
                colorLerp = Color.Lerp(_startColor, _trailColorArray[_audioBand[i]] * _colorMultiplier, _audioColorFloat(_audioBand[i]));
            }
            if (!_colorOnAudio)
            {
                colorLerp = _trailColorArray[_audioBand[i]] * _colorMultiplier;
            }
                _trail[i].Trail.material.SetColor(_colorName, colorLerp);

            float widthLerp = 0f;
            if (_widthOnAudio)
            {
                widthLerp = Mathf.Lerp(_widthMinMax.x, _widthMinMax.y, _audioTrailWidthFloat(_audioBand[i]));
            }
            if (!_widthOnAudio)
            {
                widthLerp = _fixedWidth;
            }

            _trail[i].Trail.widthMultiplier = widthLerp;

            float timeLerp = 0f;
            if (_timeOnAudio)
            {
                timeLerp = Mathf.Lerp(_trailTimeMinMax.x, _trailTimeMinMax.y, _audioTrailTimeFloat(_audioBand[i]));
            }
            if (!_timeOnAudio)
            {
                timeLerp = _fixedTime;
            }

                _trail[i].Trail.time = timeLerp;

        }
    }

	// Update is called once per frame
	void Update () {
        Movement();
        AudioBehaviour();
	}

    private void OnValidate()
    {
        switch (initiator)
        {
            case _initiator.Triangle:
                if (_audioBand.Length != 3)
                {
                    _audioBand = new int[3];
                }
                break;
            case _initiator.Square:
                if (_audioBand.Length != 4)
                {
                    _audioBand = new int[4];
                }
                break;
            case _initiator.Pentagon:
                if (_audioBand.Length != 5)
                {
                    _audioBand = new int[5];
                }
                break;
            case _initiator.Hexagon:
                if (_audioBand.Length != 6)
                {
                    _audioBand = new int[6];
                }
                break;
            case _initiator.Heptagon:
                if (_audioBand.Length != 7)
                {
                    _audioBand = new int[7];
                }
                break;
            case _initiator.Octagon:
                if (_audioBand.Length != 8)
                {
                    _audioBand = new int[8];
                }
                break;
            default:
                if (_audioBand.Length != 3)
                {
                    _audioBand = new int[3];
                }
                break;
        };
    }

    float _audioSpeedFloat(int band)
    {
        float audioResult;
        switch (audioSpeedSetting)
        {
            case _audioSpeedSetting.Band:
                audioResult = _audioPeer._audioBand[band];
                break;
            case _audioSpeedSetting.BandBuffer:
                audioResult = _audioPeer._audioBandBuffer[band];
                break;
            case _audioSpeedSetting.Amplitude:
                audioResult = _audioPeer._Amplitude;
                break;
            case _audioSpeedSetting.AmplitudeBuffer:
                audioResult = _audioPeer._AmplitudeBuffer;
                break;
            default:
                audioResult = _audioPeer._Amplitude;
                break;
        }
        return audioResult;
    }
    float _audioColorFloat(int band)
    {
        float audioResult;
        switch (audioColorSetting)
        {
            case _audioColorSetting.Band:
                audioResult = _audioPeer._audioBand[band];
                break;
            case _audioColorSetting.BandBuffer:
                audioResult = _audioPeer._audioBandBuffer[band];
                break;
            case _audioColorSetting.Amplitude:
                audioResult = _audioPeer._Amplitude;
                break;
            case _audioColorSetting.AmplitudeBuffer:
                audioResult = _audioPeer._AmplitudeBuffer;
                break;
            default:
                audioResult = _audioPeer._Amplitude;
                break;
        }
        return audioResult;
    }
    float _audioTrailWidthFloat(int band)
    {
        float audioResult;
        switch (audioTrailWidthSetting)
        {
            case _audioTrailWidthSetting.Band:
                audioResult = _audioPeer._audioBand[band];
                break;
            case _audioTrailWidthSetting.BandBuffer:
                audioResult = _audioPeer._audioBandBuffer[band];
                break;
            case _audioTrailWidthSetting.Amplitude:
                audioResult = _audioPeer._Amplitude;
                break;
            case _audioTrailWidthSetting.AmplitudeBuffer:
                audioResult = _audioPeer._AmplitudeBuffer;
                break;
            default:
                audioResult = _audioPeer._Amplitude;
                break;
        }
        return audioResult;
    }
    float _audioTrailTimeFloat(int band)
    {
        float audioResult;
        switch (audioTrailTimeSetting)
        {
            case _audioTrailTimeSetting.Band:
                audioResult = _audioPeer._audioBand[band];
                break;
            case _audioTrailTimeSetting.BandBuffer:
                audioResult = _audioPeer._audioBandBuffer[band];
                break;
            case _audioTrailTimeSetting.Amplitude:
                audioResult = _audioPeer._Amplitude;
                break;
            case _audioTrailTimeSetting.AmplitudeBuffer:
                audioResult = _audioPeer._AmplitudeBuffer;
                break;
            default:
                audioResult = _audioPeer._Amplitude;
                break;
        }
        return audioResult;
    }
}
//
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
