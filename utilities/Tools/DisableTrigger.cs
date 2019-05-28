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

public class DisableTrigger : MonoBehaviour
{
    List<Collider> purgatory = new List<Collider>();

    Material disableMat;
    Material enableMat;

    private bool triggered = false;
    public bool Triggered
    {
        get { return triggered; }
        set { triggered = value; }
    }

    private bool enable = false;
    public bool Enable
    {
        get { return enable; }
        set { enable = value; }
    }

    private AddRemove addRem = AddRemove.Remove;
    public AddRemove AddRem
    {
        get { return addRem; }
        set { addRem = value; }
    }

    static Dictionary<int, List<Connection>> Removed = new Dictionary<int, List<Connection>>();

    private void Start()
    {
        enableMat = Resources.Load<Material>("Materials/PlacedMaterial");
        disableMat = Resources.Load<Material>("Materials/lowlitPlacedMaterial");
    }

    private void OnTriggerEnter(Collider other)
    {
        EnableDisable(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (purgatory.Contains(other))
        {
            purgatory.Remove(other);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (Triggered == true)
        {
            foreach (Collider col in purgatory)
            {
                EnableDisable(col);
            }

            purgatory.Clear();

            Triggered = false;
        }

        
    }

    private void EnableDisable(Collider other)
    {
        if (Enable && AddRem == AddRemove.Remove)
        {
            Part p = other.gameObject.GetComponent<Part>();
            if (p != null && !Removed.ContainsKey((int)p.ID))
            {
                List<Connection> tempCons = new List<Connection>();
                foreach (int i in p.ActiveConnections)
                {
                    ConnectionVoxelContainer.RemoveConnection(p.Connections[i]);
                    tempCons.Add(p.Connections[i]);
                }
                Removed.Add((int)p.ID, tempCons);
            }

            other.GetComponent<MeshRenderer>().material = disableMat;
        }
        else if (Enable && AddRem == AddRemove.Add)
        {
            Part p = other.gameObject.GetComponent<Part>();
            if (p != null && Removed.ContainsKey((int)p.ID))
            {
                foreach (Connection c in Removed[(int)p.ID])
                {
                    ConnectionVoxelContainer.StoreConnection(c);
                }

                Removed.Remove((int)p.ID);
                other.GetComponent<MeshRenderer>().material = enableMat;
            }
        }
        else
        {
            purgatory.Add(other);
        }
    }

    public enum AddRemove
    {
        Remove = 0,
        Add = 1
    }
}
