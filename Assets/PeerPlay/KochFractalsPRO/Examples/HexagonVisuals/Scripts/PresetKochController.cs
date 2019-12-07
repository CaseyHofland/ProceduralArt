using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresetKochController : MonoBehaviour {
    public SpheresOnAmplitude _spheresOnAmplitude;
    public RotateAround _rotateCam;
    public GameObject _innerLines, _outterlines, _trailbendedjump, _trailaround, _trailfractalinside, _trailbendedjumpOutside;
	// Use this for initialization
	void Start () {
        _rotateCam._rotateAxis = new Vector3(0, 0, 0);
        _spheresOnAmplitude.state = SpheresOnAmplitude._state.Off;
        _innerLines.SetActive(false);
        _outterlines.SetActive(false);
        _trailbendedjump.SetActive(false);
        _trailaround.SetActive(false);
        _trailfractalinside.SetActive(false);
        _trailbendedjumpOutside.SetActive(false);


    }
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _rotateCam._rotateAxis = new Vector3(0, 0, 0);
            _spheresOnAmplitude.state = SpheresOnAmplitude._state.Off;
            _innerLines.SetActive(true);
            _outterlines.SetActive(false);
            _trailbendedjump.SetActive(false);
            _trailaround.SetActive(false);
            _trailfractalinside.SetActive(false);
            _trailbendedjumpOutside.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _rotateCam._rotateAxis = new Vector3(0, 0, 0);
            _spheresOnAmplitude.state = SpheresOnAmplitude._state.Off;
            _innerLines.SetActive(true);
            _outterlines.SetActive(true);
            _trailbendedjump.SetActive(false);
            _trailaround.SetActive(false);
            _trailfractalinside.SetActive(false);
            _trailbendedjumpOutside.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _rotateCam._rotateAxis = new Vector3(0, 0, 0);
            _spheresOnAmplitude.state = SpheresOnAmplitude._state.OnAmplitude;
            _innerLines.SetActive(true);
            _outterlines.SetActive(true);
            _trailbendedjump.SetActive(false);
            _trailaround.SetActive(false);
            _trailfractalinside.SetActive(false);
            _trailbendedjumpOutside.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            _rotateCam._rotateAxis = new Vector3(0, 1f, 0);
            _spheresOnAmplitude.state = SpheresOnAmplitude._state.OnAmplitude;
            _innerLines.SetActive(true);
            _outterlines.SetActive(true);
            _trailbendedjump.SetActive(false);
            _trailaround.SetActive(false);
            _trailfractalinside.SetActive(false);
            _trailbendedjumpOutside.SetActive(false);
        }


        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            _rotateCam._rotateAxis = new Vector3(0, 1f, 0);
            _spheresOnAmplitude.state = SpheresOnAmplitude._state.OnAmplitude;
            _innerLines.SetActive(true);
            _outterlines.SetActive(true);
            _trailbendedjump.SetActive(true);
            _trailaround.SetActive(false);
            _trailfractalinside.SetActive(false);
            _trailbendedjumpOutside.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            _rotateCam._rotateAxis = new Vector3(0, -4, 0);
            _rotateCam._rotateAxis = new Vector3(0, 1f, 0);
            _spheresOnAmplitude.state = SpheresOnAmplitude._state.OnAmplitude;
            _innerLines.SetActive(true);
            _outterlines.SetActive(true);
            _trailbendedjump.SetActive(false);
            _trailaround.SetActive(true);
            _trailfractalinside.SetActive(false);

        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            _rotateCam._rotateAxis = new Vector3(0, 1f, 0);
            _spheresOnAmplitude.state = SpheresOnAmplitude._state.OnAmplitude;
            _innerLines.SetActive(true);
            _outterlines.SetActive(true);
            _trailbendedjump.SetActive(false);
            _trailaround.SetActive(false);
            _trailfractalinside.SetActive(true);
            _trailbendedjumpOutside.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            _rotateCam._rotateAxis = new Vector3(0, 1f, 0);
            _spheresOnAmplitude.state = SpheresOnAmplitude._state.OnAmplitude;
            _innerLines.SetActive(true);
            _outterlines.SetActive(true);
            _trailbendedjump.SetActive(true);
            _trailaround.SetActive(false);
            _trailfractalinside.SetActive(false);
            _trailbendedjumpOutside.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            _rotateCam._rotateAxis = new Vector3(0, 1f, 0);
            _spheresOnAmplitude.state = SpheresOnAmplitude._state.Off;
            _innerLines.SetActive(true);
            _outterlines.SetActive(false);
            _trailbendedjump.SetActive(true);
            _trailaround.SetActive(false);
            _trailfractalinside.SetActive(false);
            _trailbendedjumpOutside.SetActive(true);
        }

    } 
}
