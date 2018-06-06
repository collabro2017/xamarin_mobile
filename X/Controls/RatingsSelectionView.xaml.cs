using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace X
{
    public partial class RatingsSelectionView : ContentView
    {
        public RatingsSelectionView()
        {
            InitializeComponent();
            System.Diagnostics.Debug.WriteLine("RatingsSelectionView::Init");
        }
    
        protected override Xamarin.Forms.SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            System.Diagnostics.Debug.WriteLine("RatingsSelectionView::OnMeasure");
            return base.OnMeasure(widthConstraint, heightConstraint);
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            System.Diagnostics.Debug.WriteLine("RatingsSelectionView::OnSizeAllocated");
            base.OnSizeAllocated(width, height);
        }

        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            System.Diagnostics.Debug.WriteLine("RatingsSelectionView::LayoutChildren");
            base.LayoutChildren(x, y, width, height);

        }
    }
}
