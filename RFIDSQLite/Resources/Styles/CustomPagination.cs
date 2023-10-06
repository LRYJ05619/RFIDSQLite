using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RFIDSQLite.ViewModel;

namespace RFIDSQLite.Resources.Styles
{
    public class CustomPagination : ContentView
    {
        private StackLayout paginationLayout;
        private Button prevButton;
        private Button nextButton;

        public event EventHandler<int> PageChanged;

        public CustomPagination()
        {
            paginationLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.CenterAndExpand
            };

            prevButton = new Button
            {
                Text = "<",
                BackgroundColor = Colors.White,
                TextColor = Colors.Black,
                CornerRadius = 5,
                WidthRequest = 40,
                HeightRequest = 40,
                Margin = 5,
            };
            prevButton.Clicked += OnPrevButtonClicked;

            nextButton = new Button
            {
                Text = ">",
                BackgroundColor = Colors.White,
                TextColor = Colors.Black,
                CornerRadius = 5,
                WidthRequest = 40,
                HeightRequest = 40,
                Margin = 5,
            };
            nextButton.Clicked += OnNextButtonClicked;

            paginationLayout.Children.Add(prevButton);
            paginationLayout.Children.Add(nextButton);

            Content = paginationLayout;
        }

        public static readonly BindableProperty TotalPagesProperty =
            BindableProperty.Create(
                nameof(TotalPages),
                typeof(int),
                typeof(CustomPagination),
                default(int),
                propertyChanged: OnTotalPagesPropertyChanged);

        public static readonly BindableProperty CurrentPageProperty =
            BindableProperty.Create(
                nameof(CurrentPage),
                typeof(int),
                typeof(CustomPagination),
                default(int),
                BindingMode.TwoWay,
                propertyChanged: OnCurrentPagePropertyChanged);

        public int TotalPages
        {
            get => (int)GetValue(TotalPagesProperty);
            set => SetValue(TotalPagesProperty, value);
        }

        public int CurrentPage
        {
            get => (int)GetValue(CurrentPageProperty);
            set => SetValue(CurrentPageProperty, value);
        }

        private static void OnTotalPagesPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var pagination = bindable as CustomPagination;
            pagination?.UpdatePaginationButtons();
        }

        private static void OnCurrentPagePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var pagination = bindable as CustomPagination;
            if (pagination != null)
            {
                pagination.UpdatePaginationButtons();
                pagination.CurrentPage = (int)newValue; // 更新绑定项的属性
            }
        }


        private void UpdatePaginationButtons()
        {
            paginationLayout.Children.Clear();
            paginationLayout.Children.Add(prevButton);

            if (TotalPages <= 7)
            {
                for (int page = 1; page <= TotalPages; page++)
                {
                    AddPageButton(page);
                }
            }
            else
            {
                if (CurrentPage <= 4)
                {
                    for (int page = 1; page <= 5; page++)
                    {
                        AddPageButton(page);
                    }
                    AddEllipsisButton();
                    AddPageButton(TotalPages);
                }
                else if (CurrentPage >= TotalPages - 3)
                {
                    AddPageButton(1);
                    AddEllipsisButton();
                    for (int page = TotalPages - 4; page <= TotalPages; page++)
                    {
                        AddPageButton(page);
                    }
                }
                else
                {
                    AddPageButton(1);
                    AddEllipsisButton();
                    for (int page = CurrentPage - 1; page <= CurrentPage + 1; page++)
                    {
                        AddPageButton(page);
                    }
                    AddEllipsisButton();
                    AddPageButton(TotalPages);
                }
            }

            paginationLayout.Children.Add(nextButton);

            var pageEntry = new Entry
            {
                Placeholder = "跳转页码",
                Keyboard = Keyboard.Numeric,
                WidthRequest = 80,
                Margin = new Thickness(20, 8, 10, 5),
                HorizontalTextAlignment = TextAlignment.Center,
            };

            var jumpButton = new Button
            {
                Text = "确定",
                BackgroundColor = Color.FromArgb("#00A2FF"),
                TextColor = Colors.White,
                CornerRadius = 5,
                WidthRequest = 80,
                HeightRequest = 40,
                Margin = 5,
            };

            jumpButton.Clicked += (sender, args) =>
            {
                if (int.TryParse(pageEntry.Text, out int targetPage) && targetPage >= 1 && targetPage <= TotalPages)
                {
                    CurrentPage = targetPage;
                    PageChanged?.Invoke(this, targetPage);
                }
                else
                {
                    // 无效页码输入，恢复显示当前页码
                    pageEntry.Text = CurrentPage.ToString();
                }
            };

            var jumpLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Center,
                Children = { pageEntry, jumpButton },
            };

            paginationLayout.Children.Add(jumpLayout);
        }
        
        private void AddPageButton(int page)
        {
            var pageButton = new Button
            {
                Text = page.ToString(),
                BackgroundColor = (page == CurrentPage) ? Color.FromArgb("#00A2FF") : Colors.White,
                TextColor = (page == CurrentPage) ? Colors.White : Colors.Black,
                CornerRadius = 5,
                WidthRequest = 40,
                HeightRequest = 40,
                Margin = new Thickness(0, 5, 0, 5),
            };

            pageButton.Clicked += (sender, args) =>
            {
                CurrentPage = page;
                PageChanged?.Invoke(this, page);
            };

            paginationLayout.Children.Add(pageButton);
        }

        private void AddEllipsisButton()
        {
            var ellipsisButton = new Button
            {
                Text = "...",
                BackgroundColor = Colors.LightGray,
                TextColor = Colors.Black,
                CornerRadius = 5,
                WidthRequest = 40,
                HeightRequest = 40,
                Margin = new Thickness(0, 5, 0, 5),
                IsEnabled = false,
            };

            paginationLayout.Children.Add(ellipsisButton);
        }

        private void OnPrevButtonClicked(object sender, EventArgs e)
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                PageChanged?.Invoke(this, CurrentPage);
            }
        }

        private void OnNextButtonClicked(object sender, EventArgs e)
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                PageChanged?.Invoke(this, CurrentPage);
            }
        }
    }
}
