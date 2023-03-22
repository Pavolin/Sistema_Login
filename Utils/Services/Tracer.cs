using System;
using System.IO;
using System.Threading;

using Utils.Enums;
using Utils.Interfaces;
using Utils.Exceptions;

namespace Utils.Services
{
    public class Tracer : ITracer
    {

        #region Attributes
        /// <summary> 
        /// area das referencias ao atributos usados nessa classe
        /// </summary>
        private const string _bodyLine = @"[{0}].[{2}] - [{1}] : {3}";
        private readonly SemaphoreSlim _tracerLock = new SemaphoreSlim(1, 1);
        private TraceLevel _level { get; set; }
        private string _folder { get; set; }
        public bool Enabled { get; set; }
        public bool DetailedException { get; set; }
        public string ActualFile { get; private set; }
        public int? MaxSize { get; private set; }
        #endregion

        #region Constructor Methods
        /// <summary> 
        /// area dos contrustores usados nessa classe
        /// </summary>
        public Tracer(string folder, bool enabled = false, TraceLevel level = TraceLevel.Full)
        {
            if (folder == string.Empty) throw new TracerExceptions("Tracer.Folder can't be empty");
            if (!Directory.Exists(folder)) throw new TracerExceptions("The Folder: " + folder + " not Exists");
            if (folder[folder.Length - 1] != Path.DirectorySeparatorChar) folder += Path.DirectorySeparatorChar;
            this._folder = folder;
            this._createNewFile();
            this.Enabled = enabled;
            this.DetailedException = false;
            this._level = level;
            this.MaxSize = null;
        }
        #endregion

        #region Methods
        /// <summary> 
        /// area dos metodos usados em outras classes
        /// </summary>
        private void _createNewFile()
        {
            this.ActualFile = this._folder + "Trace_" + DateTime.Now.ToString("ddMMyyyy_HHmmssfff") + ".log";
        }

        private int? _convertToMaxSize(string value)
        {
            int? maxSize = 0;
            foreach (char c in value)
            {
                try
                {
                    maxSize = int.Parse(c + "") + maxSize * 10;
                }
                catch
                {
                    switch (c)
                    {
                        case 'K':
                            maxSize *= 1000;
                            break;
                        case 'M':
                            maxSize *= 1000000;
                            break;
                        case 'G':
                            maxSize *= 1000000000;
                            break;
                    }
                    break;
                }
            }
            return maxSize <= 0 ? null : maxSize;
        }

        public void WriteLine(string line, TraceLevel level)
        {
            try
            {
                string formatedLine = string.Format(_bodyLine,
                                                        DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"),
                                                        level.ToString(),
                                                        Thread.CurrentThread.ManagedThreadId.ToString(),
                                                        line);

                if (System.Diagnostics.Debugger.IsAttached)
                    Console.WriteLine(formatedLine);

                if (this.Enabled)
                {
                    this._tracerLock.Wait();
                    using (StreamWriter fw = File.AppendText(this.ActualFile))
                    {
                        if (this._level == level || this._level == TraceLevel.Full)
                            fw.WriteLine(formatedLine);
                    }
                    if (this.MaxSize != null && this.MaxSize > 0)
                    {
                        long FileLeng = new FileInfo(this.ActualFile).Length;
                        if (FileLeng >= this.MaxSize)
                        {
                            this._createNewFile();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new TracerExceptions(ex.Message);
            }
            finally
            {
                _tracerLock.Release();
            }
        }

        public void WriteLine(Exception ex)
        {
            string message = "Exception - " + ex.Message;
            if (this.DetailedException)
            {
                message += "\r\n---Source: " + ex.Source;
                message += "\r\n---StackTrace: " + ex.StackTrace;
            }
            this.WriteLine(message, TraceLevel.Error);
        }

        public void SetTracerLevel(string level)
        {
            try
            {
                this._level = (TraceLevel)Enum.Parse(typeof(TraceLevel), level, true);
            }
            catch (Exception ex)
            {
                throw new TracerExceptions(ex.Message);
            }
        }

        public void SetMaxSize(string maxSize)
        {
            this.MaxSize = this._convertToMaxSize(maxSize);
        }

        #endregion
    }
}