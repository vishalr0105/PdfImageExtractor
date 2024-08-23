using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.IO.Image;
using System.IO;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Geom;

class Program
{
    static void Main(string[] args)
    {
        // Specify the path to your PDF and the output directory
        string pdfPath = @"D:\Akshay\Test\Image-Extractor\input_pdf\input.pdf";
        string outputDirectory = @"D:\Test\Image-Extractor\output_images";

        ExtractImagesFromPdf(pdfPath, outputDirectory);
    }

    public static void ExtractImagesFromPdf(string pdfPath, string outputDirectory)
    {
        using (PdfReader reader = new PdfReader(pdfPath))
        using (PdfDocument pdfDoc = new PdfDocument(reader))
        {
            for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
            {
                PdfPage page = pdfDoc.GetPage(i);
                var strategy = new SimpleTextExtractionStrategy();
                PdfCanvasProcessor processor = new PdfCanvasProcessor(new ImageFilterListener(outputDirectory, i));
                processor.ProcessPageContent(page);
            }
        }
    }

    public class ImageFilterListener : IEventListener
    {
        private readonly string _outputDirectory;
        private readonly int _pageNumber;
        private int _imageCount = 0;

        public ImageFilterListener(string outputDirectory, int pageNumber)
        {
            _outputDirectory = outputDirectory;
            _pageNumber = pageNumber;
        }

        public void EventOccurred(IEventData data, EventType eventType)
        {
            if (eventType == EventType.RENDER_IMAGE)
            {
                var renderInfo = (ImageRenderInfo)data;
                var imageObject = renderInfo.GetImage();
                var imageBytes = imageObject.GetImageBytes(true);

                // Get image position
                var position = renderInfo.GetImageCtm();
                float x = position.Get(Matrix.I31);
                float y = position.Get(Matrix.I32);

                // Save image to file
                string outputPath = System.IO.Path.Combine(_outputDirectory, $"Image_{_pageNumber}_{_imageCount}.png");
                File.WriteAllBytes(outputPath, imageBytes);

                // Output image position
                Console.WriteLine($"Image_{_pageNumber}_{_imageCount}.png - Position: (X: {x}, Y: {y})");

                _imageCount++;
            }
        }

        public ICollection<EventType> GetSupportedEvents()
        {
            return null;
        }
    }
}