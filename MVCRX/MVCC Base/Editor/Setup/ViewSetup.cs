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
    public class ViewSetup
    {

        static string currentProject = string.Empty;
        private string _newViewName = string.Empty;
        private string _newViewNavName = string.Empty;
		private string _extraNamespace = string.Empty;
		
		private bool _addViewToObject = true;
        private bool _addControllerToObject = true;

        private bool _doAddControllerToObject;
        private bool _doAddViewToObject;
		private bool _addExtraNamespace = false;
		private bool _runSetup = false;


        private string[] _namespacesOptions;
        private int _selectedNamespace;

		static string outputFolder;
        static string pathSource;

        private string _addViewControllerType = string.Empty;
        private string _addViewType = string.Empty;

        public void Setup(string newProject) 
        {
			currentProject = EditorUtil.GetCurrentMVCC();
            outputFolder = Application.dataPath + "/" + currentProject + "/Scripts/";
            pathSource = "Packages/com.mvcrx.main/MVCRX/MVCC Base/Editor/Setup/Source/";

            var asm = EditorUtil.GetProjectAssembly();

            _namespacesOptions = asm.GetTypes()
                         .Select(t => t.Namespace).Where(s => s != null && (s.StartsWith(currentProject + ".View", StringComparison.Ordinal) || s.EndsWith(currentProject, StringComparison.Ordinal)))
                         .Distinct().ToArray();

            var temp = PlayerPrefs.GetString("MVCC_ADD_VIEWCONTROLLER", "");
            if (temp != "")
            {
                _doAddControllerToObject = true;
                _addViewControllerType = temp;
                PlayerPrefs.DeleteKey("MVCC_ADD_VIEWCONTROLLER");
            }

            temp = PlayerPrefs.GetString("MVCC_ADD_VIEW", "");
            if (temp != "")
            {
                _doAddViewToObject = true;
                _addViewType = temp;
                PlayerPrefs.DeleteKey("MVCC_ADD_VIEW");
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

            if (string.IsNullOrEmpty(currentProject))
            {
                EditorGUILayout.LabelField("You dont have a MVCC project setup yet.");
                return;
            }

            if (_runSetup)
			{
				_runSetup = false;
				Setup(currentProject);
			}

            if (_doAddControllerToObject)
            {
                _doAddControllerToObject = false;
                AssetDatabase.Refresh();
                var temp = Selection.activeGameObject;
                Assembly asm = typeof(MVCC.App).Assembly;
                var newType = asm.GetType(_addViewControllerType);
                var src = temp.AddComponent(newType);
                temp.name = newType.Name;
                (src as MVCC.AppMonoController).controllerType = CONTROLLER_TYPE.NAV;
                var r = (temp.transform as RectTransform);
                r.anchorMin = Vector2.zero;
                r.anchorMax = Vector2.one;
                r.sizeDelta = Vector2.zero;

                _newViewNavName = string.Empty;
                return;
            }

            if (_doAddViewToObject)
            {
                _doAddViewToObject = false; 
                AssetDatabase.Refresh();

                Assembly asm = typeof(MVCC.App).Assembly;
                var newType = asm.GetType(_addViewType);

                var temp = Selection.activeGameObject;

                var hasViewController = temp.GetComponent<UIViewController>();
                if (hasViewController != null)
                {
                    var newChild = new GameObject(newType.Name);
                    newChild.transform.parent = Selection.activeGameObject.transform;
                    newChild.transform.localPosition = Vector3.zero;
                    newChild.transform.localScale = Vector3.one;
                    newChild.layer = Selection.activeGameObject.layer;
                    temp = newChild;
                    temp.AddComponent<RectTransform>();
                }
                
                temp.AddComponent(newType);
                temp.name = newType.Name;
                _newViewName = string.Empty;
                return;
            }
			GUILayout.Space(20);

			EditorGUILayout.LabelField("View Controller Options:");
            GUILayout.Space(10);
            _addControllerToObject = GUILayout.Toggle(_addControllerToObject, "Add to selected gameObject");
			_addExtraNamespace = GUILayout.Toggle(_addExtraNamespace, "Extra Namespace");
            if (_addExtraNamespace)
			{
				_extraNamespace = GUILayout.TextField("Extra Namespace:", _extraNamespace);
				GUILayout.Space(10);
			}
            else
			{
				_extraNamespace = string.Empty;
			}
			_newViewNavName = EditorGUILayout.TextField("New ViewController Name", _newViewNavName);
            if (GUILayout.Button("Generate New ViewController Script"))
            {
                string content = File.ReadAllText(pathSource + "ViewController.txt");
                content = content.Replace("%NAMESPACE%", currentProject);
				content = content.Replace("%EXTRANAMESPACE%", (string.IsNullOrEmpty(_extraNamespace) ? "" : "." + _extraNamespace));

				var folder = "Controllers/" + (string.IsNullOrEmpty(_extraNamespace) ? "" : _extraNamespace + "/");

				EditorUtil.CreateDir(outputFolder + "Controllers/");
                if (!string.IsNullOrEmpty(_extraNamespace))
				{
					EditorUtil.CreateDir(outputFolder + folder);
				}

				content = content.Replace("%VIEWCONTROLLER%", _newViewNavName);
                EditorUtil.WriteData(outputFolder + folder, _newViewNavName + ".cs", content);
                if (_addControllerToObject && Selection.activeGameObject != null)
                {
                    //_doAddControllerToObject = true;
                    PlayerPrefs.SetString("MVCC_ADD_VIEWCONTROLLER", currentProject + (string.IsNullOrEmpty(_extraNamespace) ? "" : "." + _extraNamespace) + ".Controller." + _newViewNavName);
                }

                AssetDatabase.Refresh();
            }
            GUILayout.Space(20);
            EditorGUILayout.LabelField("View Options:");
            GUILayout.Space(10);
            _addViewToObject = GUILayout.Toggle(_addViewToObject, "Add to selected gameObject");
            _newViewName = EditorGUILayout.TextField("New View Name", _newViewName);
            _selectedNamespace = EditorGUILayout.Popup("Namespace", _selectedNamespace, _namespacesOptions);
            if (GUILayout.Button("Generate New View Script") && _selectedNamespace != -1)
            {
                string content = File.ReadAllText(pathSource + "View.txt");
                content = content.Replace("%NAMESPACE%", _namespacesOptions[_selectedNamespace]);
                content = content.Replace("%VIEW%", _newViewName);
                EditorUtil.WriteData(outputFolder + "Views/", _newViewName + ".cs", content);
                if (_addViewToObject && Selection.activeGameObject != null)
                {
                    //_doAddViewToObject = true;
                    PlayerPrefs.SetString("MVCC_ADD_VIEW", _namespacesOptions[_selectedNamespace] + "." + _newViewName);
                }
                AssetDatabase.Refresh();
            }
        }
    }
}