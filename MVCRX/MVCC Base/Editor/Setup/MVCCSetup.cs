/*
Jefferson Scomacao
https://www.jscomacao.com

GitHub - Source Code
Project: MVCC 3.0 (MVCRX)

Unity3D MVC Framework for Unity2019 and higher

Based on Previous work available here:
- https://github.com/jeffscm/mvcrx
- https://github.com/jeffscm/mvcc2
- https://github.com/jeffscm/mvccunity

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
	public class MVCCSetup
	{
        private int _selected = -1;
        private int _selectedParam = -1;
        private int _selectedNamespace = -1;

        private string[] _options, _paramOptions;
        private string[] _namespacesOptions;
        private Type[] _types;

        private static string _currentProject = string.Empty;
		private string _newProject = string.Empty;        

		private static bool _setupNewProjectAfterCompiling = false;
		private bool _hasDefaultAnim = true;
		private bool _hasDefaultHttp = true;
		private bool _startWithViewController = true;

		private int _instructionPage = -1;
		private Texture2D _iconLogo;

        private BuildTargetGroup _selectedBuild;
        private Vector2 _scrollPos;

        private MVCCValues _mvccValues;

        private int _counterTest = 0;

        public void Setup(string newProject)
		{
            _currentProject = EditorUtil.GetCurrentMVCC();
            _types = EditorUtil.GetAllEntities().ToArray();
			_options = _types.Select(x => x.Name).ToArray();
			_paramOptions = EditorUtil.GetAllParamEntities().Select(x => x.Name).ToArray();

            var asm = EditorUtil.GetProjectAssembly();
            _namespacesOptions = asm.GetTypes()
                         .Select(t => t.Namespace).Where(s => s != null && s.StartsWith(_currentProject + ".", StringComparison.Ordinal))
                         .Distinct().ToArray();

            
            var temp = EditorUtil.GetCurrentData().GetKey();
            if (temp == "TRUE")
            {
                _setupNewProjectAfterCompiling = true;
                EditorUtil.GetCurrentData().SetKey(string.Empty);

			}
            _mvccValues = EditorUtil.GetCurrentData();
        }

		public bool Display(EditorWindow context)
		{
			if (_iconLogo == null)
				_iconLogo = Resources.Load("mvccicon") as Texture2D;

			GUILayout.Box(_iconLogo, GUILayout.MaxWidth(500));

			if (EditorApplication.isUpdating || EditorApplication.isCompiling)
			{
				EditorGUILayout.LabelField("Wait for it...");
				return false;
			}

			if (_instructionPage > 0)
			{
				EditorGUILayout.LabelField("New project create - Next Steps");
				GUILayout.Space(20);

				switch (_instructionPage)
				{
					case 3:
						GUILayout.Label($"You have setup your MVCC project.\n\n Your project is inside folder {_currentProject}");
						break;
					case 2:
						GUILayout.Label("You have setup your MVCC project.\n\n Next Step: Create your scene and add mvcc prefab");
						break;
					case 1:
						GUILayout.Label("Next panel will let you create new Events");
						break;
				}

				GUILayout.Space(20);

				if (GUILayout.Button((_instructionPage == 1) ? "Finish" : "Next"))
				{
					_instructionPage--;
				}
				return false;
			}

            _counterTest++;

            if (_counterTest > 50)
            {
                _counterTest = 0;
                _mvccValues = EditorUtil.GetCurrentData();
                if (_mvccValues.GetKey() == "TRUE")
                {
                    _setupNewProjectAfterCompiling = true;
                    _mvccValues.SetKey(string.Empty);
                }
			}

            if (_setupNewProjectAfterCompiling)
			{
				_setupNewProjectAfterCompiling = false;
				AssetDatabase.Refresh();
				string outputFolder = Application.dataPath + "/" + _currentProject;
                
                var obj = new GameObject();
				obj.name = "MVCC - " + _currentProject;

                var asm = EditorUtil.GetProjectAssembly();
                var newType = asm.GetType("MVCC.Controller.DefaultStart");
				var src = obj.AddComponent(newType);
				(src as MVCC.AppMonoController).controllerType = MVCC.CONTROLLER_TYPE.ALL;
				obj.AddComponent<AudioSource>(); //default sound system
				PrefabUtility.SaveAsPrefabAssetAndConnect(obj, outputFolder + "/Resources/mvcc.prefab", InteractionMode.UserAction);
                
                MVCCLog.Log("<color=green>MVCC RX Project Setup Success...</color>");
                UnityEngine.Object.DestroyImmediate(obj);

                ScriptableObjectUtility.CreateAsset("SoundComponent", "Assets/" + _currentProject + "/Resources/SoundComponent.asset");

                _instructionPage = 3;
				return false;
			}

			if (string.IsNullOrEmpty(_currentProject))
			{
				_newProject = EditorGUILayout.TextField("MVCC project name", _newProject);
				GUILayout.Space(10);
				_hasDefaultAnim = GUILayout.Toggle(_hasDefaultAnim, "Add Default Animation");
				GUILayout.Space(5);
				_hasDefaultHttp = GUILayout.Toggle(_hasDefaultHttp, "Add Default Http Service");
				GUILayout.Space(5);
				_startWithViewController = GUILayout.Toggle(_startWithViewController, "Start With ViewController");
				GUILayout.Space(5);

                if (_mvccValues == null)
                    _mvccValues = EditorUtil.GetCurrentData();

                //_selectedBuild = (BuildTargetGroup)EditorGUILayout.EnumPopup("Platform:" , _selectedBuild);
                //if (GUILayout.Button("Add Platform"))
                //{
                //    if (_selectedBuild != BuildTargetGroup.Unknown)
                //    {
                //        if (!_mvccValues.supportedPlatforms.Contains(_selectedBuild))
                //        {
                //            _mvccValues.supportedPlatforms.Add(_selectedBuild);
                //        }
                //    }
                //}
                //GUILayout.Space(10);
                //EditorGUILayout.LabelField("Selected Platforms");
                //GUILayout.Space(10);
                //_scrollPos = GUILayout.BeginScrollView(_scrollPos);

                //int deleteItem = -1;

                //for(int i = 0; i < _mvccValues.supportedPlatforms.Count; i++)
                //{
                //    if (GUILayout.Button(_mvccValues.supportedPlatforms[i].ToString()))
                //    {
                //        deleteItem = i;
                //    }
                //}

                //if (_mvccValues.supportedPlatforms.Count == 0)
                //{
                //    GUILayout.Space(10);
                //    EditorGUILayout.LabelField("No Platforms added");
                //    GUILayout.Space(10);
                //    GUILayout.EndScrollView();
                //    return false;
                //}

                //if (deleteItem > -1)
                //{
                //    _mvccValues.supportedPlatforms.RemoveAt(deleteItem);
                //}

                //GUILayout.EndScrollView();

                GUILayout.Space(10);
                if (GUILayout.Button("Generate MVCC Project"))
				{
					string outputFolder = Application.dataPath + "/" + _newProject;

					EditorUtil.CreateDir(outputFolder);
					EditorUtil.CreateDir(outputFolder + "/Definitions");
                    EditorUtil.CreateDir(outputFolder + "/Resources");
                    EditorUtil.CreateDir(outputFolder + "/Scripts");
					EditorUtil.CreateDir(outputFolder + "/Scripts/Controllers");
					EditorUtil.CreateDir(outputFolder + "/Scripts/Views");
                    EditorUtil.CreateDir(outputFolder + "/Scripts/Models");

                    string pathSource = "Packages/com.mvcrx.main/MVCRX/MVCC Base/Editor/Setup/Source/";

					string content = File.ReadAllText(pathSource + ((_startWithViewController) ? "AppView.txt" : "App.txt"));
					content = content.Replace("%NAMESPACE%", _newProject);
					if (_hasDefaultAnim)
					{
						EditorUtil.ReplaceNameSpace(pathSource + "Anim.txt", outputFolder + "/Definitions/DefaultAnim.cs", _newProject);
						content = content.Replace("%ADDANIM%", "MVCCStart.RegisterAnim(new DefaultAnim());");
					}
					else
					{
						content = content.Replace("%ADDANIM%", string.Empty);
					}
                    if (_hasDefaultHttp)
					{
						EditorUtil.ReplaceNameSpace(pathSource + "Http.txt", outputFolder + "/Definitions/DefaultHttpHelper.cs", _newProject);

						content = content.Replace("%ADDHTTPHELPER%", "MVCCStart.RegisterHttpHelper(new DefaultHttpHelper());");
					}
					else
					{
						content = content.Replace("%ADDHTTPHELPER%", string.Empty);
					}
                    File.WriteAllText(outputFolder + "/Definitions/DefaultStart.cs", content);
					EditorUtil.ReplaceNameSpace(pathSource + "Constants.txt", outputFolder + "/Definitions/Constants.cs", _newProject);
					if (_startWithViewController)
					{
						EditorUtil.ReplaceNameSpace(pathSource + "DefaultNavController.txt", outputFolder + "/Scripts/Controllers/DefaultNavController.cs", _newProject);
					}
					else
					{
						EditorUtil.ReplaceNameSpace(pathSource + "DefaultController.txt", outputFolder + "/Scripts/Controllers/DefaultController.cs", _newProject);
					}
                    _currentProject = _newProject;
                    if (_mvccValues == null)
                        _mvccValues = EditorUtil.GetCurrentData();

                    _mvccValues.SetKey("TRUE");
					_mvccValues.SetupNewProject(_currentProject);
                    

					AssetDatabase.Refresh();
					return true;
				}
				return false;
			}

			// When we have the project
			EditorGUILayout.LabelField("MVCC Project", _currentProject);
			GUILayout.Space(10);

			EditorGUILayout.LabelField("Available Events");
			GUILayout.Space(5);
			_selected = EditorGUILayout.Popup("INotify", _selected, _options);
			GUILayout.Space(5);
			_selectedParam = EditorGUILayout.Popup("INotifyParam", _selectedParam, _paramOptions);
            GUILayout.Space(5);
            _selectedNamespace = EditorGUILayout.Popup("Namespaces", _selectedNamespace, _namespacesOptions);
            GUILayout.Space(10);

            EditorUtil.DrawUILine(Color.gray);
            GUILayout.Space(10);
            if (GUILayout.Button("Show instructions"))
			{
				_instructionPage = 3;
			}
            GUILayout.Space(10);
            EditorUtil.DrawUILine(Color.gray);

            context.Repaint();

			return false;
		}

	}

}