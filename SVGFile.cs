using SVGImage.SVG;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Media;

namespace SVGViewer
{
    public class SVGFile
    {
        private bool _isLogged;
        DrawingGroup _drawing = null;
        public SVGFile(string fullpath)
        {
            FullPath = fullpath;
            Filename = Path.GetFileNameWithoutExtension(fullpath);
        }

        public string FullPath { get; private set; }
        public string Filename { get; private set; }
        public SVGRender SVGRender { get; private set; }
        public DrawingGroup SVGImage
        {
            get
            {
                EnsureLoaded();
                return _drawing;
            }
        }

        public void Reload()
        {
            _drawing = SVGRender.LoadDrawing(FullPath);
        }

        void EnsureLoaded()
        {
            try
            {
                if (_drawing != null)
                    return;
                SVGRender = new SVGRender();
                _drawing = SVGRender.LoadDrawing(FullPath);
            }
            catch (Exception ex)
            {
                if (_isLogged)
                {
                    return;
                }
                _isLogged = true;
                Trace.TraceError("Exception Loading: " + this.FullPath);
                Trace.WriteLine(ex.ToString());
                Trace.WriteLine(string.Empty);
            }
        }
    }
}
