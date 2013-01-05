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
    /// <summary>
    ///	Represents two related values
    /// </summary>
    public struct Tuple<A, B> : IEquatable<Tuple<A, B>> {
        #region Fields and Properties

        /// <summary></summary>
        public readonly A First;
        /// <summary></summary>
        public readonly B Second;

        #endregion Fields and Properties

        #region Construction and Destruction
        public Tuple(A first, B second) {
            this.First = first;
            this.Second = second;
        }
        #endregion Construction and Destruction

        #region IEquatable<Tuple<A,B>> Implementation

        public bool Equals(Tuple<A, B> other) {
            return First.Equals(other.First) &&
                   Second.Equals(other.Second);

        }

        public override bool Equals(object other) {
            if (other is Tuple<A, B>) {
                return this.Equals((Tuple<A, B>)other);
            }
            return false;
        }

        #endregion IEquatable<Tuple<A,B>> Implementation
    }

    /// <summary>
    /// Represents three related values
    /// </summary>
    /// <typeparam name="A"></typeparam>
    /// <typeparam name="B"></typeparam>
    /// <typeparam name="C"></typeparam>
    public struct Tuple<A, B, C> : IEquatable<Tuple<A, B, C>> {
        #region Fields and Properties

        /// <summary></summary>
        public readonly A First;
        /// <summary></summary>
        public readonly B Second;
        /// <summary></summary>
        public readonly C Third;

        #endregion Fields and Properties

        #region Construction and Destruction
        public Tuple(A first, B second, C Third) {
            this.First = first;
            this.Second = second;
            this.Third = Third;
        }
        #endregion Construction and Destruction

        #region IEquatable<Tuple<A,B,C>> Implementation

        public bool Equals(Tuple<A, B, C> other) {
            return First.Equals(other.First)
                    && Second.Equals(other.Second)
                    && Third.Equals(other.Third);

        }

        public override bool Equals(object other) {
            if (other is Tuple<A, B, C>) {
                return this.Equals((Tuple<A, B, C>)other);
            }
            return false;
        }

        #endregion IEquatable<Tuple<A,B,C>> Implementation
    }

    /// <summary>
    /// Represents four related values
    /// </summary>
    /// <typeparam name="A"></typeparam>
    /// <typeparam name="B"></typeparam>
    /// <typeparam name="C"></typeparam>
    /// <typeparam name="D"></typeparam>
    public struct Tuple<A, B, C, D> : IEquatable<Tuple<A, B, C, D>> {
        #region Fields and Properties

        /// <summary></summary>
        public readonly A First;
        /// <summary></summary>
        public readonly B Second;
        /// <summary></summary>
        public readonly C Third;
        /// <summary></summary>
        public readonly D Fourth;

        #endregion Fields and Properties

        #region Construction and Destruction
        public Tuple(A first, B second, C third, D fourth) {
            this.First = first;
            this.Second = second;
            this.Third = third;
            this.Fourth = fourth;
        }
        #endregion Construction and Destruction

        #region IEquatable<Tuple<A,B,C,D>> Implementation

        public bool Equals(Tuple<A, B, C, D> other) {
            return First.Equals(other.First)
                    && Second.Equals(other.Second)
                    && Third.Equals(other.Third);

        }

        public bool Equals(object other) {
            if (other is Tuple<A, B, C, D>) {
                return this.Equals((Tuple<A, B, C, D>)other);
            }
            return false;
        }

        #endregion IEquatable<Tuple<A,B,C,D>> Implementation
    }
}
