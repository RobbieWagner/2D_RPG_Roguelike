using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;

namespace RobbieWagnerGames.ProcGen
{
    #nullable enable
    public class WaveFunctionCollapse
    {
        private static int CountUnsetCells(List<List<ProcGenCell>> grid)
        {
            return grid.SelectMany(x => x).Where(x => x.value == -1).Count();
        }

        public static async Task<List<List<ProcGenCell>>> CreateProceduralGridAsync(int x, int y, GenerationDetails details)
        {
            return await Task.Run(() =>
            {
                return CreateProceduralGrid(x, y, details);
            });
        }

        public static List<List<ProcGenCell>> CreateProceduralGrid(int x, int y, GenerationDetails details)
        {
            // initialize resources
            List<List<ProcGenCell>> grid = new List<List<ProcGenCell>>();

            Random rand;
            if(details.seed < 0)
                rand = new Random();
            else
                rand = new Random(details.seed);

            List<int> cellOptions = new List<int>();
            for (int i = 0; i < details.possibilities; i++)
                cellOptions.Add(i);

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
            
            // collapse a tile
            int firstX = rand.Next(0, x);
            int firstY = rand.Next(0, y);
            ProcGenCell firstCell = grid[firstY][firstX];


            SetCell(ref grid, details, firstCell, rand);

            // fill out the rest of the grid
            while(CountUnsetCells(grid) > 0 && !grid.SelectMany(x => x).Where(x => x.options.Count <= 0).Any())
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
        private static void SetNextCell(ref List<List<ProcGenCell>> grid, GenerationDetails details, Random rand)
        {
            ProcGenCell nextCell = grid.SelectMany(x => x)
                            .Where(cell => cell.value == -1)
                            .OrderBy(x => x.options.Count).FirstOrDefault();

            SetCell(ref grid, details, nextCell, rand);
        }

        private static void SetCell(ref List<List<ProcGenCell>> grid, GenerationDetails details, ProcGenCell cell, Random rand)
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