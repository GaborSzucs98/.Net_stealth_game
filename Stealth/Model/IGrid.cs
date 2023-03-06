using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stealth.Model
{
    public interface IGrid
    {
        string FileContent { get; }

        public int Size { get;}

        public Gridnode[,] gridnodes { get; }

        public Gridnode Player { get; }

        void Load();

        int MovePlayer(char dir);

        void Writeout();

        bool MoveGuards();

    }
}
