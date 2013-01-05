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
// PagedGeometry is Copyright (C) 2009 Xavier Vergu韓 Gonz醠ez <xavierverguin@hotmail.com> <xavyiy@gmail.com>

// MogreLib.PagedGeometry is Copyright (C) 2013 rains <andyhebear@hotmail.com> <andyhebear@gmail.com>
// MogreLib.PagedGeometry svn: http://mogrelibrarys.googlecode.com/svn/trunk/  Author Blog http://hi.baidu.com/andyhebear/    http://hi.baidu.com/new/rainssoft/

#endregion MIT/X11 License

using System;
using System.Collections.Generic;
using System.Text;

namespace MogreLib.Math {
    ///// <summary>
    /////    Type of intersection detected between 2 object.
    ///// </summary>
    //public enum Intersection {
    //    /// <summary>
    //    ///    The objects are not intersecting.
    //    /// </summary>
    //    None,
    //    /// <summary>
    //    ///    An object is fully contained within another object.
    //    /// </summary>
    //    Contained,
    //    /// <summary>
    //    ///    An object fully contains another object.
    //    /// </summary>
    //    Contains,
    //    /// <summary>
    //    ///    The objects are partially intersecting each other.
    //    /// </summary>
    //    Partial
    //}

    /// <summary>
    /// The "positive side" of the plane is the half space to which the
    /// plane normal points. The "negative side" is the other half
    /// space. The flag "no side" indicates the plane itself.
    /// </summary>
    public enum PlaneSide {
        None,
        Positive,
        Negative,
        Both
    }
}
