/// <summary>
/// ファイルのダウンロードを管理する
/// 
/// 2015/03/06
/// </summary>
using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Collections.Generic;

public class DownloadSet
{
	const int DefaultConnectionLimit = 5;

	LinkedList<IDownloader> downloadingList = new LinkedList<IDownloader>();
	Queue<IDownloader> waitDownloadList = new Queue<IDownloader>();

	public int Count { get { return downloadingList.Count + waitDownloadList.Count; } }
	public int TotalProgressPercentage { get; private set; }

	public int dCount { get { return downloadingList.Count; } }
	public int wCount { get { return waitDownloadList.Count; } }

	public DownloadSet()
	{
		ServicePointManager.DefaultConnectionLimit = DefaultConnectionLimit;
	}

	public void Update()
	{
		{
			TotalProgressPercentage = 0;
			var downloader = downloadingList.First;
			while(downloader != null)
			{
				var next = downloader.Next;
				if(downloader.Value.Completed)
				{
					downloadingList.Remove(downloader);
				}
				else
				{
					TotalProgressPercentage += downloader.Value.ProgressPercentage;
				}
				downloader = next;
			}
		}

		if(downloadingList.Count < DefaultConnectionLimit &&
		   0 < waitDownloadList.Count)
		{
			var downloader = waitDownloadList.Dequeue();
			if(downloader != null)
			{
				downloader.RunAsync();
				downloadingList.AddLast(downloader);
			}
		}
	}

	public DownloadFile AddDownload_File(Uri url, string filePath)
	{
		var downloadFile = new DownloadFile(url, filePath);
		this.waitDownloadList.Enqueue(downloadFile);
		return downloadFile;
	}

	public LinkedList<IDownloader> GetDownloadingList()
	{
		return downloadingList;
	}
	
	public interface IDownloader
	{
		Uri Uri { get; }
		bool IsBusy { get; }
		int ProgressPercentage { get; }
		bool Completed { get; }
		Exception Error { get; }
		bool Cancelled { get; }
		bool IsSuccess { get; }

		void RunAsync();
	}

	public class DownloadFile : IDownloader
	{
		public Uri Uri { get; private set; }
		public bool IsBusy { get { return webClient != null ? webClient.IsBusy : false; } }
		public int ProgressPercentage { get; private set; }
		public bool Completed { get; private set; }
		public bool Cancelled { get; private set; }
		public bool IsSuccess { get { return Completed && !Cancelled && this.Error == null; } }
		public Exception Error { get; private set; }

		private WebClient webClient;
		public string FilePath{ get; private set; }

		public DownloadFile(Uri url, string filePath)
		{
			// コールバックを設定して読み込み開始.
			this.Uri = url;
			this.FilePath = filePath;
		}
		
		public void RunAsync()
		{
			webClient = new System.Net.WebClient();
			webClient.DownloadProgressChanged += this.Event_ProgressChanged;
			webClient.DownloadFileCompleted += Event_Completed;
			webClient.DownloadFileAsync(Uri, FilePath);
		}

		private void Event_ProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
		{
			this.ProgressPercentage = e.ProgressPercentage;
			//e.UserState;
			//e.BytesReceived;
			//e.TotalBytesToReceive;
		}

		private void Event_Completed(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
		{
			this.Completed = true;
			this.Error = e.Error;
			this.Cancelled = e.Cancelled;
		}
	}
}
