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
    public class StateSetup
    {

        private string[] _options, _paramOptions;

        private string _currentAsset = string.Empty;
        Type[] _types;

        private static string currentProject = string.Empty;
        private string _newStateName = string.Empty;
        private string _newParamName = string.Empty;
        
        private bool _addDefaultIParam = true;
		private bool _addDefaultUIParam = true;
		private bool _addDefaultUIState = true;
		

		private int _selected;
		private int _selectedParam;
		private bool _runSetup = false;

		static string outputFolder;
        static string pathSource;

        public void Setup(string newProject)
        {

            currentProject = EditorUtil.GetCurrentMVCC();

            outputFolder = Application.dataPath + "/" + currentProject + "/Scripts/";
            pathSource = "Packages/com.mvcrx.main/MVCRX/MVCC Base/Editor/Setup/Source/";

            _types = EditorUtil.GetAllEntities().ToArray();
            _options = _types.Select(x => x.Name).ToArray();
			_paramOptions = EditorUtil.GetAllParamEntities().Select(x => x.Name).ToArray();

            var temp = PlayerPrefs.GetString("MVCC_UISTATE", "");
            if (temp != "")
            {
                _currentAsset = temp;
                PlayerPrefs.DeleteKey("MVCC_UISTATE");
            }
        }

        public void Display(EditorWindow context)
        {
			
			if (EditorApplication.isUpdating || EditorApplication.isCompiling)
            {
                EditorGUILayout.LabelField("Wait for it...");
				_runSetup = true;
				return;
            }

            if (_runSetup)
			{
				Setup(currentProject);
				_runSetup = false;
			}

            if (string.IsNullOrEmpty(currentProject))
            {
                EditorGUILayout.LabelField("You dont have a MVCC project setup yet.");
                Setup(string.Empty);
                return;
            }
			GUILayout.Space(20);

			EditorGUILayout.LabelField("INotify:");
            GUILayout.Space(10);
            _addDefaultIParam = GUILayout.Toggle(_addDefaultIParam, "Add default INotifyParam interface");
			_addDefaultUIState = GUILayout.Toggle(_addDefaultUIState, "Add UI Scriptable Object");
			_newStateName = EditorGUILayout.TextField("New INotify Name", _newStateName);

            if (GUILayout.Button("Create New INotify"))
            {
                _newStateName = EditorUtil.CamelToPascalCase(_newStateName);
                string content = File.ReadAllText(pathSource + "NewState.txt");
                content = content.Replace("%NAMESPACE%", currentProject);
                content = content.Replace("%NEWSTATE%", _newStateName);
                
                if (_addDefaultIParam)
                {
                    var contentParam = File.ReadAllText(pathSource + "NewParam.txt");
					contentParam = contentParam.Replace("%NAMESPACE%", currentProject);
					contentParam = contentParam.Replace("%NEWSTATE%", _newStateName);
                    EditorUtil.WriteData(outputFolder + "Events/", _newStateName + "Param.cs", contentParam);

					content = content.Replace("%NEWPARAM%", $"{_newStateName}Param {EditorUtil.PascalToCamelCase(_newStateName)}Param");
				}
                else
				{
					content = content.Replace("%NEWPARAM%", string.Empty);
				}
				EditorUtil.WriteData(outputFolder + "Events/", "I" + _newStateName + ".cs", content);

                if (_addDefaultUIState)
                {
                    CreateUIState(_newStateName, (_addDefaultIParam) ? _newStateName + "Param" : string.Empty);
                }

				AssetDatabase.Refresh();
            }

            EditorUtil.DrawUILine(Color.grey);
            GUILayout.Space(20);

			EditorGUILayout.LabelField("Create Notify Param Only:");
            GUILayout.Space(10);
            _newParamName = EditorGUILayout.TextField("New Param Name", _newParamName);
            if (GUILayout.Button("Generate New INotifyParam"))
            {
                _newParamName = EditorUtil.CamelToPascalCase(_newParamName);
                string content = File.ReadAllText(pathSource + "NewParam.txt");
                content = content.Replace("%NAMESPACE%", currentProject);
                content = content.Replace("%NEWSTATE%", _newParamName);
                EditorUtil.WriteData(outputFolder + "Events/", _newParamName + "Param.cs", content);          
                AssetDatabase.Refresh();
            }
            EditorUtil.DrawUILine(Color.grey);

            GUILayout.Space(20);

			EditorGUILayout.LabelField("UI Events");

			_selected = EditorGUILayout.Popup("INotify Available:", _selected, _options);

			_addDefaultUIParam = GUILayout.Toggle(_addDefaultUIParam, "Add IParam to UI State");
			if (_addDefaultUIParam)
			{
				_selectedParam = EditorGUILayout.Popup("INotifyParam Available:", _selectedParam, _paramOptions);
			}
            else
			{
				_selectedParam = -1;
			}
			
			if (!EditorApplication.isCompiling)
			{
				if (!string.IsNullOrEmpty(_currentAsset))
				{
					if (!ScriptableObjectUtility.CreateAsset(_currentAsset, "Assets/" + currentProject + "/Scripts/Events/UIEvents/" + _currentAsset + ".asset"))
                    {
                        PlayerPrefs.DeleteKey("MVCC_UISTATE");
                    }
					_currentAsset = string.Empty;
				}
			}

            GUILayout.Space(20);

            if (GUILayout.Button("Add Notify to UI elements:"))
			{
				if (_selected != -1)
				{
                    CreateUIState(_types[_selected].Name.Substring(1, _types[_selected].Name.Length - 1), _paramOptions[_selectedParam]);
				}
			}

            EditorUtil.DrawUILine(Color.grey);
        }

        void CreateUIState(string newState, string newParam)
        {

            _currentAsset = newState;
            var newfile = File.ReadAllText(pathSource + "NewMonoState.txt");
            newfile = newfile.Replace("%NAMESPACE%", currentProject);
            newfile = newfile.Replace("%NEWMONO%", _currentAsset);
            newfile = newfile.Replace("%NEWSTATE%", "I" + _currentAsset);

            if (!string.IsNullOrEmpty(newParam))
            {
                newfile = newfile.Replace("%NEWPARAMDECL%", $"public {newParam} {EditorUtil.PascalToCamelCase(newParam)};");
                newfile = newfile.Replace("%NEWPARAM%", EditorUtil.PascalToCamelCase(newParam));
            }
            else
            {
                newfile = newfile.Replace("%NEWPARAMDECL%", string.Empty);
                newfile = newfile.Replace("%NEWPARAM%", string.Empty);
            }
            EditorUtil.CreateDir(outputFolder + "Events/UIEvents/");
            string path = outputFolder + "Events/UIEvents/" + _currentAsset + ".cs";
            File.WriteAllText(path, newfile);
            AssetDatabase.Refresh();
            PlayerPrefs.SetString("MVCC_UISTATE", _currentAsset);
        }

    }

	public static class ScriptableObjectUtility
	{
		public static bool CreateAsset(string newInstance, string pathAsset)
		{
            if (!string.IsNullOrEmpty(newInstance))
            {
                Debug.Log($"New AssetState: {newInstance}");
                var asset = ScriptableObject.CreateInstance(newInstance);
                if (asset != null)
                {
                    AssetDatabase.CreateAsset(asset, pathAsset);

                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    EditorUtility.FocusProjectWindow();
                    Selection.activeObject = asset;
                    return true;
                }                
            }
            return false;
		}
	}
}