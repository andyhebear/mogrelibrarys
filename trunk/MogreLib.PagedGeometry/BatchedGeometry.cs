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
using System.Diagnostics;
using MogreLib.Core;
using MogreLib.Graphics;
using MogreLib.Math;
using Mogre;
using ColorEx = Mogre.ColourValue;
using System.Reflection;

namespace MogreLib.PagedGeometry {
    interface IMovableObject {
    }
    interface ISimpleRenderable {
    }
    public class BatchedGeometry : IMovableObject {
        class testManualObject : ManualObject {
            public unsafe testManualObject(string name)
                : base(null) {
                using (SceneManagerEnumerator.SceneManagerIterator smi = Root.Singleton.GetSceneManagerIterator()) {
                    smi.MoveNext();
                    ManualObject dummyObject = smi.Current.CreateManualObject(name);
                    typeof(ManualObject).GetField("_native", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(this, (IntPtr)dummyObject.NativePtr);
                }
            }
            void test() { 
                
            }
        }
        class testSimpleRenderable : SimpleRenderable {
            public unsafe testSimpleRenderable(string name) 
                : base(null) {
                using (SceneManagerEnumerator.SceneManagerIterator smi = Root.Singleton.GetSceneManagerIterator()) {
                    smi.MoveNext();
                    MovableObject dummyObject = smi.Current.CreateMovableObject(name, "MovableObject");
                    typeof(MovableObject).GetField("_native", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(this, (IntPtr)dummyObject.NativePtr);
                }
            }
            void test() { 
                
            }
        }
        class testSimpleRenderableFactory : MovableObjectFactory {
            void test() {
                Root.Singleton.AddMovableObjectFactory(new MovableObjectFactory(null), true);
            }
        }
        protected MovableObject mMovableObj;
        protected string mMovableObjName;
        static int name = 0;
        #region - fields -
        private Vector3 mCenter;
        private AxisAlignedBox mBounds;
        private bool mBoundsUndefinded;
        private float mRadius;
        private SceneManager mSceneMgr;
        private SceneNode mSceneNode;
        private SceneNode mParentSceneNode;
        private float mMinDistanceSquared;
        private bool mWithinFarDistance;
        private bool mBuild;
        private Dictionary<string, SubBatch> mSubBatches = new Dictionary<string, SubBatch>();
        #endregion

        #region - properties -
        /// <summary>
        /// 
        /// </summary>
        public float MinDistanceSquared {
            get { return mMinDistanceSquared; }
        }
        /// <summary>
        /// 
        /// </summary>
        public Vector3 Center {
            get { return mCenter; }
        }
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, SubBatch> SubBatches {
            get { return mSubBatches; }
        }
        /// <summary>
        /// 
        /// </summary>
        public SceneNode SceneNode {
            get { return mSceneNode; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsVisible {
            get {
                return this.mMovableObj.Visible && mWithinFarDistance;
                //return this.isVisible && mWithinFarDistance;
            }
            set {
                //base.IsVisible = value;
                this.mMovableObj.Visible = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public AxisAlignedBox BoundingBox {
            get { return mBounds; }
        }
        /// <summary>
        /// 
        /// </summary>
        public float BoundingRadius {
            get { return mRadius; }
        }
        ///// <summary>
        ///// 
        ///// </summary>
        //public override string MovableType
        //{
        //    get
        //    {
        //        return "BatchedGeometry";
        //    }
        //    //set
        //    //{
        //    //    base.MovableType = value;
        //    //}
        //}
        #endregion
        private BatchedGeometry() {

            this.isVisible = true;
            // set default RenderQueueGroupID for this movable object
            this.renderQueueID = RenderQueueGroupID.Main;
            this.queryFlags = DefaultQueryFlags;
            this.visibilityFlags = DefaultVisibilityFlags;
            this.worldAABB = AxisAlignedBox.Null;
            this.castShadows = true;
        }
        public BatchedGeometry(string name)
            : this() {
            this.mMovableObjName = name;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mgr"></param>
        /// <param name="rootNode"></param>
        public BatchedGeometry(SceneManager mgr, SceneNode rootNode)
            : this("BatchedGeom" + name++.ToString()) {

            mWithinFarDistance = false;
            mMinDistanceSquared = 0;
            mSceneNode = null;
            mSceneMgr = mgr;
            mBuild = false;
            mBounds = new AxisAlignedBox(new Vector3(0, 0, 0), new Vector3(0, 0, 0));
            mBoundsUndefinded = true;
            mParentSceneNode = rootNode;

            Clear();

        }

        #region - AddEntity Overloads -
        public void AddEntity(Entity ent, Vector3 position) {
            AddEntity(ent, position, Quaternion.Identity, Vector3.UnitScale, ColorEx.White);
        }
        public void AddEntity(Entity ent, Vector3 position, Quaternion orientation) {
            AddEntity(ent, position, orientation, Vector3.UnitScale, ColorEx.White);
        }
        public void AddEntity(Entity ent, Vector3 position, Quaternion orientation, Vector3 scale) {
            AddEntity(ent, position, orientation, scale, ColorEx.White);
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ent"></param>
        /// <param name="position"></param>
        /// <param name="orientation"></param>
        /// <param name="scale"></param>
        /// <param name="color"></param>
        public void AddEntity(Entity ent, Vector3 position, Quaternion orientation, Vector3 scale, ColorEx color) {
            Mesh mesh = ent.Mesh;
            if (mesh.SharedVertexData != null)
                throw new Exception("Shared vertex data not allowed");

            //For each subentity
            for (int i = 0; i < ent.SubEntityCount; i++) {
                //get the subentity
                SubEntity subEntity = ent.GetSubEntity(i);
                SubMesh subMesh = subEntity.SubMesh;

                //Generate a format string that uniquely identifies this material & vertex/index format
                if (subMesh.vertexData == null)
                    throw new Exception("Submesh vertex data not found!");

                string formatStr = GetFormatString(subEntity);
                //If a batch using an identical format exists...
                SubBatch batch = null;
                if (!mSubBatches.TryGetValue(formatStr, out batch)) {
                    batch = new SubBatch(this, subEntity);
                    mSubBatches.Add(formatStr, batch);
                }
                //Now add the submesh to the compatible batch
                batch.AddSubEntity(subEntity, position, orientation, scale, color);
            }//end for

            //Update bounding box
            Matrix4 mat = Matrix4.FromMatrix3(orientation.ToRotationMatrix());
            mat.Scale = scale;
            AxisAlignedBox entBounds = ent.BoundingBox;
            entBounds.Transform(mat);
            if (mBoundsUndefinded) {
                mBounds.Minimum = entBounds.Minimum + position;
                mBounds.Maximum = entBounds.Maximum + position;
                mBoundsUndefinded = false;
            }
            else {
                Vector3 min = mBounds.Minimum;
                Vector3 max = mBounds.Maximum;

                min.Floor(entBounds.Minimum + position);
                max.Ceil(entBounds.Maximum + position);
                mBounds.Minimum = min;
                mBounds.Maximum = max;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void Build() {
            ///Make sure the batch hasn't already been build
            if (mBuild)
                throw new Exception("Invalid call to build() - geometry is allready batched (call clear() first)");

            if (mSubBatches.Count != 0) {
                //Finish bounds information
                mCenter = mBounds.Center;
                mBounds.Minimum = mBounds.Minimum - mCenter;
                mBounds.Maximum = mBounds.Maximum - mCenter;

                mRadius = mBounds.Maximum.Length;

                //create scene node
                mSceneNode = mParentSceneNode.CreateChildSceneNode(mCenter);

                //build each batch
                foreach (SubBatch batch in mSubBatches.Values)
                    batch.Build();

                //Attach the batch to the scene node
                mSceneNode.AttachObject(this);

                //Debug
                //mSceneNode.ShowBoundingBox(true);

                mBuild = true;

            }

        }
        /// <summary>
        /// 
        /// </summary>
        public void Clear() {
            if (mSceneNode != null) {
                mSceneNode.RemoveAllChildren();
                mSceneMgr.DestroySceneNode(mSceneNode);
                mSceneNode = null;
            }

            mBoundsUndefinded = true;
            mCenter = Vector3.Zero;
            mRadius = 0;

            mSubBatches.Clear();
            mBuild = false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="globalVec"></param>
        /// <returns></returns>
        public Vector3 ConvertToLocal(Vector3 globalVec) {
            Debug.Assert(mParentSceneNode != null, "Parent scenenode can not be null!");

            //Convert from the given global position to the local coordinate system of the parent scene node.
            return (mParentSceneNode.Orientation.Inverse() * globalVec);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="camera"></param>
        public override void NotifyCurrentCamera(Camera camera) {
#warning add MovableObject.RenderingDistance
            if (false) {
                mWithinFarDistance = true;
            }
            else {
                //Calculate camera distance
                Vector3 camVec = ConvertToLocal(camera.DerivedPosition) - mCenter;
                float centerDistanceSquared = camVec.LengthSquared;
                mMinDistanceSquared = System.Math.Max(0.0f, centerDistanceSquared - (mRadius * mRadius));
                //Note: centerDistanceSquared measures the distance between the camera and the center of the GeomBatch,
                //while minDistanceSquared measures the closest distance between the camera and the closest edge of the
                //geometry's bounding sphere.

                //Determine whether the BatchedGeometry is within the far rendering distance
#warning add MovableObject.RenderingDistance
                mWithinFarDistance = mMinDistanceSquared <= MogreLibMath.Utility.Sqr(0);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="queue"></param>
        public override void UpdateRenderQueue(RenderQueue queue) {
            //If visible...
            if (IsVisible) {
                //Ask each batch to add itself to the render queue if appropriate
                foreach (SubBatch batch in mSubBatches.Values)
                    batch.AddSelfToRenderQueue(queue, base.renderQueueID);//base.RenderQueueGroup
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ent"></param>
        /// <returns></returns>
        private string GetFormatString(SubEntity ent) {
            string str = string.Empty;

            str += ent.MaterialName + "|";
            str += ent.SubMesh.indexData.indexBuffer.Type + "|";

            List<VertexElement> elemList = ent.SubMesh.vertexData.vertexDeclaration.Elements;
            foreach (VertexElement element in elemList) {
                str += element.Source + "|";
                str += element.Semantic + "|";
                str += element.Type + "|";
            }

            return str;
        }
        #region - subBatch -
        /// <summary>
        /// 
        /// </summary>
        public class SubBatch : ISimpleRenderable {
            protected SimpleRenderable mSimpleRenderable;
            //protected string mSimpleRenderableName;
            /// <summary>
            /// A structure defining the desired position/orientation/scale of a batched mesh.
            /// The SubMesh is not specified since that can be determined by which MeshQueue this belongs to.
            /// </summary>
            public struct QueuedMesh {
                public SubMesh Mesh;
                public Vector3 Postion;
                public Quaternion Orientation;
                public Vector3 Scale;
                public ColorEx Color;
            }

            #region - fields -
            private bool mBuild;
            private bool mRequireVertexColors;
            private SubMesh mMeshType;
            private BatchedGeometry mParent;
            private Material mMaterial;
            private Technique mBestTechnique;
            private List<QueuedMesh> mMeshQueue = new List<QueuedMesh>();
            #endregion
            /// <summary>
            /// 
            /// </summary>
            public override bool CastShadows {
                get {
                    return mParent.CastShadows;
                }
                set {
                    base.CastShadows = value;
                }
            }
            /// <summary>
            /// 
            /// </summary>
            public override float BoundingRadius {
                get { return mParent.BoundingRadius; }
            }
            /// <summary>
            /// 
            /// </summary>
            public override Material Material {
                get {
                    return base.Material;
                }
                set {
                    base.Material = value;
                }
            }
            /// <summary>
            /// 
            /// </summary>
            public override Quaternion WorldOrientation {
                get {
                    return mParent.SceneNode.DerivedOrientation;
                }
            }
            /// <summary>
            /// 
            /// </summary>
            public override Vector3 WorldPosition {
                get {
                    return mParent.SceneNode.DerivedPosition;
                }
            }
            /// <summary>
            /// 
            /// </summary>
            public string MaterialName {
                get { return Material.Name; }
            }
            public override void GetWorldTransforms(Matrix4[] matrices) {
                matrices[0] = mParent.ParentNodeFullTransform;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="parent"></param>
            /// <param name="ent"></param>
            public SubBatch(BatchedGeometry parent, SubEntity ent) {
                mMeshType = ent.SubMesh;
                mParent = parent;
                mBuild = false;
                mRequireVertexColors = false;
                // Material must always exist
                Material origMat = (Material)MaterialManager.Instance.GetByName(ent.MaterialName);
                if (origMat != null) {
                    material = (Material)MaterialManager.Instance.GetByName(GetMaterialClone(origMat).Name);
                }
                else {
                    Tuple<Resource, bool> result = MaterialManager.Instance.CreateOrRetrieve("PagedGeometry_Batched_Material", "General");
                    if (result.First == null)
                        throw new Exception("BatchedGeometry failed to create a material for entity with invalid material.");

                    material = (Material)result.First;
                }

                //Setup vertex/index data structure
                vertexData = mMeshType.vertexData.Clone(false);
                indexData = mMeshType.indexData.Clone(false);

                //Remove blend weights from vertex format
                VertexElement blendIndices = vertexData.vertexDeclaration.FindElementBySemantic(VertexElementSemantic.BlendIndices);
                VertexElement blendWeights = vertexData.vertexDeclaration.FindElementBySemantic(VertexElementSemantic.BlendWeights);

                if (blendIndices != null && blendWeights != null) {
                    Debug.Assert(blendIndices.Source == blendWeights.Source, "Blend indices and weights should be in the same buffer");
                    Debug.Assert(blendIndices.Size + blendWeights.Size == vertexData.vertexBufferBinding.GetBuffer(blendIndices.Source).VertexSize,
                        "Blend indices and blend buffers should have buffer to themselves!");

                    //Remove the blend weights
                    vertexData.vertexBufferBinding.UnsetBinding(blendIndices.Source);
                    vertexData.vertexDeclaration.RemoveElement(VertexElementSemantic.BlendIndices);
                    vertexData.vertexDeclaration.RemoveElement(VertexElementSemantic.BlendWeights);
                }

                //Reset vertex/index count
                vertexData.vertexStart = 0;
                vertexData.vertexCount = 0;
                indexData.indexStart = 0;
                indexData.indexCount = 0;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="queue"></param>
            /// <param name="group"></param>
            public void AddSelfToRenderQueue(RenderQueue queue, RenderQueueGroupID group) {
                if (mBuild) {
                    //Update material technique based on camera distance
                    Debug.Assert(material != null);
#warning missing function getLodIndexSquaredDepth
                    mBestTechnique = material.GetBestTechnique(material.GetLodIndex(mParent.MinDistanceSquared));

                    //Add to render queue
                    queue.AddRenderable(this, group);
                }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="ent"></param>
            /// <param name="position"></param>
            /// <param name="orientation"></param>
            /// <param name="scale"></param>
            public void AddSubEntity(SubEntity ent, Vector3 position, Quaternion orientation, Vector3 scale) {
                AddSubEntity(ent, position, orientation, scale, ColorEx.White);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="ent"></param>
            /// <param name="position"></param>
            /// <param name="orientation"></param>
            /// <param name="scale"></param>
            /// <param name="color"></param>
            public void AddSubEntity(SubEntity ent, Vector3 position, Quaternion orientation, Vector3 scale, ColorEx color) {
                Debug.Assert(!mBuild);
                //Add this submesh to the queue
                QueuedMesh newMesh = new QueuedMesh();
                newMesh.Mesh = ent.SubMesh;
                newMesh.Postion = position;
                newMesh.Orientation = orientation;
                newMesh.Scale = scale;
                newMesh.Color = color;

                if (newMesh.Color != ColorEx.White) {
                    mRequireVertexColors = true;
                    //VertexElementType format = Root.Singleton.RenderSystem.Colo
                    throw new NotSupportedException("Please use ColorEx.White for now!");
                }

                mMeshQueue.Add(newMesh);
                //Increment the vertex/index count so the buffers will have room for this mesh
                vertexData.vertexCount += ent.SubMesh.vertexData.vertexCount;
                indexData.indexCount += ent.SubMesh.indexData.indexCount;
            }
            /// <summary>
            /// 
            /// </summary>
            public void Build() {
                Debug.Assert(!mBuild);

                //Misc. setup
                Vector3 batchCenter = mParent.Center;

                IndexType srcIndexType = mMeshType.indexData.indexBuffer.Type;
                IndexType destIndexType;

                if (vertexData.vertexCount > 0xFFFF || srcIndexType == IndexType.Size32)
                    destIndexType = IndexType.Size32;
                else
                    destIndexType = IndexType.Size16;

                //Allocate the index buffer
                indexData.indexBuffer = HardwareBufferManager.Instance.CreateIndexBuffer(
                    destIndexType, indexData.indexCount, BufferUsage.StaticWriteOnly);

                unsafe {
                    uint* indexBuffer32 = (uint*)IntPtr.Zero;
                    ushort* indexBuffer16 = (ushort*)IntPtr.Zero;
                    if (destIndexType == IndexType.Size32)
                        indexBuffer32 = (uint*)indexData.indexBuffer.Lock(BufferLocking.Discard);
                    else
                        indexBuffer16 = (ushort*)indexData.indexBuffer.Lock(BufferLocking.Discard);

                    //Allocate & lock the vertex buffers
                    List<IntPtr> vertexBuffers = new List<IntPtr>();
                    List<List<VertexElement>> vertexBufferElements = new List<List<VertexElement>>();

                    VertexBufferBinding vertBinding = vertexData.vertexBufferBinding;
                    VertexDeclaration vertDecl = vertexData.vertexDeclaration;

                    for (short i = 0; i < vertBinding.BindingCount; i++) {
                        HardwareVertexBuffer buffer = HardwareBufferManager.Instance.CreateVertexBuffer(
                            vertDecl/*.GetVertexSize(i)*/, vertexData.vertexCount, BufferUsage.StaticWriteOnly);
                        vertBinding.SetBinding(i, buffer);

                        vertexBuffers.Add(buffer.Lock(BufferLocking.Discard));
                        vertexBufferElements.Add(vertDecl.FindElementBySource(i));
                    }

                    //If no vertex colors are used, make sure the final batch includes them (so the shade values work)
                    if (mRequireVertexColors) {
                        if (vertexData.vertexDeclaration.FindElementBySemantic(VertexElementSemantic.Diffuse) == null) {
                            short i = (short)vertBinding.BindingCount;
                            vertDecl.AddElement(i, 0, VertexElementType.Color, VertexElementSemantic.Diffuse);

                            HardwareVertexBuffer buffer = HardwareBufferManager.Instance.CreateVertexBuffer(
                                vertDecl/*.GetVertexSize(i)*/, vertexData.vertexCount, BufferUsage.StaticWriteOnly);

                            vertexBuffers.Add(buffer.Lock(BufferLocking.Discard));
                            vertexBufferElements.Add(vertDecl.FindElementBySource(i));
                        }
                        Pass p = material.GetTechnique(0).GetPass(0);
                        p.VertexColorTracking = TrackVertexColor.Ambient;
#warning missing function p->setVertexColourTracking(TVC_AMBIENT);
                    }
                    //For each queued mesh...
                    int indexOffset = 0;
                    foreach (QueuedMesh it in mMeshQueue) {
                        QueuedMesh queuedMesh = it;
                        IndexData sourceIndexData = queuedMesh.Mesh.indexData;
                        VertexData sourceVerterxData = queuedMesh.Mesh.vertexData;

                        //Copy mesh vertex data into the vertex buffer
                        VertexBufferBinding sourceBinds = sourceVerterxData.vertexBufferBinding;
                        VertexBufferBinding destBinds = vertexData.vertexBufferBinding;
                        for (short i = 0; i < destBinds.BindingCount; i++) {
                            if (i < sourceBinds.BindingCount) {
                                //Lock the input buffer
                                HardwareVertexBuffer sourceBuffer = sourceBinds.GetBuffer(i);
                                byte* sourceBase = (byte*)(sourceBuffer.Lock(BufferLocking.ReadOnly));

                                //Get the locked output buffer
                                byte* destBase = (byte*)vertexBuffers[i];
                                /* pReal = (float*)(vertex + posElem.Offset);
                        Vector3 pt = new Vector3(pReal[0], pReal[1], pReal[2]);
                        vertices[current_offset + j] = (orientation * (pt * scale)) + position;
                                 */
                                //Copy vertices
                                float* sourcePtr;
                                float* destPtr;
                                for (int v = 0; v < sourceVerterxData.vertexCount; v++) {
                                    // Iterate over vertex elements
                                    List<VertexElement> elems = vertexBufferElements[i];
                                    foreach (VertexElement ei in elems) {
                                        VertexElement elem = ei;
                                        IntPtr st = IntPtr.Zero;
                                        IntPtr dt = IntPtr.Zero;
                                        BaseVertexToPointerElement((IntPtr)sourceBase, out st, elem.Offset);
                                        BaseVertexToPointerElement((IntPtr)destBase, out dt, elem.Offset);
                                        sourcePtr = (float*)st;
                                        destPtr = (float*)dt;
                                        Vector3 tmp = Vector3.Zero;
                                        uint tmpColor = 0;
                                        byte tmpR = 0, tmpG = 0, tmpB = 0, tmpA = 0;
                                        switch (elem.Semantic) {
                                            case VertexElementSemantic.Position:
                                                tmp.x = *sourcePtr++;
                                                tmp.y = *sourcePtr++;
                                                tmp.z = *sourcePtr++;

                                                //transform
                                                tmp = (queuedMesh.Orientation * (tmp * queuedMesh.Scale)) + queuedMesh.Postion;
                                                tmp -= batchCenter; //Adjust for batch center

                                                *destPtr++ = tmp.x;
                                                *destPtr++ = tmp.y;
                                                *destPtr++ = tmp.z;
                                                break;
                                            case VertexElementSemantic.Normal:
                                                tmp.x = *sourcePtr++;
                                                tmp.y = *sourcePtr++;
                                                tmp.z = *sourcePtr++;

                                                //rotate
                                                tmp = queuedMesh.Orientation * tmp;

                                                *destPtr++ = tmp.x;
                                                *destPtr++ = tmp.y;
                                                *destPtr++ = tmp.z;
                                                break;
                                            case VertexElementSemantic.Diffuse:
                                                tmpColor = *((uint*)sourcePtr++);
                                                tmpR = (byte)(((tmpColor) & 0xFF) * queuedMesh.Color.r);
                                                tmpG = (byte)(((tmpColor >> 8) & 0xFF) * queuedMesh.Color.g);
                                                tmpB = (byte)(((tmpColor >> 16) & 0xFF) * queuedMesh.Color.b);
                                                tmpA = (byte)((tmpColor >> 24) & 0xFF);

                                                tmpColor = (uint)(tmpR | (tmpG << 8) | (tmpB << 16) | (tmpA << 24));
                                                *((uint*)destPtr++) = tmpColor;
                                                break;
                                            case VertexElementSemantic.Tangent:
                                            case VertexElementSemantic.Binormal:
                                                tmp.x = *sourcePtr++;
                                                tmp.y = *sourcePtr++;
                                                tmp.z = *sourcePtr++;

                                                //rotate
                                                tmp = queuedMesh.Orientation * tmp;

                                                *destPtr++ = tmp.x;
                                                *destPtr++ = tmp.y;
                                                *destPtr++ = tmp.z;
                                                break;
                                            default:
                                                //raw copy
                                                Memory.Copy((IntPtr)sourcePtr, (IntPtr)destPtr, VertexElement.GetTypeSize(elem.Type));
                                                break;
                                        }
                                    }
                                    // Increment both pointers
                                    destBase += sourceBuffer.VertexSize;
                                    sourceBase += sourceBuffer.VertexSize;
                                }
                                //Unlock the input buffer
                                vertexBuffers[i] = (IntPtr)destBase;
                                sourceBuffer.Unlock();
                            }//end if
                            else {
                                Debug.Assert(mRequireVertexColors);

                                //get the locket outout buffer
                                uint* startPtr = (uint*)vertexBuffers[vertBinding.BindingCount - 1];
                                uint* endPtr = startPtr + sourceVerterxData.vertexCount;

                                //Generate color
                                byte tmpR = (byte)(queuedMesh.Color.r * 255);
                                byte tmpG = (byte)(queuedMesh.Color.g * 255);
                                byte tmpB = (byte)(queuedMesh.Color.b * 255);
                                uint tmpColor = (uint)(tmpR | (tmpG << 8) | (tmpB << 16) | (0xFF << 24));
                                //Copy colors
                                while (startPtr < endPtr) {
                                    *startPtr++ = tmpColor;
                                }

                                int idx = vertBinding.BindingCount - 1;
                                ushort* bf = (ushort*)vertexBuffers[idx];
                                bf += (sizeof(uint) * sourceVerterxData.vertexCount);
                                vertexBuffers[idx] = (IntPtr)bf;
                            }
                        }//end for

                        //Copy mesh index data into the index buffer
                        if (srcIndexType == IndexType.Size32) {
                            //Lock the input buffer
                            uint* source = (uint*)sourceIndexData.indexBuffer.Lock(
                                sourceIndexData.indexStart, sourceIndexData.indexCount, BufferLocking.ReadOnly);

                            uint* sourceEnd = source + sourceIndexData.indexCount;

                            //And copy it to the output buffer
                            while (source != sourceEnd) {
                                *indexBuffer32++ = (uint)(*source++ + indexOffset);
                            }

                            //Unlock the input buffer
                            sourceIndexData.indexBuffer.Unlock();
                            //Increment the index offset
                            indexOffset += sourceVerterxData.vertexCount;
                        }
                        else {
                            if (destIndexType == IndexType.Size32) {
                                //-- Convert 16 bit to 32 bit indices --
                                //Lock the input buffer
                                ushort* source = (ushort*)sourceIndexData.indexBuffer.Lock(
                                    sourceIndexData.indexStart, sourceIndexData.indexCount, BufferLocking.ReadOnly);
                                ushort* sourceEnd = source + sourceIndexData.indexCount;

                                //And copy it to the output buffer
                                while (source != sourceEnd) {
                                    uint indx = *source++;
                                    *indexBuffer32++ = (uint)(indx + indexOffset);
                                }

                                //Unlock the input buffer
                                sourceIndexData.indexBuffer.Unlock();

                                //Increment the index offset
                                indexOffset += sourceVerterxData.vertexCount;
                            }
                            else {
                                //Lock the input buffer
                                ushort* source = (ushort*)sourceIndexData.indexBuffer.Lock(
                                    sourceIndexData.indexStart, sourceIndexData.indexCount, BufferLocking.Normal);
                                ushort* sourceEnd = source + sourceIndexData.indexCount;

                                //And copy it to the output buffer
                                while (source != sourceEnd) {
                                    *indexBuffer16++ = (ushort)(*source++ + indexOffset);
                                }

                                //Unlock the input buffer
                                sourceIndexData.indexBuffer.Unlock();

                                //Increment the index offset
                                indexOffset += sourceVerterxData.vertexCount;

                            }
                        }
                    }
                    //Unlock buffers
                    indexData.indexBuffer.Unlock();
                    for (short i = 0; i < vertBinding.BindingCount; i++)
                        vertBinding.GetBuffer(i).Unlock();

                    //Clear mesh queue
                    mMeshQueue.Clear();
                    mBuild = true;
                }
            }

            private void BaseVertexToPointerElement(IntPtr pBase, out IntPtr pElem, int offset) {
                unsafe {
                    pElem = (IntPtr)(byte*)((byte*)(pBase) + offset);
                }
            }
            /// <summary>
            /// 
            /// </summary>
            public void Clear() {
                //If built, delete the batch
                if (mBuild) {
                    //Delete buffers
                    indexData.indexBuffer = null;
                    vertexData.vertexBufferBinding.UnsetAllBindings();

                    //Reset vertex/index count
                    vertexData.vertexStart = 0;
                    vertexData.vertexCount = 0;
                    indexData.indexStart = 0;
                    indexData.indexCount = 0;
                }

                //Clear mesh queue
                mMeshQueue.Clear();

                mBuild = false;
            }

            public override RenderOperation RenderOperation {
                get {
                    base.renderOperation.operationType = OperationType.TriangleList;
                    base.renderOperation.useIndices = true;
                    base.renderOperation.vertexData = vertexData;
                    base.renderOperation.indexData = indexData;
                    return base.renderOperation;
                }
            }
            //            /// <summary>
            //            /// 
            //            /// </summary>
            //            /// <param name="op"></param>
            //            public override void GetRenderOperation(RenderOperation op)
            //            {
            //                op.operationType = OperationType.TriangleList;
            //#warning: missing op.srcRenderable
            //                op.useIndices = true;
            //                op.vertexData = vertexData;
            //                op.indexData = indexData;
            //            }
#warning missing LightListProperty
            /// <summary>
            /// 
            /// </summary>
            /// <param name="camera"></param>
            /// <returns></returns>
            public override float GetSquaredViewDepth(Camera camera) {
                Vector3 camVec = mParent.ConvertToLocal(camera.DerivedPosition) - mParent.Center;
                return camVec.LengthSquared;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="mat"></param>
            /// <returns></returns>
            private Material GetMaterialClone(Material mat) {
                string clonedName = mat.Name + "_Batched";
                Material clonedMat = (Material)MaterialManager.Instance.GetByName(clonedName);
                if (clonedMat == null)
                    clonedMat = mat.Clone(clonedName);

                return clonedMat;
            }

        }
        #endregion

    }

    public class DebugDrawer : IDisposable {

        private static DebugDrawer DD;
        public const int DEFAULT_ICOSPHERE_RECURSION_LEVEL = 0;
        public static DebugDrawer Singleton {
            get { return DD; }
        }
        public static void SetDebugDrawer(DebugDrawer DD) {
            DebugDrawer.DD = DD;
        }

        private SceneManager sceneManager;
        private ManualObject manualObject;
        private float fillAlpha;

        private IcoSphere _icoSphere = new IcoSphere();
        private LinkedList<KeyValuePair<Vector3, ColourValue>> lineVertices = new LinkedList<KeyValuePair<Vector3, ColourValue>>();
        private LinkedList<KeyValuePair<Vector3, ColourValue>> triangleVertices = new LinkedList<KeyValuePair<Vector3, ColourValue>>();
        private LinkedList<int> lineIndices = new LinkedList<int>();

        private LinkedList<int> triangleIndices = new LinkedList<int>();
        private int linesIndex;

        private int trianglesIndex;
        public class IcoSphere {
            public IcoSphere() {
                index = 0;
            }
            public void Dispose() {
            }
            public void create(int recursionLevel) {
                vertices.Clear();
                _lineIndices.Clear();
                _triangleIndices.Clear();
                faces.Clear();
                middlePointIndexCache.Clear();
                index = 0;

                float t = (1f + Mogre.Math.Sqrt(5f)) / 2f;

                addVertex(new Vector3(-1f, t, 0f));
                addVertex(new Vector3(1f, t, 0f));
                addVertex(new Vector3(-1f, -t, 0f));
                addVertex(new Vector3(1f, -t, 0f));

                addVertex(new Vector3(0f, -1f, t));
                addVertex(new Vector3(0f, 1f, t));
                addVertex(new Vector3(0f, -1f, -t));
                addVertex(new Vector3(0f, 1f, -t));

                addVertex(new Vector3(t, 0f, -1f));
                addVertex(new Vector3(t, 0f, 1f));
                addVertex(new Vector3(-t, 0f, -1f));
                addVertex(new Vector3(-t, 0f, 1f));

                addFace(0, 11, 5);
                addFace(0, 5, 1);
                addFace(0, 1, 7);
                addFace(0, 7, 10);
                addFace(0, 10, 11);

                addFace(1, 5, 9);
                addFace(5, 11, 4);
                addFace(11, 10, 2);
                addFace(10, 7, 6);
                addFace(7, 1, 8);

                addFace(3, 9, 4);
                addFace(3, 4, 2);
                addFace(3, 2, 6);
                addFace(3, 6, 8);
                addFace(3, 8, 9);

                addFace(4, 9, 5);
                addFace(2, 4, 11);
                addFace(6, 2, 10);
                addFace(8, 6, 7);
                addFace(9, 8, 1);

                addLineIndices(1, 0);
                addLineIndices(1, 5);
                addLineIndices(1, 7);
                addLineIndices(1, 8);
                addLineIndices(1, 9);

                addLineIndices(2, 3);
                addLineIndices(2, 4);
                addLineIndices(2, 6);
                addLineIndices(2, 10);
                addLineIndices(2, 11);

                addLineIndices(0, 5);
                addLineIndices(5, 9);
                addLineIndices(9, 8);
                addLineIndices(8, 7);
                addLineIndices(7, 0);

                addLineIndices(10, 11);
                addLineIndices(11, 4);
                addLineIndices(4, 3);
                addLineIndices(3, 6);
                addLineIndices(6, 10);

                addLineIndices(0, 11);
                addLineIndices(11, 5);
                addLineIndices(5, 4);
                addLineIndices(4, 9);
                addLineIndices(9, 3);
                addLineIndices(3, 8);
                addLineIndices(8, 6);
                addLineIndices(6, 7);
                addLineIndices(7, 10);
                addLineIndices(10, 0);

                for (int i = 0; i <= recursionLevel - 1; i++) {
                    LinkedList<TriangleIndices> faces2 = new LinkedList<TriangleIndices>();

                    var j = faces.GetEnumerator();
                    while (j.MoveNext()) {
                        TriangleIndices f = j.Current;
                        int a = getMiddlePoint(f.v1, f.v2);
                        int b = getMiddlePoint(f.v2, f.v3);
                        int c = getMiddlePoint(f.v3, f.v1);

                        removeLineIndices(f.v1, f.v2);
                        removeLineIndices(f.v2, f.v3);
                        removeLineIndices(f.v3, f.v1);

                        faces2.AddLast(new LinkedListNode<TriangleIndices>(new TriangleIndices(f.v1, a, c)));
                        faces2.AddLast(new LinkedListNode<TriangleIndices>(new TriangleIndices(f.v2, b, a)));
                        faces2.AddLast(new LinkedListNode<TriangleIndices>(new TriangleIndices(f.v3, c, b)));
                        faces2.AddLast(new LinkedListNode<TriangleIndices>(new TriangleIndices(a, b, c)));

                        addTriangleLines(f.v1, a, c);
                        addTriangleLines(f.v2, b, a);
                        addTriangleLines(f.v3, c, b);
                    }
                    j.Dispose();
                    faces = faces2;
                }
            }
            public void addLineIndices(int index0, int index1) {
                _lineIndices.AddLast(new LinkedListNode<LineIndices>(new LineIndices(index0, index1)));
            }
            public void removeLineIndices(int index0, int index1) {
                var result = _lineIndices.Find(new LineIndices(index0, index1));

                if (result != null) {
                    _lineIndices.Remove(result);
                }
            }
            public void addTriangleLines(int index0, int index1, int index2) {
                addLineIndices(index0, index1);
                addLineIndices(index1, index2);
                addLineIndices(index2, index0);
            }
            public int addVertex(Vector3 vertex) {
                float length = vertex.Length;
                vertices.Add(new Vector3(vertex.x / length, vertex.y / length, vertex.z / length));
                index += 1;
                return index;
            }
            public int getMiddlePoint(int index0, int index1) {
                bool isFirstSmaller = index0 < index1;
                long smallerIndex = isFirstSmaller ? index0 : index1;
                long largerIndex = isFirstSmaller ? index1 : index0;
                long key = (smallerIndex << 32) | largerIndex;

                if (middlePointIndexCache.ContainsKey(key) && middlePointIndexCache[key] != middlePointIndexCache.Keys.Count) {
                    return middlePointIndexCache[key];
                }

                Vector3 point1 = vertices[index0];
                Vector3 point2 = vertices[index1];
                Vector3 middle = point1.MidPoint(point2);

                int index = addVertex(middle);
                if (middlePointIndexCache.ContainsKey(key) == false) {
                    middlePointIndexCache.Add(key, index);
                }
                else {
                    middlePointIndexCache[key] = index;

                }
                return index;
            }
            public void addFace(int index0, int index1, int index2) {
                faces.AddLast(new TriangleIndices(index0, index1, index2));
            }
            public void addToLineIndices(int baseIndex, LinkedList<int> target) {
                LinkedList<LineIndices>.Enumerator i = _lineIndices.GetEnumerator();
                while (i.MoveNext()) {
                    target.AddLast(baseIndex + i.Current.v1);
                    target.AddLast(baseIndex + i.Current.v2);
                }
                i.Dispose();
            }
            public void addToTriangleIndices(int baseIndex, LinkedList<int> target) {
                LinkedList<TriangleIndices>.Enumerator i = faces.GetEnumerator();
                while (i.MoveNext()) {
                    target.AddLast(baseIndex + i.Current.v1);
                    target.AddLast(baseIndex + i.Current.v2);
                    target.AddLast(baseIndex + i.Current.v3);
                }
                i.Dispose();
            }
            public int addToVertices(LinkedList<KeyValuePair<Vector3, ColourValue>> target, Vector3 position, ColourValue colour, float scale) {
                Matrix4 transform = Matrix4.IDENTITY;
                transform.SetTrans(position);
                transform.SetScale(new Vector3(scale, scale, scale));

                for (int i = 0; i <= Convert.ToInt32(vertices.Count) - 1; i++) {
                    target.AddLast(new KeyValuePair<Vector3, ColourValue>(transform * vertices[i], colour));
                }

                return vertices.Count;
            }
            public class TriangleIndices {
                public int v1;
                public int v2;

                public int v3;
                public TriangleIndices(int _v1, int _v2, int _v3) {
                    v1 = _v1;
                    v2 = _v2;
                    v3 = _v3;
                }

                public static bool operator <(TriangleIndices ImpliedObject, TriangleIndices o) {
                    return ImpliedObject.v1 < o.v1 && ImpliedObject.v2 < o.v2 && ImpliedObject.v3 < o.v3;
                }
                public static bool operator >(TriangleIndices ImpliedObject, TriangleIndices o) {
                    return ImpliedObject.v1 > o.v1 && ImpliedObject.v2 > o.v2 && ImpliedObject.v3 > o.v3;
                }
            }

            public class LineIndices {
                public int v1;

                public int v2;
                public LineIndices(int _v1, int _v2) {
                    v1 = _v1;
                    v2 = _v2;
                }

                //C++ TO VB CONVERTER WARNING: 'const' methods are not available in VB:
                //ORIGINAL LINE: Boolean operator == (const LineIndices &o) const
                public static bool operator ==(LineIndices ImpliedObject, LineIndices o) {
                    return (ImpliedObject.v1 == o.v1 && ImpliedObject.v2 == o.v2) || (ImpliedObject.v1 == o.v2 && ImpliedObject.v2 == o.v1);
                }
                public static bool operator !=(LineIndices ImpliedObject, LineIndices o) {
                    return (ImpliedObject.v1 != o.v1 && ImpliedObject.v2 != o.v2) || (ImpliedObject.v1 != o.v2 && ImpliedObject.v2 != o.v1);
                }
                public override bool Equals(object obj) {
                    LineIndices o = obj as LineIndices;
                    if (o == null) {
                        return false;
                    }
                    return v1 == o.v1 && v2 == o.v2;
                }
                public override int GetHashCode() {
                    return v1 ^ v2;
                }
            }


            private List<Vector3> vertices = new List<Vector3>();
            private LinkedList<LineIndices> _lineIndices = new LinkedList<LineIndices>();
            private LinkedList<int> _triangleIndices = new LinkedList<int>();
            private LinkedList<TriangleIndices> faces = new LinkedList<TriangleIndices>();
            private Dictionary<long, int> middlePointIndexCache = new Dictionary<long, int>();
            private int index;
        }


        public DebugDrawer(SceneManager _sceneManager, float _fillAlpha) {
            sceneManager = _sceneManager;
            fillAlpha = _fillAlpha;
            manualObject = null;
            linesIndex = 0;
            trianglesIndex = 0;
        }

        public void Initialise() {
            manualObject = sceneManager.CreateManualObject("debug_object");
            sceneManager.RootSceneNode.CreateChildSceneNode("debug_object").AttachObject(manualObject);
            manualObject.Dynamic = true;

            _icoSphere.create(DEFAULT_ICOSPHERE_RECURSION_LEVEL);

            manualObject.Begin("debug_draw", RenderOperation.OperationTypes.OT_LINE_LIST);
            manualObject.Position(Vector3.ZERO);
            manualObject.Colour(ColourValue.ZERO);
            manualObject.Index(0);
            manualObject.End();
            manualObject.Begin("debug_draw", RenderOperation.OperationTypes.OT_TRIANGLE_LIST);
            manualObject.Position(Vector3.ZERO);
            manualObject.Colour(ColourValue.ZERO);
            manualObject.Index(0);
            manualObject.End();

            trianglesIndex = 0;
            linesIndex = trianglesIndex;
        }
        public void SetIcoSphereRecursionLevel(int recursionLevel) {
            _icoSphere.create(recursionLevel);
        }
        private void Shutdown() {
            sceneManager.DestroySceneNode("debug_object");
            sceneManager.DestroyManualObject(manualObject);
        }
        public void BuildLine(Vector3 start, Vector3 end, ColourValue colour, float alpha) {
            int i = AddLineVertex(start, new ColourValue(colour.r, colour.g, colour.b, alpha));
            AddLineVertex(end, new ColourValue(colour.r, colour.g, colour.b, alpha));

            AddLineIndices(i, i + 1);
        }
        public void BuildQuad(Vector3[] vertices, ColourValue colour, float alpha) {
            int index = AddLineVertex(vertices[0], new ColourValue(colour.r, colour.g, colour.b, alpha));
            AddLineVertex(vertices[1], new ColourValue(colour.r, colour.g, colour.b, alpha));
            AddLineVertex(vertices[2], new ColourValue(colour.r, colour.g, colour.b, alpha));
            AddLineVertex(vertices[3], new ColourValue(colour.r, colour.g, colour.b, alpha));

            for (int i = 0; i <= 3; i++) {
                AddLineIndices(index + i, index + ((i + 1) % 4));
            }
        }
        public void BuildCircle(Vector3 centre, float radius, int segmentsCount, ColourValue colour, float alpha) {
            int index = linesIndex;
            float increment = (float)(2 * Mogre.Math.PI / segmentsCount);
            float angle = 0f;

            for (int i = 0; i <= segmentsCount - 1; i++) {
                AddLineVertex(new Vector3(centre.x + radius * Mogre.Math.Cos(angle), centre.y, centre.z + radius * Mogre.Math.Sin(angle)), new ColourValue(colour.r, colour.g, colour.b, alpha));
                angle += increment;
            }

            for (int i = 0; i <= segmentsCount - 1; i++) {
                AddLineIndices(index + i, i + 1 < segmentsCount ? index + i + 1 : index);
            }
        }
        public void BuildFilledCircle(Vector3 centre, float radius, int segmentsCount, ColourValue colour, float alpha) {
            int index = trianglesIndex;
            float increment = 2 * Mogre.Math.PI / segmentsCount;
            float angle = 0f;

            for (int i = 0; i <= segmentsCount - 1; i++) {
                AddTriangleVertex(new Vector3(centre.x + radius * Mogre.Math.Cos(angle), centre.y, centre.z + radius * Mogre.Math.Sin(angle)), new ColourValue(colour.r, colour.g, colour.b, alpha));
                angle += increment;
            }

            AddTriangleVertex(centre, new ColourValue(colour.r, colour.g, colour.b, alpha));

            for (int i = 0; i <= segmentsCount - 1; i++) {
                AddTriangleIndices(i + 1 < segmentsCount ? index + i + 1 : index, index + i, index + segmentsCount);
            }
        }
        /// <summary>
        /// Create a tetrahedron with point of origin in middle of volume. 
        /// It will be added to the SceneManager as ManualObject. The material must still exists.
        /// (Tubulii: Thanks to Beauty for sharing this code| !Slightly modified!)
        /// </summary>
        /// <param name="position">Position in scene</param>
        /// <param name="scale">Size of the tetrahedron</param>
        /// <param name="Color">The color of the tetrahedron</param>
        public void DrawTetrahedron(Vector3 position, Single scale, ColourValue Color, bool IsFilled) {
            //Dim manObTetra As ManualObject = sceneManager.CreateManualObject(name)
            //manObTetra.CastShadows = False

            //' render just before overlays (so all objects behind the transparent tetrahedron are visible)
            //manObTetra.RenderQueueGroup = CByte(RenderQueueGroupID.RENDER_QUEUE_OVERLAY) - 1
            // = 99
            Vector3[] c = new Vector3[4];
            // corners
            // calculate corners of tetrahedron (with point of origin in middle of volume)
            Single mbot = scale * 0.2f;
            // distance middle to bottom
            Single mtop = scale * 0.62f;
            // distance middle to top    
            Single mf = scale * 0.289f;
            // distance middle to front
            Single mb = scale * 0.577f;
            // distance middle to back
            Single mlr = scale * 0.5f;
            // distance middle to left right 
            //               width / height / depth
            c[0] = new Vector3(-mlr, -mbot, mf);
            // left bottom front
            c[1] = new Vector3(mlr, -mbot, mf);
            // right bottom front
            c[2] = new Vector3(0, -mbot, -mb);
            // (middle) bottom back
            c[3] = new Vector3(0, mtop, 0);
            // (middle) top (middle)
            // add position offset for all corners (move tetrahedron)
            for (Int16 i = 0; i <= 3; i++) {
                c[i] += position;
            }

            // create lines
            // bottom
            BuildLine(c[2], c[1], Color, 1);
            BuildLine(c[1], c[0], Color, 1);
            BuildLine(c[0], c[2], Color, 1);
            // rest
            BuildLine(c[2], c[3], Color, 1);
            BuildLine(c[1], c[3], Color, 1);
            BuildLine(c[0], c[3], Color, 1);

            if (IsFilled) {
                // create bottom
                BuildFilledTriangle(new Vector3[] {
            c[2],
            c[1],
            c[0]
         }, Color, fillAlpha);

                // create right back side
                BuildFilledTriangle(new Vector3[] {
            c[1],
            c[2],
            c[3]
         }, Color, fillAlpha);

                // create left back side
                BuildFilledTriangle(new Vector3[] {
            c[3],
            c[2],
            c[0]
         }, Color, fillAlpha);


                // create front side
                BuildFilledTriangle(new Vector3[] {
            c[0],
            c[1],
            c[3]
         }, Color, fillAlpha);
            }




        }

        public void BuildCuboid(Vector3[] vertices, ColourValue colour, float alpha) {
            int index = AddLineVertex(vertices[0], new ColourValue(colour.r, colour.g, colour.b, alpha));
            for (int i = 1; i <= 7; i++) {
                AddLineVertex(vertices[i], new ColourValue(colour.r, colour.g, colour.b, alpha));
            }

            for (int i = 0; i <= 3; i++) {
                AddLineIndices(index + i, index + ((i + 1) % 4));
            }
            for (int i = 4; i <= 7; i++) {
                AddLineIndices(index + i, i == 7 ? index + 4 : index + i + 1);
            }
            AddLineIndices(index + 1, index + 5);
            AddLineIndices(index + 2, index + 4);
            AddLineIndices(index, index + 6);
            AddLineIndices(index + 3, index + 7);
        }
        public void BuildFilledCuboid(Vector3[] vertices, ColourValue colour, float alpha) {
            int index = AddTriangleVertex(vertices[0], new ColourValue(colour.r, colour.g, colour.b, alpha));
            for (int i = 1; i <= 7; i++) {
                AddTriangleVertex(vertices[i], new ColourValue(colour.r, colour.g, colour.b, alpha));
            }

            AddQuadIndices(index, index + 1, index + 2, index + 3);
            AddQuadIndices(index + 4, index + 5, index + 6, index + 7);

            AddQuadIndices(index + 1, index + 5, index + 4, index + 2);
            AddQuadIndices(index, index + 3, index + 7, index + 6);

            AddQuadIndices(index + 1, index, index + 6, index + 5);
            AddQuadIndices(index + 4, index + 7, index + 3, index + 2);
        }
        public void BuildFilledQuad(Vector3[] vertices, ColourValue colour, float alpha) {
            int index = AddTriangleVertex(vertices[0], new ColourValue(colour.r, colour.g, colour.b, alpha));
            AddTriangleVertex(vertices[1], new ColourValue(colour.r, colour.g, colour.b, alpha));
            AddTriangleVertex(vertices[2], new ColourValue(colour.r, colour.g, colour.b, alpha));
            AddTriangleVertex(vertices[3], new ColourValue(colour.r, colour.g, colour.b, alpha));

            AddQuadIndices(index, index + 1, index + 2, index + 3);
        }
        public void BuildFilledTriangle(Vector3[] vertices, ColourValue colour, float alpha) {
            int index = AddTriangleVertex(vertices[0], new ColourValue(colour.r, colour.g, colour.b, alpha));
            AddTriangleVertex(vertices[1], new ColourValue(colour.r, colour.g, colour.b, alpha));
            AddTriangleVertex(vertices[2], new ColourValue(colour.r, colour.g, colour.b, alpha));

            AddTriangleIndices(index, index + 1, index + 2);
        }
        public void DrawLine(Vector3 start, Vector3 end, ColourValue colour) {
            BuildLine(start, end, colour, 1);
        }
        public void drawCircle(Vector3 centre, float radius, int segmentsCount, ColourValue colour, bool isFilled) {
            BuildCircle(centre, radius, segmentsCount, colour, 1);
            if (isFilled) {
                BuildFilledCircle(centre, radius, segmentsCount, colour, fillAlpha);
            }
        }
        public void DrawQuad(Vector3[] vertices, ColourValue colour, bool isFilled) {
            BuildQuad(vertices, colour, 1);
            if (isFilled) {
                BuildFilledQuad(vertices, colour, fillAlpha);
            }
        }
        public void DrawCuboid(Vector3[] vertices, ColourValue colour, bool isFilled) {
            BuildCuboid(vertices, colour, fillAlpha);
            if (isFilled) {
                BuildFilledCuboid(vertices, colour, fillAlpha);
            }
        }
        public void DrawSphere(Vector3 centre, float radius, ColourValue colour, bool isFilled) {
            int baseIndex = linesIndex;
            linesIndex += _icoSphere.addToVertices(lineVertices, centre, colour, radius);
            _icoSphere.addToLineIndices(baseIndex, lineIndices);

            if (isFilled) {
                baseIndex = trianglesIndex;
                trianglesIndex += _icoSphere.addToVertices(triangleVertices, centre, new ColourValue(colour.r, colour.g, colour.b, fillAlpha), radius);
                _icoSphere.addToTriangleIndices(baseIndex, triangleIndices);
            }
        }

        public void Build() {

            if (lineVertices.Count > 0) {
                manualObject.BeginUpdate(0);
                manualObject.EstimateVertexCount((uint)lineVertices.Count + 1);
                manualObject.EstimateIndexCount((uint)lineIndices.Count + 1);
                var i = lineVertices.GetEnumerator();
                while (i.MoveNext()) {
                    manualObject.Position(i.Current.Key);
                    manualObject.Colour(i.Current.Value);
                }
                i.Dispose();
                LinkedList<int>.Enumerator i2 = lineIndices.GetEnumerator();
                while (i2.MoveNext()) {
                    manualObject.Index((ushort)i2.Current);
                }
                i2.Dispose();
                manualObject.End();

            }

            if (triangleVertices.Count > 0) {
                manualObject.BeginUpdate(1);
                manualObject.EstimateVertexCount((uint)triangleVertices.Count + 1);
                manualObject.EstimateIndexCount((uint)triangleIndices.Count + 1);
                var i = triangleVertices.GetEnumerator();
                while (i.MoveNext()) {
                    manualObject.Position(i.Current.Key);
                    manualObject.Colour(i.Current.Value.r, i.Current.Value.g, i.Current.Value.b, fillAlpha);
                }
                i.Dispose();
                LinkedList<int>.Enumerator i2 = triangleIndices.GetEnumerator();
                while (i2.MoveNext()) {
                    manualObject.Index((ushort)i2.Current);
                }
                i2.Dispose();
                manualObject.End();
            }
        }
        public void Clear() {
            lineVertices.Clear();
            triangleVertices.Clear();
            lineIndices.Clear();
            triangleIndices.Clear();
            trianglesIndex = 0;
            linesIndex = trianglesIndex;
        }
        public int AddLineVertex(Vector3 vertex, ColourValue colour) {
            lineVertices.AddLast(new KeyValuePair<Vector3, ColourValue>(vertex, colour));

            linesIndex += 1;
            return linesIndex - 1;
        }
        public void AddLineIndices(int index1, int index2) {
            lineIndices.AddLast(index1);
            lineIndices.AddLast(index2);
        }
        public int AddTriangleVertex(Vector3 vertex, ColourValue colour) {
            triangleVertices.AddLast(new KeyValuePair<Vector3, ColourValue>(vertex, colour));

            trianglesIndex += 1;
            return trianglesIndex - 1;
        }
        public void AddTriangleIndices(int index1, int index2, int index3) {
            triangleIndices.AddLast(index1);
            triangleIndices.AddLast(index2);
            triangleIndices.AddLast(index3);
        }
        public void AddQuadIndices(int index1, int index2, int index3, int index4) {
            triangleIndices.AddLast(index1);
            triangleIndices.AddLast(index2);
            triangleIndices.AddLast(index3);

            triangleIndices.AddLast(index1);
            triangleIndices.AddLast(index3);
            triangleIndices.AddLast(index4);
        }

        #region "IDisposable Support"
        // So ermitteln Sie überflüssige Aufrufe
        private bool disposedValue;

        // IDisposable
        protected virtual void Dispose(bool disposing) {
            if (!this.disposedValue) {
                if (disposing) {
                    // TODO: Verwalteten Zustand löschen (verwaltete Objekte).
                }
                Shutdown();
                // TODO: Nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalize() unten überschreiben.
                // TODO: Große Felder auf NULL festlegen.
            }
            this.disposedValue = true;
        }

        // TODO: Finalize() nur überschreiben, wenn Dispose(ByVal disposing As Boolean) oben über Code zum Freigeben von nicht verwalteten Ressourcen verfügt.
        //Protected Overrides Sub Finalize()
        //    ' Ändern Sie diesen Code nicht. Fügen Sie oben in Dispose(ByVal disposing As Boolean) Bereinigungscode ein.
        //    Dispose(False)
        //    MyBase.Finalize()
        //End Sub

        // Dieser Code wird von Visual Basic hinzugefügt, um das Dispose-Muster richtig zu implementieren.
        public void Dispose() {
            // Ändern Sie diesen Code nicht. Fügen Sie oben in Dispose(ByVal disposing As Boolean) Bereinigungscode ein.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
