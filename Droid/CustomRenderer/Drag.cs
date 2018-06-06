using System;
using X;
using X.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(DragLayout), typeof(DragAndDropLayoutRenderer))]
namespace X.Droid
{
    public class DragAndDropLayoutRenderer : VisualElementRenderer<RelativeLayout>
    {
        DragLayout element;
        DragLayout LayoutElement
        {
            get { return element ?? (element = this.Element as DragLayout); }
        }
        IDrag currentDraggable;

        private double startX, startY;
        private const double TAP_THRESHOLD = 10;

        public override bool OnInterceptTouchEvent(Android.Views.MotionEvent ev)
        {
            if (!LayoutElement.GestureEnabled)
            {
                return false;
            }

            double x = (ev.GetX() * LayoutElement.Width) / this.Width;
            double y = (ev.GetY() * LayoutElement.Height) / this.Height;

            IDrag draggable;
            if (LayoutElement.IDragTouched(x, y, out draggable))
            {
                currentDraggable = draggable;
                return true;
            }

            return false;
        }

        public override bool OnTouchEvent(Android.Views.MotionEvent e)
        {
            double x = (e.GetX() * LayoutElement.Width) / this.Width;
            double y = (e.GetY() * LayoutElement.Height) / this.Height;

            switch (e.Action)
            {
                case Android.Views.MotionEventActions.Down:
                    HandleTouchStart(x, y);
                    startX = x;
                    startY = y;
                    break;
                case Android.Views.MotionEventActions.Move:
                    if (LayoutElement.Dragging)
                    {
                        LayoutElement.HandleTouch(x, y);
                    }
                    break;
                case Android.Views.MotionEventActions.Up:
                    if (currentDraggable != null && ((currentDraggable.Status == DragState.None || currentDraggable.Status == DragState.Started) || BelowTapThreshold(x, y)))
                    {
                        if (currentDraggable.TapEnabled)
                        {
                            currentDraggable.OnTapped();
                        }
                        currentDraggable = null;
                    }

                    if (LayoutElement.Dragging)
                    {
                        MessagingCenter.Send<DragAndDropLayoutRenderer, bool>(this, "IsDragging", false);
                        LayoutElement.StopDragging();
                        LayoutElement.Dragging = false;
                        currentDraggable = null;
                    }
                    break;
                default:
                    break;
            }

            return true;
        }

        void HandleTouchStart(double x, double y)
        {

            IDrag draggable;
            if (LayoutElement.IDragTouched(x, y, out draggable))
            {
                MessagingCenter.Send<DragAndDropLayoutRenderer, bool>(this, "IsDragging", true);
                draggable.OnDragStarted();
                LayoutElement.Dragging = true;
            }

            //foreach (var child in LayoutElement.Children) {
            //  if (child.Bounds.Contains(x,y)) {
            //      var draggable = child as IDrag;
            //      if (draggable != null) {
            //          draggable.OnDragStarted();
            //          LayoutElement.Dragging = true;
            //      }
            //  }
            //}
        }

        bool BelowTapThreshold(double x, double y)
        {
            double dX = Math.Abs(x - startX);
            double dY = Math.Abs(y - startY);

            return dX <= TAP_THRESHOLD && dY <= TAP_THRESHOLD;
        }
    }
}

