using GleamTech.FileUltimate;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace MvcPhoenix.Controllers
{
    public class DocumentsController : Controller
    {
        private const string EventLogSessionKey = "EventLog.CS";

        public ActionResult Index()
        {
            var fileManager = new FileManager
            {
                Width = Unit.Percentage(100),
                Height = 750,
                CollapseRibbon = true,
                DisplayLanguage = "en"
            };
            fileManager.RootFolders.Add(new FileManagerRootFolder
            {
                Name = "Root",
                Location = "~/App_Data/DocsDir/ROOT"
            });
            fileManager.RootFolders[0].AccessControls.Add(new FileManagerAccessControl
            {
                Path = @"\",
                AllowedPermissions = FileManagerPermissions.Full
            });

            //After Events which are fired after the action is completed.
            fileManager.Expanded += FileManagerExpanded;
            fileManager.Created += FileManagerCreated;
            fileManager.Deleted += FileManagerDeleted;
            fileManager.Renamed += FileManagerRenamed;
            fileManager.Copied += FileManagerCopied;
            fileManager.Moved += FileManagerMoved;
            fileManager.Compressed += FileManagerCompressed;
            fileManager.Extracted += FileManagerExtracted;
            fileManager.Uploaded += FileManagerUploaded;
            fileManager.Downloaded += FileManagerDownloaded;
            fileManager.Previewed += FileManagerPreviewed;
            fileManager.Failed += FileManagerFailed;

            return View(fileManager);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Activity()
        {
            string filepath = Server.MapPath("\\App_Data\\DocsDir\\eventslog.txt");
            string content = string.Empty;

            try
            {
                using (var stream = new StreamReader(filepath))
                {
                    content = stream.ReadToEnd();
                }
            }
            catch (Exception)
            {
                return Content("Uh oh!");
            }

            return Content(content, "text/plain");
        }

        private static void LogEvents()
        {
            var context = System.Web.HttpContext.Current;
            var eventLog = context.Session[EventLogSessionKey] as Stack<string>;

            var sb = new StringBuilder();

            if (eventLog == null || eventLog.Count == 0)
                sb.AppendLine("No events."); //meaningless because I removed it from log but left it here for easy readings.
            else
                while (eventLog.Count > 0)
                {
                    sb.AppendLine(new String('-', 80));
                    sb.AppendLine("Event " + eventLog.Count + " - " + DateTime.Now.ToString("R"));
                    sb.AppendLine("User: " + context.User.Identity.Name.ToString());
                    sb.AppendLine(eventLog.Pop());
                }

            System.IO.File.AppendAllText(@"C:\inetpub\wwwroot\Phoenix\App_Data\DocsDir\eventslog.txt", sb.ToString());
        }

        #region Event handlers for before events

        private static void FileManagerExpanding(object sender, FileManagerExpandingEventArgs e)
        {
            SaveEventInfo(new Dictionary<string, object>
                {
                    {"Event Type", "Expanding"},
                    {"Path", e.Folder.FullPath},
                    {"Is Refresh", e.IsRefresh}
                });
        }

        private static void FileManagerListing(object sender, FileManagerListingEventArgs e)
        {
            SaveEventInfo(new Dictionary<string, object>
                {
                    {"Event Type", "Listing"},
                    {"Path", e.Folder.FullPath},
                    {"Is Refresh", e.IsRefresh}
                });
        }

        private static void FileManagerCreating(object sender, FileManagerCreatingEventArgs e)
        {
            SaveEventInfo(new Dictionary<string, object>
                {
                    {"Event Type", "Creating"},
                    {"Path", e.Folder.FullPath},
                    {"Creating Folder", e.ItemName}
                });
        }

        private static void FileManagerDeleting(object sender, FileManagerDeletingEventArgs e)
        {
            SaveEventInfo(new Dictionary<string, object>
                {
                    {"Event Type", "Deleting"},
                    {"Path", e.Folder.FullPath},
                    {"Deleting Items", e.ItemNames}
                });
        }

        private static void FileManagerRenaming(object sender, FileManagerRenamingEventArgs e)
        {
            SaveEventInfo(new Dictionary<string, object>
                {
                    {"Event Type", "Renaming"},
                    {"Path", e.Folder.FullPath},
                    {"Item Old Name", e.ItemName},
                    {"Item New Name", e.ItemNewName}
                });
        }

        private static void FileManagerCopying(object sender, FileManagerCopyingEventArgs e)
        {
            SaveEventInfo(new Dictionary<string, object>
                {
                    {"Event Type", "Copying"},
                    {"From Path", e.Folder.FullPath},
                    {"Copying Items", e.ItemNames},
                    {"To Path", e.TargetFolder.FullPath},
                });
        }

        private static void FileManagerMoving(object sender, FileManagerMovingEventArgs e)
        {
            SaveEventInfo(new Dictionary<string, object>
                {
                    {"Event Type", "Moving"},
                    {"From Path", e.Folder.FullPath},
                    {"Moving Items", e.ItemNames},
                    {"To Target Path", e.TargetFolder.FullPath},
                });
        }

        private static void FileManagerCompressing(object sender, FileManagerCompressingEventArgs e)
        {
            SaveEventInfo(new Dictionary<string, object>
                {
                    {"Event Type", "Compressing"},
                    {"Path", e.Folder.FullPath},
                    {"Compressing Items", e.ItemNames},
                    {"Zip File", e.ZipFileName}
                });
        }

        private static void FileManagerExtracting(object sender, FileManagerExtractingEventArgs e)
        {
            SaveEventInfo(new Dictionary<string, object>
                {
                    {"Event Type", "Extracting"},
                    {"Path", e.Folder.FullPath},
                    {"Extracting to Subfolder", e.ToSubfolder},
                    {"Archive File", e.ArchiveFileName}
                });
        }

        private static void FileManagerUploading(object sender, FileManagerUploadingEventArgs e)
        {
            SaveEventInfo(new Dictionary<string, object>
                {
                    {"Event Type", "Uploading"},
                    {"Path", e.Folder.FullPath},
                    {"Upload Method", e.Method},
                    {"Uploading Files", e.Validations.Select(validation => new Dictionary<string, object>
                        {
                            {"Name", validation.Name},
                            {"Content Type", validation.ContentType},
                            {"Size", validation.Size.HasValue ? new ByteSizeValue(validation.Size.Value).ToFileSize().ToString() : "<unknown>"}
                        })
                    }
                });
        }

        private static void FileManagerDownloading(object sender, FileManagerDownloadingEventArgs e)
        {
            SaveEventInfo(new Dictionary<string, object>
                {
                    {"Event Type", "Downloading"},
                    {"Path", e.Folder.FullPath},
                    {"Downloading Items", e.ItemNames},
                    {"Downloading File Name", e.DownloadFileName},
                    {"Opening in browser", e.OpenInBrowser}
                });
        }

        private static void FileManagerPreviewing(object sender, FileManagerPreviewingEventArgs e)
        {
            SaveEventInfo(new Dictionary<string, object>
                {
                    {"Event Type", "Previewing"},
                    {"Path", e.Folder.FullPath},
                    {"Previewing File", e.ItemName},
                    {"Previewer", e.PreviewerType}
                });
        }

        #endregion Event handlers for before events

        #region Event handlers for after events

        private static void FileManagerExpanded(object sender, FileManagerExpandedEventArgs e)
        {
            SaveEventInfo(new Dictionary<string, object>
                {
                    {"Event Type", "Expanded"},
                    {"Path", e.Folder.FullPath},
                    {"Is Refresh", e.IsRefresh}
                });
        }

        private static void FileManagerListed(object sender, FileManagerListedEventArgs e)
        {
            SaveEventInfo(new Dictionary<string, object>
                {
                    {"Event Type", "Listed"},
                    {"Path", e.Folder.FullPath},
                    {"Is Refresh", e.IsRefresh}
                });
        }

        private static void FileManagerCreated(object sender, FileManagerCreatedEventArgs e)
        {
            SaveEventInfo(new Dictionary<string, object>
                {
                    {"Event Type", "Created"},
                    {"Path", e.Folder.FullPath},
                    {"Created Folder", e.ItemName}
                });

            LogEvents();
        }

        private static void FileManagerDeleted(object sender, FileManagerDeletedEventArgs e)
        {
            SaveEventInfo(new Dictionary<string, object>
                {
                    {"Event Type", "Deleted"},
                    {"Path", e.Folder.FullPath},
                    {"Deleted Items", e.ItemNames}
                });

            LogEvents();
        }

        private static void FileManagerRenamed(object sender, FileManagerRenamedEventArgs e)
        {
            SaveEventInfo(new Dictionary<string, object>
                {
                    {"Event Type", "Renamed"},
                    {"Path", e.Folder.FullPath},
                    {"Item Old Name", e.ItemName},
                    {"Item New Name", e.ItemNewName}
                });

            LogEvents();
        }

        private static void FileManagerCopied(object sender, FileManagerCopiedEventArgs e)
        {
            SaveEventInfo(new Dictionary<string, object>
                {
                    {"Event Type", "Copied"},
                    {"From Path", e.Folder.FullPath},
                    {"Source Items", e.ItemNames},
                    {"To Path", e.TargetFolder.FullPath},
                    {"Copied Items", e.TargetItemNames}
                });

            LogEvents();
        }

        private static void FileManagerMoved(object sender, FileManagerMovedEventArgs e)
        {
            SaveEventInfo(new Dictionary<string, object>
                {
                    {"Event Type", "Moved"},
                    {"From Path", e.Folder.FullPath},
                    {"Moved Items", e.ItemNames},
                    {"To Path", e.TargetFolder.FullPath},
                });

            LogEvents();
        }

        private static void FileManagerCompressed(object sender, FileManagerCompressedEventArgs e)
        {
            SaveEventInfo(new Dictionary<string, object>
                {
                    {"Event Type", "Compressed"},
                    {"Path", e.Folder.FullPath},
                    {"Compressed Items", e.ItemNames},
                    {"Zip File", e.ZipFileName}
                });

            LogEvents();
        }

        private static void FileManagerExtracted(object sender, FileManagerExtractedEventArgs e)
        {
            SaveEventInfo(new Dictionary<string, object>
                {
                    {"Event Type", "Extracted"},
                    {"Path", e.Folder.FullPath},
                    {"Extracted to Subfolder", e.ToSubfolder},
                    {"Archive File", e.ArchiveFileName}
                });

            LogEvents();
        }

        private static void FileManagerUploaded(object sender, FileManagerUploadedEventArgs e)
        {
            SaveEventInfo(new Dictionary<string, object>
                {
                    {"Event Type", "Uploaded"},
                    {"Path", e.Folder.FullPath},
                    {"Upload Method", e.Method},
                    {"Uploading Files", e.Items.Select(item => new Dictionary<string, object>
                        {
                            {"Name", (item.Status == UploadItemStatus.Completed) ? item.ReceivedName : item.ReceivingName},
                            {"Content Type", (item.Status == UploadItemStatus.Completed) ? item.ReceivedContentType : item.ReceivingContentType},
                            {"Size", new ByteSizeValue((item.Status == UploadItemStatus.Completed) ? item.ReceivedSize : item.ReceivingSize).ToFileSize()},
                            {"Status", item.Status},
                            {"Status Message", (!string.IsNullOrEmpty(item.StatusMessage)) ? Regex.Replace(item.StatusMessage, @"\r\n+", "\r\n\t\t") : "<none>"}
                        })
                    },
                    {"Total Size", new ByteSizeValue(e.TotalSize).ToFileSize()},
                    {"Elapsed Time", e.ElapsedTime},
                    {"Transfer Rate", e.TransferRate}
                });

            LogEvents();
        }

        private static void FileManagerDownloaded(object sender, FileManagerDownloadedEventArgs e)
        {
            SaveEventInfo(new Dictionary<string, object>
                {
                    {"Event Type", "Downloaded"},
                    {"Path", e.Folder.FullPath},
                    {"Downloaded Items", e.ItemNames},
                    {"Downloaded File Name", e.DownloadFileName},
                    {"Opened in browser", e.OpenInBrowser},
                    {"Total Size", new ByteSizeValue(e.TotalSize).ToFileSize()},
                    {"Elapsed Time", e.ElapsedTime},
                    {"Transfer Rate", e.TransferRate}
                });

            LogEvents();
        }

        private static void FileManagerPreviewed(object sender, FileManagerPreviewedEventArgs e)
        {
            SaveEventInfo(new Dictionary<string, object>
                {
                    {"Event Type", "Previewed"},
                    {"Path", e.Folder.FullPath},
                    {"Previewed File", e.ItemName},
                    {"Previewer", e.PreviewerType}
                });

            LogEvents();
        }

        private static void FileManagerFailed(object sender, FileManagerFailedEventArgs e)
        {
            SaveEventInfo(new Dictionary<string, object>
                {
                    {"Event Type", "Failed"},
                    {"Failed Action", e.FailedActionInfo.Name},
                    {"Parameters", e.FailedActionInfo.Parameters},
                    {"Error", e.Exception.ToString().Replace("\r\n", "\r\n\t")},
                });

            LogEvents();
        }

        #endregion Event handlers for after events

        private static void SaveEventInfo(Dictionary<string, object> eventInfo)
        {
            var resultText = new StringBuilder();
            foreach (var kvp in eventInfo)
            {
                resultText.Append(kvp.Key);
                resultText.Append(": \r\n");

                var enumerable = kvp.Value as IEnumerable;
                if (enumerable is string)
                    enumerable = null;
                if (enumerable != null)
                    foreach (var item in enumerable)
                    {
                        var subDictionary = item as Dictionary<string, object>;
                        if (subDictionary != null)
                            foreach (var subKvp in subDictionary)
                            {
                                resultText.Append("\t");
                                resultText.Append(subKvp.Key);
                                resultText.Append(": ");
                                resultText.Append(subKvp.Value);
                                resultText.Append("\r\n");
                            }
                        else
                        {
                            resultText.Append("\t");
                            resultText.Append(item);
                            resultText.Append("\r\n");
                        }
                        resultText.Append("\r\n");
                    }
                else
                {
                    resultText.Append("\t");
                    resultText.Append(kvp.Value);
                    resultText.Append("\r\n");
                }
            }

            var context = System.Web.HttpContext.Current;
            var eventLog = context.Session[EventLogSessionKey] as Stack<string>;
            if (eventLog == null)
            {
                eventLog = new Stack<string>();
                context.Session[EventLogSessionKey] = eventLog;
            }
            if (eventLog.Count > 500000)
                eventLog.Clear();

            eventLog.Push(resultText.ToString());
        }
    }
}