using AYellowpaper.SerializedCollections;
using RobbieWagnerGames;
using RobbieWagnerGames.ProcGen;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWaveFunctionCollapse : MonoBehaviour
{
    [SerializeField] private int possibilities;
    [SerializeField] private int seed;
    [SerializeField] private int xSize;
    [SerializeField] private int ySize;

    [SerializeField]
    [SerializedDictionary("above", "values")] private SerializedDictionary<int, List<int>> aboveAllowList;
    [SerializeField]
    [SerializedDictionary("above", "values")] private SerializedDictionary<int, List<int>> belowAllowList;
    [SerializeField]
    [SerializedDictionary("above", "values")] private SerializedDictionary<int, List<int>> leftAllowList;
    [SerializeField]
    [SerializedDictionary("above", "values")] private SerializedDictionary<int, List<int>> rightAllowList;

    private void Awake()
    {
        GenerationDetails details = new GenerationDetails();
        details.seed = seed;
        details.possibilities = possibilities;
        details.aboveAllowList = aboveAllowList;
        details.belowAllowList = belowAllowList;
        details.leftAllowList = leftAllowList;
        details.rightAllowList = rightAllowList;

        List<List<ProcGenCell>> grid = WaveFunctionCollapse.CreateProceduralGrid(xSize, ySize, details);

        string printString = "";
        for(int i = grid.Count-1; i >= 0; i--)
        {
            var list = grid[i];
            foreach (var cell in list)
                printString += cell.value + ", ";
            printString += "\n";
        }
        Debug.Log(printString);
    }
}
