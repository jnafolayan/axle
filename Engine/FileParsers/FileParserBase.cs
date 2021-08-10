using System;

namespace Axle.Engine.FileParsers
{
    /// <summary>
    /// Base class for file parsers
    /// </summary>
    public abstract class FileParserBase
    {
        /// <summary>
        /// Parses a local file into text
        /// </summary>
        /// <param name="filepath">The path to the file</param>
        /// <returns>The text contained in the file</returns>
        public virtual string ParseLocalFile(string filePath) 
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Parses a remote file into text
        /// </summary>
        /// <param name="fileURL">The url to the file</param>
        /// <returns>The text contained in the file</returns>        
        public virtual string ParseRemoteFile(string fileURL)
        {
            throw new NotImplementedException();
        }
    }
}