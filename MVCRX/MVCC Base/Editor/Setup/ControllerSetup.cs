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
using System;
using System.Linq;
using System.IO;
using System.Reflection;

namespace MVCC.Editor
{
    public class ControllerSetup
    {
        static string currentProject = string.Empty;
        private string _newViewName = string.Empty;
        private string _newViewNavName = string.Empty;
        private string _newSCName = string.Empty;
        private bool _addViewToObject = true;
        private bool _addControllerToObject = true;

        private bool _doAddControllerToObject;
        private bool _doAddViewToObject;
        private string _currentAsset = string.Empty;

        static string outputFolder;
        static string pathSource;

        public void Setup(string newProject)
        {
            currentProject = EditorUtil.GetCurrentMVCC();
            outputFolder = Application.dataPath + "/" + currentProject + "/Scripts/";
            pathSource = "Packages/com.mvcrx.main/MVCRX/MVCC Base/Editor/Setup/Source/";

            var scControllerCreate = PlayerPrefs.GetString("MVCC_SCC", "");
            if (!string.IsNullOrEmpty(scControllerCreate))
            {
                _currentAsset = scControllerCreate;
            }
        }

        public void Display(EditorWindow context)
        {

			if (EditorApplication.isUpdating || EditorApplication.isCompiling)
            {
                EditorGUILayout.LabelField("Wait for it...");
                return;
            }

            if (string.IsNullOrEmpty(currentProject))
            {
                EditorGUILayout.LabelField("You dont have a MVCC project setup yet.");
                return;
            }

            if (!EditorApplication.isCompiling)
            {
                if (!string.IsNullOrEmpty(_currentAsset))
                {
                    ScriptableObjectUtility.CreateAsset(_currentAsset, "Assets/" + currentProject + "/Scripts/Controllers/" + _currentAsset + ".asset");
                    _currentAsset = string.Empty;
                }
            }

            if (_doAddControllerToObject)
            {
                _doAddControllerToObject = false;
                AssetDatabase.Refresh();
                var temp = Selection.activeGameObject;
                Assembly asm = typeof(MVCC.App).Assembly;
                var newType = asm.GetType(currentProject + ".Controller." + _newViewNavName);
                var src = temp.AddComponent(newType);
                (src as MVCC.AppMonoController).controllerType = CONTROLLER_TYPE.NAV;
                var r = (temp.transform as RectTransform);
                r.anchorMin = Vector2.zero;
                r.anchorMax = Vector2.one;
                r.sizeDelta = Vector2.zero;
                temp.name = _newViewNavName;
                _newViewNavName = string.Empty;
                return;
            }

            if (_doAddViewToObject)
            {
                _doAddViewToObject = false; 
                AssetDatabase.Refresh();
                var temp = Selection.activeGameObject;
                Assembly asm = typeof(MVCC.App).Assembly;
                MVCCLog.Log(currentProject + ".Controller." + _newViewName);
                var newType = asm.GetType(currentProject + ".Controller." + _newViewName);
                var src = temp.AddComponent(newType);
                (src as MVCC.AppMonoController).controllerType = CONTROLLER_TYPE.ALL;
                _newViewName = string.Empty;
                return;
            }
			GUILayout.Space(20);

			EditorGUILayout.LabelField("View Controller:");
            GUILayout.Space(10);
            _addControllerToObject = GUILayout.Toggle(_addControllerToObject, "Add to selected gameObject");
            _newViewNavName = EditorGUILayout.TextField("New ViewController Name", _newViewNavName);
            if (GUILayout.Button("Generate New ViewController Script"))
            {
                string content = File.ReadAllText(pathSource + "ViewController.txt");
                content = content.Replace("%NAMESPACE%", currentProject);
                content = content.Replace("%VIEWCONTROLLER%", _newViewNavName);
                content = content.Replace("%EXTRANAMESPACE%", string.Empty);
                EditorUtil.WriteData(outputFolder + "Controllers/", _newViewNavName + ".cs", content);
                if (_addControllerToObject && Selection.activeGameObject != null)
                {
                    _doAddControllerToObject = true;
                }
                AssetDatabase.Refresh();
            }
            EditorUtil.DrawUILine(Color.gray);
            GUILayout.Space(20);
            EditorGUILayout.LabelField("Mono Controller:");
            GUILayout.Space(10);
            _addViewToObject = GUILayout.Toggle(_addViewToObject, "Add to selected gameObject");
            _newViewName = EditorGUILayout.TextField("New Controller Name", _newViewName);
            if (GUILayout.Button("Generate New Mono Controller Script"))
            {
                string content = File.ReadAllText(pathSource + "Controller.txt");
                content = content.Replace("%NAMESPACE%", currentProject);
                content = content.Replace("%CONTROLLER%", _newViewName);
                content = content.Replace("%EXTRANAMESPACE%", string.Empty);
                
                EditorUtil.WriteData(outputFolder + "Controllers/", _newViewName + ".cs", content);
                if (_addViewToObject && Selection.activeGameObject != null)
                {
                    _doAddViewToObject = true;
                }
                AssetDatabase.Refresh();
            }
            EditorUtil.DrawUILine(Color.gray);
            GUILayout.Space(20);
            EditorGUILayout.LabelField("SC Controller:");
            GUILayout.Space(10);
            _newSCName = EditorGUILayout.TextField("New SC Controller Name", _newSCName);
            if (GUILayout.Button("Generate New SC Controller Script and Asset"))
            {
                string content = File.ReadAllText(pathSource + "CSController.txt");
                content = content.Replace("%NAMESPACE%", currentProject);
                content = content.Replace("%CONTROLLER%", _newSCName);
                content = content.Replace("%EXTRANAMESPACE%", string.Empty);
                EditorUtil.WriteData(outputFolder + "Controllers/", _newSCName + ".cs", content);
                PlayerPrefs.SetString("MVCC_SCC", _newSCName);
                AssetDatabase.Refresh();
            }

        }
    }
}