using System;

namespace Axle.Engine.FileParsers.Exceptions
{
    /// <summary>
    /// Thrown when the file was not found
    /// </summary>
    public class FileNotFoundException : Exception
    {
        public FileNotFoundException(string uri) : base($"File not found: {uri}")
        {}
    }
}