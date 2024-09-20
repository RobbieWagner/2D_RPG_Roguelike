using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace RobbieWagnerGames.ProcGen
{
#nullable enable
    public class WaveFunctionCollapse
    {
        private static int CountUnsetCells(List<List<ProcGenCell>> grid)
        {
            return grid.SelectMany(x => x).Where(x => x.value == -1).Count();
        }

        public static async Task<List<List<ProcGenCell>>> CreateProceduralGridAsync(int x, int y, GenerationDetails details, System.Random? random = null)
        {
            return await Task.Run(() =>
            {
                return CreateProceduralGrid(x, y, details, random);
            });
        }

        public static List<List<ProcGenCell>> CreateProceduralGrid(int x, int y, GenerationDetails details, System.Random? random = null)
        {
            // initialize resources
            System.Random rand;
            if (random != null)
                rand = random;
            else if (details.seed < 0)
                rand = new System.Random();
            else
                rand = new System.Random(details.seed);
            
            List<int> cellOptions = GenerateCellOptions(details);
            
            List<List<ProcGenCell>> grid = InitializeGrid(x, y, cellOptions);

            // collapse a tile
            int firstX = rand.Next(0, x);
            int firstY = rand.Next(0, y);
            ProcGenCell firstCell = grid[firstY][firstX];


            SetCell(ref grid, details, firstCell, rand);

            // fill out the rest of the grid
            while (CountUnsetCells(grid) > 0 && !grid.SelectMany(x => x).Where(x => x.options.Count <= 0).Any())
            {
                SetNextCell(ref grid, details, rand);
            }

            if (grid.SelectMany(x => x).Where(x => x.options.Count <= 0).Any())
            {
                string printString = "";
                foreach (var list in grid)
                {
                    foreach (var cell in list)
                        printString += cell.value + ", ";
                    printString += "\n";
                }
                throw new InvalidOperationException($"could not complete operation on grid: found no possible tiles to place at a cell\n {printString}");
            }
            // validate completed grid
            if (CountUnsetCells(grid) == 0)
                return grid;

            throw new Exception("Failed to generate grid, please try again.");
        }

        public static List<List<ProcGenCell>> InitializeGrid(int x, int y, List<int> cellOptions)
        {
            List<List<ProcGenCell>> grid = new List<List<ProcGenCell>>();
            for (int i = 0; i < y; i++)
            {
                grid.Add(new List<ProcGenCell>());
                for (int j = 0; j < x; j++)
                {
                    ProcGenCell cell = new ProcGenCell(j, i);
                    cell.options = cellOptions.ToList();
                    grid[i].Add(cell);
                }
            }

            return grid;
        }

        public static List<int> GenerateCellOptions(GenerationDetails details)
        {
            List<int> cellOptions = new List<int>();
            for (int i = 0; i < details.possibilities; i++)
            {
                int weight = -1;
                if (details.weights != null && details.weights.TryGetValue(i, out weight))
                    for (int addition = 0; addition < weight; addition++)
                        cellOptions.Add(i);
                else
                    cellOptions.Add(i);
            }

            return cellOptions;
        }

        public static List<List<ProcGenCell>> CompleteGridList(List<List<ProcGenCell>> grid, GenerationDetails details, System.Random? random = null)
        {
            System.Random rand;
            if (random != null)
                rand = random;
            else if (details.seed < 0)
                rand = new System.Random();
            else
                rand = new System.Random(details.seed);

            List<int> cellOptions = new List<int>();
            for (int i = 0; i < details.possibilities; i++)
            {
                int weight = -1;
                if (details.weights != null && details.weights.TryGetValue(i, out weight))
                    for (int addition = 0; addition < weight; addition++)
                        cellOptions.Add(i);
                else
                    cellOptions.Add(i);
            }

            while (CountUnsetCells(grid) > 0 && !grid.SelectMany(x => x).Where(x => x.options.Count <= 0).Any())
            {
                SetNextCell(ref grid, details, rand);
            }

            if (grid.SelectMany(x => x).Where(x => x.options.Count <= 0).Any())
            {
                string printString = "";
                foreach (var list in grid)
                {
                    foreach (var cell in list)
                        printString += cell.value + ", ";
                    printString += "\n";
                }
                throw new InvalidOperationException($"could not complete operation on grid: found no possible tiles to place at a cell\n {printString}");
            }
            // validate completed grid
            if (CountUnsetCells(grid) == 0)
                return grid;

            throw new Exception("Failed to generate grid, please try again.");
        }

        #region Generation
        private static void SetNextCell(ref List<List<ProcGenCell>> grid, GenerationDetails details, System.Random rand)
        {
            ProcGenCell nextCell = grid.SelectMany(x => x)
                            .Where(cell => cell.value == -1)
                            .OrderBy(x => x.options.Count).FirstOrDefault();

            SetCell(ref grid, details, nextCell, rand);
        }

        public static void SetCell(ref List<List<ProcGenCell>> grid, GenerationDetails details, ProcGenCell cell, System.Random rand)
        {
            int cellValue = cell.options[rand.Next(0, cell.options.Count)];
            cell.value = cellValue;

            //Update adjacent tiles            
            ProcGenCell? above = cell.y < grid.Count-1 ? grid[cell.y+1][cell.x] : null;
            ProcGenCell? below = cell.y > 0 ? grid[cell.y-1][cell.x] : null;
            ProcGenCell? left = cell.x > 0 ? grid[cell.y][cell.x-1] : null;
            ProcGenCell? right = cell.x < grid[0].Count-1 ? grid[cell.y][cell.x+1] : null;

            if (above != null)
                above.options = details.aboveAllowList[cellValue].Where(x => above.options.Contains(x)).ToList();
            if (below != null)
                below.options = details.belowAllowList[cellValue].Where(x => below.options.Contains(x)).ToList();
            if (left != null)
                left.options = details.leftAllowList[cellValue].Where(x => left.options.Contains(x)).ToList();
            if (right != null)
                right.options = details.rightAllowList[cellValue].Where(x => right.options.Contains(x)).ToList();
        }
        #endregion
    }
}