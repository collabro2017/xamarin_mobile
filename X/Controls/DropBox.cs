using System;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Windows.Input;
using System.Diagnostics;

namespace X
{
    public class DropBox : Button, IDrop
    {
        #region Bindable Properties
        public static readonly BindableProperty DateProperty = BindableProperty.Create(nameof(Date), typeof(DateTime), typeof(DropBox), default(DateTime),
                propertyChanged: (bindable, oldValue, newValue) => {
                    var view = bindable as DropBox;
                    view._date = (DateTime)newValue;
                    if (DateTime.Today > view._date)
                    {
                        view.Status = DroppableState.Full;
                        view.Text = "X";
                        view.TextColor = Color.Black;
                    }
                }
        );
        public DateTime Date
        {
            get { return (DateTime)GetValue(DateProperty); }
            set { SetValue(DateProperty, value); }
        }
        #endregion

        #region Public properties
        public DroppableState Status
        {
            get { return _status; }
            set { _status = value; }
        }
        DroppableState _status;

        public bool DropEnabled
        {
            get
            {
                //System.Diagnostics.Debug.WriteLine("Box {0} Enabled {1}\tStatus: {2}\tChildren.Count: {3}", Date.ToString("d"), Status == DroppableState.Accepting && DraggableChildren.Count == 0, Status, DraggableChildren.Count);
                return /* _date >= DateTime.Today && */ Status == DroppableState.Accepting ;
                //return /* _date >= DateTime.Today && */ Status == DroppableState.Accepting && DraggableChildren.Count == 0;
            }
        }

        public bool IntersectedWithDraggable
        {
            get { return _intersected; }
            set { _intersected = value; }
        }
        protected bool _intersected = true;

        public List<IDrag> DraggableChildren
        {
            get { return _draggableChildren; }
            set { _draggableChildren = value; }
        }

        public int position
        {
            get { return _position; }
            set { _position = value; }
        }
        public int _position;

        private List<IDrag> _draggableChildren = new List<IDrag>();
        #endregion

        private DateTime _date;
        public DropBox()
        {
            Status = DroppableState.Accepting;
            IntersectedWithDraggable = false;
        }

        public DropBox(DateTime date)
        {
            _date = date;
            if (DateTime.Today <= _date)
            {
                Status = DroppableState.Accepting;
            }
            else
            {
                Status = DroppableState.Full;
                FontSize = 40;
                IsEnabled = false;
                Text = "X";
            }

            IntersectedWithDraggable = false;
        }

        #region IDroppable implementation
        public void OnDraggableNear()
        {
            //System.Diagnostics.Debug.WriteLine("Box {0} OnDraggableNear", Date.ToString("d"));

            this.BorderWidth = 1.5;
            this.Scale = 1.1;
            this.BorderColor = Color.Transparent;
        }

        public void OnDraggableFar()
        {
            //System.Diagnostics.Debug.WriteLine("Box {0} OnDraggableFar", Date.ToString("d"));

            this.BorderWidth = 1;
            this.Scale = 1;
            //this.BorderColor = Color.Black;
        }

        public void OnDraggableDropped(IDrag draggable)
        {
            //System.Diagnostics.Debug.WriteLine("Box {0} OnDraggableDropped", Date.ToString("d"));
            this.Scale = 1;

            if (DraggableChildren == null)
                DraggableChildren = new List<IDrag>();

            DraggableChildren.Add(draggable);

            Status = DroppableState.Accepting;
            Debug.WriteLine("DraggableChildren: "+ DraggableChildren.Count);
        }


        public void OnDraggableRemoved(IDrag draggable)
        {
            //System.Diagnostics.Debug.WriteLine("Box {0} OnDraggableRemoved", Date.ToString("d"));

            DraggableChildren.Remove(draggable);
            Status = DroppableState.Accepting;
            Debug.WriteLine("RemovedChildren"+ DraggableChildren.Count);

        }
        #endregion

    }
}

