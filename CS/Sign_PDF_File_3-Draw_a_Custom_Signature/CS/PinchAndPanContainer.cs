using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignPDF;

public class PinchAndPanContainer : ContentView {
    double currentScale = 1;
    double startScale = 1;
    double xOffset = 0;
    double yOffset = 0;
    bool isPinchRunning;

    public PinchAndPanContainer() {

        PinchGestureRecognizer pinchGesture = new PinchGestureRecognizer();
        pinchGesture.PinchUpdated += OnPinchUpdated;
        GestureRecognizers.Add(pinchGesture);

        PanGestureRecognizer panGesture = new PanGestureRecognizer();
        panGesture.PanUpdated += OnPanUpdated;
        GestureRecognizers.Add(panGesture);
    }

    void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e) {
        if (e.Status == GestureStatus.Started) {
            isPinchRunning = true;
            startScale = Content.Scale;
            Content.AnchorX = 0;
            Content.AnchorY = 0;
        }
        if (e.Status == GestureStatus.Running) {
            currentScale += (e.Scale - 1) * startScale;
            currentScale = Math.Max(1, currentScale);

            double renderedX = Content.X + xOffset;
            double deltaX = renderedX / Width;
            double deltaWidth = Width / (Content.Width * startScale);
            double originX = (e.ScaleOrigin.X - deltaX) * deltaWidth;

            double renderedY = Content.Y + yOffset;
            double deltaY = renderedY / Height;
            double deltaHeight = Height / (Content.Height * startScale);
            double originY = (e.ScaleOrigin.Y - deltaY) * deltaHeight;

            double targetX = xOffset - (originX * Content.Width) * (currentScale - startScale);
            double targetY = yOffset - (originY * Content.Height) * (currentScale - startScale);

            Content.TranslationX = Math.Clamp(targetX, -Content.Width * (currentScale - 1), 0);
            Content.TranslationY = Math.Clamp(targetY, -Content.Height * (currentScale - 1), 0);

            Content.Scale = currentScale;
        }
        if (e.Status == GestureStatus.Completed) {
            xOffset = Content.TranslationX;
            yOffset = Content.TranslationY;

            Console.WriteLine($"zoom completed");
        }
    }

    void OnPanUpdated(object sender, PanUpdatedEventArgs e) {
        if (isPinchRunning)
            return;
        switch (e.StatusType) {
            case GestureStatus.Running:
                Content.TranslationX = Math.Max(Math.Min(0, xOffset + e.TotalX), -Math.Abs(Content.Width - DeviceDisplay.MainDisplayInfo.Width));
                Content.TranslationY = Math.Max(Math.Min(0, yOffset + e.TotalY), -Math.Abs(Content.Height - DeviceDisplay.MainDisplayInfo.Height));
                Console.WriteLine($"x: {Content.TranslationX}, y:{Content.TranslationY}");
                break;

            case GestureStatus.Completed:
                xOffset = Content.TranslationX;
                yOffset = Content.TranslationY;
                break;
        }
    }
}

