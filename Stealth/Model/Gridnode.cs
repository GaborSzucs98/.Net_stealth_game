using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Stealth.Model
{
    public enum Direction
    {
        up,
        right,
        down,
        left
    }

    public enum Status
    {
        Clear,
        Wall,
        Player,
        Guard,
        Seen,
        Exit
    }

    public class Gridnode
    {
        public int Posx { get; set; }

        public int Posy { get; set; }

        public Gridnode? Up { get; set; }

        public Gridnode? Right { get; set; }

        public Gridnode? Down { get; set; }

        public Gridnode? Left { get; set; }

        public Status Status { get; set; }

        public Gridnode (char stat,int x,int y) {

            switch (stat)
            {
                case 'C':
                    Status = Status.Clear;
                    break;
                case 'W':
                    Status = Status.Wall;
                    break;
                case 'P':
                    Status = Status.Player;
                    break;
                case 'G':
                    Status = Status.Guard;
                    break;
                case 'E':
                    Status = Status.Exit;
                    break;
            }

            Posx = x;

            Posy = y;

        }

        public char Getstatus()
        {
            switch (Status)
            {
                case Status.Clear:
                    return 'C';

                case Status.Wall:
                    return 'W';

                case Status.Player:
                    return 'P';

                case Status.Guard:
                    return 'G';

                case Status.Exit:
                    return 'E';

                case Status.Seen:
                    return 'S';

                default:
                    return 'X';
            }

        }

    }

}
