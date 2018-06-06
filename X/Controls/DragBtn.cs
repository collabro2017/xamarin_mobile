using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace X
{
    public class DragBtn : Button, IDrag
    {
        #region Bindable properties
        public static readonly BindableProperty DraggedCommandProperty = BindableProperty.Create(nameof(DraggedCommand), typeof(ICommand), typeof(DragBtn), default(ICommand),
                propertyChanged: (bindable, oldValue, newValue) => {
                    var view = bindable as DragBtn;
                }
        );
        public ICommand DraggedCommand
        {
            get { return (ICommand)GetValue(DraggedCommandProperty); }
            set { SetValue(DraggedCommandProperty, value); }
        }

        public static readonly BindableProperty TappedCommandProperty = BindableProperty.Create(nameof(TappedCommand), typeof(ICommand), typeof(DragBtn), default(ICommand),
                propertyChanged: (bindable, oldValue, newValue) => {
                    var view = bindable as DragBtn;
                }
        );
        public ICommand TappedCommand
        {
            get { return (ICommand)GetValue(TappedCommandProperty); }
            set { SetValue(TappedCommandProperty, value); }
        }
        #endregion

        #region Public properties
        public DragState Status
        {
            get { return _status; }
            set { _status = value; }
        }
        DragState _status;


        public Type TargetType
        {
            get { return _target; }
            set { _target = value; }
        }
        protected Type _target = null;

        public Rectangle CurrentBounds
        {
            get { return _bounds; }
            set { _bounds = value; }
        }
        protected Rectangle _bounds;

        public Rectangle OriginalBounds
        {
            get { return _origBounds; }
            set { _origBounds = value; }
        }
        protected Rectangle _origBounds;

        public bool DragEnabled
        {
            get { return _dragEnabled; }
            set { _dragEnabled = value; }
        }
        protected bool _dragEnabled = true;

        public bool TapEnabled
        {
            get { return _tapEnabled; }
            set { _tapEnabled = value; }
        }
        protected bool _tapEnabled = true;

        public int WorkoutId
        {
            get { return _workoutId; }
            set { _workoutId = value; }
        }

        public IDrop CurrentDropBox { 
            get { return _currentDropBox; }
            set { _currentDropBox = value; }
            }
        private IDrop _currentDropBox;

        protected int _workoutId;
        #endregion

        public event EventHandler<EventArgs> Tapped;

        bool start = false;

        public DragBtn()
        {
            TargetType = typeof(DropBox);
            Initialize();
        }

        void Initialize()
        {
            Status = DragState.None;

            this.SizeChanged += (object sender, EventArgs e) =>
            {
                if (!start)
                {
                    //System.Diagnostics.Debug.WriteLine("Original bounds: {0}", Bounds);
                    CurrentBounds = Bounds;
                    OriginalBounds = Bounds;
                    start = true;
                }
            };

            TapEnabled = true;
        }
        #region IDraggable implementation
        public void OnDragStarted()
        {
            if (Status == DragState.Started)
                return;

            this.Scale = 0.9;
            this.Opacity = 0.5;
            onswap = true;
            Status = DragState.Started;
        }

        public IDrop CurrentTarget;
        public async void OnDragAborted()
        {
            onswap = false;
            Status = DragState.Aborted;
            this.Scale = 1;
            this.Opacity = 1;
            FontSize = 14;
            await this.LayoutTo(CurrentBounds, 100, Easing.CubicIn);

            OnDragCompleted(CurrentTarget);

            if (DraggedCommand != null && DraggedCommand.CanExecute(new string[] { "-", WorkoutId.ToString() }))
            {
                DraggedCommand.Execute(new string[] { "-", WorkoutId.ToString() });
            }

            TapEnabled = true;
        }

        public void OnDragCompleted()
        {
            if (Status == DragState.Completed)
                return;

            Status = DragState.Completed;
            TapEnabled = false;
            this.Scale = 1;
            this.Opacity = 1;
        }
        bool onswap;
        public async void OnDragCompleted(IDrop target)
        {   
            if(target == CurrentDropBox)
            {
                onswap = false;
            }
            if (TargetType != target.GetType())
                return;
            
            Constants.CurrentCategoryOrder.Remove(int.Parse(this.ClassId));
            Constants.CurrentCategoryOrder.Insert(target.position,int.Parse(this.ClassId));

            if(target.DraggableChildren.Count>0 && onswap)
            {
                target.DraggableChildren[0].OnDragCompleted(CurrentDropBox);
                CurrentDropBox.OnDraggableDropped(target.DraggableChildren[0]);
                target.DraggableChildren[0].CurrentDropBox = CurrentDropBox;
                onswap = false;
            }
            CurrentTarget = target;
            CurrentDropBox = target;
            this.Scale = 1;
            this.Opacity = 1;

            FontSize = 10;

            var view = target as View;

            if (view != null)
            {
                var bounds = view.Bounds;
                await this.LayoutTo(bounds, 0, Easing.CubicIn);
                CurrentBounds = bounds;
                //System.Diagnostics.Debug.WriteLine("{0} current bounds: {1}", Text, CurrentBounds);
            } 
            Status = DragState.Completed;
            TapEnabled = false;

            if (DraggedCommand != null && DraggedCommand.CanExecute(new string[] { ((DropBox)target).Date.ToString("D"), WorkoutId.ToString() }))
            {
                DraggedCommand.Execute(new string[] { ((DropBox)target).Date.ToString("d"), WorkoutId.ToString() });
            }
        }

        public void OnDragCompleted(double x, double y)
        {
            this.TranslateTo(x, y, 100, Easing.CubicIn);

            Status = DragState.Completed;
            TapEnabled = false;
        }

        public void OnNearTarget()
        {
        }

        public void OnTapped()
        {
            System.Diagnostics.Debug.WriteLine("{0} was tapped", Text);
            if (TappedCommand != null && TappedCommand.CanExecute(WorkoutId.ToString()))
            {
                TappedCommand.Execute(WorkoutId.ToString());
            }

            var handler = Tapped;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
       
        #endregion
    }
}

