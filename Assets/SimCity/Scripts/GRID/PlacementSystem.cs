using SimCity.FinalController;
using System;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField]
    private InputManager inputManager;

    [SerializeField]
    private Grid grid;

    [SerializeField]
    private ObjectsDatabaseSO database;

    [SerializeField]
    private GameObject gridVisualization;

    [SerializeField]
    private AudioClip wrongPlacementClip;

    [SerializeField]
    private AudioSource source;

    private GridData gridData;

    [SerializeField]
    private PreviewSystem preview;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    [SerializeField]
    private ObjectPlacer objectPlacer;

    IBuildingState buildingState;

    [SerializeField]
    private SoundFeedback soundFeedback;

    private UIInput _UIInput;
    GameObject targetGameObject;

    private void Start()
    {
        targetGameObject = GameObject.Find("Player");
        updateEditMode();
        gridData = new();
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        gridVisualization.SetActive(_UIInput.EditModeToggleOn);
        buildingState = new PlacementState(ID, grid, preview, database, gridData, objectPlacer, soundFeedback);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    public void StartRemoving()
    {
        StopPlacement();
        gridVisualization.SetActive(_UIInput.EditModeToggleOn);
        buildingState = new RemovingState(grid, preview, gridData, objectPlacer, soundFeedback);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    private void PlaceStructure()
    {
        if (inputManager.IsPointerOverUI())
        {
            return;
        }

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        buildingState.OnAction(gridPosition);
    }

    private void StopPlacement()
    {
        soundFeedback.PlaySound(SoundType.Click);

        if (buildingState == null)
            return;

        gridVisualization.SetActive(false);
        buildingState.EndState();
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        lastDetectedPosition = Vector3Int.zero;
        buildingState = null;
    }

    public void updateEditMode()
    {
        if (targetGameObject != null)
        {
            _UIInput = targetGameObject.GetComponent<UIInput>();
        }
        else
        {
            Debug.LogError("未找到指定名称的游戏对象");
        }
        gridVisualization.SetActive(_UIInput.EditModeToggleOn);
    }

    private void Update()
    {
        if (buildingState == null)
            return;

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        if (lastDetectedPosition != gridPosition)
        {
            buildingState.UpdateState(gridPosition);
            lastDetectedPosition = gridPosition;
        }
    }
}