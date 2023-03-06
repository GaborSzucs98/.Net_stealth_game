using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Asn1.X500;
using Stealth.Persistence;

namespace Stealth.Model
{
    public class Grid : IGrid
    {
        private readonly IFileManager _fileManager;

        public int Size { get; private set; }

        public string FileContent { get; private set; }

        public Gridnode Player { get; private set; }

        private List<Gridnode> Guards { get; set; } 

        private Direction[] Directions { get; set; }

        public Gridnode[,] gridnodes { get; private set; }

        public Grid(IFileManager fileManager) {

            _fileManager = fileManager;
            FileContent = string.Empty;
            Guards = new List<Gridnode>();

        }

        public void Load() {

            FileContent = _fileManager.Load();

            string[] Tiles = FileContent
                .Split()
                .Where(s => s.Length > 0)
                .ToArray();

            Size = Int32.Parse(Tiles[Tiles.Length-1]);

            gridnodes = new Gridnode[Size,Size];

            int db = 0;

            for (int i = 0; i < Size; ++i) {
                for (int j = 0; j < Size; ++j) {
                    char current = Tiles[db].ToCharArray()[0];
                    if (current == 'P')
                    {
                        Gridnode entity = new Gridnode(current, i, j);
                        Player = entity;
                        gridnodes[i, j] = entity;
                    }
                    else
                    {
                        if (current == 'G')
                        {
                            Gridnode Guard = new Gridnode(current, i, j);
                            Guards.Add(Guard);
                            gridnodes[i, j] = Guard;
                        }
                        else
                        {

                            gridnodes[i, j] = new Gridnode(current, i, j);

                        }

                    }
                    db++;
                }
            
            }

            int guardsn = Guards.Count;
            Directions = new Direction[guardsn];

            Random rand = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < guardsn; ++i)
            {
                Directions[i] = (Direction)(rand.Next() % 4);
            }

            for (int i = 0; i < Size; ++i)
            {
                for (int j = 0; j < Size; ++j)
                {
                    if (i == 0)
                    {
                        gridnodes[i, j].Up = null;
                    }
                    else 
                    {
                        gridnodes[i, j].Up = gridnodes[i - 1, j];
                    }

                    if (i == Size - 1)
                    {
                        gridnodes[i, j].Down = null;
                    }
                    else
                    {
                        gridnodes[i, j].Down = gridnodes[i + 1, j];
                    }

                    if (j == 0)
                    {
                        gridnodes[i, j].Left = null;
                    }
                    else
                    {
                        gridnodes[i, j].Left = gridnodes[i, j - 1];
                    }

                    if (j == Size - 1)
                    {
                        gridnodes[i, j].Right = null;
                    }
                    else
                    {
                        gridnodes[i, j].Right = gridnodes[i, j + 1];
                    }

                }

            }

            for (int i = 0; i < guardsn; ++i)
            {
                CheckZones(i);
            }


        }

        public int MovePlayer(char dir) {

            Gridnode? next;

                switch (dir)
                {
                    case 'w':
                        next = gridnodes[Player.Posx, Player.Posy].Up;
                        break;
                    case 'a':
                        next = gridnodes[Player.Posx, Player.Posy].Left;
                        break;
                    case 's':
                        next = gridnodes[Player.Posx, Player.Posy].Down;
                        break;
                    case 'd':
                        next = gridnodes[Player.Posx, Player.Posy].Right;
                        break;
                    default:
                        next = null;
                        break;
                }

            if (next == null || next.Status == Status.Wall)
            {
                return 2;
            }
            else if (next.Status == Status.Exit) 
            {
                Player.Status = Status.Clear;
                next.Status = Status.Player;
                Player = next;
                return 1;
            }
            else {
                Player.Status = Status.Clear;
                next.Status = Status.Player;
                Player = next;
                return 0;
            }
       
        }

        public void Writeout()
        {
            Console.WriteLine("---------------------");
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    Console.Write($"|{gridnodes[i, j].Getstatus()}");
                }
                Console.WriteLine("|");

                Console.WriteLine("---------------------");
            }

        }

        public bool MoveGuards()
        {
            bool found = false;
            ClearSeen();
            for (int i = 0; i < Guards.Count; ++i) {

                MoveGuard(i);
                found = CheckZones(i) || found;

            }
            return found;
        }

        private void MoveGuard(int i) 
        {
            Direction nextdir = Directions[i];
            Gridnode guard = Guards[i];
            bool success = false;
            Gridnode? next;
            Random rand = new Random(DateTime.Now.Millisecond);

            do
            {
                switch (nextdir)
                {
                    case Direction.up:
                        next = gridnodes[guard.Posx, guard.Posy].Up;
                        break;
                    case Direction.left:
                        next = gridnodes[guard.Posx, guard.Posy].Left;
                        break;
                    case Direction.down:
                        next = gridnodes[guard.Posx, guard.Posy].Down;
                        break;
                    case Direction.right:
                        next = gridnodes[guard.Posx, guard.Posy].Right;
                        break;
                    default:
                        next = null;
                        break;
                }

                if (next == null || next.Status == Status.Wall || next.Status == Status.Exit)
                {
                    nextdir = (Direction)(rand.Next() % 4);
                }
                else
                {
                    guard.Status = Status.Clear;
                    next.Status = Status.Guard;
                    Guards[i] = next;
                    Directions[i] = nextdir;

                    success = true;
                }

            } while (!success);
        }

        private static int Next(int x) {
            if (x < 3) { return x + 1; }
            else { return 0; }
        }

        private static int Prev(int x)
        {
            if (x > 1) { return x - 1; }
            else { return 3; }
        }

        private static Gridnode? Checkzone(int x, Gridnode gridnode) {
            
            switch (x)
            {
                case 0:
                    return gridnode.Up;
                    
                case 1:
                    return gridnode.Left;

                case 2:
                    return gridnode.Down;

                case 3:
                    return gridnode.Right;

                default:
                    return null;
            }

        }
        
        private void ClearSeen() {
            for (int i = 0; i < Size; ++i) {
                for (int j = 0; j < Size; ++j) {
                    if (gridnodes[i, j].Status == Status.Seen) {
                        gridnodes[i, j].Status = Status.Clear;
                    }
                }
            }
        }

        private static bool Check(Gridnode node)
        {
            bool found = false;
            if (node.Status == Status.Player) { found = true; }
            if (node.Status == Status.Clear)
            {
                node.Status = Status.Seen;
            }

            return found;
        }

        private bool CheckZones(int i) 
        {
            bool found = false;

            Gridnode guard = Guards[i];

            for (int k = 0; k < 4; ++k)
            {
                if (Checkzone(k, guard) != null)
                {
                    if (Checkzone(k, guard)!.Status != Status.Wall)
                    {
                        found = Check(Checkzone(k, guard)!) || found;

                        if (Checkzone(k, Checkzone(k, guard)!) != null)
                        {
                            found = Check(Checkzone(k, Checkzone(k, guard)!)!) || found;

                            if (Checkzone(Next(k), Checkzone(k, Checkzone(k, guard)!)!) != null) 
                            {
                                found = Check(Checkzone(Next(k), Checkzone(k, Checkzone(k, guard)!)!)!) || found;
                            }
                            if (Checkzone(Prev(k), Checkzone(k, Checkzone(k, guard)!)!) != null)
                            {
                                found = Check(Checkzone(Prev(k), Checkzone(k, Checkzone(k, guard)!)!)!) || found;
                            }
                        }
                    }

                    if (Checkzone(Next(k), Checkzone(k, guard)!) != null)
                    {
                        if (Checkzone(Next(k), Checkzone(k, guard)!)!.Status != Status.Wall)
                        {
                            found = Check(Checkzone(Next(k), Checkzone(k, guard)!)!) || found;

                            if (Checkzone(Next(k),Checkzone(Next(k), Checkzone(k, guard)!)!) != null)
                            {
                                found = Check(Checkzone(Next(k), Checkzone(Next(k), Checkzone(k, guard)!)!)!) || found;
                                if (Checkzone(k, Checkzone(Next(k), Checkzone(Next(k), Checkzone(k, guard)!)!)!) != null)
                                {
                                    found = Check(Checkzone(k, Checkzone(Next(k), Checkzone(k, guard)!)!)!) || found;
                                    found = Check(Checkzone(Next(k), Checkzone(k, Checkzone(Next(k), Checkzone(k, guard)!)!)!)!) || found;
                                }
                            }
                        }
                    }
                }

            }

            return found;
        }




    }
}
