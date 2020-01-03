using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NotesApp.View
{
    /// <summary>
    /// Interaction logic for NotesWindow.xaml
    /// </summary>
    public partial class NotesWindow : Window
    {
        SpeechRecognitionEngine recognizer;
        List<double> fontSizes;
        public NotesWindow()
        {
            InitializeComponent();

            var currentCulture = (from r in SpeechRecognitionEngine.InstalledRecognizers()
                                 where r.Culture.Equals(Thread.CurrentThread.CurrentCulture)
                                 select r).FirstOrDefault();
            recognizer = new SpeechRecognitionEngine(currentCulture);

            GrammarBuilder builder = new GrammarBuilder();
            builder.AppendDictation();
            Grammar grammar = new Grammar(builder);

            recognizer.LoadGrammar(grammar);
            recognizer.SetInputToDefaultAudioDevice();
            recognizer.SpeechRecognized += Recognizer_SpeechRecognized;

            var fontFamilies = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            fontFamilyComboBox.ItemsSource = fontFamilies;

            fontSizes = new List<double>() { 8, 9, 10, 11, 12, 14, 16, 28, 48,72 };
            fontSizeComboBox.ItemsSource = fontSizes;
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            if (string.IsNullOrEmpty(App.UserId))
            {
                LogInWindow logInWindow = new LogInWindow();
                logInWindow.ShowDialog();
            }
        }

        private void Recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string recognizedText = e.Result.Text;

            contentRichTextBox.Document.Blocks.Add(new Paragraph(new Run(recognizedText)));
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void contentRichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int amountOfCharacters = new TextRange(contentRichTextBox.Document.ContentStart, contentRichTextBox.Document.ContentEnd).Text.Length;
            statusTextBlock.Text = $"Document lenght: {amountOfCharacters} characters";
        }

        private void boldButton_Click(object sender, RoutedEventArgs e)
        {
            bool isButtonChecked = (sender as ToggleButton).IsChecked ?? false;
            if (isButtonChecked)
                contentRichTextBox.Selection.ApplyPropertyValue(Inline.FontWeightProperty, FontWeights.Bold);
            else
                contentRichTextBox.Selection.ApplyPropertyValue(Inline.FontWeightProperty, FontWeights.Normal);
        }

        private void SpeechButton_Click(object sender, RoutedEventArgs e)
        {
            bool isButtonChecked = (sender as ToggleButton).IsChecked ?? false;
            if(isButtonChecked)
            {
                recognizer.RecognizeAsync(RecognizeMode.Multiple);
                Debug.WriteLine("Speech is on");
            }
            else
            {
                recognizer.RecognizeAsyncStop();
                Debug.WriteLine("Speech is off");
            }
        }

        private void contentRichTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var selectedState =contentRichTextBox.Selection.GetPropertyValue(Inline.FontWeightProperty);
            var selectedItalics = contentRichTextBox.Selection.GetPropertyValue(Inline.FontStyleProperty);
            var selectedUnderline = contentRichTextBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            var selectedStrikethrough = contentRichTextBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            TextDecorationCollection textDecorations;
            textDecorations = (contentRichTextBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty) as TextDecorationCollection);
            var selectedFontFamily = contentRichTextBox.Selection.GetPropertyValue(Inline.FontFamilyProperty);
            var selectedFontSize = contentRichTextBox.Selection.GetPropertyValue(Inline.FontSizeProperty);


            //Debug.WriteLine($"96: {textDecorations.Contains(TextDecorations.Underline)}")

            boldButton.IsChecked = (selectedState != DependencyProperty.UnsetValue) && (selectedState.Equals(FontWeights.Bold));
            italicsButton.IsChecked = (selectedItalics != DependencyProperty.UnsetValue) && (selectedItalics.Equals(FontStyles.Italic));

            fontFamilyComboBox.SelectedItem = selectedFontFamily;
            fontSizeComboBox.Text = (selectedFontSize).ToString();

            underlineButton.IsChecked = false;
            strikethroughButton.IsChecked = false;
            foreach(TextDecoration textDecoration in textDecorations)
            {
                if(textDecoration.Location == TextDecorationLocation.Underline)
                {
                    underlineButton.IsChecked = true;
                }
                if(textDecoration.Location == TextDecorationLocation.Strikethrough)
                {
                    strikethroughButton.IsChecked = true;
                }
            }
            //underlineButton.IsChecked = (selectedUnderline != DependencyProperty.UnsetValue) && (selectedUnderline.Equals(TextDecorations.Underline));
            //strikethroughButton.IsChecked = (selectedStrikethrough != DependencyProperty.UnsetValue) && (selectedStrikethrough.Equals(TextDecorations.Strikethrough));
        }

        private void italicsButton_Click(object sender, RoutedEventArgs e)
        {
            bool isButtonChecked = (sender as ToggleButton).IsChecked ?? false;
            if (isButtonChecked)
                contentRichTextBox.Selection.ApplyPropertyValue(Inline.FontStyleProperty, FontStyles.Italic);
            else
                contentRichTextBox.Selection.ApplyPropertyValue(Inline.FontStyleProperty, FontStyles.Normal);
        }

        private void underlineButton_Click(object sender, RoutedEventArgs e)
        {
            bool isButtonChecked = (sender as ToggleButton).IsChecked ?? false;
            if (isButtonChecked)
            {
                TextDecorationCollection textDecorations;
                textDecorations = (contentRichTextBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty) as TextDecorationCollection).Clone();
                textDecorations.Add(TextDecorations.Underline);
                contentRichTextBox.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, textDecorations);
                //contentRichTextBox.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, TextDecorations.Underline);
            }
            else
            {
                TextDecorationCollection textDecorations;
                (contentRichTextBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty) as TextDecorationCollection).TryRemove(TextDecorations.Underline, out textDecorations);
                contentRichTextBox.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, textDecorations);
            }
                
        }

        private void strikethroughButton_Click(object sender, RoutedEventArgs e)
        {
            bool isButtonChecked = (sender as ToggleButton).IsChecked ?? false;
            if (isButtonChecked)
            {
                TextDecorationCollection textDecorations;
                textDecorations = (contentRichTextBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty) as TextDecorationCollection).Clone();
                textDecorations.Add(TextDecorations.Strikethrough);
                contentRichTextBox.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, textDecorations);
            }
                
            else
            {
                TextDecorationCollection textDecorations;
                (contentRichTextBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty) as TextDecorationCollection).TryRemove(TextDecorations.Strikethrough, out textDecorations);
                contentRichTextBox.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, textDecorations);
            }
        }

        private void fontFamilyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //var selectedFontFamily = contentRichTextBox.Selection.GetPropertyValue(Inline.FontFamilyProperty);
            if(fontFamilyComboBox.SelectedItem != null)
            {
                contentRichTextBox.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, fontFamilyComboBox.SelectedItem);
            }
            
        }

        private void fontSizeComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            contentRichTextBox.Selection.ApplyPropertyValue(Inline.FontSizeProperty, fontSizeComboBox.Text);

            ////Array<double> fontSizes = fontSizeComboBox.ItemsSource
            //double fontSize = double.Parse(fontSizeComboBox.Text);
            //if (fontSizes.Contains(FontSize))
            //{
            //    contentRichTextBox.Selection.ApplyPropertyValue(Inline.FontSizeProperty, fontSizeComboBox.Text);
            //}
            //else
            //{
            //    //fontSizes.Add(fontSize);
            //    //fontSizeComboBox.
            //    //fontSizeComboBox.ItemsSource = fontSizes;
            //    contentRichTextBox.Selection.ApplyPropertyValue(Inline.FontSizeProperty, fontSizeComboBox.Text);
            //}
        }
    }
}
