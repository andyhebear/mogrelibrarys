#region MIT/X11 License
// This file is part of the MogreLib.SkyX project
//
// Copyright (c) 2009 Michael Cummings <cummings.michael@gmail.com>
// Copyright (c) 2009 Bostich <bostich@googlemail.com>

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
// MogreLib.SkyX is a reimplementation of the SkyX project for .Net/Mono
// SkyX is Copyright (C) 2009 Xavier Verguín González <xavierverguin@hotmail.com> <xavyiy@gmail.com>

// MogreLib.SkyX is Copyright (C) 2012 rains <andyhebear@hotmail.com> <andyhebear@gmail.com>
// MogreLib.SkyX svn: http://mogrelibrarys.googlecode.com/svn/trunk/  Author Blog http://hi.baidu.com/andyhebear/    http://hi.baidu.com/rainssoft/

#endregion MIT/X11 License
namespace MogreLib.SkyX
{
    /// <summary>
    /// Lighting mode enumeration
    /// </summary>
    /// <remarks>
    /// SkyX is designed for true HDR rendering, but there're a lot of applications
    /// that doesn't use HDR rendering, due to this a little exponential tone-mapping 
    /// algoritm is applied to SkyX materials if LightingMode.Ldr is selected. <see cref="AtmosphereManager.Options.Exposure"/>
    /// Select LightingMode.Hdr if your app is designed for true HDR rendering.
    /// </remarks>
    public enum LightingMode
    {
        /// Low dynamic range
        Ldr = 0,
        /// High dynamic range
        Hdr = 1
    }
}