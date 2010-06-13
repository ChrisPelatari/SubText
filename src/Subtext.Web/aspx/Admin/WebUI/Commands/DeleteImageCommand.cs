#region Disclaimer/Info

///////////////////////////////////////////////////////////////////////////////////////////////////
// Subtext WebLog
// 
// Subtext is an open source weblog system that is a fork of the .TEXT
// weblog system.
//
// For updated news and information please visit http://subtextproject.com/
// Subtext is hosted at Google Code at http://code.google.com/p/subtext/
// The development mailing list is at subtext@googlegroups.com 
//
// This project is licensed under the BSD license.  See the License.txt file for more information.
///////////////////////////////////////////////////////////////////////////////////////////////////

#endregion

using System;
using System.Globalization;
using System.IO;
using Subtext.Framework;
using Subtext.Framework.Components;

namespace Subtext.Web.Admin.Commands
{
    [Serializable]
    public class DeleteImageCommand : DeleteTitledTargetCommand
    {
        public DeleteImageCommand(Image image, string galleryDirectoryPath)
            : this(galleryDirectoryPath, image, "Image " + image.ImageID.ToString(CultureInfo.InvariantCulture))
        {
        }

        public DeleteImageCommand(string galleryDirectoryPath, Image image, string imageTitle)
        {
            _targetName = "Image";
            itemTitle = imageTitle;
            Image = image;
        }

        public string GalleryDirectoryPath { get; private set; }
        public Image Image { get; private set; }

        public override string Execute()
        {
            try
            {
                Image currentImage = Image;

                // The following line should be fully encapsulated and handle files + data
                // For now, I commented out the file trys in the the object so it can do just
                // data without exception. I'll do the files locally until we decide to really
                // do the framework class

                Images.DeleteImage(currentImage);

                // now delete the associated files if they exist
                string galleryFolder = GalleryDirectoryPath;
                if(Directory.Exists(galleryFolder))
                {
                    DeleteFile(galleryFolder, currentImage.OriginalFile);
                    DeleteFile(galleryFolder, currentImage.ResizedFile);
                    DeleteFile(galleryFolder, currentImage.ThumbNailFile);
                }

                return FormatMessage(ExecuteSuccessMessage, _targetName, itemTitle);
            }
            catch(Exception ex)
            {
                return FormatMessage(ExecuteFailureMessage, _targetName, Image.ImageID, ex.Message);
            }
        }

        private void DeleteFile(string path, string filename)
        {
            string localPath = Path.Combine(path, filename);
            if(File.Exists(localPath))
            {
                File.Delete(localPath);
            }
        }
    }
}