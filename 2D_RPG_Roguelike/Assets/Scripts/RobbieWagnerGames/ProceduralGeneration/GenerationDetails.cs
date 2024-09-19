using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobbieWagnerGames.ProcGen
{
    public class GenerationDetails
    {
        public int possibilities;
        public int seed = -1;
        public Dictionary<int, List<int>> aboveAllowList;
        public Dictionary<int, List<int>> belowAllowList;
        public Dictionary<int, List<int>> leftAllowList;
        public Dictionary<int, List<int>> rightAllowList;
    }
}
