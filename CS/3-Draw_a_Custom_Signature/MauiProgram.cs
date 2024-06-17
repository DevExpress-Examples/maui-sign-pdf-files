using CommunityToolkit.Maui;
using DevExpress.Drawing.Internal;
using DevExpress.Maui;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace SignPDF;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        // DXDrawingEngine.ForceSkia();

        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
			.UseDevExpress()
			.UseDevExpressCollectionView()
			.UseDevExpressControls()
			.UseDevExpressEditors()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .UseSkiaSharp()
            .UseMauiCommunityToolkit();
        return builder.Build();
    }
}
