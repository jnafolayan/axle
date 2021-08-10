using System;

namespace Axle.Engine.FileParsers
{
    public abstract class FileParserBase
    {
        public virtual string ParseLocalFile(string filePath) 
        {
            throw new NotImplementedException();
        }
        
        public virtual string ParseRemoteFile(string fileURL)
        {
            throw new NotImplementedException();
        }
    }
}