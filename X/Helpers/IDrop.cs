using System;
using System.Collections.Generic;

namespace X
{
    public enum DroppableState
    {
        Full, Accepting
    }

    // The name is a bit unfortunate, but this is for views that are targets for draggable elements.
    public interface IDrop
    {
        DroppableState Status { get; set; }
        List<IDrag> DraggableChildren { get; set; }

        bool DropEnabled { get; }
        bool IntersectedWithDraggable { get; set; }
        int position { get; set; }
        void OnDraggableNear();
        void OnDraggableFar(); // lol
        void OnDraggableDropped(IDrag draggable);
        void OnDraggableRemoved(IDrag draggable);
    }
}

