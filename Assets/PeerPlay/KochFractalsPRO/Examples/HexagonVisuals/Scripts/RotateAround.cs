using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAround : MonoBehaviour {
    public AudioPeer _audioPeer;
    public Vector3 _rotateAxis;
    public Vector3 _rotateSpeed;

    float _rotateResultx, _rotateResulty, _rotateResultz;
    public bool _left;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        _rotateResultx = _rotateAxis.x * _rotateSpeed.x * Time.deltaTime * _audioPeer._audioBandBuffer[1];
        _rotateResulty = _rotateAxis.y * _rotateSpeed.y * Time.deltaTime * _audioPeer._audioBandBuffer[1];
        _rotateResultz = _rotateAxis.z * _rotateSpeed.z * Time.deltaTime * _audioPeer._audioBandBuffer[5];

        if (_left)
        {
            this.transform.Rotate(_rotateResultx, _rotateResulty, _rotateResultz);
        }
        if (!_left)
        {
            this.transform.Rotate(-_rotateResultx, -_rotateResulty, -_rotateResultz);
        }
    }
}
