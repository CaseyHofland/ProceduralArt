//Copyright (c) 2018 Peter Olthof, Peer Play
//http://www.peerplay.nl, info @ peerplay.nl 
//--------------------------------------------
//This script can be used in commercial and non-commercial software
//Please credit either Peer Play, or Peter Olthof in the final product

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class KochLine : KochGenerator
{
    LineRenderer _lineRenderer;
    Vector3[] _lerpPosition;
    private float[] _lerpAudio;

    
    public AudioPeer _audioPeer;
    [Range(0, 7)]
    public int[] _audioBand;
    public Material _material;

    [Header("Color")]
    public bool _colorOnAudio;

    public Color _color;
    private Material _matInstance;
    public string _colorName;
    [Range(0, 7)]
    public int _audioBandMaterial;
    [Range(1f,4f)]
    public float _emissionMultiplier;

    public float _lineWidth;

    [Header("Position")]
    public bool _linePosOnAudio;

    [Range(0f,1f)]
    public float _linePosSlider;

    [Header("Width")]
    public bool _lineWidthOnAudio;

    [MinMaxSlider(0f,10f)]
    public Vector2 _lineWidthMinMax;
    [Range(0,7)]
    public int _audioBandWidth;

    public bool _useBufferOnColor;


    protected enum _audioLerpPosSetting { Band, BandBuffer, Amplitude, AmplitudeBuffer };
    [SerializeField]
    protected _audioLerpPosSetting audioLerpPosSetting = new _audioLerpPosSetting();

    protected enum _audioColorSetting { Band, BandBuffer, Amplitude, AmplitudeBuffer };
    [SerializeField]
    protected _audioColorSetting audioColorSetting = new _audioColorSetting();

    protected enum _audioWidthSetting { Band, BandBuffer, Amplitude, AmplitudeBuffer };
    [SerializeField]
    protected _audioWidthSetting audioWidthSetting = new _audioWidthSetting();


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

    // Use this for initialization
    void Start()
    {
        _lerpAudio = new float[_initiatorPointAmount];
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.enabled = true;
        _lineRenderer.useWorldSpace = false;
        _lineRenderer.loop = true;
        _lineRenderer.widthMultiplier = _lineWidth;
        _lineRenderer.positionCount = _position.Length;
        _lineRenderer.SetPositions(_position);
        _lerpPosition = new Vector3[_position.Length];
        //apply material
        _matInstance = new Material(_material);
        _matInstance.SetColor(_colorName, _color * _emissionMultiplier);
        _lineRenderer.material = _matInstance;

    }

    // Update is called once per frame
    void Update()
    {
        if (_colorOnAudio)
        {
                _matInstance.SetColor(_colorName, _color * _audioColorFloat(_audioBandMaterial) * _emissionMultiplier);
        }
        if (!_colorOnAudio)
        {
            _matInstance.SetColor(_colorName, _color * _emissionMultiplier);
        }
        if (_lineWidthOnAudio)
        {
            _lineRenderer.widthMultiplier = Mathf.Lerp(_lineWidthMinMax.x, _lineWidthMinMax.y, _audioWidthFloat(_audioBandWidth));
        }
        if (!_lineWidthOnAudio)
        {
            _lineRenderer.widthMultiplier = _lineWidth;
        }


            if (_generationCount != 0)
        {
            int count = 0;
            for (int i = 0; i < _initiatorPointAmount; i++)
            {
                _lerpAudio[i] = _audioLerpPosFloat(_audioBand[i]);
                for (int j = 0; j < (_position.Length - 1) / _initiatorPointAmount; j++)
                {
                    if (_linePosOnAudio)
                    {
                        _lerpPosition[count] = Vector3.Lerp(_position[count], _targetPosition[count], _lerpAudio[i]);
                    }
                    else
                    {
                        _lerpPosition[count] = Vector3.Lerp(_position[count], _targetPosition[count], _linePosSlider);
                    }
                    count++;
                }
            }
            if (_linePosOnAudio)
            {
                _lerpPosition[count] = Vector3.Lerp(_position[count], _targetPosition[count], _lerpAudio[_initiatorPointAmount - 1]);
            }
            if (!_linePosOnAudio)
            {
                _lerpPosition[count] = Vector3.Lerp(_position[count], _targetPosition[count], _linePosSlider);
            }

                if (_useBezierCurves)
            {
                _bezierPosition = BezierCurve(_lerpPosition, _bezierVertexCount);
                _lineRenderer.positionCount = _bezierPosition.Length;
                _lineRenderer.SetPositions(_bezierPosition);
            }
            else
            {
                _lineRenderer.positionCount = _lerpPosition.Length;
                _lineRenderer.SetPositions(_lerpPosition);
            }

        }

    }

    float _audioLerpPosFloat(int band)
    {
        float audiolerpPos;
        switch (audioLerpPosSetting)
        {
            case _audioLerpPosSetting.Band:
                audiolerpPos = _audioPeer._audioBand[band];
                break;
            case _audioLerpPosSetting.BandBuffer:
                audiolerpPos = _audioPeer._audioBandBuffer[band];
                break;
            case _audioLerpPosSetting.Amplitude:
                audiolerpPos = _audioPeer._Amplitude;
                break;
            case _audioLerpPosSetting.AmplitudeBuffer:
                audiolerpPos = _audioPeer._AmplitudeBuffer;
                break;
            default:
                audiolerpPos = _audioPeer._Amplitude;
                break;
        }
        return audiolerpPos;

    }
    float _audioColorFloat(int band)
    {
        float audiocolor;
        switch (audioColorSetting)
        {
            case _audioColorSetting.Band:

                audiocolor = _audioPeer._audioBand[band];
                break;
            case _audioColorSetting.BandBuffer:
                audiocolor = _audioPeer._audioBandBuffer[band];
                break;
            case _audioColorSetting.Amplitude:
                audiocolor = _audioPeer._Amplitude;
                break;
            case _audioColorSetting.AmplitudeBuffer:
                audiocolor = _audioPeer._AmplitudeBuffer;
                break;
            default:
                audiocolor = _audioPeer._Amplitude;
                break;
        }
        return audiocolor;

    }
    float _audioWidthFloat(int band)
    {
        float audiocolor;
        switch (audioWidthSetting)
        {
            case _audioWidthSetting.Band:

                audiocolor = _audioPeer._audioBand[band];
                break;
            case _audioWidthSetting.BandBuffer:
                audiocolor = _audioPeer._audioBandBuffer[band];
                break;
            case _audioWidthSetting.Amplitude:
                audiocolor = _audioPeer._Amplitude;
                break;
            case _audioWidthSetting.AmplitudeBuffer:
                audiocolor = _audioPeer._AmplitudeBuffer;
                break;
            default:
                audiocolor = _audioPeer._Amplitude;
                break;
        }
        return audiocolor;

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