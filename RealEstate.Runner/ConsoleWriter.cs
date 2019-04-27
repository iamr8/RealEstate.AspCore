using System;
using System.IO;
using System.Text;

namespace RealEstate.Runner
{
    public class ConsoleWriterEventArgs : EventArgs
    {
        public string Value { get; private set; }

        public ConsoleWriterEventArgs(string value)
        {
            Value = value;
        }
    }

    public class ConsoleWriter : TextWriter
    {
        public override Encoding Encoding => Encoding.UTF8;

        public override void Write(string value)
        {
            WriteEvent?.Invoke(this, new ConsoleWriterEventArgs(value));
            base.Write(value);
        }

        public override void WriteLine(string value)
        {
            WriteLineEvent?.Invoke(this, new ConsoleWriterEventArgs(value));
            base.WriteLine(value);
        }

        public event EventHandler<ConsoleWriterEventArgs> WriteEvent;

        public event EventHandler<ConsoleWriterEventArgs> WriteLineEvent;
    }
}