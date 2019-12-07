using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SpheresOnAmplitude : MonoBehaviour {
    public AudioPeer _audioPeer;

    public enum _state
    {
        Off,
        OnAmplitude
    };
    public _state state = new _state();


    Transform[] _sphereTransform;
    Material[] _sphereMat;
    int _sphereCount;
    public Material _material;
    public Color _colorOff;
    public Gradient _colorGradient;
    private Color[] _colorOn;
    public float _emissionMultiplier;
    public float _threshold;
    // Use this for initialization
    void Start () {
        _sphereCount = transform.childCount;
        _sphereTransform = new Transform[_sphereCount];
        _sphereMat = new Material[_sphereCount];
        _colorOn = new Color[_sphereCount];

        for (int i = 0; i < _sphereCount; i++)
        {
            Material matInstance = new Material(_material);
            transform.GetChild(i).GetComponent<MeshRenderer>().material = matInstance;
            _colorOn[i] = _colorGradient.Evaluate(i * (1.0f / (_sphereCount - 1)));
            _sphereMat[i] = matInstance;
            _sphereTransform[i] = transform.GetChild(i);
        }

      
	}
	
	// Update is called once per frame
	void Update () {
            StateSelect();

    }

    void StateSelect()
    {
        switch(state)
        {
            case _state.Off:
                for (int i = 0; i < _sphereCount; i++)
                {
                    _sphereMat[i].SetColor("_EmissionColor", _colorOff);
                }
                break;

            case _state.OnAmplitude:
                for (int i = 0; i < _sphereCount; i++)
                {
                    if (_audioPeer._audioBandBuffer[i] > _threshold)
                    {
                        _sphereMat[i].SetColor("_EmissionColor", _colorOn[i] * _emissionMultiplier * _audioPeer._audioBandBuffer[i]);
                    }
                    else
                    {
                        _sphereMat[i].SetColor("_EmissionColor", _colorOff);
                    }
                }
                   
                break;

            default:
                for (int i = 0; i < _sphereCount; i++)
                {
                    _sphereMat[i].SetColor("_EmissionColor", _colorOff);
                }
                break;
        }
    }
}
