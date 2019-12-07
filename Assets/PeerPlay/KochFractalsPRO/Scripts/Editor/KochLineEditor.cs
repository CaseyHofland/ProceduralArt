//Copyright (c) 2018 Peter Olthof, Peer Play
//http://www.peerplay.nl, info @ peerplay.nl 
//--------------------------------------------
//This script can be used in commercial and non-commercial software
//Please credit either Peer Play, or Peter Olthof in the final product

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(KochLine)), CanEditMultipleObjects]
public class KochLineEditor : Editor
{
    GUIStyle boxStyle;

    SerializedObject _kochLine;
    SerializedProperty _audioPeer;
    SerializedProperty _axis;
    SerializedProperty _initiator;
    SerializedProperty _generator;
    SerializedProperty _startGen;
    SerializedProperty _useBezierCurves, _bezierVertexCount, _initiatorSize;

    SerializedProperty _material;

    SerializedProperty _audioBand;
    SerializedProperty _colorOnAudio;
    SerializedProperty _color;
    SerializedProperty _colorName;
    SerializedProperty audioLerpPosSetting, audioColorSetting, audioWidthSetting;
    SerializedProperty _audioBandMaterial;
    SerializedProperty _emissionMultiplier;
    SerializedProperty _lineWidth, _lineWidthOnAudio, _lineWidthMinMax, _audioBandWidth;
    SerializedProperty _linePosOnAudio, _linePosSlider;

    bool _setupFoldout,_kochFoldout, _audioFoldout;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (boxStyle == null) { boxStyle = GUI.skin.FindStyle("box"); }

        EditorGUILayout.Separator();

        EditorGUILayout.LabelField("Koch Fractals Line Pro v1.0", EditorStyles.largeLabel, GUILayout.Height(20));
        EditorGUILayout.LabelField("Peer Play", EditorStyles.largeLabel, GUILayout.Height(20));
        EditorGUILayout.Separator();


        if (GUILayout.Button("Setup", EditorStyles.toolbarDropDown)) { _setupFoldout = !_setupFoldout; }
        if (_setupFoldout)
        {
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(_audioPeer, new GUIContent("AudioPeer", "Select the AudioPeer object"));
            EditorGUILayout.PropertyField(_material, new GUIContent("Material", "Select the material for the line renderer"));
            EditorGUILayout.PropertyField(_color, new GUIContent("Color", "Select color of the material"));
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
            }
            EditorGUILayout.PropertyField(_useBezierCurves, new GUIContent("Use Bezier Curves", "If selected, lines will be drawn as bezier curves."));
            if (_useBezierCurves.boolValue)
            {
                EditorGUILayout.PropertyField(_bezierVertexCount, new GUIContent("Vertex Count", "The amount of points each bezier curve consists out of. Higher amount is more smooth, but takes more memory"));
            }
            EditorGUILayout.PropertyField(_lineWidth, new GUIContent("Line Width", "Set a static width for the line renderer. Will be overrided, if using width on audio"));
        }

        EditorGUILayout.Separator();
        if (GUILayout.Button("Audio", EditorStyles.toolbarDropDown)) { _audioFoldout = !_audioFoldout; }

        if (_audioFoldout)
        {
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(_audioBand, true);

            EditorGUILayout.PropertyField(_linePosOnAudio, new GUIContent("Position On Audio", "Select to either lerp the positions of the line on audio, or set a fixed lerp amount"));
            if (_linePosOnAudio.boolValue)
            {
                EditorGUILayout.PropertyField(audioLerpPosSetting, new GUIContent("Frequency", "Select on which frequencies the position lerping of the lines behave"));
            }
            else
            {
                EditorGUILayout.PropertyField(_linePosSlider, new GUIContent("Lerp Percentage", "Set the fixed amount, to lerp the positions of the line"));
            }
           
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(_colorOnAudio, new GUIContent("Color On Audio", "Select to either change the color on audio, or use a static color"));
            if (_colorOnAudio.boolValue)
            {
                EditorGUILayout.PropertyField(audioColorSetting, new GUIContent("Frequency", "Select on which frequency the color is changing"));
                if (audioColorSetting.enumValueIndex == 0 || audioColorSetting.enumValueIndex == 1)
                {
                    EditorGUILayout.PropertyField(_audioBandMaterial, new GUIContent("Audio Band", "Select the specific audio band, to control the color"));
                }
                
            }
            EditorGUILayout.PropertyField(_emissionMultiplier, new GUIContent("Multiplier", "Multiplying the color, useful for emissive colors, otherwise keep at 1"));
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(_lineWidthOnAudio, new GUIContent("Width On Audio", "Select to either change the width on audio, or use a static value"));
            if (_lineWidthOnAudio.boolValue)
            {
                EditorGUILayout.PropertyField(audioWidthSetting, new GUIContent("Frequency", "Select on which frequencies the width of the line increases/decreases"));
                if (audioColorSetting.enumValueIndex == 0 || audioColorSetting.enumValueIndex == 1)
                {
                    EditorGUILayout.PropertyField(_audioBandWidth, new GUIContent("Audio Band", "Select the specific audio band, to control the width"));
                }
                EditorGUILayout.PropertyField(_lineWidthMinMax, new GUIContent("Min/Max", "Specify the minimum and maximum width of the line on audio"));
            }




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
        _audioFoldout = true;

        _axis = serializedObject.FindProperty("axis");
        _initiator = serializedObject.FindProperty("initiator");
        _generator = serializedObject.FindProperty("_generator");
        _startGen = serializedObject.FindProperty("_startGen");
        _useBezierCurves = serializedObject.FindProperty("_useBezierCurves");
        _bezierVertexCount = serializedObject.FindProperty("_bezierVertexCount");
        _initiatorSize = serializedObject.FindProperty("_initiatorSize");
        _audioPeer = serializedObject.FindProperty("_audioPeer");
        _material = serializedObject.FindProperty("_material");
        _audioBand = serializedObject.FindProperty("_audioBand");
        _color = serializedObject.FindProperty("_color");
        _colorOnAudio = serializedObject.FindProperty("_colorOnAudio");
        _colorName = serializedObject.FindProperty("_colorName");
        audioLerpPosSetting = serializedObject.FindProperty("audioLerpPosSetting");
        audioColorSetting = serializedObject.FindProperty("audioColorSetting");
        _audioBandMaterial = serializedObject.FindProperty("_audioBandMaterial");
        _emissionMultiplier = serializedObject.FindProperty("_emissionMultiplier");

        audioWidthSetting = serializedObject.FindProperty("audioWidthSetting");
        _lineWidth = serializedObject.FindProperty("_lineWidth");
        _lineWidthOnAudio = serializedObject.FindProperty("_lineWidthOnAudio");
        _lineWidthMinMax = serializedObject.FindProperty("_lineWidthMinMax");
        _audioBandWidth = serializedObject.FindProperty("_audioBandWidth");
        _linePosOnAudio = serializedObject.FindProperty("_linePosOnAudio");
        _linePosSlider = serializedObject.FindProperty("_linePosSlider");
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