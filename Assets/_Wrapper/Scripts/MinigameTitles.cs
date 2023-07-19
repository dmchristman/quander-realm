using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Filament.Content;

namespace Wrapper
{
	[ContentCollection]
	public class MinigameTitles : ContentAsset
	{
		[SerializeField, ContentValue("entries")]
		List<string> entries = new List<string>();

		public List<string> Entries { get => entries; }

		[Serializable]
		public class Entry
		{
			[SerializeField, ContentValue("Game Name")]
			string title = string.Empty;

			public string Title { get { return title; } }

			public Entry(string title)
			{
				this.title = title;
			}
		}

#if UNITY_EDITOR
		public override void Clear()
		{
			entries.Clear();
		}

		public override void OnImportCollectionEntry(IImportUtil util)
		{
			if (util.Group == name)
			{
				entries.Add(util.AsString("Game Name"));
			}
		}
#endif
	}
}
