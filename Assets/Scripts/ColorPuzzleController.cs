using UnityEngine;
using UnityEngine.Tilemaps;

public class ColorPuzzleController : MonoBehaviour
{
    [Header("Puzzle Setup")]
    [Tooltip("Drag the puzzle mushrooms here IN ORDER")]
    public SpringMushroom[] puzzleMushrooms;
    
    [Tooltip("The correct sprite index for each mushroom (e.g., 0, 2, 1)")]
    public int[] solutionIndices;

    [Header("Tilemap Feedback")]
    public Tilemap backgroundTilemap;
    [Tooltip("Place empty GameObjects over the tiles you want to change, and drag them here IN ORDER")]
    public Transform[] panelMarkers;
    
    [Tooltip("Drag the actual Tile file from your project folder for the RED / Wrong state")]
    public TileBase wrongTile;
    
    [Tooltip("Drag the actual Tile file from your project folder for the GREEN / Right state")]
    public TileBase rightTile;

    [Header("Door Settings")]
    public GameObject doorToOpen;
    public Vector3 doorOpenOffset = new Vector3(0, 3f, 0);
    public float doorSpeed = 5f;

    private Vector3 doorClosedPos;
    private Vector3 doorTargetPos;
    private bool isSolved = false;
    private Vector3Int[] panelTilePositions; 

    void Start()
    {
        if (doorToOpen != null)
        {
            doorClosedPos = doorToOpen.transform.position;
            doorTargetPos = doorClosedPos;
        }

        if (backgroundTilemap != null && panelMarkers.Length > 0)
        {
            panelTilePositions = new Vector3Int[panelMarkers.Length];
            
            for (int i = 0; i < panelMarkers.Length; i++)
            {
                panelTilePositions[i] = backgroundTilemap.WorldToCell(panelMarkers[i].position);
                
                if (wrongTile != null)
                {
                    backgroundTilemap.SetTile(panelTilePositions[i], wrongTile);
                }
            }
        }
        
        CheckPuzzle();
    }

    void Update()
    {
        if (doorToOpen != null)
        {
            doorToOpen.transform.position = Vector3.Lerp(doorToOpen.transform.position, doorTargetPos, Time.deltaTime * doorSpeed);
        }
    }

    public void CheckPuzzle()
    {
        if (isSolved) return; 

        bool allCorrect = true;

        for (int i = 0; i < puzzleMushrooms.Length; i++)
        {
            bool isThisOneCorrect = (puzzleMushrooms[i].currentColorIndex == solutionIndices[i]);

            if (backgroundTilemap != null && i < panelTilePositions.Length)
            {
                TileBase tileToPlace = isThisOneCorrect ? rightTile : wrongTile;
                if (tileToPlace != null)
                {
                    backgroundTilemap.SetTile(panelTilePositions[i], tileToPlace);
                }
            }

            if (!isThisOneCorrect)
            {
                allCorrect = false;
            }
        }

        if (allCorrect)
        {
            isSolved = true;
            doorTargetPos = doorClosedPos + doorOpenOffset;
            Debug.Log("Mushroom Puzzle Solved!");
        }
    }
}