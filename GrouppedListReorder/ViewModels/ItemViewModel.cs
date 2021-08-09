using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.CommunityToolkit.ObjectModel;

namespace GrouppedListReorder.ViewModels
{
    public class ItemsGroupViewModel : ObservableCollection<ItemViewModel>, INotifyPropertyChanged
    {
        public string Name { get; set; }

        public new event PropertyChangedEventHandler PropertyChanged;

        private bool _isBeingDragged;

        public bool IsBeingDragged
        {
            get { return _isBeingDragged; }
            set
            {
                _isBeingDragged = value;
                NotifyPropertyChanged();
            }
        }

        private bool _isBeingDraggedOver;

        public bool IsBeingDraggedOver
        {
            get { return _isBeingDraggedOver; }
            set
            {
                _isBeingDraggedOver = value;
                NotifyPropertyChanged();
            }
        }

        public ItemsGroupViewModel(string name, IEnumerable<ItemViewModel> items)
            : base(items)
        {
            Name = name;
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ItemViewModel : ObservableObject
    {
        public string Category { get; set; }
        public string Title { get; set; }
    }
}
