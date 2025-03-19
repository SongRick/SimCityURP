using UnityEngine;

public class MovingState : IBuildingState
{
    private int movingIndex;
    private Vector2Int objectSize;
    private int ID;
    private Grid grid;
    private PreviewSystem previewSystem;
    private GridData gridData;
    private ObjectPlacer objectPlacer;
    private SoundFeedback soundFeedback;
    private Vector3Int originalPosition;

    public MovingState(int index, Vector3Int gridPosition, Vector2Int size, int id,
                       Grid grid, PreviewSystem preview, GridData gridData,
                       ObjectPlacer placer, SoundFeedback sound)
    {
        movingIndex = index;
        objectSize = size;
        ID = id;
        this.grid = grid;
        previewSystem = preview;
        this.gridData = gridData;
        objectPlacer = placer;
        soundFeedback = sound;
        originalPosition = gridPosition;

        previewSystem.StartShowingPlacementPreview(
            placer.placedGameObjects[index],
            size
        );
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        bool isValid = gridData.CanPlaceObejctAt(gridPosition, objectSize);

        if (isValid)
        {
            // 更新数据
            gridData.MoveObject(originalPosition, gridPosition, objectSize, ID, movingIndex);
            // 移动物体
            objectPlacer.MoveObject(movingIndex, grid.CellToWorld(gridPosition));
            soundFeedback.PlaySound(SoundType.Place);
        }
        else
        {
            soundFeedback.PlaySound(SoundType.wrongPlacement);
        }
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool isValid = gridData.CanPlaceObejctAt(gridPosition, objectSize);
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), isValid);
    }
}