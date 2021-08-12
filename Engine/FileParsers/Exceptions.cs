using System;

namespace Axle.Engine.FileParsers.Exceptions
{
    /// <summary>
    /// Thrown when the file was not found
    /// </summary>
    public class TextExtractionNotSupportedException : Exception
    {
        public TextExtractionNotSupportedException(string uri) : base($"Text extraction not supported: {uri}")
        {}
    }
}