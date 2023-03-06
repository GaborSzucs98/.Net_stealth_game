using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace StealthWPF.ViewModel
{
     public class GridButton : ViewModelBase
    {
        private Int32 _x;
        private Int32 _y;
        private string?  _Content;
        private SolidColorBrush? _Background;

        public Int32 GridX { get { return _x; } }

        public Int32 GridY { get { return _y; } }

        public string? Content
        {
            get { return _Content; }
            set
            {
                if (_Content != value)
                {
                    _Content = value;
                    OnPropertyChanged();
                }
            }
        }

        public SolidColorBrush? Background
        {
            get { return _Background; }
            set
            {
                if (_Background != value)
                {
                    _Background = value;
                    OnPropertyChanged();
                }
            }
        }

        public GridButton(Int32 x, Int32 y) { _x = x; _y = y;}

        public DelegateCommand? ClickCommand { get; set; }
    }
}
