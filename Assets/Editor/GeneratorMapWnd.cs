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
                    autoFillMaps();
                    // getTiles();
                    isGeneratedBtnClicked = false;
                }
            GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.EndVertical();
    }

    private void autoFillMaps()
    {
        if (_mapTilesRoot)
        {
            float top = 0.0f, bottom = 0.0f;
            float left = 0.0f, right = 0.0f;
            int childCount = _mapTilesRoot.transform.childCount;
            if (childCount > 0)
            {
                Transform cTrans = _mapTilesRoot.transform.GetChild(0);
                if (cTrans)
                {
                    left = right = cTrans.localPosition.x;
                    top = bottom = cTrans.localPosition.y;
                }
            }
            
            Dictionary<int,Vector3> pointCaches = new Dictionary<int, Vector3>();
            // 先找出四个角点
            for (int i = 0; i < childCount; ++i)
            {
                Transform c = _mapTilesRoot.transform.GetChild(i);
                if (c)
                {
                    if (left >= c.localPosition.x)
                    {
                        left = c.localPosition.x;
                    }
                    if (right <= c.localPosition.x)
                    {
                        right = c.localPosition.x;
                    }
                    if (top <= c.localPosition.y)
                    {
                        top = c.localPosition.y;
                    }
                    if (bottom >= c.localPosition.y)
                    {
                        bottom = c.localPosition.y;
                    }
                }
            }
            
            int width  = (int) (right - left) / 2 + 1;
            for (int i = 0; i < childCount; ++i)
            {
                Transform cTrans = _mapTilesRoot.transform.GetChild(i);
                if (cTrans)
                {
                    float x = (cTrans.localPosition.x - left) / 2;
                    float y = Math.Abs(cTrans.localPosition.y - top) / 2;
                    int index = (int) (y * width + x);
                    
                    cTrans.name = string.Format("water_{0}", index);

                    pointCaches[index] = new Vector3(x, y, 0);
                }
            }
            
            Vector3 leftTopPos = new Vector3( left, top, 0 );
            GameObject tmplGObj = MapGenerator.getInstance().getTemplateObj();
            if (tmplGObj)
            {
                int height = (int) (top - bottom) / 2 + 1;
                for (int i = 0; i < height; ++i)
                {
                    for (int j = 0; j < width; ++j)
                    {
                        Vector3 pos = leftTopPos;
                        pos.x += j * 2;
                        pos.y -= i * 2;
                        
                        int index = (int) (i * width + j);
                        if (!pointCaches.ContainsKey(index))
                        {
                            GameObject cloneTmplGObj = GameObject.Instantiate(tmplGObj);
                            if (cloneTmplGObj)
                            {
                                cloneTmplGObj.name = string.Format("ClonedTile_{0}", index);
                                cloneTmplGObj.transform.parent = _mapTilesRoot.transform;
                                cloneTmplGObj.transform.localPosition = pos;
                            }
                        }
                        else
                        {
                            Debug.LogWarning(string.Format(" ## Uni Output ## cls:GeneraterMapWnd func:autoFillMaps info: x:{0} y:{1}"
                                ,pos.x,pos.y));
                        }
                    }
                }
            }
            
            Debug.Log(string.Format(" ## Uni Output ## cls:GeneraterMapWnd func:autoFillMaps info: left:{0} right:{1} top:{2} bottom:{3} width:{4} height:{5}"
                                                ,left,right,top,bottom, (right - left), (bottom - top)));
        }
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
