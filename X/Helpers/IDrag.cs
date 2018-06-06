using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace X
{
    public enum DragState
    {
        Started, Running, Completed, Aborted, None
    }

    public interface IDrag
    {
        Rectangle CurrentBounds { get; set; } // needed so that the custom layout will 'remember' where this control is supposed to be
        DragState Status { get; set; }
        Type TargetType { get; set; }   // TODO: Make into List<IDroppable>

        bool DragEnabled { get; set; }
        bool TapEnabled { get; set; }
        IDrop CurrentDropBox { get; set; }
        void OnDragStarted();
        void OnDragAborted();
        void OnDragCompleted();
        void OnDragCompleted(IDrop target);
        void OnDragCompleted(double x, double y);
        void OnNearTarget();
        void OnTapped();

    }

    //public interface IDraggable<T> where T: IDroppable
    //{
    //  Rectangle CurrentBounds { get; set; } // needed so that the custom layout will 'remember' where this control is supposed to be
    //  DragState Status { get; set; }
    //  List<T> Targets { get; set; }

    //  void OnDragStarted();
    //  void OnDragAborted();
    //  void OnDragCompleted();
    //  void OnDragCompleted(IDroppable target);
    //  void OnDragCompleted(double x, double y);
    //  void OnNearTarget();
    //}

    //public interface IDraggableType
    //{
    //  Rectangle CurrentBounds { get; set; } // needed so that the custom layout will 'remember' where this control is supposed to be
    //  DragState Status { get; set; }
    //  Type TargetType { get; set; }

    //  void OnDragStarted();
    //  void OnDragAborted();
    //  void OnDragCompleted();
    //  void OnDragCompleted(IDroppable target);
    //  void OnDragCompleted(double x, double y);
    //  void OnNearTarget();
    //}

}

