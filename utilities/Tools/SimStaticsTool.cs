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

public class SimStaticsTool : MonoBehaviour
{
    //local variables
    #region
    [SerializeField]
    private TextMeshPro textMesh = null;
    private new MeshRenderer renderer;

    static bool simulationInProgress = false;

    static List<SimulationProcessHandler> handlers = new List<SimulationProcessHandler>();
    static int handlerCallBack = 0;

    static string tempPath = "";
    #endregion


    //Monobehaviour methods
    #region
    void Start()
    {
        renderer = textMesh.gameObject.GetComponent<MeshRenderer>();
        tempPath = Application.dataPath + "/Resources/temp.json"; 
    }
    #endregion


    //methods
    #region
    public void ChangeTool(ToolsetControls.InputX input)
    {
        if (input == ToolsetControls.InputX.Enter && !simulationInProgress)
        {
            textMesh.text = "Simulation\nin progress...";

            ParentAndSimulate(textMesh);

            textMesh.faceColor = GlobalReferences.AffectedColor;
            simulationInProgress = true;
        }
    }


    public static void ParentAndSimulate(TextMeshPro tm)
    {
        List<Rigidbody> rbs = new List<Rigidbody>();

        AssemblyIO.Save(GlobalReferences.FrozenParts, 1, tempPath);

        foreach (GameObject go in GlobalReferences.FrozenParts.Values)
        {
            int? parentId = go.GetComponent<Part>().Parent;

            Destroy(go.GetComponent<ConstantForce>());
            

            if (parentId != null && GlobalReferences.FrozenParts.ContainsKey((int)parentId))
            {
                go.transform.parent = GlobalReferences.FrozenParts[(int)parentId].transform;
                Destroy(go.GetComponent<Rigidbody>());
            }
            else
            {
                rbs.Add(go.GetComponent<Rigidbody>());
                SimulationProcessHandler handler = go.AddComponent<SimulationProcessHandler>();
                handlers.Add(handler);
                handler.TM = tm;
                handler.SimulationTerminated += new SimulationProcessHandler.SimulationHandler(AfterSimulation);
            }
        }
        foreach (Rigidbody rb in rbs)
        {
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.None;
        }

        ChangeColliders(false);
    }

    public static void AfterSimulation(TextMeshPro tm)
    {
        ++handlerCallBack;
        if (handlerCallBack == handlers.Count)
        {
            SaveLoadTool.SaveGame("Sim_");

            List<GameObject> tempGos = new List<GameObject>();
            foreach (GameObject go in GlobalReferences.FrozenParts.Values)
            {
                tempGos.Add(go);
            }

            GlobalReferences.FrozenParts.Clear();

            ChangeColliders(true);            

            for (int i = tempGos.Count - 1; i >= 0; --i)
            {
                Destroy(tempGos[i]);
            }

            AssemblyIO.Load(tempPath);
            GlobalReferences.RebuildIndices();

            tm.text = "Run\nSimulation";
            tm.faceColor = GlobalReferences.UnaffectedColor;

            simulationInProgress = false;

            handlers.Clear();
            handlerCallBack = 0;
        }
    }

    private static void ChangeColliders(bool onOff)
    {
        foreach (GameObject go in GlobalReferences.FreeParts)
        {
            List<Collider> cols = new List<Collider>();
            cols.AddRange(go.GetComponents<MeshCollider>());
            cols.AddRange(go.GetComponents<BoxCollider>());
            cols.AddRange(go.GetComponents<SphereCollider>());
            cols.AddRange(go.GetComponents<CapsuleCollider>());

            foreach (Collider col in cols)
            {
                col.enabled = onOff;
            }
        }
    }
    #endregion
}
