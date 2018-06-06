using System;
using Xamarin.Forms;

namespace X
{
    public class Resizer
    {
        public static Element scaleChild(Element child)
        {
            var label = child as Label;
            var button = child as Button;
            var entry = child as Entry;
            var editor = child as Editor;
            var listView = child as ListView;

            var stackLayout = child as StackLayout;
            var gridLayout = child as Grid;
            var relativeLayout = child as RelativeLayout;
            var absoluteLayout = child as AbsoluteLayout;
            var contentView = child as ContentView;
            var frame = child as Frame;
            var scrollView = child as ScrollView;

            var visualElement = child as VisualElement;
            var view = child as View;
            var layout = child as Layout;

            if (visualElement != null)
            {
                visualElement.WidthRequest = visualElement.WidthRequest < 0 ? visualElement.WidthRequest : visualElement.WidthRequest * App.scale;
                visualElement.HeightRequest = visualElement.HeightRequest < 0 ? visualElement.HeightRequest : visualElement.HeightRequest * App.scale;
                visualElement.MinimumWidthRequest = visualElement.MinimumWidthRequest < 0 ? visualElement.MinimumWidthRequest : visualElement.MinimumWidthRequest * App.scale;
                visualElement.MinimumHeightRequest = visualElement.MinimumHeightRequest < 0 ? visualElement.MinimumHeightRequest : visualElement.MinimumHeightRequest * App.scale;
            }

            if (view != null)
            {
                view.Margin = new Thickness(view.Margin.Left * App.scale,
                                               view.Margin.Top * App.scale,
                                               view.Margin.Right * App.scale,
                                            view.Margin.Bottom * App.scale);

            }

            if (layout != null)
            {
                layout.Padding = new Thickness(layout.Padding.Left * App.scale,
                                               layout.Padding.Top * App.scale,
                                               layout.Padding.Right * App.scale,
                                               layout.Padding.Bottom * App.scale);
            }

            // View type selection
            //if (contentView != null)
            //{
            //    if (!(contentView is RTRContentView)) throw new Exception($"Please use {nameof(RTRContentView)} because this calls the resizer");
            //}
            //else if (scrollView != null)
            //{
            //    if (!(scrollView is RTRScrollView)) throw new Exception($"Please use {nameof(RTRScrollView)} because this calls the resizer");
            //}
            //else if (gridLayout != null)
            //{
            //    if (!(gridLayout is RTRGrid)) throw new Exception($"Please use {nameof(RTRGrid)} because this calls the resizer");
            //    gridLayout.RowSpacing = gridLayout.RowSpacing * App.scale;
            //    gridLayout.ColumnSpacing = gridLayout.ColumnSpacing * App.scale;
            //}
            else if (frame != null)
            {
                if (!(frame is ScaledFrame)) throw new Exception($"Please use {nameof(ScaledFrame)} because this calls the resizer");
                frame.CornerRadius = (float)(frame.CornerRadius * App.scale);
                frame.HeightRequest = (float)(frame.HeightRequest * App.scale);
            }

            else if (stackLayout != null)
            {
                if (!(stackLayout is ScaledStackLayout)) throw new Exception($"Please use {nameof(ScaledStackLayout)} because this calls the resizer");
                stackLayout.Spacing = stackLayout.Spacing * App.scale;
            }
           
            else if (relativeLayout != null)
            {
                //if (!(relativeLayout is RTRRelativeLayout)) throw new Exception($"Please use {nameof(RTRRelativeLayout)} because this calls the resizer");
            }
            else if (absoluteLayout != null)
            {
                //if (!(absoluteLayout is RTRAbsoluteLayout)) throw new Exception($"Please use {nameof(RTRAbsoluteLayout)} because this calls the resizer");
            }
            else if (label != null)
            {
                label.FontSize = label.FontSize * App.scale;
            }
            else if (button != null)
            {
                if (!(button is CustomButtonRenderer)) throw new Exception($"Please use {nameof(CustomButtonRenderer)} because on android this enables border radius by using droid app compat version of ButtonRenderer");
                button.BorderWidth = button.BorderWidth < 0 ? button.BorderWidth : button.BorderWidth * App.scale;
                button.BorderRadius = (int)(button.BorderRadius * App.scale);
                button.FontSize = button.FontSize * App.scale;
            }
            else if (entry != null)
            {
                entry.FontSize = entry.FontSize * App.scale;
            }
            else if (editor != null)
            {
                entry.FontSize = editor.FontSize * App.scale;
            }
            else if (listView != null)
            {
                listView.RowHeight = (int)(listView.RowHeight * App.scale);
            }

            return child;
        }
    }
}
