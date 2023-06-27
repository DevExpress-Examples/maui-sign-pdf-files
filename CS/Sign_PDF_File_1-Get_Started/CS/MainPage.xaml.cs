using DevExpress.Office.DigitalSignatures;
using DevExpress.Pdf;

namespace SignPdfExample;

public partial class MainPage : ContentPage
{
    string imagePath;
    string signaturePath;
    public MainPage()
    {
        InitializeComponent();
    }
    async Task InitFiles()
    {
        signaturePath = await CopyWorkingFilesToAppData("pfxCertificate.pfx");
        imagePath = await CopyWorkingFilesToAppData("Faximile.emf");
    }
    public async Task<string> CopyWorkingFilesToAppData(string fileName)
    {
        using Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync(fileName);
        string targetFile = Path.Combine(FileSystem.Current.AppDataDirectory, fileName);
        using FileStream outputStream = File.OpenWrite(targetFile);
        fileStream.CopyTo(outputStream);
        return targetFile;
    }
    private async void OnOpenWebButtonClicked(object sender, EventArgs e)
    {
        try
        {
            if (signaturePath == null || imagePath == null)
            {
                await InitFiles();
            }
            var result = await FilePicker.Default.PickAsync(PickOptions.Default);
            if (result != null)
            {
                if (result.FileName.EndsWith("pdf", StringComparison.OrdinalIgnoreCase))
                {
                    var stream = await result.OpenReadAsync();
                    SignPDF(stream, Path.GetFileNameWithoutExtension(result.FullPath));
                }
            }
        }
        catch (Exception ex)
        {
            // The user canceled or something went wrong
        }
    }
    async void SignPDF(Stream inputStream, string fileName)
    {
        using var processor = new PdfDocumentProcessor();
        processor.LoadDocument(inputStream);
        PdfDocumentFacade documentFacade = processor.DocumentFacade;

        PdfSignatureFormFieldFacade signatureFacade = documentFacade.AcroForm.GetFields().
            FirstOrDefault(ff => ff.Type == PdfFormFieldType.Signature) as PdfSignatureFormFieldFacade;

        PdfRectangle rectangle;
        string fieldName;
        PdfSignatureBuilder signature;

        if (signatureFacade != null)
        {

            fieldName = signatureFacade.FullName;
            signature = CreateSignature(signaturePath, "123", "USA", "Alex", "Just a Test", imagePath, fieldName);
        }
        else
        {

            rectangle = new PdfRectangle(394, 254, 482, 286);
            signature = CreateSignature(signaturePath, "123", "USA", "Alex", "Just a Test", imagePath, rectangle);
        }

        using PdfDocumentSigner documentSigner = new PdfDocumentSigner(inputStream);
        string resultFile = Path.Combine(FileSystem.Current.AppDataDirectory, fileName.TrimEnd() + "_Signed.pdf");
        documentSigner.SaveDocument(resultFile, signature);

        await ShareFile(resultFile);
    }
    PdfSignatureBuilder CreateSignature(string certificateFilePath, string password, string location, string contactInfo, string reason, string imagePath, PdfRectangle rectnagle)
    {
        var fieldInfo = new PdfSignatureFieldInfo(1);
        fieldInfo.SignatureBounds = rectnagle;

        var signer = new Pkcs7Signer(certificateFilePath, password, HashAlgorithmType.SHA256);
        PdfSignatureBuilder signature = new PdfSignatureBuilder(signer, fieldInfo);
        signature.SetImageData(System.IO.File.ReadAllBytes(imagePath));
        signature.Location = location;
        signature.ContactInfo = contactInfo;
        signature.Reason = reason;
        return signature;
    }

    PdfSignatureBuilder CreateSignature(string certificateFilePath, string password, string location, string contactInfo, string reason, string imagePath, string fieldName)
    {

        var signer = new Pkcs7Signer(certificateFilePath, password, HashAlgorithmType.SHA256);
        PdfSignatureBuilder signature = new PdfSignatureBuilder(signer, fieldName);
        signature.SetImageData(System.IO.File.ReadAllBytes(imagePath));
        signature.Location = location;
        signature.ContactInfo = contactInfo;
        signature.Reason = reason;
        return signature;
    }
    public async Task ShareFile(string filename)
    {
        await Share.Default.RequestAsync(new ShareFileRequest
        {
            Title = "Share PDF file",
            File = new ShareFile(Path.Combine(FileSystem.Current.AppDataDirectory, filename))
        });
    }
}
