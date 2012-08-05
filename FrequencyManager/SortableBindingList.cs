using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace SDRSharp.FrequencyManager
{
    public class SortableBindingList<T>: BindingList<T>
    {
        private bool _isSorted = false;
        private PropertyDescriptor _sortProperty;
        private ListSortDirection _sortDirection;

        protected override bool SupportsSortingCore
        {
            get { return true; }
        }

        protected override ListSortDirection SortDirectionCore
        {
            get { return _sortDirection; }
        }

        protected override PropertyDescriptor SortPropertyCore
        {
            get { return _sortProperty; }
        }

        protected override bool IsSortedCore
        {
            get { return _isSorted; }
        }

        protected override void ApplySortCore(PropertyDescriptor property, ListSortDirection direction)
        {
            
            var items = (List<T>) this.Items;
            
            if (items != null)
            {                
                var pc = new SortableBindingListComparer<T>(property.Name, direction);
               
                items.Sort(pc);

                _isSorted = true;
                
            }
            else
            {
                _isSorted = false;
            }

            _sortProperty = property;
            _sortDirection = direction;
            
            
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

       
    }

    public class SortableBindingListComparer<T> : IComparer<T>
    {
        private PropertyInfo _sortProperty;
        private ListSortDirection _sortDirection;

        public SortableBindingListComparer(string sortProperty, ListSortDirection sortDirection)
	    {
		    _sortProperty = typeof(T).GetProperty(sortProperty);
		    _sortDirection = sortDirection;
        }

        public int Compare(T x, T y)
        {
            IComparable oX = (IComparable)_sortProperty.GetValue(x, null);
            IComparable oY = (IComparable)_sortProperty.GetValue(y, null);
            
            if (_sortDirection == ListSortDirection.Ascending)
                return oX.CompareTo(oY);
            else
                return oY.CompareTo(oX);
           
        }

    }
}
