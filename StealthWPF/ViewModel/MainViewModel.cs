using iText.Layout.Properties;
using Microsoft.Win32;
using StealthWPF.Model;
using StealthWPF.Persistence;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static iText.Signatures.LtvVerification;

namespace StealthWPF.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private Model.Grid? model;
        private bool GameOver;
        private int GameWon;

        private GridButton PlayerPos;

        private int _Size;
        private int _Lepes;

        public int Size {

            get { return _Size; }
            private set
            {
                if (_Size != value)
                {
                    _Size = value;
                    OnPropertyChanged();
                }
            }
        }
        public int Lepes {

            get { return _Lepes; }
            private set
            {
                if (_Lepes != value)
                {
                    _Lepes = value;
                    OnPropertyChanged();
                }
            }
        }

        private const int leptek = 2;
        public ObservableCollection<GridButton>? GridButtons { get; set; }
        public DelegateCommand NewgameCommand { get; set; }
        public DelegateCommand PauseCommand { get; set; }
        public DelegateCommand Keypress { get; set; }

        public MainViewModel()
        {
            Lepes = 0;
            GameWon = 0;
            GameOver = false;
            Size = 0;


            NewgameCommand = new DelegateCommand(param =>
            {
                    
                OpenFileDialog openFileDialog = new OpenFileDialog();
                
                    openFileDialog.InitialDirectory = "C:\\";
                    openFileDialog.Filter = "Text files (*.txt)|*.txt|PDF files (*.pdf)|*.pdf";
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == true)
                    {
                        IFileManager? fileManager = FileManagerFactory.CreateForPath(openFileDialog.FileName);

                        if (fileManager == null)
                        {
                            MessageBox.Show("File reading is unsuccessful!\nUnsupported file format.",
                            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        try
                        {
                            model = new Model.Grid(fileManager);
                            model.Load();
                            Size = model!.Size;
                            Draw();
                    }
                        catch (FileManagerException ex)
                        {
                            MessageBox.Show("File reading is unsuccessful!\n" + ex.Message,
                            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }

            });

            PauseCommand = new DelegateCommand(param =>
            {
                MessageBox.Show("Game is Paused!\n", "Pause", MessageBoxButton.OK, MessageBoxImage.Information);
            });

            GridButtons = new ObservableCollection<GridButton>();

            Keypress = new DelegateCommand(param =>
            {
                Keystep(Convert.ToChar(param!));
            });
        }

        private void Draw()
        {
            GridButton newbutton;

            for (int i = 0; i < model!.Size; i++)
            {
                for (int j = 0; j < model.Size; j++)
                {
                    newbutton = new GridButton(i, j);

                    newbutton.ClickCommand = new DelegateCommand(param =>
                    {
                        if (param is GridButton button)
                        {
                            GridButton_Click(button);
                        }
                    });

                    switch (model.gridnodes[i, j].Status)
                    {
                        case Status.Clear:
                            newbutton.Background = Brushes.White;
                            break;
                        case Status.Exit:
                            newbutton.Background = Brushes.Blue;
                            newbutton.Content = "E";
                            break;
                        case Status.Guard:
                            newbutton.Background = Brushes.Red;
                            newbutton.Content = "G";
                            break;
                        case Status.Wall:
                            newbutton.Background = Brushes.Black;
                            break;
                        case Status.Player:
                            newbutton.Background = Brushes.Orange;
                            newbutton.Content = "P";
                            PlayerPos = newbutton;
                            break;
                        case Status.Seen:
                            newbutton.Background = Brushes.Yellow;
                            break;
                        default:
                            newbutton.Background = Brushes.White;
                            break;
                    }
                    GridButtons!.Add(newbutton);
                }
            }
        }
        public GridButton Findbutton(int x, int y) {
            for (int i = 1; i < GridButtons!.Count; i++) {
                if (GridButtons[i].GridX == x && GridButtons[i].GridY == y) {
                    return GridButtons[i];
                }
            }
            return null!;
        }

        public void Keystep(char key) {
            switch (key)
            {
                case 'w':
                    GridButton_Click(Findbutton(PlayerPos.GridX + 1, PlayerPos.GridY));
                    break;
                case 'a':
                    GridButton_Click(Findbutton(PlayerPos.GridX, PlayerPos.GridY-1));
                    break;
                case 's':
                    GridButton_Click(Findbutton(PlayerPos.GridX - 1, PlayerPos.GridY));
                    break;
                case 'd':
                    GridButton_Click(Findbutton(PlayerPos.GridX, PlayerPos.GridY+1));
                    break;
                default:

                    break;
            }
        }

        public void GridButton_Click(GridButton button)
        {
            if (button != null)
            {
                int x = button.GridX;
                int y = button.GridY;

                GameWon = 0;

                if (x == model!.Player.Posx + 1 && y == model.Player.Posy)
                {
                    GameWon = model.MovePlayer('s');
                    Lepes++;
                    if (Lepes % leptek == 0) { GameOver = model.MoveGuards(); }
                    Coloring();
                    PlayerPos = button;
                }
                else if (x == model.Player.Posx && y == model.Player.Posy + 1)
                {
                    GameWon = model.MovePlayer('d');
                    Lepes++;
                    if (Lepes % leptek == 0) { GameOver = model.MoveGuards(); }
                    Coloring();
                    PlayerPos = button;
                }
                else if (x == model.Player.Posx - 1 && y == model.Player.Posy)
                {
                    GameWon = model.MovePlayer('w');
                    Lepes++;
                    if (Lepes % leptek == 0) { GameOver = model.MoveGuards(); }
                    Coloring();
                    PlayerPos = button;
                }
                else if (x == model.Player.Posx && y == model.Player.Posy - 1)
                {
                    GameWon = model.MovePlayer('a');
                    Lepes++;
                    if (Lepes % leptek == 0) { GameOver = model.MoveGuards(); }
                    Coloring();
                    PlayerPos = button;
                }
                else
                {

                }

                if (GameOver)
                {
                    MessageBox.Show("Megláttak az őrök!\n", "LOSE", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    Lepes = 0;
                    GridButtons!.Clear();
                }

                if (GameWon == 1)
                {
                    MessageBox.Show("Sikeresen kijutottál!\n", "WIN", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    Lepes = 0;
                    GridButtons!.Clear();
                }
            }
        }

        private void Coloring()
        {
            int index = 0;
            GridButton current;

            for (Int32 i = 0; i < model!.Size; i++)
            {
                for (Int32 j = 0; j < model!.Size; j++)
                {
                    current = GridButtons![index];
                    switch (model.gridnodes[i, j].Status)
                    {
                        case Status.Clear:
                            current.Background = Brushes.White;
                            current.Content = "";
                            break;
                        case Status.Exit:
                            current.Background = Brushes.Blue;
                            current.Content = "E";
                            break;
                        case Status.Guard:
                            current.Background = Brushes.Red;
                            current.Content = "G";
                            break;
                        case Status.Wall:
                            current.Background = Brushes.Black;
                            current.Content = "";
                            break;
                        case Status.Player:
                            current.Background = Brushes.Orange;
                            current.Content = "P";
                            break;
                        case Status.Seen:
                            current.Background = Brushes.Yellow;
                            current.Content = "";
                            break;
                        default:
                            current.Background = Brushes.White;
                            current.Content = "";
                            break;
                    }
                    index++;
                }
            }
        }
    }
}