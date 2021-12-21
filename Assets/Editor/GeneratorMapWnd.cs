using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GeneratorMapWnd : EditorWindow
{
    private GameObject _mapTilesRoot;
    
    private bool isGeneratedBtnClicked = false;
    private static GeneratorMapWnd _generatorWnd;
    private static float initialWndWidth;
    
    private string _tilesContext = "...";
    private int _rowNum = 0;
    private int _colNum = 0;
    
    [MenuItem("GeneratorMapEditor/Editor")]
    public static void ShowWindow()
    {
        _generatorWnd = EditorWindow.GetWindow<GeneratorMapWnd>();
        initialWndWidth = _generatorWnd.position.width;
        Debug.Log(string.Format(" ## Map Generator Output ## cls:GeneratorMapWnd func:ShowWindow info: initial wnd width:{0} height:{1}",
            _generatorWnd.position.width, _generatorWnd.position.height));
    }
    
    private void OnGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.Label("地图数据生成器");
        GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("地图块根结点",GUILayout.Width(initialWndWidth * 0.18f));
            _mapTilesRoot = (GameObject) EditorGUILayout.ObjectField(_mapTilesRoot, typeof(GameObject), true, 
                GUILayout.Width(_generatorWnd.position.width - initialWndWidth * 0.16f - 20f));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
            GUILayout.Label("地图宽 Grid: ", GUILayout.Width(80));
            GUILayout.Label(_rowNum.ToString(),GUILayout.Width(18));
            GUILayout.Label(" x ",GUILayout.Width(12));
            GUILayout.Label("地图高 Grid: ",GUILayout.Width(80));
            GUILayout.Label(_colNum.ToString(),GUILayout.Width(18) );
        GUILayout.EndHorizontal();
        GUILayout.BeginVertical();
            EditorGUILayout.BeginScrollView(new Vector2(_generatorWnd.position.width,
            _generatorWnd.position.height - 80));
                EditorGUILayout.LabelField(_tilesContext);
            EditorGUILayout.EndScrollView();
            GUILayout.BeginHorizontal();
                GUILayout.Space(15);
                isGeneratedBtnClicked = GUILayout.Button(" 生成数据 ", GUILayout.Width(_generatorWnd.position.width * 0.3f), GUILayout.Height(35));
                if (isGeneratedBtnClicked)
                {
                    getTiles();
                    isGeneratedBtnClicked = false;
                }
            GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.EndVertical();
    }

    private GameObject[] getTiles()
    {
        GameObject[] objs = null;
        if (_mapTilesRoot)
        {
            int childCount = _mapTilesRoot.transform.childCount;
            _rowNum = (int)Mathf.Sqrt(childCount);
            _colNum = (int)Mathf.Sqrt(childCount);
            float mX = 0.0f;
            float mY = 0.0f;
            for (int i = 0; i < childCount; ++i)
            {
                Transform cTrans = _mapTilesRoot.transform.GetChild(i);
                if (cTrans && cTrans.gameObject.activeSelf)
                {
                    mX += cTrans.localPosition.x;
                    mY += cTrans.localPosition.y;
                }
            }
            mX = mX / (_colNum * _rowNum);
            mY = mY / (_colNum * _rowNum);
            
            Debug.Log(string.Format(" ## Uni Output ## cls:GeneratorMapWnd func:getTiles info: middle_x:{0} middle_y:{1} ", mX, mY));
            
            for (int i = 0; i < childCount; ++i)
            {
                Transform cTrans = _mapTilesRoot.transform.GetChild(i);
                if (cTrans)
                {
                    int xIdx = (int)(cTrans.localPosition.x - mX);
                    int yIdx = cTrans.localPosition.y > mY ?  (int)(cTrans.localPosition.y - mY) : (int)(cTrans.localPosition.y - mY) + 14;

                    // cTrans.gameObject.name = "GroundTile";
                    // cTrans.gameObject.name += string.Format("{0}_{1}", xIdx, yIdx);
                }
            }
        }
        return objs;
    }
}
