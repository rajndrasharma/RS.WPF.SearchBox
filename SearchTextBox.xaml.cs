using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace RS.WPF.SearchBox;

/// <summary>
/// Interaction logic for UserControl1.xaml
/// </summary>
public partial class SearchTextBox : UserControl,INotifyPropertyChanged
{
    public static readonly DependencyProperty HintTextProperty =
        DependencyProperty.Register("HintText", typeof(string), typeof(SearchTextBox),
            new PropertyMetadata(""));
    public static readonly DependencyProperty TextValueProperty =
        DependencyProperty.Register("TextValue", typeof(string), typeof(SearchTextBox),
            new PropertyMetadata(""));
    public static readonly DependencyProperty TextBoxTypeProperty =
       DependencyProperty.Register("TextBoxType", typeof(SearchTextBoxType), typeof(SearchTextBox),
           new PropertyMetadata(SearchTextBoxType.AlphaNumeric));
    public enum SearchTextBoxType
    {
        AlphaNumeric = 0,
        OnlyNumbers = 1,
        FloatValue = 2,
        IntegerValue = 3,
        LongValue = 4,
        Date = 5
    }
    public event PropertyChangedEventHandler PropertyChanged;
    public event EventHandler TextChanged;

    public SearchTextBox()
    {
        InitializeComponent();

        this.GrdSearchBox.DataContext = this;

    }
    public string HintText
    {
        get { return (string)GetValue(HintTextProperty); }
        set { SetValue(HintTextProperty, value); }
    }
    public SearchTextBoxType TextBoxType
    {
        get { return (SearchTextBoxType)GetValue(TextBoxTypeProperty); }
        set { SetValue(TextBoxTypeProperty, value); }
    }
    public string TextValue
    {
        get { return this.txtSearch.Text.ToUpper() == this.HintText.ToUpper() ? "" : this.txtSearch.Text.Trim(); }
        set { SetValue(TextValueProperty, value); }
    }
    public int IntValue
    {
        get { int.TryParse(TextValue, out int xInt); return xInt; }
    }
    public long LongValue
    {
        get { long.TryParse(TextValue, out long xLong); return xLong; }
    }
    public float FloatValue
    {
        get { float.TryParse(TextValue, out float xFloat); return xFloat; }
    }
    public decimal DecimalValue
    {
        get { decimal.TryParse(TextValue, out decimal xDecimal); return xDecimal; }
    }
    public DateTime DateValue
    {
        get { DateTime.TryParse(TextValue, out DateTime xDate); return xDate; }
    }
    public void SelectAll()
    {
        txtSearch.SelectAll();
    }

    #region EVENTS
    protected void NotifyPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        if (!txtSearch.IsFocused)
        {
            if (string.IsNullOrWhiteSpace(this.txtSearch.Text))
            {
                this.txtSearch.Text = this.HintText;
            }
        }
    }
    private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (this.txtSearch.Text.ToUpper() == this.HintText.ToUpper())
        {
            this.txtSearch.Foreground = Brushes.Gray;
        }
        else
        {
            this.txtSearch.Foreground = Brushes.Black;
        }
        TextChanged?.Invoke(this, EventArgs.Empty);
    }
    private void TxtSearch_GotFocus(object sender, RoutedEventArgs e)
    {
        if (this.txtSearch.Text.ToUpper() == this.HintText.ToUpper()) 
        { 
            this.txtSearch.Text = ""; 
        }
        else
        {
            this.txtSearch.SelectAll();
        }
        this.BrderSearchBox.Background = Brushes.LightGoldenrodYellow;
    }
    private void txtSearch_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (this.txtSearch.Text.ToUpper() == this.HintText.ToUpper())
        {
            this.txtSearch.Text = "";
        }
        else
        {
            this.txtSearch.SelectAll();
        }
        this.BrderSearchBox.Background = Brushes.LightGoldenrodYellow;
    }
    private void TxtSearch_LostFocus(object sender, RoutedEventArgs e)
    {
        this.BrderSearchBox.Background = Brushes.White;
        if (string.IsNullOrWhiteSpace(this.txtSearch.Text))
        {
            this.txtSearch.Text = this.HintText;
        }
    }
    private void txtSearch_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        this.BrderSearchBox.Background = Brushes.White;
        if (string.IsNullOrWhiteSpace(this.txtSearch.Text))
        {
            this.txtSearch.Text = this.HintText;
        }
    }
    private void TxtSearch_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        if (this.TextBoxType == SearchTextBoxType.FloatValue)
        {
            Regex regex = new Regex("^[.][0-9]+$|^[0-9]*[.]{0,1}[0-9]*$");
            e.Handled = !regex.IsMatch(txtSearch.Text.Insert(txtSearch.SelectionStart, e.Text));
        }
        else if (this.TextBoxType == SearchTextBoxType.OnlyNumbers || this.TextBoxType == SearchTextBoxType.IntegerValue ||
            this.TextBoxType == SearchTextBoxType.LongValue)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
    #endregion
}