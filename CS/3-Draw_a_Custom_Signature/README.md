
# Use the DevExpress Office File API to Draw a Signature and Sign a PDF File

This project uses the [.NET MAUI Community Toolkit DrawingView](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/maui/views/drawingview#using-the-drawingview) to display a signature pad. This pad allows you to draw your signature and place it in the specified PDF document (using the DevExpress Office File API). Once the file is signed, the app can share the file with other applications.

<img src="./media/drawable_signature.gif" alt="drawing" width="300"/>

## Implementation Details

> **Note**
>
> PDF-related functionality described herein requires the DevExpress [Office File API](https://www.devexpress.com/products/net/office-file-api/) or [DevExpress Universal](https://www.devexpress.com/subscriptions/universal.xml) Subscription. For purchase/licensing information, please visit [devexpress.com](https://www.devexpress.com).

Our [PdfDocumentProcessor](https://docs.devexpress.com/OfficeFileAPI/DevExpress.Pdf.PdfDocumentProcessor) class includes APIs designed to modify PDF files.

To open and sign a PDF file in a .NET MAUI application, you must copy PDF and PFX certificate files from the application bundle to a device folder:

```csharp
public async Task<string> CopyWorkingFilesToAppData(string fileName) {
    using Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync(fileName);
    string targetFile = Path.Combine(FileSystem.Current.AppDataDirectory, fileName);
    using FileStream outputStream = File.OpenWrite(targetFile);
    fileStream.CopyTo(outputStream);
    return targetFile;
}
```

Once implemented, you can use the [PdfDocumentProcessor.LoadDocument](https://docs.devexpress.com/OfficeFileAPI/DevExpress.Pdf.PdfDocumentProcessor.LoadDocument.overloads) method to open the PDF file.

This project uses the following members to find the first available signature field: 

* [PdfAcroFormFacade.GetFields](https://docs.devexpress.com/OfficeFileAPI/DevExpress.Pdf.PdfAcroFormFacade.GetFields) method: finds all fields in the PDF file. 
* The [PdfFormFieldFacade.Type](https://docs.devexpress.com/OfficeFileAPI/DevExpress.Pdf.PdfFormFieldFacade.Type) property: helps locate signature fields.  

The [PDFSignatureBuilder](https://docs.devexpress.com/OfficeFileAPI/DevExpress.Pdf.PdfSignatureBuilder) class stores the following information about the signature:

* PFX certificate file
* Location of the person who signs the document
* Name of the person who signs the document
* Reason for signing the document
* Signature image that is embedded in the PDF file

This project uses the [DrawingView](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/maui/views/drawingview#multiline-usage) control to draw a signature. To convert a drawn signature to a JPEG image, the project uses the following code:

```csharp
using Stream origJpgStream = await drawingView.GetImageStream(200, 200);
origJpgStream.Seek(0, SeekOrigin.Begin);
Microsoft.Maui.Graphics.IImage img = PlatformImage.FromStream(origJpgStream, ImageFormat.Jpeg);
var jpegImageBytes = img.AsBytes(ImageFormat.Png);
```

After the document is signed, the [PdfDocumentProcessor.SaveDocument](https://docs.devexpress.com/OfficeFileAPI/DevExpress.Pdf.PdfDocumentProcessor.SaveDocument.overloads) method saves the signed PDF file.

## Documentation

* [Use Office File API in .NET MAUI Applications (macOS, iOS, Android)](https://docs.devexpress.com/OfficeFileAPI/404423/use-pdf-document-api-in-net-maui-applications?v=23.1)

## More Examples

* [DevExpress .NET MAUI Controls - Send Template Messages with Mail Merge](https://github.com/DevExpress-Examples/maui-mail-merge)
