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
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;

public class InitializeLauncher : MonoBehaviour
{
    [SerializeField]
    GameObject FPSController;

    [SerializeField]
    GameObject ViveController;

    PartsHolder partsHolder = null;
    InitializeGameArea initArea = null;

    string LoadLoc;
    string SaveLoc;

    int VrFps = 0;

    float SnapThresh = 0.1f;

    int NumParts = 100;

    float minX = -5;
    float maxX = 5;
    float minY = 0;
    float maxY = 3;
    float minZ = -3;
    float maxZ = 3;

    float ConRes = 0.1f;
    float ColRes = 0.5f;

    private void Awake()
    {
        partsHolder = gameObject.GetComponent<PartsHolder>();
        initArea = gameObject.GetComponent<InitializeGameArea>();

        LoadJson(Application.dataPath + "/Resources/config.json");

        initArea.Initialize(minX, maxX, minY, maxY, minZ, maxZ, ConRes, ColRes);
        partsHolder.Initialize(minX, maxX, minY, maxY, minZ, maxZ, LoadLoc, NumParts);

        ConnectionScanning.ConnectionThreshold = SnapThresh;
        SaveLoadTool.SavePath = SaveLoc;

        InitializeController(VrFps);
    }

    private void InitializeController(int i)
    {
        if (i == 0)
        {
            ViveController.SetActive(true);
            FPSController.SetActive(false);
            GlobalReferences.Controller = ViveController.GetComponentInChildren<PlacementBehaviour>().gameObject;
        }
        else
        {
            FPSController.SetActive(true);
            ViveController.SetActive(false);
            GlobalReferences.Controller = FPSController.GetComponentInChildren<PlacementBehaviour>().gameObject;
        }
    }

    private void LoadJson(string path)
    {
        JsonInitContainer container = null;

        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(JsonInitContainer));

        using (FileStream stream = new FileStream(path, FileMode.Open))
        {
            container = serializer.ReadObject(stream) as JsonInitContainer;
        }

        if (container != null)
        {
            SaveLoc = container.saveLoc;
            LoadLoc = container.loadLoc;

            VrFps = container.vrFps;

            SnapThresh = container.snapThreshold;
            NumParts = container.numberOfParts;

            minX = container.minX;
            maxX = container.maxX;

            minY = container.minY;
            maxY = container.maxY;

            minZ = container.minZ;
            maxZ = container.maxZ;

            ConRes = container.conRes;
            ColRes = container.colRes;
        }
    }
}

[DataContract(Name = "References")]
public class JsonInitContainer
{
    [DataMember(Name = "LoadLocation")]
    public string loadLoc;

    [DataMember(Name = "SaveLocation")]
    public string saveLoc;

    [DataMember(Name = "VrFps")]
    public int vrFps;

    [DataMember(Name = "SnapThreshold")]
    public float snapThreshold;

    [DataMember(Name = "NumberOfParts")]
    public int numberOfParts;


    [DataMember(Name = "MinX")]
    public float minX;

    [DataMember(Name = "MaxX")]
    public float maxX;

    [DataMember(Name = "MinY")]
    public float minY;

    [DataMember(Name = "MaxY")]
    public float maxY;

    [DataMember(Name = "MinZ")]
    public float minZ;

    [DataMember(Name = "MaxZ")]
    public float maxZ;


    [DataMember(Name = "ConnectionResolution")]
    public float conRes;

    [DataMember(Name = "CollisionResolution")]
    public float colRes;

}
