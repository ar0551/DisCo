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
using System;

public class ConnectionScanning : MonoBehaviour
{
    //private variables
    #region
    List<Connection> connections;
    List<Connection> foundC = new List<Connection>();

    Connection closestConnection = null;
    Connection bestOnPart = null;

    float dist = 100;
    float angle = 1000;
    float rot = 1000;
    
    float lastDistAngle = 10000;
    #endregion


    //properties
    #region
    static float connectionThreshold = 0.1f;
    public static float ConnectionThreshold
    {
        get { return connectionThreshold; }
        set { connectionThreshold = value; }
    }

    float distAngle = 0;
    public float DistAngle
    {
        get { return distAngle; }
    }
    #endregion

    //MonoBehaviour methods
    #region
    void Start()
    {
        connections = gameObject.GetComponent<Part>().Connections;
    }

    
    void Update()
    {
        dist = 100;
        angle = 1000;
        lastDistAngle = 10000;


        CheckForCloseConnections();

        RealizeConnection();
    }
    #endregion

    //methods
    #region
    private void CheckForCloseConnections()
    {
        distAngle = 100;
        lastDistAngle = 1000;
        rot = 1000;
        foreach (Connection c in connections)
        {
            foundC = ConnectionVoxelContainer.RevealConnections(c);

            if (foundC.Count > 0)
            {
                foreach (Connection _c in foundC)
                {
                    if (_c.Pln.Parent != null)
                    {
                        dist = Vector3.Distance(c.Pln.Origin, _c.Pln.Origin);
                        angle = AlignPlane.BuildAngle(c.Pln.LocalZVector, _c.Pln.LocalZVector, true);
                        rot = AlignPlane.BuildAngle(c.Pln.XVector, _c.Pln.XVector, false) * 10 * connectionThreshold;
                        distAngle = dist + AngleTightening(angle) + rot / 1000 * connectionThreshold;

                        string grammer = _c.ConType + ">" + c.ConType;
                        if (distAngle < lastDistAngle && _c.CheckForRule(c) != -1 && RuleActive(grammer))
                        { 
                            closestConnection = _c;
                            bestOnPart = c;
                            lastDistAngle = distAngle;
                            if (lastDistAngle < connectionThreshold / 5)
                            {
                                return;
                            }
                        }
                    }
                }
            }
        }
    }

    private void RealizeConnection()
    {
        if (lastDistAngle < connectionThreshold && ConnectionVoxelContainer.RevealConnections(bestOnPart).Contains(closestConnection))
        {
            Vector3 pos = gameObject.transform.position;
            Quaternion rot = gameObject.transform.rotation;

            AlignPlane.Orient(bestOnPart.Pln, closestConnection.Pln, gameObject);

            if (!CollisionDetection())
            {
                Part p = gameObject.GetComponent<Part>();
                p.FreezePart();
                p.Parent = closestConnection.ParentPart.ID;
                p.ParentCon = closestConnection.ParentPart.Connections.IndexOf(closestConnection);

                ConnectionVoxelContainer.RemoveConnection(closestConnection);
                ConnectionVoxelContainer.RemoveConnection(bestOnPart);

                bestOnPart.ParentPart.SetInactive(bestOnPart);
                closestConnection.ParentPart.SetInactive(closestConnection);
                closestConnection.ParentPart.ChildCons.Add(bestOnPart.ParentPart.Connections.IndexOf(bestOnPart));

                closestConnection.ParentPart.Children.Add((int)p.ID);

                GameObject _g = PartsHolder.SpawnPart(p.TemplateID);
                _g.SetActive(true);
                if (GlobalReferences.PlacementType == PlacementTypeTool.PlaceChoreo.Choreo)
                {
                    GlobalReferences.AffectPart(_g);
                    GlobalReferences.FreeParts.Remove(_g);
                }

                ConnectionScanningHandler handler = gameObject.GetComponent<ConnectionScanningHandler>();
                if (handler != null)
                {
                    handler.TerminateConnection();
                }
            }
            else
            {
                gameObject.transform.position = pos;
                gameObject.transform.rotation = rot;
            }
        }
    }

    private bool CollisionDetection()
    {
        bool isCollide = false;
        List<GameObject> collisionGo = new List<GameObject>();
        MeshCollider[] selfColliders = gameObject.GetComponents<MeshCollider>();
        collisionGo = CollisionVoxelContainer.RevealCloseGos(gameObject);

        foreach (GameObject closeGo in collisionGo)
        {
            if (closeGo != null)
            {
                MeshCollider[] otherColliders = closeGo.GetComponents<MeshCollider>();
                foreach (MeshCollider otherCol in otherColliders)
                {
                    foreach (MeshCollider selfCol in selfColliders)
                    {
                        Vector3 vec = Vector3.up;
                        float f = 0;
                        if (Physics.ComputePenetration(selfCol, gameObject.transform.position, gameObject.transform.rotation, otherCol, closeGo.transform.position, closeGo.transform.rotation, out vec, out f))
                        {
                            isCollide = true;
                        }
                    }
                }
            }
        }

        if (true)
        {
            foreach (GameObject go in GlobalReferences.AdditionalGeometry)
            {
                foreach (MeshCollider mc in selfColliders)
                {
                    Vector3 vec = Vector3.up;
                    float f = 0;
                    if (go.GetComponent<MeshCollider>() != null && Physics.ComputePenetration(mc, gameObject.transform.position, gameObject.transform.rotation, go.GetComponent<MeshCollider>(), go.transform.position, go.transform.rotation, out vec, out f))
                    {
                        isCollide = true;
                    }
                }
            }
        }

        return isCollide;
    }

    private bool RuleActive(string s)
    {
        foreach (string _s in GlobalReferences.ActiveRulesGrammer)
        {
            if (string.Compare(_s,s) == 0)
            {
                return true;
            }
        }
        return false;
    }

    

    private float AngleTightening(float angle)
    {
        return 0.01f * (float)Math.Pow(1.15, (angle - 15));
    }
    #endregion
}
