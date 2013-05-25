using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace StartMenuWin8
{
    public class TreeViewBinding
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public ImageSource Icon { get; set; }

        public TreeViewBinding() { }
        public TreeViewBinding(string name, string path) { this.Name = name; this.Path = path; }
        public TreeViewBinding(string name, string path, ImageSource icon)
        {
            this.Name = name;
            this.Path = path;
            this.Icon = icon;
        }
    }

    public class TreeViewItemDirectoryFolder : TreeViewBinding
    {
        public TreeViewItemDirectoryFolder() { }
        public TreeViewItemDirectoryFolder(string name, string path) : base(name, path)
        {
            try
            {
                System.Drawing.Icon ico = System.Drawing.Icon.ExtractAssociatedIcon(path);
                Bitmap bitmap = ico.ToBitmap();

                System.Windows.Media.Imaging.BitmapSource bitmapSource =
                    System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                        bitmap.GetHbitmap(),
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions()
                    );

                this.Icon = bitmapSource;
            }
            catch (Exception) { }
        }
    }

    public class TreeViewExecutable : TreeViewBinding
    {
        public TreeViewExecutable() { }
        public TreeViewExecutable(string name, string path) : base(name, path) 
        {
            try
            {
                System.Drawing.Icon ico = System.Drawing.Icon.ExtractAssociatedIcon(path);
                Bitmap bitmap = ico.ToBitmap();

                System.Windows.Media.Imaging.BitmapSource bitmapSource = 
                    System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                        bitmap.GetHbitmap(),
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions()
                    );

                this.Icon = bitmapSource;
            }
            catch (Exception) { }
        }
    }
}
