#region MIT/X11 License
// This file is part of the MogreLib.PagedGeometry project
//
// Copyright (c) 2009 Michael Cummings <cummings.michael@gmail.com>
// Copyright (c) 2009 Bostich <bostich83@googlemail.com>

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
//
// MogreLib.PagedGeometry is a reimplementation of the PagedGeometry project for .Net/Mono
// PagedGeometry is Copyright (C) 2009 Xavier Verguín González <xavierverguin@hotmail.com> <xavyiy@gmail.com>

// MogreLib.PagedGeometry is Copyright (C) 2013 rains <andyhebear@hotmail.com> <andyhebear@gmail.com>
// MogreLib.PagedGeometry svn: http://mogrelibrarys.googlecode.com/svn/trunk/  Author Blog http://hi.baidu.com/andyhebear/    http://hi.baidu.com/new/rainssoft/

#endregion MIT/X11 License

#region Namespace Declarations

using System;
using System.Runtime.InteropServices;

using System.Diagnostics;

#endregion Namespace Declarations

namespace MogreLib.Core
{
	/// <summary>
	///		Encapsulates the functionality of the platform's highest resolution timer available.
	/// </summary>
	/// <remarks>
	/// based on an vb.net implementation by createdbyx as posted in SourceForge Tracker #: [1612705]
	/// </remarks>
	public class Timer : ITimer
	{
		#region Private Fields

		private Stopwatch _timer = new Stopwatch();

		#endregion Private Fields

		#region Methods

		/// <summary>
		/// Start this instance's timer.
		/// </summary>
		public void Start()
		{
			_timer.Start();
		}

		#endregion Methods

		#region Public Properties
		/// <summary>
		/// Gets a <see cref="System.Int64" /> representing the 
		/// current tick count of the timer.
		/// </summary>
		public long Count
		{
			get
			{
				return _timer.ElapsedTicks;
			}
		}

		/// <summary>
		/// Gets a <see cref="System.Int64" /> representing the 
		/// frequency of the counter in ticks-per-second.
		/// </summary>
		public long Frequency
		{
			get
			{
				return Stopwatch.Frequency;
			}
		}

		/// <summary>
		/// Gets a <see cref="System.Boolean" /> representing whether the 
		/// timer has been started and is currently running.
		/// </summary>
		public bool IsRunning
		{
			get
			{
				return _timer.IsRunning;
			}
		}

		/// <summary>
		/// Gets a <see cref="System.Double" /> representing the 
		/// resolution of the timer in seconds.
		/// </summary>
		public float Resolution
		{
			get
			{
				return ( (float)1.0 / (float)Frequency );
			}
		}

		/// <summary>
		/// Gets a <see cref="System.Int64" /> representing the 
		/// tick count at the start of the timer's run.
		/// </summary>
		public long StartCount
		{
			get
			{
				return 0;
			}
		}

		#endregion Public Properties

		#region ITimer Members

		/// <summary>
		///		Reset this instance's timer.
		/// </summary>
		public void Reset()
		{
			// reset by restarting the timer
			_timer.Reset();
			_timer.Start();
		}

		public long Microseconds
		{
			get
			{
				return _timer.ElapsedMilliseconds / 10;
			}
		}

		public long Milliseconds
		{
			get
			{
				return _timer.ElapsedMilliseconds;
			}
		}

		#endregion
	}
}
