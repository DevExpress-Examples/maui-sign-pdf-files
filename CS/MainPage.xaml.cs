using DevExpress.Office.DigitalSignatures;
using DevExpress.Pdf;

namespace SignPdfExample;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnOpenWebButtonClicked(object sender, EventArgs e)
    {

        await CopyWorkingFilesToAppData("pfxCertificate.pfx");
        await CopyWorkingFilesToAppData("Faximile.emf");

        try
        {
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

    public async Task CopyWorkingFilesToAppData(string fileName)
    {
        using Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync(fileName);
        string targetFile = Path.Combine(FileSystem.Current.AppDataDirectory, fileName);
        using FileStream outputStream = File.OpenWrite(targetFile);
        fileStream.CopyTo(outputStream);
    }


    async void SignPDF(Stream inputStream, string fileName)
    {
        string resultFile = Path.Combine(FileSystem.Current.AppDataDirectory, fileName.TrimEnd() + "_Signed.pdf");

        using (PdfDocumentSigner documentSigner = new PdfDocumentSigner(inputStream))
        {
            var fieldInfo = new PdfSignatureFieldInfo(1);
            fieldInfo.SignatureBounds = new PdfRectangle(394, 254, 482, 286);

            string signaturePath = Path.Combine(FileSystem.Current.AppDataDirectory, "pfxCertificate.pfx");
            var signer = new Pkcs7Signer(signaturePath, "123", HashAlgorithmType.SHA256);
            PdfSignatureBuilder signature = new PdfSignatureBuilder(signer, fieldInfo);
            string imagePath = Path.Combine(FileSystem.Current.AppDataDirectory, "Faximile.emf");
            signature.SetImageData(System.IO.File.ReadAllBytes(imagePath));
            signature.Location = "USA";
            signature.ContactInfo = "Alex";
            signature.Reason = "Just a test";

            documentSigner.SaveDocument(resultFile, signature);
        }

        // Share the resulting file
        await ShareFile(resultFile);

    }
    public async Task ShareFile(string filename)
    {
        string file = Path.Combine(FileSystem.Current.AppDataDirectory, filename);

        await Share.Default.RequestAsync(new ShareFileRequest
        {
            Title = "Share PDF file",
            File = new ShareFile(file)
        });
    }

}

