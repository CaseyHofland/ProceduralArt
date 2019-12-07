//Copyright (c) 2018 Peter Olthof, Peer Play
//http://www.peerplay.nl, info @ peerplay.nl 
//--------------------------------------------
//This script can be used in commercial and non-commercial software
//Please credit either Peer Play, or Peter Olthof in the final product

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(KochTrail)), CanEditMultipleObjects]
public class KochTrailEditor : Editor
{
    GUIStyle boxStyle;

    SerializedObject _kochTrail;
    SerializedProperty _audioPeer, _audioBand;
    SerializedProperty _axis;
    SerializedProperty _initiator;
    SerializedProperty _generator;
    SerializedProperty _startGen;
    SerializedProperty _useBezierCurves, _bezierVertexCount, _initiatorSize;
    SerializedProperty _trailMaterial, _trailColor;
    SerializedProperty _colorName, _colorMultiplier;
    SerializedProperty _trailWidthCurve, _trailEndCapVertices;
    SerializedProperty _speedMinMax, _widthMinMax, _trailTimeMinMax;
    SerializedProperty _audioBandSpeed;
    SerializedProperty _audioSpeedSetting, _audioColorSetting, _audioTrailWidthSetting, _audioTrailTimeSetting;
    SerializedProperty _speedOnAudio, _widthOnAudio, _timeOnAudio, _colorOnAudio;
    SerializedProperty _fixedSpeed, _fixedWidth, _fixedTime;

    bool _setupFoldout, _kochFoldout, _trailFoldout, _audioFoldout;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (boxStyle == null) { boxStyle = GUI.skin.FindStyle("box"); }

        EditorGUILayout.Separator();

        EditorGUILayout.LabelField("Koch Fractals Trail Pro v1.0", EditorStyles.largeLabel, GUILayout.Height(20));
        EditorGUILayout.LabelField("Peer Play", EditorStyles.largeLabel, GUILayout.Height(20));
        EditorGUILayout.Separator();


        if (GUILayout.Button("Setup", EditorStyles.toolbarDropDown)) { _setupFoldout = !_setupFoldout; }
        if (_setupFoldout)
        {
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(_audioPeer, new GUIContent("AudioPeer", "Select the AudioPeer object"));
            EditorGUILayout.PropertyField(_trailMaterial, new GUIContent("Material", "Select the material for the trail renderers"));
            EditorGUILayout.PropertyField(_trailColor, new GUIContent("Color", "Specify the colours of the trails. Gradient is divided by the amount of initiator points"));
            EditorGUILayout.PropertyField(_colorName, new GUIContent("Color Name", "The name of the color property in the shader of the selected material"));
        }

        EditorGUILayout.Separator();
        if (GUILayout.Button("Koch", EditorStyles.toolbarDropDown)) { _kochFoldout = !_kochFoldout; }

        if (_kochFoldout)
        {
            EditorGUILayout.Separator();
            if (!Application.isPlaying)
            {
                EditorGUILayout.PropertyField(_axis, new GUIContent("Axis", "Select on which axis the initiator points are drawn"));
                EditorGUILayout.PropertyField(_initiator, new GUIContent("Initiator", "Select the initiator points to start with"));
                EditorGUILayout.PropertyField(_initiatorSize, new GUIContent("Initiator Scale", "The scale of the initiator"));
                EditorGUILayout.PropertyField(_generator, new GUIContent("Generator", "Draw points in between the start/end point, to specify the recursive generator on each segment"));
                EditorGUILayout.PropertyField(_startGen, true);
                EditorGUILayout.PropertyField(_useBezierCurves, new GUIContent("Use Bezier Curves", "If selected, lines will be drawn as bezier curves."));
                if (_useBezierCurves.boolValue)
                {
                    EditorGUILayout.PropertyField(_bezierVertexCount, new GUIContent("Vertex Count", "The amount of points each bezier curve consists out of. Higher amount is more smooth, but takes more memory"));
                }
            } 
        }

        EditorGUILayout.Separator();

        if (GUILayout.Button("Trail", EditorStyles.toolbarDropDown)) { _trailFoldout = !_trailFoldout; }
        if (_trailFoldout)
        {
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(_trailWidthCurve, new GUIContent("Width Curve", "Specify the curve along which the width of the trail is shown"));
            EditorGUILayout.PropertyField(_trailEndCapVertices, new GUIContent("End Cap Vertices", "The amount of end cap vertices of the trail"));
        }


        if (GUILayout.Button("Audio", EditorStyles.toolbarDropDown)) { _audioFoldout = !_audioFoldout; }

        if (_audioFoldout)
        {
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(_audioBand, true);

            EditorGUILayout.PropertyField(_speedOnAudio, new GUIContent("Speed On Audio", "Select to either increase/decrease speed of the trail on audio, or use a fixed speed"));
            if (_speedOnAudio.boolValue)
            {
                EditorGUILayout.PropertyField(_audioSpeedSetting, new GUIContent("Frequency", "The audio frequency on which the trails increase their speed"));
                if (_audioSpeedSetting.enumValueIndex == 0 || _audioSpeedSetting.enumValueIndex == 1)
                {
                    EditorGUILayout.PropertyField(_audioBandSpeed, new GUIContent("Audio Band", "Select specific audio band"));
                }
                EditorGUILayout.PropertyField(_speedMinMax, new GUIContent("Min/Max", "Select the minimum and maximum speed, in between which the trails move, based on audio"));
            }
            else
            {
                EditorGUILayout.PropertyField(_fixedSpeed, new GUIContent("Fixed Speed", "Set the fixed speed of the trails"));
            }

            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(_widthOnAudio, new GUIContent("Width On Audio", "Select to either increase/decrease width of the trail on audio, or use a fixed width"));
            if (_widthOnAudio.boolValue)
            {
                EditorGUILayout.PropertyField(_audioTrailWidthSetting, new GUIContent("Frequency", "The audio frequency on which the trails increase their width"));
                EditorGUILayout.PropertyField(_widthMinMax, new GUIContent("Min/Max", "Select the minimum and maximum width of the trails, based on audio"));
            }
            else
            {
                EditorGUILayout.PropertyField(_fixedWidth, new GUIContent("Fixed Width", "Set the fixed width of the trails"));
            }

            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(_timeOnAudio, new GUIContent("Time On Audio", "Select to either increase/decrease the lifetime of the trail on audio, or use a fixed time"));
            if (_timeOnAudio.boolValue)
            {
                EditorGUILayout.PropertyField(_audioTrailTimeSetting, new GUIContent("Frequency", "The audio frequency on which the trail lifetime increases or decreases"));
                EditorGUILayout.PropertyField(_trailTimeMinMax, new GUIContent("Min/Max", "Select the minimum and maximum time of the trails, based on audio"));
            }
            else
            {
                EditorGUILayout.PropertyField(_fixedTime, new GUIContent("Fixed Time", "Set the fixed lifetime of the trails"));
            }

            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(_colorOnAudio, new GUIContent("Color On Audio", "Select to either change the color of the trails on audio, or use a fixed color"));
            if (_colorOnAudio.boolValue)
            {
                EditorGUILayout.PropertyField(_audioColorSetting, new GUIContent("Frequency", "The audio frequency on which the colors of the trails change"));
            }
                EditorGUILayout.PropertyField(_colorMultiplier, new GUIContent("Multiplier", "Multiplying the color strength"));

        }



        for (int i = 0; i < targets.Length; i++)

        {
            serializedObject.ApplyModifiedProperties();
        }
        serializedObject.ApplyModifiedProperties();
    }

    public void OnEnable()
    {
        Initialize();

    }

    public void Initialize()
    {
        _setupFoldout = true;
        _kochFoldout = true;
        _trailFoldout = true;
        _audioFoldout = true;

        _axis = serializedObject.FindProperty("axis");
        _initiator = serializedObject.FindProperty("initiator");
        _generator = serializedObject.FindProperty("_generator");
        _startGen = serializedObject.FindProperty("_startGen");
        _useBezierCurves = serializedObject.FindProperty("_useBezierCurves");
        _bezierVertexCount = serializedObject.FindProperty("_bezierVertexCount");
        _initiatorSize = serializedObject.FindProperty("_initiatorSize");
        _audioPeer = serializedObject.FindProperty("_audioPeer");
        _trailMaterial = serializedObject.FindProperty("_trailMaterial");
        _trailColor = serializedObject.FindProperty("_trailColor");
        _audioBand = serializedObject.FindProperty("_audioBand");
        _colorName = serializedObject.FindProperty("_colorName");
        _trailWidthCurve = serializedObject.FindProperty("_trailWidthCurve");
        _trailEndCapVertices = serializedObject.FindProperty("_trailEndCapVertices");

        _speedMinMax = serializedObject.FindProperty("_speedMinMax");
        _widthMinMax = serializedObject.FindProperty("_widthMinMax");
        _trailTimeMinMax = serializedObject.FindProperty("_trailTimeMinMax");
        _audioBandSpeed = serializedObject.FindProperty("_audioBandSpeed");
        _colorMultiplier = serializedObject.FindProperty("_colorMultiplier");

        _audioSpeedSetting = serializedObject.FindProperty("audioSpeedSetting");
        _audioColorSetting = serializedObject.FindProperty("audioColorSetting");
        _audioTrailWidthSetting = serializedObject.FindProperty("audioTrailWidthSetting");
        _audioTrailTimeSetting = serializedObject.FindProperty("audioTrailTimeSetting");

        _speedOnAudio = serializedObject.FindProperty("_speedOnAudio");
        _widthOnAudio = serializedObject.FindProperty("_widthOnAudio");
        _timeOnAudio = serializedObject.FindProperty("_timeOnAudio");
        _colorOnAudio = serializedObject.FindProperty("_colorOnAudio");
        _fixedSpeed = serializedObject.FindProperty("_fixedSpeed");
        _fixedWidth = serializedObject.FindProperty("_fixedWidth");
        _fixedTime = serializedObject.FindProperty("_fixedTime");

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