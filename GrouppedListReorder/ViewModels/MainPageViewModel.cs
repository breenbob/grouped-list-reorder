﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GrouppedListReorder.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace GrouppedListReorder.ViewModels
{
    public class MainPageViewModel : ObservableObject
    {
        private ObservableCollection<ItemViewModel> _items = new ObservableCollection<ItemViewModel>();

        public ObservableCollection<ItemViewModel> Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }

        private ObservableCollection<ItemsGroupViewModel> _groupedItems = new ObservableCollection<ItemsGroupViewModel>();

        public ObservableCollection<ItemsGroupViewModel> GroupedItems
        {
            get { return _groupedItems; }
            set { SetProperty(ref _groupedItems, value); }
        }

        public ICommand StateRefresh { get; }

        public ICommand StateReset { get; }

        public ICommand StateTest { get; }

        public ICommand ItemDragged { get; }

        public ICommand ItemDraggedOver { get; }

        public ICommand ItemDragLeave { get; }

        public ICommand ItemDropped { get; }

        public MainPageViewModel()
        {
            StateRefresh = new Command(OnStateRefresh);
            StateReset = new Command(OnStateReset);
            StateTest = new Command(OnStateTest);
            ItemDragged = new Command<ItemsGroupViewModel>(OnItemDragged);
            ItemDraggedOver = new Command<ItemsGroupViewModel>(OnItemDraggedOver);
            ItemDragLeave = new Command<ItemsGroupViewModel>(OnItemDragLeave);
            ItemDropped = new Command<ItemsGroupViewModel>(i => OnItemDropped(i));
            ResetItemsState();
        }

        private void OnStateRefresh()
        {
            Debug.WriteLine($"OnStateRefresh");
            OnPropertyChanged(nameof(Items));
            PrintItemsState();
        }

        private void OnStateReset()
        {
            Debug.WriteLine($"OnStateReset");
            ResetItemsState();
            PrintItemsState();
        }

        private void OnStateTest()
        {
            Items.RemoveAt(4);
            Items.Insert(0, new ItemViewModel { Title = "Item new 1" });
            PrintItemsState();
        }

        private void OnItemDragged(ItemsGroupViewModel item)
        {
            Debug.WriteLine($"OnItemDragged: {item?.Name}");
            GroupedItems.ForEach(i => i.IsBeingDragged = item == i);
        }

        private void OnItemDraggedOver(ItemsGroupViewModel item)
        {
            Debug.WriteLine($"OnItemDraggedOver: {item?.Name}");
            var itemBeingDragged = GroupedItems.FirstOrDefault(i => i.IsBeingDragged);
            GroupedItems.ForEach(i => i.IsBeingDraggedOver = item == i && item != itemBeingDragged);
        }

        private void OnItemDragLeave(ItemsGroupViewModel item)
        {
            Debug.WriteLine($"OnItemDragLeave: {item?.Name}");
            GroupedItems.ForEach(i => i.IsBeingDraggedOver = false);
        }

        private async Task OnItemDropped(ItemsGroupViewModel item)
        {
            var itemToMove = GroupedItems.First(i => i.IsBeingDragged);
            var itemToInsertBefore = item;

            if (itemToMove == null || itemToInsertBefore == null || itemToMove == itemToInsertBefore)
                return;

            //var categoryToMoveFrom = GroupedItems.First(g => g.Contains(itemToMove));
            //categoryToMoveFrom.Remove(itemToMove);

            // Wait for remove animation to be completed
            // https://github.com/xamarin/Xamarin.Forms/issues/13791
            // await Task.Delay(1000);

            //var categoryToMoveTo = GroupedItems.First(g => g.Contains(itemToInsertBefore));
            var currentIndex = GroupedItems.IndexOf(itemToMove);
            var insertAtIndex = GroupedItems.IndexOf(itemToInsertBefore);
            //itemToMove.Category = categoryToMoveFrom.Name;
            //categoryToMoveTo.Insert(insertAtIndex, itemToMove);
            GroupedItems.Move(currentIndex, insertAtIndex);
            itemToMove.IsBeingDragged = false;
            itemToInsertBefore.IsBeingDraggedOver = false;
            Debug.WriteLine($"OnItemDropped: [{itemToMove?.Name}] => [{itemToInsertBefore?.Name}], target index = [{insertAtIndex}]");

            PrintItemsState();
        }

        private void ResetItemsState()
        {
            Items.Clear();

            Items.Add(new ItemViewModel { Category = "Category 1", Title = "Item 1" });
            Items.Add(new ItemViewModel { Category = "Category 1", Title = "Item 2" });
            Items.Add(new ItemViewModel { Category = "Category 2", Title = "Item 3" });
            Items.Add(new ItemViewModel { Category = "Category 2", Title = "Item 4" });
            Items.Add(new ItemViewModel { Category = "Category 2", Title = "Item 5" });
            Items.Add(new ItemViewModel { Category = "Category 2", Title = "Item 6" });
            Items.Add(new ItemViewModel { Category = "Category 3", Title = "Item 7" });
            Items.Add(new ItemViewModel { Category = "Category 3", Title = "Item 8" });
            Items.Add(new ItemViewModel { Category = "Category 3", Title = "Item 9" });
            Items.Add(new ItemViewModel { Category = "Category 3", Title = "Item 10" });
            Items.Add(new ItemViewModel { Category = "Category 3", Title = "Item 11" });
            Items.Add(new ItemViewModel { Category = "Category 3", Title = "Item 12" });
            Items.Add(new ItemViewModel { Category = "Category 3", Title = "Item 13" });
            Items.Add(new ItemViewModel { Category = "Category 3", Title = "Item 14" });
            Items.Add(new ItemViewModel { Category = "Category 3", Title = "Item 15" });

            GroupedItems = Items
                .GroupBy(i => i.Category)
                .Select(g => new ItemsGroupViewModel(g.Key, g))
                .ToObservableCollection();

            // Example of a group with no items - needs to be added after above linq statement as grouping items by category
            // will mean these groups are stripped out if they have no items
            GroupedItems.Insert(0, new ItemsGroupViewModel("No Items Category 1", new List<ItemViewModel>()));
            GroupedItems.Insert(1, new ItemsGroupViewModel("No Items Category 2", new List<ItemViewModel>()));
        }

        private void PrintItemsState()
        {
            Debug.WriteLine($"Items {Items.Count}, state:");
            for (int i = 0; i < Items.Count; i++)
            {
                Debug.WriteLine($"\t{i}: Group: {Items[i].Category} | Item: {Items[i].Title}");
            }
        }
    }
}
