using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace DotNetPad32
{
    public class Document : DependencyObject
    {
        public string? FileName { get; set; } = "Untitled.txt";

        public string? Contents { get; set; } = "";

        public static readonly DependencyProperty TextHasChangedProperty =
            DependencyProperty.Register("TextHasChanged", typeof(bool), typeof(Document), new PropertyMetadata(false));

        public bool TextHasChanged
        {
            get
            {
                return (bool)GetValue(TextHasChangedProperty);
            }
            set
            {
                SetValue(TextHasChangedProperty, value);
            }
        }

        public bool DocumentIsSaved { get; set; } = false;

        public int CaretIndex { get; set; } = 0;

        public int SelectionLength { get; set; } = 0;

        public string FindTextString { get; set; } = "";

        public int FindLastIndexFound { get; set; } = 0;

        public void NewDocument(MainWindow mw)
        {
            mw.TextBox1.Clear();

            FileName = "Untitled.txt";
            Contents = "";
            TextHasChanged = false;
            DocumentIsSaved = false;

            mw.Title = System.IO.Path.GetFileNameWithoutExtension(FileName);
        }

        public bool OpenDocument(TextBox tb)
        {
            bool success = false;

            OpenFileDialog openFileDialog = new()
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    FileName = System.IO.Path.GetFullPath(openFileDialog.FileName);
                    Contents = System.IO.File.ReadAllText(openFileDialog.FileName);
                    tb.Text = Contents;
                    TextHasChanged = false;
                    DocumentIsSaved = true;

                    success = true;
                }
                catch
                {
                    //
                }
            }
            else
            {
                success = false;
            }

            return success;
        }

        public bool NeedsToBeSaved(string text)
        {
            bool Successful = true;

            if (TextHasChanged)
            {
                if (FileName != null)
                {
                    SaveConfirmationDialog dialog = new SaveConfirmationDialog(FileName);
                    dialog.ShowDialog();
                    switch (dialog.Choice)
                    {
                        case "Save":
                            Successful = DocumentIsSaved ? SaveDocument(text) : SaveDocumentAs(text);
                            break;
                        case "Don't save":
                            Successful = true;
                            break;
                        case "Cancel":
                            Successful = false;
                            break;
                    }
                }
            }
            return Successful;
        }

        public bool SaveDocument(string text)
        {
            bool Saved = false;

            if (DocumentIsSaved && FileName != null)
            {
                try
                {
                    File.WriteAllText(FileName, text);
                    TextHasChanged = false;
                    Saved = true;
                }
                catch
                {
                    //
                }
            }
            return Saved;
        }

        public bool SaveDocumentAs(string text)
        {
            bool Saved = false;

            SaveFileDialog saveFileDialog = new()
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                DefaultExt = ".txt",
                FileName = FileName
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    if (saveFileDialog.FileName != null)
                    {
                        File.WriteAllText(saveFileDialog.FileName, text);
                        FileName = saveFileDialog.FileName;
                        TextHasChanged = false;
                        DocumentIsSaved = true;
                        Contents = text;

                        Saved = true;
                    }
                }
                catch
                {
                    //
                }
            }

            return Saved;
        }

    }
}
