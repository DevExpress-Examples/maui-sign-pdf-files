using CommunityToolkit.Maui.Views;
using DevExpress.Drawing;
using DevExpress.Maui.Core.Internal;
using DevExpress.Office.DigitalSignatures;
using DevExpress.Pdf;
using Microsoft.Maui.Graphics.Platform;
using SkiaSharp;
using SkiaSharp.Views.Maui.Controls;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SignPDF;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }
}