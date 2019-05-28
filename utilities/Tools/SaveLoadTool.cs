/*
Project DisCo (Discrete Choreography) (GPL) initiated by Jan Philipp Drude

This file is part of Project DisCo.

Copyright (c) 2019, Jan Philipp Drude <jpdrude@gmail.com>

A full build of Project DisCo is available at <http://www.project-disco.com>

Project DisCo's underlaying Source Code is free to use; you can 
redistribute it and/or modify it under the terms of the GNU 
General Public License as published by the Free Software Foundation; 
either version 3 of the License, or (at your option) any later version. 

The Project DisCo source code is distributed in the hope that it will 
be useful, but WITHOUT ANY WARRANTY; without even the implied warranty 
of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
with the DisCo-Classes repository; 
If not, see <http://www.gnu.org/licenses/>.

@license GPL-3.0 <https://www.gnu.org/licenses/gpl.html>

The Project DisCo base classes build on Wasp developed by Andrea Rossi.
You can find Wasp at: <https://github.com/ar0551/Wasp>

Significant parts of Project DisCo have been developed by Jan Philipp Drude
as part of research on virtual reality, digital materials and 
discrete design at: 
dMA - digital Methods in Architecture - Prof. Mirco Becker
Leibniz University Hannover
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using System;

public class SaveLoadTool : MonoBehaviour
{
    //local variables
    #region
    static SaveLoad offset = 0;


    [SerializeField]
    private List<TextMeshPro> textMeshes = new List<TextMeshPro>();
    private List<MeshRenderer> renderers = new List<MeshRenderer>();

    bool saving = false;
    bool loading = false;
    bool reallyLoad = false;
    bool reallyNew = false;

    string[] loadList;
    int loadListIndex = 0;

    Vector3 translateOffset = new Vector3(0.1f, 0, 0);

    float fontSize = 0.22f;
    #endregion


    //properties
    #region
    private static string savePath = "";
    public static string SavePath
    {
        get { return savePath; }
        set { savePath = value; }
    }
    #endregion


    //MonoBehaviour methods
    #region
    private void Start()
    {
        foreach (TextMeshPro tm in textMeshes)
        {
            renderers.Add(tm.gameObject.GetComponent<MeshRenderer>());
        }

        ChangeColor();

        if (savePath == "")
        {
            savePath = Application.dataPath + "/Resources/SaveGames/";
        }

        ToggleSelected(false);
    }
    #endregion


    //controller handling methods
    #region
    public void Initialize()
    {
        loading = false;
        reallyLoad = false;
        reallyNew = false;
        int i = 0;
        foreach (TextMeshPro tm in textMeshes)
        {
            tm.text = ((SaveLoad)i).ToString();
            tm.enableAutoSizing = false;
            tm.fontSize = fontSize;
            ++i;
        }
        ChangeColor();
    }


    public void ChangeTool(ToolsetControls.InputX input)
    {
        switch (input)
        {
            case ToolsetControls.InputX.Enter:
                {
                    switch (offset)
                    {
                        case SaveLoad.Save:
                            {
                                if (!saving)
                                {
                                    saving = true;
                                    string timeCode = SaveGame("Save_");
                                    StartCoroutine(ShowStringForDeltaTime(timeCode, textMeshes[(int)offset], 3));
                                }
                                break;
                            }
                        case SaveLoad.Load:
                            {
                                if (!loading)
                                {
                                    loadList = GetStoredFiles(SavePath);
                                    Array.Sort(loadList, StringComparer.InvariantCulture);
                                    Array.Reverse(loadList);
                                    loading = true;
                                    textMeshes[(int)SaveLoad.Save].text = "";
                                    textMeshes[(int)SaveLoad.New].text = "";
                                    textMeshes[(int)SaveLoad.Load].enableAutoSizing = true;
                                    if (loadList.Length > 0)
                                    {
                                        textMeshes[(int)SaveLoad.Load].text = loadList[0];
                                    }
                                    else
                                    {
                                        textMeshes[(int)SaveLoad.Load].text = "no Files found";
                                    }
                                }
                                else if (loading && !reallyLoad)
                                {
                                    if (loadList.Length == 0)
                                    {
                                        Initialize();
                                    }
                                    else
                                    {
                                        textMeshes[(int)SaveLoad.Load].text = "Really\nLoad?";
                                        reallyLoad = true;
                                    }
                                }
                                else if (reallyLoad)
                                {
                                    LoadGame(loadList[loadListIndex]);
                                    Initialize();
                                }
                                break;
                            }
                        case SaveLoad.New:
                            {
                                if (!reallyNew)
                                {
                                    reallyNew = true;
                                    textMeshes[(int)SaveLoad.New].text = "Really\nNew?";
                                }
                                else
                                {
                                    NewGame();
                                    Initialize();
                                }
                                break;
                            }
                    }
                    break;
                }
            case ToolsetControls.InputX.Left:
                {
                    if (offset > 0 && !loading)
                    {
                        --offset;
                        GameObject go = gameObject.transform.GetChild(0).gameObject;
                        go.transform.localPosition = go.transform.localPosition - translateOffset;
                        ChangeColor();
                    }
                    else if (loading && !reallyLoad)
                    {
                        if (loadListIndex > 0)
                        {
                            --loadListIndex;
                            textMeshes[(int)SaveLoad.Load].text = loadList[loadListIndex];
                        }
                        else
                        {
                            loadListIndex = loadList.Length - 1;
                            textMeshes[(int)SaveLoad.Load].text = loadList[loadListIndex];
                        }
                    }
                    else if (reallyLoad)
                    {
                        Initialize();
                    }
                    break;
                }
            case ToolsetControls.InputX.Right:
                {
                    if ((int)offset < textMeshes.Count - 1 && !loading)
                    {
                        ++offset;
                        GameObject go = gameObject.transform.GetChild(0).gameObject;
                        go.transform.localPosition = go.transform.localPosition + translateOffset;
                        ChangeColor();
                    }
                    else if (loading && !reallyLoad)
                    {
                        if (loadListIndex < loadList.Length - 1)
                        {
                            ++loadListIndex;
                            textMeshes[(int)SaveLoad.Load].text = loadList[loadListIndex];
                        }
                        else
                        {
                            loadListIndex = 0;
                            textMeshes[(int)SaveLoad.Load].text = loadList[loadListIndex];
                        }
                    }
                    else if (reallyLoad)
                    {
                        Initialize();
                    }
                }
                break;
        }
    }


    private void ChangeColor()
    {
        for (int i = 0; i < textMeshes.Count; ++i)
        {
            if (i == (int)offset)
            {
                textMeshes[i].faceColor = GlobalReferences.AffectedColor;
            }
            else
            {
                textMeshes[i].faceColor = GlobalReferences.UnaffectedColor;
            }
        }
    }

    public void ToggleSelected(bool selected)
    {
        if (!selected)
        {
            for (int i = 0; i < textMeshes.Count; ++i)
            {
                if (i != (int)offset)
                {
                    renderers[i].enabled = false;
                }
                else
                {
                    renderers[i].enabled = true;
                }
            }
        }
        else
        {
            for (int i = 0; i < textMeshes.Count; ++i)
            {
                renderers[i].enabled = true;
            }
        }
    }


    private static bool ContainsFile(string timeCode, string dirPath)
    {
        foreach (string s in GetStoredFiles(dirPath))
        {
            if (string.Compare(s, timeCode) == 0)
            {
                return true;
            }
        }
        return false;
    }


    private static string[] GetStoredFiles(string dirPath)
    {
        DirectoryInfo info = new DirectoryInfo(dirPath);
        FileInfo[] fileinfo = info.GetFiles("*.json");

        string[] filenames = new string[fileinfo.Length];
        int count = 0;

        foreach (FileInfo _fileInf in fileinfo)
        {
            filenames[count] = _fileInf.Name;
            count += 1;
        }

        return filenames;
    }
    #endregion


    //Save, Load and New methods
    #region
    public static string SaveGame(string prefix)
    {
        string year = (System.DateTime.Now.Year - 2000).ToString("D2");
        string month = System.DateTime.Now.Month.ToString("D2");
        string day = System.DateTime.Now.Day.ToString("D2");
        string hour = System.DateTime.Now.TimeOfDay.Hours.ToString("D2");
        string minute = System.DateTime.Now.TimeOfDay.Minutes.ToString("D2");

        string timeCode = prefix + year + month + day + "_" + hour + "-" + minute + ".json";
        string path = SavePath + "\\";

        int i = 2;
        while (ContainsFile(timeCode, path))
        {
            timeCode = prefix + year + month + day + "_" + hour + "-" + minute + "_" + i.ToString() + ".json";
            ++i;
        }

        path = path + timeCode;

        AssemblyIO.Save(GlobalReferences.FrozenParts, 10, path);

        return timeCode;
    }


    private static void LoadGame(string path)
    {
        Debug.Log(SavePath + "\\" + path);
        AssemblyIO.Load(SavePath + "\\" + path);
        GlobalReferences.RebuildIndices();
    }


    private static void NewGame()
    {
        List<GameObject> tempGos = new List<GameObject>();
        tempGos.AddRange(GlobalReferences.FreeParts);
        tempGos.AddRange(GlobalReferences.AffectedParts);
        
        foreach(GameObject go in GlobalReferences.FrozenParts.Values)
        {
            tempGos.Add(go);
        }

        GlobalReferences.FreeParts.Clear();
        GlobalReferences.FrozenParts.Clear();
        GlobalReferences.AffectedParts.Clear();
        GlobalReferences.NumOfParts = 0;

        for (int i = tempGos.Count - 1; i >= 0; --i)
        {
            Destroy(tempGos[i]);
        }

        PartsHolder.SpawnMultiple(PartsHolder.NumParts);
    }
    #endregion


    //Coroutines
    #region
    IEnumerator ShowStringForDeltaTime(string message, TextMeshPro textField, float deltaTime)
    {
        textField.enableAutoSizing = true;
        string saveOldMessage = textField.text;
        textField.text = message;
        yield return new WaitForSeconds(deltaTime);
        if (textField.text == message)
        {
            textField.text = saveOldMessage;
            textField.enableAutoSizing = false;
            textField.fontSize = fontSize;
            saving = false;
        }
    }
    #endregion


    //Enumerations
    #region
    public enum SaveLoad
    {
        Save = 0,
        Load = 1,
        New = 2
    }
    #endregion
}
