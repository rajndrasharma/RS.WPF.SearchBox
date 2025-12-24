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
    public static readonly DependencyProperty HintTextProperty =DependencyProperty.Register("HintText", typeof(string), typeof(SearchTextBox),new PropertyMetadata(""));
    public static readonly DependencyProperty TextValueProperty =DependencyProperty.Register("TextValue", typeof(string), typeof(SearchTextBox),new PropertyMetadata(""));
    public static readonly DependencyProperty TextBoxTypeProperty =DependencyProperty.Register("TextBoxType", typeof(SearchTextBoxType), typeof(SearchTextBox),new PropertyMetadata(SearchTextBoxType.AlphaNumeric));
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
    private static readonly Regex FloatRegex = new Regex("^[.][0-9]+$|^[0-9]*[.]{0,1}[0-9]*$", RegexOptions.Compiled);
    private static readonly Regex NumberRegex = new Regex("[^0-9]+", RegexOptions.Compiled);

    public SearchTextBox()
    {
        InitializeComponent();

        this.SearchInput.GotFocus += (s, e) => this.ContainerBorder.Background = (Brush)Resources["FocusBackgroundBrush"];
        this.SearchInput.LostFocus += (s, e) => this.ContainerBorder.Background = (Brush)Resources["DefaultBackgroundBrush"];
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
        get => (string)GetValue(TextValueProperty);
        set => SetValue(TextValueProperty, value);
    }
    public int IntValue => int.TryParse(TextValue, out int x) ? x : 0;
    public long LongValue => long.TryParse(TextValue, out long x) ? x : 0;
    public float FloatValue => float.TryParse(TextValue, out float x) ? x : 0;
    public decimal DecimalValue => decimal.TryParse(TextValue, out decimal x) ? x : 0;
    public DateTime DateValue => DateTime.TryParse(TextValue, out DateTime x) ? x : DateTime.MinValue;
    public void SelectAll()
    {
        SearchInput.SelectAll();
    }

    #region EVENTS
    protected void NotifyPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
    private void BtnClear_Click(object sender, RoutedEventArgs e)
    {
        this.TextValue = string.Empty;
        this.SearchInput.Focus();
    }

    private void SearchInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        if (this.TextBoxType == SearchTextBoxType.FloatValue)
        {
            e.Handled = !FloatRegex.IsMatch(SearchInput.Text.Insert(SearchInput.SelectionStart, e.Text));
        }
        else if (this.TextBoxType == SearchTextBoxType.OnlyNumbers ||
                 this.TextBoxType == SearchTextBoxType.IntegerValue ||
                 this.TextBoxType == SearchTextBoxType.LongValue)
        {
            e.Handled = NumberRegex.IsMatch(e.Text);
        }
    }
    private void SearchInput_TextChanged(object sender, TextChangedEventArgs e)
    {
        TextChanged?.Invoke(this, EventArgs.Empty);
    }
    #endregion
}