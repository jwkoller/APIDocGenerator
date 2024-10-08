﻿using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Color = DocumentFormat.OpenXml.Wordprocessing.Color;
using FontSize = DocumentFormat.OpenXml.Wordprocessing.FontSize;
using DocumentFormat.OpenXml;

namespace APIDocGenerator.Services
{
    public class DocumentGenerator
    {
        public string DocumentName { get; private set; }
        public WordprocessingDocument Document { get; private set; }
        public MainDocumentPart MainPart {  get; private set; }
        public Body Body { get; private set; }

        public DocumentGenerator(string destination, string fileName)
        {
            DocumentName = fileName;
            Document = WordprocessingDocument.Create($"{destination}/{fileName}.docx", WordprocessingDocumentType.Document);
            MainPart = Document.AddMainDocumentPart();
            MainPart.Document = new Document();
            Body = MainPart.Document.AppendChild(new Body());
        }

        /// <summary>
        /// Creates a new paragraph with a bolded heading.
        /// </summary>
        /// <param name="heading"></param>
        public void WriteNewParagraph(string heading)
        {
            Paragraph paragraph = Body.AppendChild(new Paragraph());
            if(!paragraph.Elements<ParagraphProperties>().Any())
            {
                paragraph.PrependChild(new ParagraphProperties());
            }

            Run run = new Run();
            RunProperties props = new RunProperties();
            props.Bold = new Bold();
            props.FontSize = new FontSize() { Val = "32"};
            
            run.Append(props);
            run.AppendChild(new Break());
            run.AppendChild(new Text() { Text = heading, Space = SpaceProcessingModeValues.Preserve });
            run.AppendChild(new Break());
            paragraph.AppendChild(run);
        }

        /// <summary>
        /// Writes a new comment section to the last paragraph element.
        /// </summary>
        /// <param name="text"></param>
        public void WriteCommentLine(string text)
        {
            Paragraph last = Body.Elements<Paragraph>().Last();         
            Run run = last.AppendChild(new Run());
            RunProperties props = new RunProperties();
            props.FontSize = new FontSize() { Val = "24" };

            run.AppendChild(props);
            run.AppendChild(new Break());
            run.AppendChild(new Text() { Text = text, Space = SpaceProcessingModeValues.Preserve });
            run.AppendChild(new Break());
        }

        /// <summary>
        /// Constructs a new comment section under the last paragraph with from the list of
        /// key value pairs. The key is bolded for each line.
        /// </summary>
        /// <param name="commentLines"></param>
        public void WriteCommentLine(List<KeyValuePair<string, string>> commentLines)
        {
            Paragraph last = Body.Elements<Paragraph>().Last();
            Run newLine = last.AppendChild(new Run());
            newLine.AppendChild(new Break());

            foreach(KeyValuePair<string, string> line in commentLines)
            {
                Run run = last.AppendChild(new Run());
                RunProperties props = new RunProperties();
                props.FontSize = new FontSize() { Val = "24" };
                props.Bold = new Bold();
                run.AppendChild(props);
                run.AppendChild(new Text() { Text = line.Key, Space = SpaceProcessingModeValues.Preserve });

                Run next = last.AppendChild(new Run());
                RunProperties nextProps = new RunProperties();
                props.FontSize = new FontSize() { Val = "24" };
                next.AppendChild(nextProps);
                next.AppendChild(new Text() { Text = line.Value, Space = SpaceProcessingModeValues.Preserve });
                next.AppendChild(new Break());
            }
        }

        /// <summary>
        /// Writes a new formatted route to the last paragraph element.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="text"></param>
        public void WriteRouteLine(string type, string text)
        {
            Paragraph last = Body.Elements<Paragraph>().Last();
            Run run = last.AppendChild(new Run());
            RunProperties props = new RunProperties();
            props.FontSize = new FontSize() { Val = "24" };
            props.Bold = new Bold();

            switch (type)
            {
                case "HttpGet":
                    props.Color = new Color() { Val = "15a612" };
                    break;
                case "HttpPost":
                    props.Color = new Color() { Val = "467be3" };
                    break;
                case "HttpPut":
                    props.Color = new Color() { Val = "e0da1d" };
                    break;
                case "HttpDelete":
                    props.Color = new Color() { Val = "e03614" };
                    break;
            }

            run.Append(props);          
            run.AppendChild(new Text() { Text = $"{type}: ", Space = SpaceProcessingModeValues.Preserve });

            Run next = last.AppendChild(new Run());
            RunProperties nextProps = new RunProperties();
            nextProps.Bold = new Bold();
            nextProps.FontSize = new FontSize() { Val = "24" };

            next.Append(nextProps);
            next.AppendChild(new Text() { Text = $"/{text}", Space = SpaceProcessingModeValues.Preserve });
            next.AppendChild(new Break());
        }

        /// <summary>
        /// Adds a 20pt font-size, bolded, centered line of text.
        /// </summary>
        /// <param name="headerText"></param>
        public void AddTitleLine(string headerText)
        {
            Paragraph paragraph = Body.AppendChild(new Paragraph());
            if (!paragraph.Elements<ParagraphProperties>().Any())
            {
                paragraph.PrependChild(new ParagraphProperties());
            }

            Justification centered = new Justification() { Val = JustificationValues.Center };
            paragraph.ParagraphProperties?.Append(centered);

            Run run = new Run();
            RunProperties props = new RunProperties();
            props.Bold = new Bold();
            props.FontSize = new FontSize() { Val = "40" };

            run.Append(props);
            run.AppendChild(new Break());
            run.AppendChild(new Text() { Text = headerText });
            run.AppendChild(new Break());
            paragraph.AppendChild(run);
        }

        /// <summary>
        /// Disposes of the active document.
        /// </summary>
        public void Save()
        {
            Document.Dispose();
        }
    }
}
