using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

public class BuildingSpawnerJson : MonoBehaviour
{
    public ObjectPlacer objectPlacer;
    // 指定的新建建筑的父 GameObject，若传入 null 则不设置父对象
    public GameObject parentObject;
    int buildingSize = 40;
    //引用和配置
    [Header("Buildings")]
    [SerializeField] private GameObject b0;
    [SerializeField] private GameObject b1;
    [SerializeField] private GameObject b2;
    [SerializeField] private GameObject b3;
    [SerializeField] private GameObject b4;
    [SerializeField] private GameObject b5;

    // 定义一个GameObject类型的数组来存放建筑
    private GameObject[] buildings;

    void Start()
    {
        // 将这些建筑添加到数组中
        buildings = new GameObject[] { b0, b1, b2, b3, b4, b5 };
    }
    public void SpawnBuilding(int row, int column, int buildingTypeID)
    {
        Vector3 newPosition = new Vector3(-buildingSize * row, 0.01f, buildingSize * column);
        objectPlacer.PlaceObject(buildings[buildingTypeID], newPosition, "category", parentObject);
    }
    public void RemoveAllBuildings()
    {
        if (parentObject != null)
        {
            // 获取父对象下所有子对象的 Transform 组件
            Transform[] allChildren = parentObject.GetComponentsInChildren<Transform>();

            // 遍历所有子对象，注意要从后往前遍历，因为销毁对象会改变子对象的数量
            for (int i = allChildren.Length - 1; i > 0; i--)
            {
                Transform child = allChildren[i];
                if (child != parentObject.transform)
                {
                    // 销毁子对象
                    Destroy(child.gameObject);
                }
            }
        }
    }
}