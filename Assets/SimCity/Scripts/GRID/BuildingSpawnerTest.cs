using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSpawnerTest : MonoBehaviour
{
    public GameObject prefab; // 引用预制体
    // 所选物体的唯一标识符
    public ObjectPlacer objectPlacer;
    // 指定的新建建筑的父 GameObject，若传入 null 则不设置父对象
    public GameObject parentObject;
    int buildingSize = 40;

    public void buildingSpawner(int size)
    {
        // 计算建筑群基准点（左上角）的坐标
        Vector3 worldPosition = new Vector3(buildingSize * size / 2, 0, -buildingSize * size / 2);
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                Vector3 newPosition = new Vector3(worldPosition.x - buildingSize * i, worldPosition.y, worldPosition.z + buildingSize * j);
                objectPlacer.PlaceObject(prefab, newPosition, "category", parentObject);
            }
        }
    }
}
