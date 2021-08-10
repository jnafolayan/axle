using System;

namespace Axle.Engine.FileParsers.Exceptions
{
    public class FileNotFoundException : Exception
    {
        public FileNotFoundException(string uri) : base($"File not found: {uri}")
        {}
    }
}