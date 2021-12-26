using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Directory = UnityEngine.Windows.Directory;

public class MapGenerator
{
    private static MapGenerator _instance;
    public static MapGenerator getInstance()
    {
        if (_instance == null)
        {
            _instance = new MapGenerator();
        }
        return _instance;
    }
    
    private Dictionary<int,Vector3> _mapDic = new Dictionary<int, Vector3>();
    private GameObject _mapTemplateObj;

    public GameObject getTemplateObj()
    {
        if (_mapTemplateObj == null)
        {
            _mapTemplateObj = GameObject.Find("GroundTemplate");
        }
        return _mapTemplateObj;
    }

    public void cacheData(int index, Vector3 pos)
    {
        _mapDic[index] = pos;
    }

    public void writeToLocal(string filePath)
    {
        if (_mapDic != null)
        {
            string mapData = JsonUtility.ToJson(_mapDic);
            
            Debug.Log(" map data = " + mapData);
            
            if ( !string.IsNullOrEmpty(filePath) && Directory.Exists(filePath) )
            {
                string fileName = "fileData.json";
                string fullPath = filePath + "/" + fileName;
                
                File.WriteAllText( fullPath, mapData );
            }
        }
    }
}
