namespace WhatsappBot.ExternalApis.Evolution;

using System;

public static class EvolutionDelayCalculator
{
    private static readonly Random _random = new Random();
    public const int DefaultSafeDelay = 1200;

    /// <summary>
    /// Calculates the specific delay (ms) to send to Evolution API 
    /// to simulate human typing for a given message length.
    /// </summary>
    /// <param name="message">The message text.</param>
    /// <param name="wpm">Typing speed (40-55 recommended).</param>
    /// <returns>Integer delay in milliseconds.</returns>
    public static int GetHumanDelay(string message, int wpm = 50)
    {
        if (string.IsNullOrEmpty(message)) return DefaultSafeDelay; // Default safe delay

        int charCount = message.Length;
        
        // 1. Base delay per char (ms) based on WPM
        double baseDelayPerChar = 60000.0 / (wpm * 5.0);
        
        // 2. Calculate raw total typing time
        double totalTypingTime = charCount * baseDelayPerChar;

        // 3. Apply Gaussian Jitter (Box-Muller) to the TOTAL time
        // This varies the total duration significantly between requests
        double u1 = 1.0 - _random.NextDouble();
        double u2 = 1.0 - _random.NextDouble();
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
        
        // Jitter = 20% of the total typing time
        double jitter = randStdNormal * (totalTypingTime * 0.20);
        
        double variedTypingTime = totalTypingTime + jitter;

        // 4. Add "Send Button" Lag (Human reflex after typing finishes)
        // Random between 200ms and 600ms
        double sendLag = 200 + (_random.NextDouble() * 400);

        double finalDelay = variedTypingTime + sendLag;

        // 5. CLAMPING (Critical for Evolution API)
        // Min: 1000ms (Avoids 'instant' flag)
        // Max: 8000ms (Avoids 'timeout' or suspicious long pauses for short texts)
        finalDelay = Math.Max(1000, Math.Min(8000, finalDelay));

        return (int)finalDelay;
    }
}   