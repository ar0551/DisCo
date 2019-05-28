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
using System;
using System.Collections.Generic;
using UnityEngine;

public class PartsHolder : MonoBehaviour
{
  //variables for spawn area initialization
    #region

    static float minX;

    static float maxX;

    static float minY;

    static float maxY;

    static float minZ;

    static float maxZ;
    #endregion


    //properties
    #region
    static int numParts = 100;
    public static int NumParts
    {
        get { return numParts; }
    }
    #endregion

    //MonoBehaviour methods - Initializes Game Beginning
    #region
    public void Initialize(float _minX, float _maxX, float _minY, float _maxY, float _minZ, float _maxZ, string loadPath, int _numParts)
    {
        minX = _minX;
        maxX = _maxX;

        minY = _minY;
        maxY = _maxY;

        minZ = _minZ;
        maxZ = _maxZ;

        if (loadPath == "")
        {
            loadPath = (Application.dataPath + "/Resources/WaspInput.json");
        }

        GlobalReferences.TemplateParts = ImplementWasp.Load(loadPath);

        numParts = _numParts;
        SpawnMultiple(NumParts);

    }
    #endregion


    //methods
    #region
    public static void SpawnMultiple(int num)
    {
        for (int i = 0; i < num; ++i)
        {
            GameObject go = SpawnPart();
            go.SetActive(true);
        }
    }


    public static GameObject SpawnPart(int idx = -1)
    {
        double r = UnityEngine.Random.Range(0, GlobalReferences.TemplateParts.Count);
        int chose = GlobalReferences.PartProb;
        if (idx != -1)
        {
            chose = idx;
        }

        Vector3 pos = new Vector3(UnityEngine.Random.Range(minX, maxX), UnityEngine.Random.Range(minY, maxY), UnityEngine.Random.Range(minZ, maxZ));
        Quaternion rot = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)));

        GameObject go = Instantiate(GlobalReferences.TemplateParts[chose], pos, rot);
        ResetPart(go, GlobalReferences.TemplateParts[chose], null);

        GlobalReferences.Parts.Add(go);
        GlobalReferences.FreeParts.Add(go);
        return go;
    }

    public static void ResetPart(GameObject go, GameObject templateGo, int? id)
    {
        List<Connection> tempCons = new List<Connection>();
        foreach (Connection con in templateGo.GetComponent<Part>().Connections)
        {
            Connection c = (Connection)con.Clone();
            tempCons.Add(c);
        }
        go.GetComponent<Part>().Initialize(templateGo.GetComponent<Part>().Name, tempCons, id, templateGo.GetComponent<Part>().TemplateID);
    }

    IEnumerator Activate()
    {   
        for (int i = 0; i < GlobalReferences.Parts.Count; ++i)
        {
            GlobalReferences.Parts[i].SetActive(true);
            yield return new WaitForSeconds(0.001f);
        }

        GlobalReferences.ChangeAffectedNumber(NumberOfPartsTool.AffectedParts);
    }
    #endregion
}
