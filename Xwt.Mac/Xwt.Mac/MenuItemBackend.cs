// 
// MenuItemBackend.cs
//  
// Author:
//       Lluis Sanchez <lluis@xamarin.com>
// 
// Copyright (c) 2011 Xamarin Inc
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using Xwt.Backends;
using Xwt.Engine;
using MonoMac.AppKit;
using System.Collections.Generic;

namespace Xwt.Mac
{
	public class MenuItemBackend: IMenuItemBackend
	{
		NSMenuItem item;
		IMenuItemEventSink eventSink;
		List<MenuItemEvent> enabledEvents;
		
		public MenuItemBackend (): this (new NSMenuItem ())
		{
		}
		
		public MenuItemBackend (NSMenuItem item)
		{
			this.item = item;
		}
		
		public NSMenuItem Item {
			get { return item; }
		}
		
		public void Initialize (IMenuItemEventSink eventSink)
		{
			this.eventSink = eventSink;
		}

		public void SetSubmenu (IMenuBackend menu)
		{
			if (menu == null)
				item.Submenu = null;
			else
				item.Submenu = ((MenuBackend)menu);
		}

		public string Label {
			get {
				return item.Title;
			}
			set {
				item.Title = value;
			}
		}
		
		public void SetImage (object imageBackend)
		{
			var img = (NSImage) imageBackend;
			item.Image = img;
		}
		
		public bool Visible {
			get {
				return !item.Hidden;
			}
			set {
				item.Hidden = !value;
			}
		}
		
		public bool Sensitive {
			get {
				return item.Enabled;
			}
			set {
				item.Enabled = value;
			}
		}
		
		public bool Checked {
			get {
				return item.State == NSCellStateValue.On;
			}
			set {
				if (value)
					item.State = NSCellStateValue.On;
				else
					item.State = NSCellStateValue.Off;
			}
		}
		
		#region IBackend implementation
		public void InitializeBackend (object frontend)
		{
		}

		public void EnableEvent (object eventId)
		{
			if (eventId is MenuItemEvent) {
				if (enabledEvents == null)
					enabledEvents = new List<MenuItemEvent> ();
				enabledEvents.Add ((MenuItemEvent)eventId);
				if ((MenuItemEvent)eventId == MenuItemEvent.Clicked)
					item.Activated += HandleItemActivated;
			}
		}

		public void DisableEvent (object eventId)
		{
			if (eventId is MenuItemEvent) {
				enabledEvents.Remove ((MenuItemEvent)eventId);
				if ((MenuItemEvent)eventId == MenuItemEvent.Clicked)
					item.Activated -= HandleItemActivated;
			}
		}
		#endregion
		
		void HandleItemActivated (object sender, EventArgs e)
		{
			Toolkit.Invoke (delegate {
				eventSink.OnClicked ();
			});
		}
	}
}

