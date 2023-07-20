using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Filament.Content;

namespace Wrapper
{
	[ContentCollection]
	public class Credits : ContentAsset
	{
		[SerializeField, ContentValue("entries")]
		List<Entry> entries = new List<Entry>();

		public List<Entry> Entries { get => entries; }

		[Serializable]
		public class Entry
		{
			[SerializeField, ContentValue("Name")]
			string name = string.Empty;
			[SerializeField, ContentValue("Title")]
			string title = string.Empty;

			public string Name { get { return name; } }
			public string Title { get { return title; } }

			public Entry(string name, string title)
			{
				this.name = name;
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
				entries.Add(new Entry(util.AsString("Name"), util.AsString("Title")));
			}
		}
#endif
	}
}
