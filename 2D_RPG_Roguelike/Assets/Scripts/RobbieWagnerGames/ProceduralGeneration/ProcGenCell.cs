using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobbieWagnerGames.ProcGen
{
    public enum CellType
    {
        BLANK,
        ROAD
    }

    public class ProcGenCell
    {
        public int value = -1;
        public int x;
        public int y;
        public List<int> options;

        public ProcGenCell()
        {

        }

        public ProcGenCell(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            string returnValue = "";
            string optionsList = "{";
            foreach (int i in options)
            {
                optionsList += i.ToString() + ",";
            }
            optionsList += "}";
            returnValue = $"({x},{y}):{optionsList}";

            return returnValue;
        }
    }
}
