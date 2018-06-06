using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using X;
using X.iOS;
using UIKit;

[assembly: ExportRenderer(typeof(DragLayout), typeof(DragAndDropLayoutRenderer))]
namespace X.iOS
{
    public class DragAndDropLayoutRenderer : VisualElementRenderer<RelativeLayout>
    {
        DragLayout element;
        DragLayout LayoutElement
        {
            get { return element ?? (element = this.Element as DragLayout); }
        }
        bool dragMightStart = false;
        double lastTouchX, lastTouchY;
        IDrag currentDraggable;
        const double DRAG_THRESHOLD = 0.25;

        public override UIView HitTest(CoreGraphics.CGPoint point, UIEvent uievent)
        {
            UIView hitTestView = base.HitTest(point, uievent);

            if (LayoutElement.GestureEnabled)
            {
                foreach (var child in LayoutElement.Children)
                {
                    if (child.Bounds.Contains((double)point.X, (double)point.Y))
                    {
                        var draggable = child as IDrag;
                        if (draggable != null && draggable.DragEnabled)
                        {
                            lastTouchX = point.X;
                            lastTouchY = point.Y;
                            dragMightStart = true;
                            currentDraggable = draggable;
                            return this;
                        }
                    }
                }
            }

            return hitTestView;
        }

        public override void TouchesMoved(Foundation.NSSet touches, UIEvent evt)
        {
            base.TouchesMoved(touches, evt);

            UITouch currTouch = touches.AnyObject as UITouch;
            var view = currTouch.View as UIView;
            var point = currTouch.LocationInView(view);

            if (dragMightStart && currentDraggable != null)
            {
                var deltaX = Math.Abs(point.X - lastTouchX);
                var deltaY = Math.Abs(point.Y - lastTouchY);

                // if no movement at all
                if (deltaX < DRAG_THRESHOLD && deltaY < DRAG_THRESHOLD)
                {
                    dragMightStart = true;
                }
                else if (deltaX > DRAG_THRESHOLD || deltaY > DRAG_THRESHOLD)
                {
                    currentDraggable.OnDragStarted();
                    element.Dragging = true;
                    dragMightStart = false; // it's started
                }

            }

            if (LayoutElement.Dragging)
            {
                LayoutElement.HandleTouch((double)point.X, (double)point.Y);
            }
        }

        public override void TouchesEnded(Foundation.NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);

            if (currentDraggable != null && (currentDraggable.Status == DragState.None || currentDraggable.Status == DragState.Started))
            {
                if (currentDraggable.TapEnabled)
                {
                    currentDraggable.OnTapped();
                }
                currentDraggable = null;
            }

            if (LayoutElement.Dragging)
            {
                LayoutElement.StopDragging();
                LayoutElement.Dragging = false;
                dragMightStart = false;
                currentDraggable = null;
            }

        }
    }
}

