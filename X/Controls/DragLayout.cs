using System;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace X
{
    public class DragLayout : RelativeLayout
    {
        public static readonly BindableProperty DraggingProperty =
            BindableProperty.Create("Dragging", typeof(bool), typeof(DragLayout), default(bool));
        public bool Dragging
        {
            get { return (bool)GetValue(DraggingProperty); }
            set { SetValue(DraggingProperty, value); }
        }

        public static readonly BindableProperty GestureEnabledProperty = BindableProperty.Create(nameof(GestureEnabled), typeof(bool), typeof(DragLayout), true);
        public bool GestureEnabled
        {
            get { return (bool)GetValue(GestureEnabledProperty); }
            set { SetValue(GestureEnabledProperty, value); }
        }

        public double DragX
        {
            get
            {
                return _dragX;
            }
        }
        protected double _dragX;
        public double DragY
        {
            get
            {
                return _dragY;
            }
        }
        protected double _dragY;

        public bool IDragTouched(double x, double y, out IDrag draggable)
        {
            draggable = null;
            foreach (var child in DraggableChildren)
            {
                if (!child.DragEnabled)
                    continue;

                var view = child as View;
                if (view.Bounds.Contains(x, y))
                {
                    draggable = child;
                    if (draggable != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private double _currDraggableWidth = -1, _currDraggableHeight = -1;

        public void HandleTouch(double x, double y)
        {
            _dragX = x;
            _dragY = y;
            //System.Diagnostics.Debug.WriteLine("Handle touch");
            foreach (var child in DraggableChildren)
            {
                var view = child as View;
                //System.Diagnostics.Debug.WriteLine("status: {0}", child.Status);
                if (child.Status == DragState.Started)
                {
                    child.Status = DragState.Running;
                    view.Opacity = 0.5;
                    this.RaiseChild(view);
                }
                else if (child.Status == DragState.Running)
                {
                    if (_currDraggableWidth < 0 && _currDraggableHeight < 0)
                    {
                        var request = view.Measure(double.PositiveInfinity, double.PositiveInfinity);
                        var w = request.Request.Width;
                        var h = request.Request.Height;
                        _currDraggableWidth = w;
                        _currDraggableHeight = h;
                    }
                    var rect = new Rectangle(x - _currDraggableWidth / 2, y - _currDraggableHeight / 2, _currDraggableWidth, _currDraggableHeight); // make sure the draggable is centered on the user's touch position
                    LayoutChildIntoBoundingRegion(view, rect);

                    // check if dragged object is near its target
                    IDrop target;
                    if (MatchNearby(child, out target))
                    {
                        //System.Diagnostics.Debug.WriteLine("Target: {0}", ((DroppableBox)target).Date.ToString("d"));
                        child.OnNearTarget();
                        target.OnDraggableNear();
                        target.IntersectedWithDraggable = true;
                        foreach (var otherTarget in DroppableChildren)
                        {
                            if (otherTarget != target)
                                otherTarget.OnDraggableFar();
                        }
                    }
                    else
                    {
                        // remove the "droppable near" state 
                        foreach (var otherTarget in DroppableChildren)
                        {
                            otherTarget.OnDraggableFar();
                        }
                    }
                }
                else if (child.Status == DragState.Completed || child.Status == DragState.Aborted)
                {
                    //System.Diagnostics.Debug.WriteLine("Stopped/aborted");
                    _currDraggableWidth = -1;
                    _currDraggableHeight = -1;
                    child.Status = DragState.None;
                    view.Opacity = 1;
                }
            }
            // check the droppables
            //foreach (var child in DroppableChildren)
            //{
            //    if (child.DraggableChildren != null && child.DraggableChildren.Count > 0)
            //    {
            //        // check if draggable still within bounds
            //        var view = child as View;
            //        bool draggableRemoved = false;
            //        IDrag draggableChild = null;
            //        foreach (var drag in child.DraggableChildren)
            //        {
            //            //System.Diagnostics.Debug.WriteLine("{0}\t{1}", view.Bounds, (drag as View).Bounds);
            //            if (view != null && !view.Bounds.Contains((drag as View).Bounds.Center))
            //            {
            //                draggableChild = drag;
            //                draggableRemoved = true;
            //            }
            //        }

            //        if (draggableRemoved)
            //        {
            //            child.OnDraggableRemoved(draggableChild);
            //        }
            //    }
            //}
        }

        public List<IDrag> DraggableChildren;
        public List<IDrop> DroppableChildren;

        public DragLayout()
        {
            DraggableChildren = new List<IDrag>();
            DroppableChildren = new List<IDrop>();
        }

        protected override void OnChildAdded(Element child)
        {
            base.OnChildAdded(child);

            var dragChild = child as IDrag;
            if (dragChild != null)
            {
                if (DraggableChildren == null)
                    DraggableChildren = new List<IDrag>();

                DraggableChildren.Add(dragChild);
            }

            var dropChild = child as IDrop;
            if (dropChild != null)
            {
                if (DroppableChildren == null)
                    DroppableChildren = new List<IDrop>();

                DroppableChildren.Add(dropChild);
            }
        }

        protected override void OnChildRemoved(Element child)
        {
            var dragChild = child as IDrag;
            if (dragChild != null && DraggableChildren.Contains(dragChild))
            {
                DraggableChildren.Remove(dragChild);
            }

            var dropChild = child as IDrop;
            if (dropChild != null && DroppableChildren.Contains(dropChild))
            {
                DroppableChildren.Remove(dropChild);
            }

            base.OnChildRemoved(child);
        }

        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            //System.Diagnostics.Debug.WriteLine("Layout children");
            base.LayoutChildren(x, y, width, height);

            foreach (var child in DraggableChildren)
            {
                var view = child as View;
                //System.Diagnostics.Debug.WriteLine("\t{0}", ((Button)child).Text);

                if (child.Status == DragState.Started)
                {
                    child.Status = DragState.Running;
                }
                else if (child.Status == DragState.Running)
                {

                }
                else if (child.Status == DragState.Completed)
                {
                    child.Status = DragState.None;
                }
                else if (child.Status == DragState.Aborted)
                {
                    child.Status = DragState.None;
                }

                if (child.Status == DragState.None)
                {
                    //System.Diagnostics.Debug.WriteLine("\t\tLayouted");
                    var bounds = child.CurrentBounds;
                    LayoutChildIntoBoundingRegion(view, bounds);
                }
            }
        }

        public async void StopDragging()
        {
            _currDraggableWidth = -1;
            _currDraggableHeight = -1;
            foreach (var child in DraggableChildren)
            {
                IDrop target;
                //System.Diagnostics.Debug.WriteLine("Child.Status = {0}", child.Status);
                if (child.Status == DragState.Running || child.Status == DragState.Started)
                {
                    //System.Diagnostics.Debug.WriteLine("child is running, see if there's a droppable target nearby");
                    if (MatchNearby(child, out target) && target.DropEnabled)
                    {
                        //System.Diagnostics.Debug.WriteLine("\tfound");
                        child.OnDragCompleted(target);
                        target.OnDraggableDropped(child);

                    }
                    else
                    {
                        //System.Diagnostics.Debug.WriteLine("\tnot found");
                        child.OnDragAborted();
                    }

                }
            }

            await Task.Delay(10);   // allow changes to propagate (esp on Android)
                                    // where lack of a delay caused the draggable to be removed as soon as it was dropped

            // check the droppables
            foreach (var child in DroppableChildren)
            {
                if (child.DraggableChildren != null && child.DraggableChildren.Count > 0)
                {
                    // check if draggable still within bounds
                    var view = child as View;
                    bool draggableRemoved = false;
                    IDrag draggableChild = null;
                    foreach (var drag in child.DraggableChildren)
                    {
                        //System.Diagnostics.Debug.WriteLine("{0}\t{1}", view.Bounds, drag.CurrentBounds);
                        if (view != null && !view.Bounds.Contains((drag).CurrentBounds.Center))
                        {
                            draggableChild = drag;
                            draggableRemoved = true;
                        }
                    }

                    if (draggableRemoved)
                    {
                        child.OnDraggableRemoved(draggableChild);
                    }
                }
            }

            this.InvalidateLayout();
        }

        bool MatchNearby(IDrag child, out IDrop container)
        {
            var view = child as View;
            container = null;

            if (child.TargetType == null || DroppableChildren.Count == 0)
                return false;

            //find all possible targets
            var targetCandidates = DroppableChildren.FindAll((t) => {
                var v = t as View;
                bool near = v.Bounds.IntersectsWith(view.Bounds);
                return t.GetType() == child.TargetType && near && t.DropEnabled;
            });

            if (targetCandidates.Count == 0)
                return false;

            // find the match which *contains* the draggable
            foreach (var target in targetCandidates)
            {
                container = target as IDrop;
                var targetView = target as View;
                if (targetView.Bounds.Contains(view.Bounds.Center))
                {
                    return true;
                }
            }

            // otherwise return the first one that intersects it
            container = targetCandidates[0] as IDrop;
            return true;

        }
    }
}

