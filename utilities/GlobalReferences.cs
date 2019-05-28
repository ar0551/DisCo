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

public class GlobalReferences : MonoBehaviour
{
    //private variables
    #region
    [SerializeField]
    float _forceScale = 1;

    Material _affectedMat = null;
    Material _unaffectedMat = null;
    #endregion


    //properties
    #region
    private static GameObject controller = null;
    public static GameObject Controller
    {
        get { return controller; }
        set { controller = value; }
    }

    private static List<GameObject> parts = new List<GameObject>();
    public static List<GameObject> Parts
    {
        get { return parts; }
        set { parts = value; }
    }

    private static List<GameObject> freeParts = new List<GameObject>();
    public static List<GameObject> FreeParts
    {
        get { return freeParts; }
        set { freeParts = value; }
    }

    private static Dictionary<int, GameObject> frozenParts = new Dictionary<int, GameObject>();
    public static Dictionary<int, GameObject> FrozenParts
    {
        get { return frozenParts; }
        set { frozenParts = value; }
    }

    private static List<GameObject> affectedParts = new List<GameObject>();
    public static List<GameObject> AffectedParts
    {
        get { return affectedParts; }
    }

    private static List<GameObject> additionalGeometry = new List<GameObject>();
    public static List<GameObject> AdditionalGeometry
    {
        get { return additionalGeometry; }
        set { additionalGeometry = value; }
    }

    private static List<string> typeFilter = new List<string>();
    public static List<string> TypeFilter
    {
        get { return typeFilter; }
        set { typeFilter = value; }
    }

    private static int numOfParts = 0;
    public static int NumOfParts
    {
        get { return numOfParts; }
        set { numOfParts = value; }
    }

    private static List<GameObject> templateParts = new List<GameObject>();
    public static List<GameObject> TemplateParts
    {
        get { return templateParts; }
        set { templateParts = value; }
    }

    private static List<float> partProbs = new List<float>();
    public static List<float> PartProbs
    {
        get { return partProbs; }
        set { partProbs = value; }
    }

    public static int PartProb
    {
        get
        {
            float rand = Random.Range(0.00f, 1.00f);
            int i = 0;
            float counting = 0f;
            foreach (float f in PartProbs)
            {
                counting = counting + f;
                if (rand <= counting)
                {
                    return i;
                }

                ++i;
            }

            return i;
        }
    }

    private static List<string> rulesGrammer = new List<string>();
    public static List<string> RulesGrammer
    {
        get { return rulesGrammer; }
        set { rulesGrammer = value; }
    }

    private static List<string> activeRulesGrammer = new List<string>();
    public static List<string> ActiveRulesGrammer
    {
        get { return activeRulesGrammer; }
        set { activeRulesGrammer = value; }
    }

    private static List<Rule> rules = new List<Rule>();
    public static List<Rule> Rules
    {
        get { return rules; }
        set { rules = value; }
    }

    private static PlacementTypeTool.PlaceChoreo placementType = PlacementTypeTool.PlaceChoreo.Place;
    public static PlacementTypeTool.PlaceChoreo PlacementType
    {
        get { return placementType; }
        set { placementType = value; }
    }


    private static Vector3 controllerPos;
    public static Vector3 ControllerPos
    {
        get { return controllerPos; }
    }

    private static Vector3 controllerVel;
    public static Vector3 ControllerVel
    {
        get { return controllerVel; }
    }

    private static float driftApartFac = 0f;
    public static float DriftApartFac
    {
        get { return driftApartFac; }
        set { driftApartFac = value; }
    }

    private static Vector3 centerOfMass;
    public static Vector3 CenterOfMass
    {
        get { return centerOfMass; }
    }

    private static float forceScale;
    public static float ForceScale
    {
        get { return forceScale; }
    }

    private static Material affectedMat = null;
    public static Material AffectedMat
    {
        get { return affectedMat; }
    }

    private static Material unaffectedMat = null;
    public static Material UnaffectedMat
    {
        get { return unaffectedMat; }
    }

    private static Color affecetedColor = new Color(0.3383838f, 0.7676765f, 1);
    public static Color AffectedColor
    {
        get { return affecetedColor; }
    }

    private static Color unaffecetedColor = new Color(0.2313177f, 0.5211295f, 0.678f);
    public static Color UnaffectedColor
    {
        get { return unaffecetedColor; }
    }
    #endregion


    //MonobBehaviour methods
    #region
    private void Awake()
    {
        forceScale = _forceScale;

        _affectedMat = Resources.Load<Material>("Materials/affectedMaterial");
        _unaffectedMat = Resources.Load<Material>("Materials/unaffectedMaterial");
        affectedMat = _affectedMat;
        unaffectedMat = _unaffectedMat;
    }

    private void Update()
    {
        controllerVel = (Controller.transform.position - controllerPos) / Time.deltaTime;
        controllerPos = Controller.transform.position;

        centerOfMass = Vector3.zero;
        int i = 0;
        foreach (GameObject go in affectedParts)
        {
            if (go != null)
            {
                centerOfMass += go.transform.position;
                ++i;
            }
        }
        centerOfMass = centerOfMass / i;
    }
    #endregion


    //static methods
    #region
    public static int TemplateIDFromName(string name)
    {
        for (int i = 0; i < templateParts.Count; ++i)
        {
            if (templateParts[i].name == name)
            {
                return i;
            }
        }

        return -1;
    }

    public static List<GameObject> ChangeAffectedNumber(int num)
    {
        CompareObjectDistance compDist = new CompareObjectDistance();
        Comparer<GameObject> defComp = compDist;

        if (num - 1 > affectedParts.Count)
        {
            freeParts.Sort(compDist);

            List<GameObject> transition = new List<GameObject>();
            foreach (GameObject go in freeParts)
            {
                if (num - 1 >= affectedParts.Count)
                {
                    if (TypeFilter.Contains(go.GetComponent<Part>().Name))
                    {
                        AffectPart(go);
                        transition.Add(go);
                    }
                }
                else
                {
                    break;
                }
            }
            foreach (GameObject go in transition)
            {
                freeParts.Remove(go);
            }
        }
        else
        {
            affectedParts.Sort(compDist);
            
            for(int i = affectedParts.Count - 1; i >= 0; --i)
            {
                if (num < affectedParts.Count)
                {
                    GameObject go = affectedParts[i];
                    go.GetComponent<ConnectionScanning>().enabled = false;
                    go.GetComponent<PartBehaviour>().enabled = false;
                    go.GetComponent<MeshRenderer>().material = unaffectedMat;
                    go.GetComponent<ConstantForce>().force = Vector3.zero;
                    affectedParts.Remove(go);
                    FreeParts.Add(go);
                }
            }
        }

        return affectedParts;
    }

    public static void AffectPart(GameObject go)
    {
        affectedParts.Add(go);
        go.GetComponent<ConnectionScanning>().enabled = true;
        go.GetComponent<PartBehaviour>().enabled = true;
        go.GetComponent<MeshRenderer>().material = affectedMat;
    }

    public static void ClearAffectedList()
    {
        foreach (GameObject go in AffectedParts)
        {
            go.GetComponent<ConnectionScanning>().enabled = false;
            go.GetComponent<MeshRenderer>().material = UnaffectedMat;
            go.GetComponent<PartBehaviour>().enabled = false;
            go.GetComponent<ConstantForce>().force = Vector3.zero;
        }
        FreeParts.AddRange(AffectedParts);
        AffectedParts.Clear();
    }

    public static void RebuildIndices()
    {
        NumOfParts = 0;

        List<GameObject> tempFrozen = new List<GameObject>();
        foreach (GameObject go in FrozenParts.Values)
        {
            tempFrozen.Add(go);
        }

        FrozenParts.Clear();

        Dictionary<int, int> IdChanges = new Dictionary<int, int>();

        foreach(GameObject go in tempFrozen)
        {
            IdChanges.Add((int)go.GetComponent<Part>().ID, NumOfParts);
            ++NumOfParts;
        }
         foreach (GameObject go in tempFrozen)
        {
            go.GetComponent<Part>().ChangeIDsFromDic(IdChanges);
            FrozenParts.Add((int)go.GetComponent<Part>().ID, go);
        }

    }

    public static void DeletePart(int id)
    {
        GameObject go = frozenParts[id];
        int? parentID = go.GetComponent<Part>().Parent;
        if (parentID != null && frozenParts.ContainsKey((int)parentID))
        {
            List<int> _children = frozenParts[(int)parentID].GetComponent<Part>().Children;
            for (int i = _children.Count - 1; i >= 0; --i)
            {
                if (_children[i] == id)
                {
                    Part parentP = frozenParts[(int)parentID].GetComponent<Part>();
                    Part p = go.GetComponent<Part>();
                    parentP.Children.RemoveAt(i);
                    if (parentP.ChildCons.Count > i)
                    {
                        parentP.ChildCons.RemoveAt(i);
                    }
                    foreach (int j in go.GetComponent<Part>().ChildCons)
                    {
                        Part parentPart = frozenParts[(int)parentID].GetComponent<Part>();
                        if (parentPart.Connections.Count > j)
                        {
                            parentPart.SetActive(parentPart.Connections[j]);
                        }
                    }
                    if (p.ChildCons.Count > i)
                    {
                        ConnectionVoxelContainer.StoreConnection(frozenParts[p.Children[i]].GetComponent<Part>().Connections[p.ChildCons[i]]);
                        //ConnectionVoxelContainer.StoreConnection(p.ChildCons[i]);
                    }
                }
            }
        }

        int[] children = go.GetComponent<Part>().Children.ToArray();

        for (int i = 0; i < children.Length; ++i)
        {
            int child = children[i];

            if (frozenParts.ContainsKey(child))
            {
                frozenParts[child].GetComponent<Part>().Parent = null;
                frozenParts[child].GetComponent<Part>().ParentCon = null;
            }

            int? parent = go.GetComponent<Part>().Parent;

            if (parent != null && frozenParts.ContainsKey((int)parent))
            {
                Part parentPart = frozenParts[(int)parent].GetComponent<Part>();
                Connection parentCon = parentPart.Connections[(int)go.GetComponent<Part>().ParentCon];
                frozenParts[children[i]].GetComponent<Part>().SetActive(parentCon);
                ConnectionVoxelContainer.StoreConnection(parentCon);
            }
            //frozenParts[children[i]].GetComponent<Part>().SetActive(go.GetComponent<Part>().ParentCon);
        }

        frozenParts.Remove(id);
        parts.Remove(go);

        

        Destroy(go);
    }
    #endregion
}
