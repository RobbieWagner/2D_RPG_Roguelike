using AYellowpaper.SerializedCollections;
using RobbieWagnerGames.ProcGen;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class GridController : MonoBehaviour
{
    [SerializeField] private int possibilities = 16;
    [SerializeField] private List<MapTile> tilePrefabs;
    [SerializeField] private int seed;
    [SerializeField] private int columns = 10;
    [SerializeField] private int rows = 10;
    [SerializeField] private Vector2 cellSize;
    [SerializeField] private Transform gridParent;

    [SerializeField]
    [SerializedDictionary("above", "values")] private SerializedDictionary<int, List<int>> aboveAllowList;
    [SerializeField]
    [SerializedDictionary("below", "values")] private SerializedDictionary<int, List<int>> belowAllowList;
    [SerializeField]
    [SerializedDictionary("left", "values")] private SerializedDictionary<int, List<int>> leftAllowList;
    [SerializeField]
    [SerializedDictionary("right", "values")] private SerializedDictionary<int, List<int>> rightAllowList;

    [SerializeField]
    [SerializedDictionary("possibility", "weight")] private SerializedDictionary<int, int> weights;

    [SerializeField] private List<List<MapTile>> gridInstance = new List<List<MapTile>>();

    public static GridController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        StartCoroutine(InitializeGrid());
    }

    public IEnumerator InitializeGrid()
    {
        // Setup details and other input
        GenerationDetails details = new GenerationDetails();
        System.Random random = new System.Random(seed);
        details.seed = seed;
        details.possibilities = possibilities;
        details.aboveAllowList = aboveAllowList;
        details.belowAllowList = belowAllowList;
        details.leftAllowList = leftAllowList;
        details.rightAllowList = rightAllowList;
        details.weights = weights;

        List<List<ProcGenCell>> grid = GenerateGrid(columns, rows, details, random); 
                                        //WaveFunctionCollapse.CreateProceduralGrid(columns, rows, details, random); //GenerateGrid(columns, rows, details, random);

        for (int i = grid.Count - 1; i >= 0; i--)
        {
            var list = grid[i];
            List<MapTile> newRow = new List<MapTile>();
            for (int j = 0; j < list.Count; j++)
            {
                var cell = list[j];
                List<MapTile> tileOptions = tilePrefabs.Where(x => x.possibilityValue == cell.value).ToList();
                int selectedOptionIndex = random.Next(0, tileOptions.Count);
                MapTile newMapTile = Instantiate(tileOptions[selectedOptionIndex], gridParent);
                newMapTile.transform.localPosition = new Vector3(cellSize.x * j,0, cellSize.y * (i - grid.Count));
                newRow.Add(newMapTile);
            }
            gridInstance.Add(newRow);
            yield return null;
        }
    }

    private List<List<ProcGenCell>> GenerateGrid(int columns, int rows, GenerationDetails details, System.Random random)
    {
        List<int> cellOptions = WaveFunctionCollapse.GenerateCellOptions(details);

        // Create an initial grid with all bordering blank
        List<List<ProcGenCell>> grid = WaveFunctionCollapse.InitializeGrid(columns, rows, cellOptions);

        foreach (ProcGenCell cell in grid[0])
            WaveFunctionCollapse.CollapseCell(ref grid, details, cell, random, 0);
        foreach(ProcGenCell cell in grid[grid.Count-1])
            WaveFunctionCollapse.CollapseCell(ref grid, details, cell, random, 0);
        for (int i = 1; i < grid.Count - 1; i++)
        {
            WaveFunctionCollapse.CollapseCell(ref grid, details, grid[i][0], random, 0);
            WaveFunctionCollapse.CollapseCell(ref grid, details, grid[i][grid[0].Count - 1], random, 0);
        }

        WaveFunctionCollapse.CollapseCell(ref grid, details, grid[random.Next(1, grid.Count - 2)][random.Next(1, grid[0].Count - 2)], random, 15);

        return WaveFunctionCollapse.CompleteGridList(grid, details, random);
    }
}
