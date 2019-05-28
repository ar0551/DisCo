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

using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;


//Json Serializable Classes for Wasp Connection
#region
[DataContract(Name = "Container")]
public class JsonContainer
{
    [DataMember(Name = "PartData")]
    public List<PartData> parts = new List<PartData>();

    [DataMember(Name = "RuleData")]
    public List<RuleData> rules = new List<RuleData>();

    [DataMember(Name = "AdditionalGeometry")]
    public List<AdditionalGeometryData> addGeometries = new List<AdditionalGeometryData>();
}

[DataContract(Name = "AdditionalGeometries")]
public class AdditionalGeometryData
{
    [DataMember(Name = "Geometry")]
    public string geometry;
}

[DataContract(Name = "Rules")]
public class RuleData
{
    [DataMember(Name = "Part1")]
    public string part1;

    [DataMember(Name = "Conn1")]
    public int conn1;

    [DataMember(Name = "Part2")]
    public string part2;

    [DataMember(Name = "Conn2")]
    public int conn2;
}

[DataContract(Name = "Connections")]
public class ConnectionData
{
    [DataMember(Name = "PlaneOriginX")]
    public float origX;

    [DataMember(Name = "PlaneOriginY")]
    public float origY;

    [DataMember(Name = "PlaneOriginZ")]
    public float origZ;

    [DataMember(Name = "PlaneXVecX")]
    public float xVecX;

    [DataMember(Name = "PlaneXVecY")]
    public float xVecY;

    [DataMember(Name = "PlaneXVecZ")]
    public float xVecZ;

    [DataMember(Name = "PlaneYVecX")]
    public float yVecX;

    [DataMember(Name = "PlaneYVecY")]
    public float yVecY;

    [DataMember(Name = "PlaneYVecZ")]
    public float yVecZ;


    [DataMember(Name = "Part")]
    public string part;

    [DataMember(Name = "ConID")]
    public int conId;

    [DataMember(Name = "ConType")]
    public string conType;
}

[DataContract(Name = "Parts")]
public class PartData
{
    [DataMember(Name = "Name")]
    public string name;

    [DataMember(Name = "TemplateID")]
    public int templateId;

    [DataMember(Name = "Probability")]
    public float probability;

    [DataMember(Name = "Geometry")]
    public string geometry;

    [DataMember(Name = "Collider")]
    public string collider;

    [DataMember(Name = "Connections")]
    public List<ConnectionData> connections;
}
#endregion

//Load Json with Template Parts from Wasp
public static class ImplementWasp
{
    public static List<GameObject> templateParts = new List<GameObject>();

    public static JsonContainer jsonCont = new JsonContainer();

    //main method
    public static List<GameObject> Load(string path)
    {
        jsonCont = LoadFromWasp(path);

        GlobalReferences.Rules = LoadRules();
        foreach (string r in GlobalReferences.RulesGrammer)
        {
            GlobalReferences.ActiveRulesGrammer.Add(string.Copy(r));
        }


        foreach (PartData p in jsonCont.parts)
        {
            templateParts.Add(BuildPart(p));
        }

        GlobalReferences.AdditionalGeometry = BuildAdditional();

        return templateParts;
    }

    //initialization methods
    #region
    private static List<GameObject> BuildAdditional()
    {
        List<GameObject> addGeos = new List<GameObject>();
        Material mat = Resources.Load<Material>("Materials/AddGeoMat");

        int i = 0;
        foreach (AdditionalGeometryData add in jsonCont.addGeometries)
        {
            GameObject go = new GameObject();
            MeshRenderer mr = go.AddComponent<MeshRenderer>();
            MeshFilter mf = go.AddComponent<MeshFilter>();
            MeshCollider mc = go.AddComponent<MeshCollider>();

            Mesh m = BuildObjMesh(add.geometry);

            go.transform.RotateAround(Vector3.zero, Vector3.right, -90);
            go.transform.localScale = new Vector3(1, -1, 1);
            go.layer = 10;
            go.name = "AddGeo_" + i.ToString();

            mf.sharedMesh = m;
            mc.sharedMesh = m;
            mr.material = mat; 

            addGeos.Add(go);

            ++i;
        }

        return addGeos;
    }

    private static List<Rule> LoadRules()
    {
        List<Rule> rules = new List<Rule>();

        foreach (RuleData r in jsonCont.rules)
        {
            string grammerA = "";
            string grammerB = "";
            string grammer = "";

             foreach (PartData p in jsonCont.parts)
            {
                foreach (ConnectionData con in p.connections)
                {
                    if (con.conId == r.conn1 && p.name == r.part1)
                    {
                        grammerA = con.conType;
                    }

                    if (con.conId == r.conn2 && p.name == r.part2)
                    {
                        grammerB = con.conType;
                    }
                }
            }

            grammer = grammerA + ">" + grammerB;
            Rule _r = new Rule(r.part1, r.conn1, r.part2, r.conn2, true);

            rules.Add(_r);

            if (!GlobalReferences.RulesGrammer.Contains(grammer))
            {
                GlobalReferences.RulesGrammer.Add(grammer);
            }
        }

        return rules;
    }

    private static GameObject BuildPart(PartData p)
    {
        GameObject tempGo = new GameObject(p.name);
        Part tempPart = tempGo.AddComponent<Part>();

        GlobalReferences.PartProbs.Add(p.probability);
        GlobalReferences.TypeFilter.Add(p.name);

        List<Connection> cons = new List<Connection>();
        List<Mesh> colliderMeshes = new List<Mesh>();

        foreach (ConnectionData conD in p.connections)
        {
            Connection con = new Connection(new AlignPlane(new Vector3(conD.origX, conD.origY, conD.origZ),
                                                    new Vector3(conD.xVecX, conD.xVecY, conD.xVecZ),
                                                    new Vector3(conD.yVecX, conD.yVecY, conD.yVecZ),
                                            tempGo.transform), conD.conType, conD.part, conD.conId);
            con.GenerateRulesTable(GlobalReferences.Rules);
            cons.Add(con);
        }

        colliderMeshes = BuildColliders(p.collider);

        if (colliderMeshes.Count > 0)
            tempPart.Initialize(p.name, cons, -1, p.templateId, BuildObjMesh(p.geometry), colliderMeshes);
        else
            tempPart.Initialize(p.name, cons, -1, p.templateId, BuildObjMesh(p.geometry));
        
        tempGo.SetActive(false);
        tempGo.layer = 8;

        return tempGo;
    }

    private static Mesh BuildObjMesh(string objSyntax)
    {
        int divider = 1;
        Mesh m = new Mesh();

        List<Vector3> mVerts = new List<Vector3>();
        List<int> mFaces = new List<int>();

        StringReader sr = new StringReader(objSyntax);

        string line;
        while ((line = sr.ReadLine()) != null)
        {
            if (line.Length == 0) continue;

            if (line[0] == 'v')
            {
                Vector3 v = new Vector3();
                float f;
                int k = 0;
                StringBuilder sb = new StringBuilder();
                foreach (char c in line)
                {
                    if (c == 'v') continue;

                    if (c == ' ')
                    {
                        if (sb.Length == 0) continue;
                        f = float.Parse(sb.ToString(), System.Globalization.CultureInfo.InvariantCulture);
                        sb.Clear();
                        if (k == 0)
                            v.x = f / divider;
                        else if (k == 1)
                            v.y = f / divider;

                        ++k;
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
                f = float.Parse(sb.ToString(), System.Globalization.CultureInfo.InvariantCulture);
                v.z = f / divider;

                mVerts.Add(v);
            }

            if (line[0] == 'f')
            {
                int i;
                StringBuilder sb = new StringBuilder();
                foreach (char c in line)
                {
                    if (c == 'f') continue;

                    if (c == ' ')
                    {
                        if (sb.Length == 0) continue;

                        i = int.Parse(sb.ToString());
                        sb.Clear();

                        mFaces.Add(i - 1);
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
                i = int.Parse(sb.ToString());
                mFaces.Add(i - 1);
            }
        }

        m.SetVertices(mVerts);
        m.triangles = mFaces.ToArray();
        m.RecalculateBounds();
        m.RecalculateNormals();
        m.RecalculateTangents();

        return m;
    }

    private static List<Mesh> BuildColliders(string colliderData)
    {
        List<string> colliders = new List<string>();

        StringBuilder sb = new StringBuilder();
        StringReader sr = new StringReader(colliderData);
        string line;
        bool firstline = true;

        while ((line = sr.ReadLine()) != null)
        {
            if (line.Length > 0 && line[0] == 'o')
            {
                if (!firstline)
                {
                    colliders.Add(sb.ToString());
                    sb.Clear();
                }
                else
                    firstline = false;
            }

            sb.AppendLine(line);
        }
        colliders.Add(sb.ToString());


        List<Mesh> colliderMeshes = new List<Mesh>();

        foreach (string s in colliders)
        {
            colliderMeshes.Add(BuildObjMesh(s));
        }

        return colliderMeshes;
    }

    private static JsonContainer LoadFromWasp(string path)
    {
        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(JsonContainer));

        JsonContainer partsAndRules = null;

        using (FileStream stream = new FileStream(path, FileMode.Open))
        {
            partsAndRules = serializer.ReadObject(stream) as JsonContainer;
        }

        return partsAndRules;
    }
    #endregion
}
