﻿using MyTend.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MyTend.Services.File
{
    public class FileControllerService
    {
        public FileSystem UpdateAvatar(HttpPostedFileBase[] files, UserSystem user)
        {
            FileSystem result = null;

            if (files != null)
            {
                var service = new FileService();

                var data = new List<FileInfo>();

                foreach (var file in files)
                {
                    if (file == null)
                    {
                        continue;
                    }

                    var fileInfo = new FileInfo()
                    {
                        Data = file.InputStream,
                        Name = file.FileName,
                        MimeType = file.ContentType
                    };

                    data.Add(fileInfo);
                }

                this.DeleteOldAvatar(user);

                var resultData = service.Save(user, data);

                if(resultData.Count > 0)
                {
                    result = resultData
                        .FirstOrDefault()
                        .File;
                }
            }

            return result;
        }

        private void DeleteOldAvatar(UserSystem user)
        {
            if (user.Avatar == null)
            {
                return;
            }

            var service = new FileService();

            var id = user.Avatar.Id;

            user.Avatar = null;
            user.Update();

            service.Delete(id);
        }
    }
}
