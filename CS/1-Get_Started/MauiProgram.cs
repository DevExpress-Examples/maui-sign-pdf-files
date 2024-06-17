using DevExpress.Drawing.Internal;
using DevExpress.Maui;
using Microsoft.Extensions.Logging;
using SkiaSharp;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace SignPdfExample;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		DXDrawingEngine.ForceSkia();

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
			.UseSkiaSharp();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
