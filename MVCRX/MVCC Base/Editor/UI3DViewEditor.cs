/*
Jefferson Scomacao
https://www.jscomacao.com

GitHub - Source Code
Project: MVCC 3.0 (MVCRX)

Unity3D MVC Framework for Unity2019 and higher

Copyright (c) 2015 Jefferson Raulino Scomação

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MVCC;
[CustomEditor(typeof(UI3DView), true)]
public class UI3DViewEditor : Editor
{

    UI3DView _instance;

    SerializedProperty controllerId;
    SerializedProperty navAnimate;

    void OnEnable()
    {
        _instance = target as UI3DView;

        controllerId = serializedObject.FindProperty("controllerId");
        navAnimate = serializedObject.FindProperty("navAnimate");
        CheckParent();

        var checkNavAnimate = _instance.GetComponent<Nav3DAnimate>();
        if (checkNavAnimate == null)
        {
            var a = _instance.gameObject.AddComponent<Nav3DAnimate>();
            _instance.navAnimate = a;
        }

    }

    void CheckParent()
    {
        var p = _instance.transform.parent;

        while(p != null)
        {
            var src = p.GetComponent<UIViewControllerBase>();
            if (src != null)
            {
                controllerId.longValue = src.controllerId;
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
                return;
            }

            p = p.transform.parent;
        }

        controllerId.longValue = -1;
        serializedObject.ApplyModifiedProperties();
        serializedObject.Update();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.LabelField("Controller ID:", controllerId.longValue.ToString());
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(navAnimate);
        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Custom Fields:");
        DrawDefaultInspector();

    }
}

